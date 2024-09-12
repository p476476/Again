using System;
using Again.Scripts.Runtime.Commands.Camera;
using Again.Scripts.Runtime.Common;
using Again.Scripts.Runtime.Enums;
using DG.Tweening;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class CameraManager : MonoBehaviour
    {
        private Camera _mainCamera;
        private Vector3 _originalPosition;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _originalPosition = _mainCamera.transform.position;
        }

        public void Reset()
        {
            _mainCamera.transform.position = _originalPosition;
        }

        public void Shake(ShakeCameraCommand command, Action onComplete = null)
        {
            var cameraTransform = _mainCamera.transform;
            switch (command.ShakeType)
            {
                case ShakeType.Horizontal:
                    cameraTransform.DOShakePosition(command.Duration, Vector3.right * command.Strength, command.Vibrato,
                            command.Randomness, command.Snapping, command.FadeOut)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShakeType.Vertical:
                    cameraTransform.DOShakePosition(command.Duration, Vector3.up * command.Strength, command.Vibrato,
                            command.Randomness, command.Snapping, command.FadeOut)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShakeType.HorizontalAndVertical:
                    cameraTransform.DOShakePosition(command.Duration, command.Strength,
                            command.Vibrato, command.Randomness, command.Snapping, command.FadeOut)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
            }
        }

        public void LookAtObject(GameObject target, float duration, float scale, Vector2 pivot,
            Action onComplete = null)
        {
            //if is orthographic camera
            if (_mainCamera.orthographic)
            {
                Debug.LogError("LookAtObject only works with perspective camera");
                onComplete?.Invoke();
                return;
            }

            PivotTool.SetPivotInWorldSpace(target.GetComponent<RectTransform>(), pivot);
            var position = target.transform.position;
            var originalDistanceZ = Mathf.Abs(_originalPosition.z - position.z);
            var offsetZ = originalDistanceZ - originalDistanceZ / scale;
            var endPosition = new Vector3(position.x, position.y, _originalPosition.z + offsetZ);
            _mainCamera.transform.DOMove(endPosition, duration).OnComplete(() => onComplete?.Invoke());
        }

        public void MoveBackCamera(MoveBackCameraCommand cameraCommand, Action onComplete = null)
        {
            //if is orthographic camera
            if (_mainCamera.orthographic)
            {
                Debug.LogError("MoveBackCamera only works with perspective camera");
                onComplete?.Invoke();
                return;
            }

            _mainCamera.transform.DOMove(_originalPosition, cameraCommand.Duration)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}