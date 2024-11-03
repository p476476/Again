using System;
using Again.Scripts.Runtime.Common;
using Again.Scripts.Runtime.Enums;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Again.Scripts.Runtime.Components
{
    internal enum TextAnimationState
    {
        Wait,
        Playing,
        Complete
    }

    public class DialogueView : MonoBehaviour, IDialogueView
    {
        private const float SpeedUpScale = 5;
        public Text characterText;
        public Text dialogueText;
        public Button nextButton;
        public float textSpeed = 0.01f;
        public int textSize = 50;
        public bool isAutoNext;
        public Sprite waitSprite;
        public Sprite nextSprite;
        public Image stateIcon;
        public GameObject visibleContainer;
        public GameObject characterContainer;

        [SerializeField] private InputActionAsset actionAsset;
        private AudioSource _audioSource;

        private Action _onComplete;
        private InputAction _speedUpAction;
        private TweenerCore<string, string, StringOptions> _textAnim;
        private TextAnimationState _textAnimationState;
        private float _textSpeedScale = 1f;

        private void Awake()
        {
            nextButton.onClick.AddListener(_OnClickNextButton);
            _speedUpAction = actionAsset.FindActionMap("Dialogue").FindAction("SpeedUpText");
            _speedUpAction.performed += OnTextSpeedUp;
            _speedUpAction.canceled += OnTextSpeedUpCanceled;
            _audioSource = GetComponent<AudioSource>();
            transform.ResetAndHide();
        }

        public void OnEnable()
        {
            _speedUpAction.Enable();
        }

        public void OnDisable()
        {
            _speedUpAction.Disable();
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            _textAnim?.Kill();
            characterText.text = "";
            dialogueText.text = "";
            stateIcon.sprite = waitSprite;
            _textAnimationState = TextAnimationState.Wait;
        }

        public void ScaleText(float scale)
        {
            dialogueText.fontSize = (int)(textSize * scale);
        }

        public void ShowText(string character, string text, Action onComplete = null)
        {
            gameObject.SetActive(true);

            if (_textAnim != null)
                _textAnim.Kill();

            _onComplete = onComplete;
            characterText.text = character;
            characterContainer.SetActive(!string.IsNullOrEmpty(character));
            dialogueText.text = "";
            if (_audioSource.gameObject.activeSelf)
                _audioSource.Play();
            _textAnim = dialogueText
                .DOText(text, text.Length * textSpeed / _textSpeedScale)
                .OnComplete(() =>
                {
                    stateIcon.sprite = nextSprite;
                    _textAnimationState = TextAnimationState.Complete;
                    if (isAutoNext || _textSpeedScale > 1)
                    {
                        _textAnimationState = TextAnimationState.Wait;
                        _onComplete?.Invoke();
                    }
                });
            stateIcon.sprite = waitSprite;
            _textAnimationState = TextAnimationState.Playing;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetVisible(bool isVisible)
        {
            visibleContainer.SetActive(isVisible);
        }

        public void SetCharacterAndText(string character, string text)
        {
            if (!gameObject.activeSelf)
                return;

            if (_textAnim != null)
                _textAnim.Kill(true);

            characterContainer.SetActive(!string.IsNullOrEmpty(character));
            characterText.text = character;
            dialogueText.text = text;
        }

        public void Shake(
            float duration,
            float strength,
            int vibrato,
            float randomness,
            bool snapping,
            bool fadeOut,
            ShakeType shakeType,
            Action onComplete = null
        )
        {
            switch (shakeType)
            {
                case ShakeType.Horizontal:
                    transform
                        .DOShakePosition(
                            duration,
                            Vector3.right * strength,
                            vibrato,
                            randomness,
                            snapping,
                            fadeOut
                        )
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShakeType.Vertical:
                    transform
                        .DOShakePosition(
                            duration,
                            Vector3.up * strength,
                            vibrato,
                            randomness,
                            snapping,
                            fadeOut
                        )
                        .OnComplete(() => onComplete?.Invoke());
                    break;
                case ShakeType.HorizontalAndVertical:
                    transform
                        .DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut)
                        .OnComplete(() => onComplete?.Invoke());
                    break;
            }
        }

        public void SpeedUpText()
        {
            if (_textAnim != null)
                _textAnim.timeScale = 10;
        }

        private void _OnClickNextButton()
        {
            if (_textAnimationState == TextAnimationState.Complete)
            {
                _textAnimationState = TextAnimationState.Wait;
                _onComplete?.Invoke();
            }
            else if (_textAnimationState == TextAnimationState.Playing)
            {
                _textAnim.Complete();
            }
        }

        private void OnTextSpeedUp(InputAction.CallbackContext obj)
        {
            if (_textAnim != null)
                _textAnim.timeScale = SpeedUpScale;
            _textSpeedScale = SpeedUpScale;

            if (_textAnimationState == TextAnimationState.Complete)
            {
                _textAnimationState = TextAnimationState.Wait;
                _onComplete?.Invoke();
            }
        }

        private void OnTextSpeedUpCanceled(InputAction.CallbackContext obj)
        {
            if (_textAnim != null)
                _textAnim.timeScale = 1;
            _textSpeedScale = 1;
        }
    }
}