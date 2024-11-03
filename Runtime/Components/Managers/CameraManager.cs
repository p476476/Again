using System;
using Again.Scripts.Runtime.Commands.Camera;
using Again.Scripts.Runtime.Common;
using Again.Scripts.Runtime.Enums;
using Again.Scripts.Runtime.SaveData;
using DG.Tweening;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class CameraManager : MonoBehaviour
    {
        public Camera avgCamera;
        private Vector3 _originalPosition;

        private void Awake()
        {
            _originalPosition = avgCamera.transform.position;
        }

        public void Reset()
        {
            if (avgCamera == null) return;
            avgCamera.transform.position = _originalPosition;
            avgCamera.enabled = false;
        }

        public void Load(string saveData)
        {
            var data = CameraManagerSaveData.FromJson(saveData);
            avgCamera.transform.position = data.Transform.position;
            avgCamera.transform.rotation = data.Transform.rotation;
            _originalPosition = data.OriginalPosition;
        }

        public string Save()
        {
            return CameraManagerSaveData.ToJson(_originalPosition, avgCamera.transform);
        }

        public void Shake(ShakeCameraCommand command, Action onComplete = null)
        {
            var cameraTransform = avgCamera.transform;
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
            if (avgCamera.orthographic)
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
            avgCamera.transform.DOMove(endPosition, duration).OnComplete(() => onComplete?.Invoke());
        }

        public void MoveBackCamera(MoveBackCameraCommand cameraCommand, Action onComplete = null)
        {
            //if is orthographic camera
            if (avgCamera.orthographic)
            {
                Debug.LogError("MoveBackCamera only works with perspective camera");
                onComplete?.Invoke();
                return;
            }

            avgCamera.transform.DOMove(_originalPosition, cameraCommand.Duration)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}