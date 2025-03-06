using System;
using Again.Runtime.Common;
using Again.Runtime.Components.Interfaces;
using Again.Runtime.Enums;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Again.Runtime.Components.Views
{
    public enum TextAnimationState
    {
        Wait,
        Playing,
        Complete
    }

    public class DialogueView : MonoBehaviour, IDialogueView
    {
        private const float SpeedUpScale = 5;
        public TMP_Text characterText;
        public TMP_Text dialogueText;
        public Button nextButton;
        public Button logButton;
        public Button autoButton;
        public Button skipButton;
        public float textSpeed = 10f;
        public int textSize = 50;
        public Sprite waitSprite;
        public Sprite nextSprite;
        public Image stateIcon;
        public GameObject visibleContainer;
        public GameObject characterContainer;

        [SerializeField] private InputActionAsset actionAsset;
        protected AudioSource _audioSource;
        protected float _completeTimer;

        protected Action _onComplete;
        private InputAction _speedUpAction;
        protected TweenerCore<string, string, StringOptions> _textAnim;
        protected TextAnimationState _textAnimationState;
        protected float _textSpeedScale = 1f;

        protected void Awake()
        {
            nextButton.onClick.AddListener(_OnClickNextButton);
            logButton.onClick.AddListener(() => AgainSystem.Instance.EventManager.Emit("ShowLog"));
            autoButton?.onClick.AddListener(_OnClickAutoButton);
            skipButton?.onClick.AddListener(_OnClickSkipButton);
            _speedUpAction = actionAsset.FindActionMap("Dialogue").FindAction("SpeedUpText");
            _speedUpAction.performed += OnTextSpeedUp;
            _speedUpAction.canceled += OnTextSpeedUpCanceled;
            _audioSource = GetComponent<AudioSource>();
            transform.ResetAndHide();
        }

        protected void Start()
        {
            var isAutoNext = AgainSystem.Instance.GetAutoNext();
            if (autoButton != null)
                autoButton.GetComponent<Image>().color = isAutoNext ? Color.white : Color.gray;
        }

        private void Update()
        {
            if (_textAnimationState == TextAnimationState.Complete)
                if (AgainSystem.Instance.GetAutoNext())
                {
                    _completeTimer += Time.deltaTime * _textSpeedScale;
                    if (_completeTimer >= 1 || AgainSystem.Instance.GetSkip())
                    {
                        _completeTimer = 0;
                        _textAnimationState = TextAnimationState.Wait;
                        _onComplete?.Invoke();
                    }
                }
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
            _onComplete = null;
        }

        public void ScaleText(float scale)
        {
            dialogueText.fontSize = (int)(textSize * scale);
        }

        public virtual void ShowText(string character, string text, bool isSkip, Action onComplete = null)
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
                .DOText(text, isSkip ? 0 : text.Length / textSpeed / _textSpeedScale)
                .OnComplete(() =>
                {
                    _completeTimer = 0;
                    stateIcon.sprite = nextSprite;
                    _textAnimationState = TextAnimationState.Complete;
                });
            stateIcon.sprite = waitSprite;
            _textAnimationState = TextAnimationState.Playing;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            AgainSystem.Instance.SetSkip(false);
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

        private void _OnClickAutoButton()
        {
            var isAutoNext = !AgainSystem.Instance.GetAutoNext();
            autoButton.GetComponent<Image>().color = isAutoNext ? Color.white : Color.gray;
            AgainSystem.Instance.SetAutoNext(isAutoNext);
        }

        private void _OnClickSkipButton()
        {
            var isSkipNext = !AgainSystem.Instance.GetSkip();
            skipButton.GetComponent<Image>().color = isSkipNext ? Color.white : Color.gray;
            AgainSystem.Instance.SetSkip(isSkipNext);
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
            AgainSystem.Instance.SetAutoNext(true);
        }

        private void OnTextSpeedUpCanceled(InputAction.CallbackContext obj)
        {
            if (_textAnim != null)
                _textAnim.timeScale = 1;
            _textSpeedScale = 1;
            AgainSystem.Instance.SetAutoNext(false);
        }
    }
}