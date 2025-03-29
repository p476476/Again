using System;
using System.Collections.Generic;
using Again.Runtime.Commands.Spine;
using Again.Runtime.Common;
using Again.Runtime.Enums;
using Again.Runtime.Save.Structs;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Again.Runtime.Components.Managers
{
    [Serializable]
    public class SpineInfo
    {
        [SerializeField] public string spineName;

        [SerializeField] public SkeletonDataAsset skeletonDataAsset;

        [SerializeField] public Vector2 offsetRatio = new(0, 0);
        [SerializeField] public Vector2 scale = new(1, 1);
    }

    public class SpineManager : MonoBehaviour
    {
        private const float ShakeFactor = 0.5f;
        private const float PhysicsFactor = 1f;
        public GameObject spineGameObjectPrefab;
        public GameObject spineView;
        private readonly List<SpineInfo> _spineInfos = new();
        private Dictionary<string, GameObject> _spineGameObjectDict;

        private void Awake()
        {
            _spineGameObjectDict = new Dictionary<string, GameObject>();
            var assets = Resources.LoadAll<SkeletonDataAsset>("Spines");
            foreach (var asset in assets)
            {
                var spineInfo = _spineInfos.Find(info => info.spineName == asset.name);
                if (spineInfo != null) spineInfo.skeletonDataAsset = asset;

                _spineInfos.Add(new SpineInfo
                {
                    spineName = asset.name,
                    skeletonDataAsset = asset
                });
            }
        }

        public void Reset()
        {
            foreach (var entry in _spineGameObjectDict)
            {
                var go = entry.Value;
                Destroy(go);
            }

            _spineGameObjectDict.Clear();
        }

        public void UpdateSpineInfos(Dictionary<string, ScriptImpoter.SpineInfo> spineDataDict)
        {
            foreach (var pair in spineDataDict)
            {
                var spineInfo = _spineInfos.Find(info => info.spineName == pair.Key);
                if (spineInfo != null)
                {
                    spineInfo.offsetRatio = new Vector2(pair.Value.OffsetX, pair.Value.OffsetY);
                    spineInfo.scale = new Vector2(pair.Value.ScaleX, pair.Value.ScaleY);
                    continue;
                }

                _spineInfos.Add(new SpineInfo
                {
                    spineName = pair.Key,
                    offsetRatio = new Vector2(pair.Value.OffsetX, pair.Value.OffsetY),
                    scale = new Vector2(pair.Value.ScaleX, pair.Value.ScaleY)
                });
            }
        }

        public void Load(string saveData)
        {
            var data = JsonUtility.FromJson<SpineManagerSaveData>(saveData);
            foreach (var spineObjectData in data.SpineObjectDataList)
            {
                var spineInfo = _spineInfos.Find(info => info.spineName == spineObjectData.name);
                if (spineInfo == null) continue;

                var spineGameObject = CreateSpineGameObject(
                    spineObjectData.name,
                    spineObjectData.animationName,
                    spineObjectData.skinName,
                    spineObjectData.isLoop,
                    10,
                    spineInfo,
                    0
                );

                var spineRT = spineGameObject.GetComponent<RectTransform>();
                spineRT.localPosition = spineObjectData.position;
                spineRT.eulerAngles = spineObjectData.rotation;
                spineRT.localScale = spineObjectData.scale;
                spineRT.sizeDelta = spineObjectData.sizeDelta;

                var material = spineGameObject.GetComponentInChildren<SkeletonAnimation>().skeletonDataAsset
                    .atlasAssets[0].PrimaryMaterial;
                material.SetColor("_Color", spineObjectData.materialColor);
                material.SetColor("_Black", spineObjectData.materialBlackColor);
            }
        }

        public string Save()
        {
            return SpineManagerSaveData.ToJson(_spineGameObjectDict);
        }

        public GameObject GetSpineObject(string name)
        {
            return _spineGameObjectDict.TryGetValue(name, out var go) ? go : null;
        }

        public void Show(ShowSpineCommand command, Action onComplete = null)
        {
            var spineInfo = _spineInfos.Find(info => info.spineName == command.SpineName);
            if (spineInfo == null)
            {
                Debug.LogError("Spine not found: " + command.Name);
                onComplete?.Invoke();
                return;
            }

            var spineGameObject = CreateSpineGameObject(
                command.Name,
                command.Animation,
                command.Skin,
                command.IsLoop,
                command.Order,
                spineInfo,
                command.Id
            );
            var spineAnimation = spineGameObject.GetComponentInChildren<SkeletonAnimation>();
            var parentWidth = spineView.GetComponent<RectTransform>().rect.width;

            var spineRT = spineGameObject.GetComponent<RectTransform>();
            var spineWidth = spineAnimation.skeletonDataAsset.GetSkeletonData(true).Width;
            var spineHeight = spineAnimation.skeletonDataAsset.GetSkeletonData(true).Height;
            var spineScale = spineAnimation.skeletonDataAsset.scale;
            spineRT.sizeDelta = new Vector2(spineWidth * spineScale, spineHeight * spineScale);
            spineRT.localPosition = new Vector3(
                command.PosX * parentWidth / 2,
                command.PosY * parentWidth / 2,
                0
            );
            spineRT.localScale = new Vector3(command.ScaleX * spineInfo.scale.x, command.ScaleY * spineInfo.scale.x, 1);

            var duration = command.IsSkip ? 0 : command.Duration;
            switch (command.ShowType)
            {
                case ShowAnimationType.None:
                    onComplete?.Invoke();
                    break;
                case ShowAnimationType.Fade:
                    spineAnimation.skeleton.A = 0;
                    DOTween
                        .To(
                            () => spineAnimation.skeleton.A,
                            x => spineAnimation.skeleton.A = x,
                            1,
                            duration
                        )
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShowAnimationType.SlideFromLeft:
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;
                    var pos = spineRT.localPosition;
                    spineRT.localPosition = new Vector3(-parentWidth / 2, pos.y, pos.z);
                    spineRT
                        .DOLocalMoveX(pos.x, duration)
                        .OnComplete(() =>
                        {
                            onComplete?.Invoke();
                            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
                        });
                    break;
                case ShowAnimationType.SlideFromRight:
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;
                    pos = spineRT.localPosition;
                    spineRT.localPosition = new Vector3(parentWidth / 2, pos.y, pos.z);
                    spineRT.transform
                        .DOLocalMoveX(pos.x, duration)
                        .OnComplete(() =>
                        {
                            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
                            onComplete?.Invoke();
                        });
                    break;
            }
        }

        private GameObject CreateSpineGameObject(string objectName, string animationName,
            string skinName, bool isLoop, int order, SpineInfo spineInfo, int id)
        {
            var spineGameObject = Instantiate(spineGameObjectPrefab, spineView.transform);
            spineGameObject.name = objectName;

            var spineAnimation = spineGameObject.GetComponentInChildren<SkeletonAnimation>();
            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
            spineAnimation.PhysicsRotationInheritanceFactor = PhysicsFactor;

            spineAnimation.skeletonDataAsset = spineInfo.skeletonDataAsset;
            _SetAnimation(spineAnimation, animationName, isLoop, id);
            _SetSkin(spineAnimation, skinName, id);
            _spineGameObjectDict.Add(objectName, spineGameObject);


            var spineWidth = spineAnimation.skeletonDataAsset.GetSkeletonData(true).Width;
            var spineHeight = spineAnimation.skeletonDataAsset.GetSkeletonData(true).Height;
            var spineScale = spineAnimation.skeletonDataAsset.scale;
            var animationRt = spineAnimation.GetComponent<RectTransform>();
            animationRt.localPosition = new Vector3(
                spineWidth * spineScale * spineInfo.offsetRatio.x,
                spineHeight * spineScale * spineInfo.offsetRatio.y,
                0
            );
            
            spineGameObject.GetComponentInChildren<MeshRenderer>().sortingOrder = order;

            var material = spineAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial;
            material.SetColor("_Color", Color.white);
            material.SetColor("_Black", Color.black);
            return spineGameObject;
        }

        public void Change(ChangeSpineCommand command)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                return;
            }

            var go = _spineGameObjectDict[command.Name];

            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();
            _SetAnimation(spineAnimation, command.Animation, command.IsLoop, command.Id);
            _SetSkin(spineAnimation, command.Skin, command.Id);

            spineAnimation.ApplyAnimation();
        }

        public void ChangeSpines(ChangeSpinesCommand command)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                return;
            }

            var go = _spineGameObjectDict[command.Name];

            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();

            for (var i = 0; i < command.Animations.Count; i++)
                _AddAnimation(spineAnimation, command.Animations[i],
                    i == command.Animations.Count - 1 && command.IsLoop,
                    command.Id);
        }

        public void Hide(HideSpineCommand command, Action onComplete = null)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                onComplete?.Invoke();
                return;
            }

            var go = _spineGameObjectDict[command.Name];

            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();
            var goRT = go.GetComponent<RectTransform>();
            var duration = command.IsSkip ? 0 : command.Duration;
            switch (command.HideType)
            {
                case HideAnimationType.None:
                    Destroy(spineAnimation.gameObject);
                    _spineGameObjectDict.Remove(command.Name);
                    onComplete?.Invoke();
                    break;
                case HideAnimationType.Fade:
                    spineAnimation.skeleton.A = 1;
                    DOTween
                        .To(
                            () => spineAnimation.skeleton.A,
                            x => spineAnimation.skeleton.A = x,
                            0,
                            duration
                        )
                        .OnComplete(() => _RemoveSpineAnimation(command.Name, onComplete));
                    break;
                case HideAnimationType.SlideToLeft:
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;
                    var spineWidth = spineAnimation.skeletonDataAsset.GetSkeletonData(true).Width *
                                     spineAnimation.skeletonDataAsset.scale;
                    goRT.DOLocalMoveX(
                            (spineView.GetComponent<RectTransform>().rect.width + spineWidth) * -0.5f,
                            duration
                        )
                        .OnComplete(() =>
                        {
                            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
                            _RemoveSpineAnimation(command.Name, onComplete);
                        });
                    break;
                case HideAnimationType.SlideToRight:
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;
                    spineWidth = spineAnimation.skeletonDataAsset.GetSkeletonData(true).Width *
                                 spineAnimation.skeletonDataAsset.scale;
                    goRT.DOLocalMoveX(
                            (spineView.GetComponent<RectTransform>().rect.width + spineWidth) * 0.5f,
                            duration
                        )
                        .OnComplete(() =>
                        {
                            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
                            _RemoveSpineAnimation(command.Name, onComplete);
                        });
                    break;
            }
        }

        public void Move(MoveSpineCommand command, Action onComplete = null)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                onComplete?.Invoke();
                return;
            }

            var go = _spineGameObjectDict[command.Name];
            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();
            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;

            var parentWidth = spineView.GetComponent<RectTransform>().rect.width;
            go.GetComponent<RectTransform>()
                .DOLocalMove(
                    new Vector3(command.PosX * parentWidth / 2, command.PosY * parentWidth / 2, 0),
                    command.IsSkip ? 0 : command.Duration
                )
                .OnComplete(() =>
                {
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one;
                    onComplete?.Invoke();
                });
        }

        public void Scale(ScaleSpineCommand command, Action onComplete = null)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                onComplete?.Invoke();
                return;
            }

            var go = _spineGameObjectDict[command.Name];

            var goRT = go.GetComponent<RectTransform>();
            PivotTool.SetPivotInWorldSpace(goRT, new Vector2(command.AnchorX, command.AnchorY));

            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();
            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;

            goRT.DOScale(
                    new Vector3(command.ScaleX, command.ScaleY, 1),
                    command.IsSkip ? 0 : command.Duration
                )
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
                });
        }

        public void Jump(JumpSpineCommand command, Action onComplete = null)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                onComplete?.Invoke();
                return;
            }

            var go = _spineGameObjectDict[command.Name];

            var position = go.GetComponent<RectTransform>().localPosition;
            go.GetComponent<RectTransform>()
                .DOLocalJump(position, command.JumpPower, command.JumpCount, command.IsSkip ? 0 : command.Duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Shake(ShakeSpineCommand command, Action onComplete = null)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                onComplete?.Invoke();
                return;
            }

            var go = _spineGameObjectDict[command.Name];

            var goRT = go.GetComponent<RectTransform>();
            var strength = command.Strength * ShakeFactor;
            var duration = command.IsSkip ? 0 : command.Duration;
            switch (command.ShakeType)
            {
                case ShakeType.Horizontal:
                    goRT.DOShakePosition(
                            duration,
                            Vector3.right * strength,
                            command.Vibrato,
                            command.Randomness,
                            command.Snapping,
                            command.FadeOut
                        )
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShakeType.Vertical:
                    goRT.DOShakePosition(
                            duration,
                            Vector3.up * strength,
                            command.Vibrato,
                            command.Randomness,
                            command.Snapping,
                            command.FadeOut
                        )
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShakeType.HorizontalAndVertical:
                    goRT.DOShakePosition(
                            duration,
                            strength,
                            command.Vibrato,
                            command.Randomness,
                            command.Snapping,
                            command.FadeOut
                        )
                        .OnComplete(() => onComplete?.Invoke());
                    break;
            }
        }

        public void ChangeColor(ChangeSpineColorCommand command)
        {
            if (!_spineGameObjectDict.ContainsKey(command.Name))
            {
                Debug.LogError("Spine not found: " + command.Name);
                return;
            }

            var go = _spineGameObjectDict[command.Name];

            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();
            var material = spineAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial;
            material.SetColor("_Color", Color.white);
            material.SetColor("_Black", Color.black);

            switch (command.ChangeColorType)
            {
                case ChangeColorType.None:
                    break;
                case ChangeColorType.Additive:
                    material.SetColor("_Black", command.ColorDelta);
                    break;
                case ChangeColorType.Subtractive:
                    material.SetColor("_Color", command.ColorDelta);
                    break;
            }
        }

        public void HideAll(Action onComplete = null)
        {
            foreach (var entry in _spineGameObjectDict)
            {
                var key = entry.Key;
                var go = entry.Value;
                var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();

                spineAnimation.skeleton.A = 1;
                DOTween
                    .To(() => spineAnimation.skeleton.A, x => spineAnimation.skeleton.A = x, 0, 1)
                    .OnComplete(() =>
                    {
                        Destroy(go);
                        _spineGameObjectDict.Remove(key);
                        if (_spineGameObjectDict.Count == 0)
                            onComplete?.Invoke();
                    });
            }
        }

        private void _RemoveSpineAnimation(string key, Action onComplete = null)
        {
            var go = _spineGameObjectDict[key];
            Destroy(go.gameObject);
            _spineGameObjectDict.Remove(key);
            onComplete?.Invoke();
        }

        private void _SetAnimation(SkeletonAnimation anim, string animationName, bool isLoop, int commandIndex)
        {
            if (string.IsNullOrEmpty(animationName))
                return;

            try
            {
                anim.AnimationState.SetAnimation(0, animationName, isLoop);
            }
            catch (Exception)
            {
                Debug.LogError($"Line {commandIndex} 找不到 Animation: {animationName}");
            }
        }

        private void _AddAnimation(SkeletonAnimation anim, string animationName, bool isLoop, int commandIndex)
        {
            if (string.IsNullOrEmpty(animationName))
                return;

            try
            {
                anim.AnimationState.AddAnimation(0, animationName, isLoop, 0);
            }
            catch (Exception)
            {
                Debug.LogError($"Line {commandIndex} 找不到 Animation: {animationName}");
            }
        }

        private void _SetSkin(SkeletonAnimation anim, string skinName, int commandIndex)
        {
            if (string.IsNullOrEmpty(skinName))
                return;

            try
            {
                anim.skeleton.SetSkin(skinName);
                anim.skeleton.SetToSetupPose();
            }
            catch (Exception)
            {
                Debug.LogError($"Line {commandIndex} 找不到Skin: {skinName}");
            }
        }
    }
}