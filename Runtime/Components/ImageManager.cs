using System;
using System.Collections.Generic;
using Again.Scripts.Runtime.Commands.Image;
using Again.Scripts.Runtime.Common;
using Again.Scripts.Runtime.Enums;
using DG.Tweening;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class ImageManager : MonoBehaviour
    {
        private const float ShakeFactor = 0.5f;
        public GameObject imageView;
        public GameObject imagePrefab;
        private readonly Dictionary<string, GameObject> _imageObjectDict = new();

        public void Reset()
        {
            foreach (var go in _imageObjectDict.Values) Destroy(go);
            _imageObjectDict.Clear();
        }

        public GameObject GetImageObject(string objectName)
        {
            _imageObjectDict.TryGetValue(objectName, out var go);
            return go;
        }

        public void Show(ShowImageCommand command, Action onComplete = null)
        {
            // find image by name in Resources/Images
            var sprite = Resources.Load<Sprite>($"Images/{command.ImageName}");
            if (sprite == null)
            {
                Debug.LogError($"Texture {command.ImageName} not found");
                onComplete?.Invoke();
                return;
            }

            // create new image object
            var go = Instantiate(imagePrefab, imageView.transform);
            var rt = go.GetComponent<RectTransform>();
            var parentWidth = imageView.GetComponent<RectTransform>().rect.width;
            var parentHeight = imageView.GetComponent<RectTransform>().rect.height;
            var spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
            go.transform.SetParent(imageView.transform, false);
            spriteRenderer.sprite = sprite;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
            rt.localScale = new Vector3(command.Scale, command.Scale, 1);
            rt.localPosition = new Vector3(
                command.PosX * parentWidth / 2,
                command.PosY * parentHeight / 2,
                0
            );

            _imageObjectDict.Add(command.Name, go);

            switch (command.ShowType)
            {
                case ShowAnimationType.None:
                    onComplete?.Invoke();
                    break;
                case ShowAnimationType.Fade:
                    spriteRenderer
                        .DOFade(1, command.Duration)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShowAnimationType.SlideFromLeft:
                    var localPosition = rt.localPosition;
                    rt.localPosition = new Vector3(-parentWidth / 2, localPosition.y, 0);
                    rt.DOLocalMoveX(localPosition.x, command.Duration)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShowAnimationType.SlideFromRight:
                    var localPosition1 = rt.localPosition;
                    rt.localPosition = new Vector3(parentWidth / 2, localPosition1.y, 0);
                    rt.DOLocalMoveX(localPosition1.x, command.Duration)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
            }
        }

        public void Change(ChangeImageCommand command)
        {
            _imageObjectDict.TryGetValue(command.Name, out var go);
            if (go == null)
            {
                Debug.LogError($"Sprite object {command.Name} not found");
                return;
            }

            var sprite = Resources.Load<Sprite>($"Images/{command.ImageName}");
            if (sprite == null)
            {
                Debug.LogError($"Sprite {command.ImageName} not found");
                return;
            }

            go.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        }

        public void Hide(HideImageCommand command, Action onComplete = null)
        {
            _imageObjectDict.TryGetValue(command.Name, out var go);
            if (go == null)
            {
                Debug.LogError($"Sprite object {command.Name} not found");
                onComplete?.Invoke();
                return;
            }

            _imageObjectDict.Remove(command.Name);
            var rt = go.GetComponent<RectTransform>();
            var spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
            switch (command.HideType)
            {
                case HideAnimationType.None:
                    Destroy(spriteRenderer.transform.parent.gameObject);
                    onComplete?.Invoke();
                    break;
                case HideAnimationType.Fade:
                    spriteRenderer
                        .DOFade(0, command.Duration)
                        .OnComplete(() =>
                        {
                            Destroy(spriteRenderer.transform.parent.gameObject);
                            onComplete?.Invoke();
                        });
                    break;
                case HideAnimationType.SlideToLeft:
                    rt.DOLocalMoveX(-rt.rect.width, command.Duration)
                        .OnComplete(() =>
                        {
                            Destroy(spriteRenderer.transform.parent.gameObject);
                            onComplete?.Invoke();
                        });
                    break;
                case HideAnimationType.SlideToRight:
                    rt.DOLocalMoveX(rt.rect.width, command.Duration)
                        .OnComplete(() =>
                        {
                            Destroy(spriteRenderer.transform.parent.gameObject);
                            onComplete?.Invoke();
                        });
                    break;
            }
        }

        public void Move(MoveImageCommand command, Action onComplete = null)
        {
            _imageObjectDict.TryGetValue(command.Name, out var go);
            if (go == null)
            {
                Debug.LogError($"Sprite object {command.Name} not found");
                onComplete?.Invoke();
                return;
            }

            var rt = go.GetComponent<RectTransform>();
            var parentWidth = imageView.GetComponent<RectTransform>().rect.width;
            var parentHeight = imageView.GetComponent<RectTransform>().rect.height;

            var targetX = command.PosX * parentWidth / 2;
            var targetY = command.PosY * parentHeight / 2;

            rt.DOLocalMove(new Vector3(targetX, targetY, 0), command.Duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Scale(ScaleImageCommand command, Action onComplete = null)
        {
            _imageObjectDict.TryGetValue(command.Name, out var go);
            if (go == null)
            {
                Debug.LogError($"Sprite object {command.Name} not found");
                onComplete?.Invoke();
                return;
            }

            var rt = go.GetComponent<RectTransform>();
            PivotTool.SetPivotInWorldSpace(rt, new Vector2(command.AnchorX, command.AnchorY));
            rt.pivot = new Vector2(command.AnchorX, command.AnchorY);
            rt.DOScale(new Vector3(command.Scale, command.Scale, 1), command.Duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Jump(JumpImageCommand command, Action onComplete = null)
        {
            _imageObjectDict.TryGetValue(command.Name, out var go);
            if (go == null)
            {
                Debug.LogError($"Sprite object {command.Name} not found");
                onComplete?.Invoke();
                return;
            }

            var rt = go.GetComponent<RectTransform>();
            var position = rt.localPosition;
            rt.DOLocalJump(position, command.JumpPower, command.JumpCount, command.Duration)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void Shake(ShakeImageCommand command, Action onComplete = null)
        {
            _imageObjectDict.TryGetValue(command.Name, out var go);
            if (go == null)
            {
                Debug.LogError($"Sprite object {command.Name} not found");
                onComplete?.Invoke();
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

        public void ChangeColor(ChangeImageColorCommand command)
        {
            _imageObjectDict.TryGetValue(command.Name, out var go);
            if (go == null)
            {
                Debug.LogError($"Sprite object {command.Name} not found");
                return;
            }

            if (command.ChangeColorType == ChangeColorType.Subtractive)
                go.GetComponentInChildren<SpriteRenderer>().color = command.ColorDelta;
            else if (command.ChangeColorType == ChangeColorType.None)
                go.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }
}