using System;
using System.Collections.Generic;
using Again.Scripts.Runtime.Commands.Spine;
using Again.Scripts.Runtime.Common;
using Again.Scripts.Runtime.Enums;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    [Serializable]
    public class SpineInfo
    {
        [SerializeField] public string spineName;

        [SerializeField] public SkeletonDataAsset skeletonDataAsset;

        [SerializeField] public Vector2 pivot = new(0.5f, 0);
    }

    public class SpineManager : MonoBehaviour
    {
        private const float ShakeFactor = 0.5f;
        private const float PhysicsFactor = 1f;
        public GameObject spineGameObjectPrefab;
        public GameObject spineView;
        private Dictionary<string, GameObject> _spineGameObjectDict;
        private List<SpineInfo> _spineInfos;

        private void Awake()
        {
            _spineGameObjectDict = new Dictionary<string, GameObject>();
            _spineInfos = new List<SpineInfo>();
            var assets = Resources.LoadAll<SkeletonDataAsset>("Spines");
            foreach (var asset in assets)
                _spineInfos.Add(new SpineInfo
                {
                    spineName = asset.name,
                    skeletonDataAsset = asset
                });
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

        public GameObject GetSpineObject(string spineName)
        {
            return _spineGameObjectDict.TryGetValue(spineName, out var go) ? go : null;
        }

        public void Show(ShowSpineCommand command, Action onComplete = null)
        {
            var spineInfo = _spineInfos.Find(info => info.spineName == command.Name);
            if (spineInfo == null)
            {
                Debug.LogError("Spine not found: " + command.Name);
                onComplete?.Invoke();
                return;
            }

            var spineGameObject = Instantiate(spineGameObjectPrefab, spineView.transform);
            var parentWidth = spineView.GetComponent<RectTransform>().rect.width;

            spineGameObject.name = command.Name;

            var spineAnimation = spineGameObject.GetComponentInChildren<SkeletonAnimation>();
            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
            spineAnimation.PhysicsRotationInheritanceFactor = PhysicsFactor;

            spineAnimation.skeletonDataAsset = spineInfo.skeletonDataAsset;
            _SetAnimation(spineAnimation, command.Animation, command.IsLoop, command.Id);
            _SetSkin(spineAnimation, command.Skin, command.Id);
            _spineGameObjectDict.Add(command.Name, spineGameObject);

            var material = spineAnimation.skeletonDataAsset.atlasAssets[0].PrimaryMaterial;
            material.SetColor("_Color", Color.white);
            material.SetColor("_Black", Color.black);

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
            spineRT.localScale = new Vector3(command.Scale, command.Scale, 1);
            spineAnimation.GetComponent<RectTransform>().localPosition = new Vector3(
                spineWidth * spineScale * (spineInfo.pivot.x - 0.5f),
                spineHeight * spineScale * (spineInfo.pivot.y - 0.5f),
                0
            );

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
                            command.Duration
                        )
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShowAnimationType.SlideFromLeft:
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;
                    var pos = spineRT.localPosition;
                    spineRT.localPosition = new Vector3(-parentWidth / 2, pos.y, pos.z);
                    spineRT
                        .DOLocalMoveX(pos.x, command.Duration)
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
                        .DOLocalMoveX(pos.x, command.Duration)
                        .OnComplete(() =>
                        {
                            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
                            onComplete?.Invoke();
                        });
                    break;
            }
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
                            command.Duration
                        )
                        .OnComplete(() => _RemoveSpineAnimation(command.Name, onComplete));
                    break;
                case HideAnimationType.SlideToLeft:
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;
                    goRT.DOLocalMoveX(
                            -spineView.GetComponent<RectTransform>().rect.width / 2,
                            command.Duration
                        )
                        .OnComplete(() =>
                        {
                            spineAnimation.PhysicsPositionInheritanceFactor = Vector2.one * PhysicsFactor;
                            _RemoveSpineAnimation(command.Name, onComplete);
                        });
                    break;
                case HideAnimationType.SlideToRight:
                    spineAnimation.PhysicsPositionInheritanceFactor = Vector2.zero;
                    goRT.DOLocalMoveX(
                            spineView.GetComponent<RectTransform>().rect.width / 2,
                            command.Duration
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
                    command.Duration
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
                    new Vector3(command.Scale, command.Scale, 1),
                    command.Duration
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
                .DOLocalJump(position, command.JumpPower, command.JumpCount, command.Duration)
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
            switch (command.ShakeType)
            {
                case ShakeType.Horizontal:
                    goRT.DOShakePosition(
                            command.Duration,
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
                            command.Duration,
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
                            command.Duration,
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