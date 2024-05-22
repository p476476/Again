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
    }

    public class SpineManager : MonoBehaviour
    {
        private const float ShakeFactor = 0.5f;
        public GameObject spineGameObjectPrefab;
        public GameObject spineView;

        [SerializeField] public List<SpineInfo> spineInfos;

        private Dictionary<string, GameObject> _spineGameObjectDict;

        private void Awake()
        {
            _spineGameObjectDict = new Dictionary<string, GameObject>();
        }

        public GameObject GetSpineObject(string spineName)
        {
            return _spineGameObjectDict[spineName];
        }

        public void Show(ShowSpineCommand command, Action onComplete = null)
        {
            var spineGameObject = Instantiate(spineGameObjectPrefab, spineView.transform);
            var parentWidth = spineView.GetComponent<RectTransform>().rect.width;

            spineGameObject.name = command.SpineName;

            var spineAnimation = spineGameObject.GetComponentInChildren<SkeletonAnimation>();
            spineAnimation.skeletonDataAsset = spineInfos
                .Find(info => info.spineName == command.SpineName)
                .skeletonDataAsset;
            spineAnimation.AnimationState.SetAnimation(0, command.Animation, command.IsLoop);
            spineAnimation.skeleton.SetSkin(command.Skin);
            spineAnimation.skeleton.SetToSetupPose();
            _spineGameObjectDict.Add(command.SpineName, spineGameObject);

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
                0,
                -spineHeight * spineScale / 2,
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
                    var pos = spineRT.localPosition;
                    spineRT.localPosition = new Vector3(-parentWidth / 2, pos.y, pos.z);
                    spineRT
                        .DOLocalMoveX(pos.x, command.Duration)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShowAnimationType.SlideFromRight:
                    pos = spineRT.localPosition;
                    spineRT.localPosition = new Vector3(parentWidth / 2, pos.y, pos.z);
                    spineRT.transform
                        .DOLocalMoveX(pos.x, command.Duration)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
            }
        }

        public void Change(ChangeSpineCommand command)
        {
            var go = _spineGameObjectDict[command.SpineName];
            if (go == null)
            {
                Debug.LogError("Spine not found: " + command.SpineName);
                return;
            }

            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();
            if (!string.IsNullOrEmpty(command.Animation))
                try
                {
                    spineAnimation.AnimationState.SetAnimation(0, command.Animation, command.IsLoop);
                }
                catch (Exception)
                {
                    Debug.LogError($"Line {command.Id} 找不到Animation: {command.Animation}");
                }

            if (!string.IsNullOrEmpty(command.Skin))
                try
                {
                    spineAnimation.skeleton.SetSkin(command.Skin);
                    spineAnimation.skeleton.SetToSetupPose();
                }
                catch (Exception)
                {
                    Debug.LogError($"Line {command.Id} 找不到Skin: {command.Skin}");
                }

            spineAnimation.ApplyAnimation();
        }

        public void Hide(HideSpineCommand command, Action onComplete = null)
        {
            var go = _spineGameObjectDict[command.SpineName];
            if (go == null)
            {
                Debug.LogError("Spine not found: " + command.SpineName);
                return;
            }

            var spineAnimation = go.GetComponentInChildren<SkeletonAnimation>();
            var goRT = go.GetComponent<RectTransform>();
            switch (command.HideType)
            {
                case HideAnimationType.None:
                    Destroy(spineAnimation.gameObject);
                    _spineGameObjectDict.Remove(command.SpineName);
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
                        .OnComplete(() => _RemoveSpineAnimation(command.SpineName, onComplete));
                    break;
                case HideAnimationType.SlideToLeft:
                    goRT.DOLocalMoveX(
                            -spineView.GetComponent<RectTransform>().rect.width / 2,
                            command.Duration
                        )
                        .OnComplete(() => _RemoveSpineAnimation(command.SpineName, onComplete));
                    break;
                case HideAnimationType.SlideToRight:
                    goRT.DOLocalMoveX(
                            spineView.GetComponent<RectTransform>().rect.width / 2,
                            command.Duration
                        )
                        .OnComplete(() => _RemoveSpineAnimation(command.SpineName, onComplete));
                    break;
            }
        }

        public void Move(MoveSpineCommand command, Action onComplete = null)
        {
            var go = _spineGameObjectDict[command.SpineName];
            if (go == null)
            {
                Debug.LogError("Spine not found: " + command.SpineName);
                return;
            }

            var parentWidth = spineView.GetComponent<RectTransform>().rect.width;
            go.GetComponent<RectTransform>()
                .DOLocalMove(
                    new Vector3(command.PosX * parentWidth / 2, command.PosY * parentWidth / 2, 0),
                    command.Duration
                )
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Scale(ScaleSpineCommand command, Action onComplete = null)
        {
            var go = _spineGameObjectDict[command.SpineName];
            if (go == null)
            {
                Debug.LogError("Spine not found: " + command.SpineName);
                return;
            }

            var goRT = go.GetComponent<RectTransform>();
            PivotTool.SetPivotInWorldSpace(goRT, new Vector2(command.AnchorX, command.AnchorY));

            goRT.DOScale(
                    new Vector3(command.Scale, command.Scale, 1),
                    command.Duration
                )
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Jump(JumpSpineCommand command, Action onComplete = null)
        {
            var go = _spineGameObjectDict[command.SpineName];
            if (go == null)
            {
                Debug.LogError("Spine not found: " + command.SpineName);
                return;
            }

            var position = go.GetComponent<RectTransform>().localPosition;
            go.GetComponent<RectTransform>()
                .DOLocalJump(position, command.JumpPower, command.JumpCount, command.Duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Shake(ShakeSpineCommand command, Action onComplete = null)
        {
            var go = _spineGameObjectDict[command.SpineName];
            if (go == null)
            {
                Debug.LogError("Spine not found: " + command.SpineName);
                return;
            }

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
            var go = _spineGameObjectDict[command.SpineName];
            if (go == null)
            {
                Debug.LogError("Spine not found: " + command.SpineName);
                return;
            }

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
    }
}