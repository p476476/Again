using System;
using Again.Scripts.Runtime.Enums;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.UI;

namespace Again.Scripts.Runtime.Components
{
    internal enum TextAnimationState
    {
        Wait,
        Playing,
        Complete
    }

    public class DialogueView : MonoBehaviour
    {
        public Text characterText;
        public Text dialogueText;
        public Button nextButton;
        public float textSpeed = 0.1f;
        public int textSize = 50;
        public bool isAutoNext;
        public Sprite waitSprite;
        public Sprite nextSprite;
        public Image stateIcon;

        private UIContainer _container;
        private Action _onComplete;
        private TweenerCore<string, string, StringOptions> _textAnim;
        private TextAnimationState _textAnimationState;

        private void Awake()
        {
            _container = GetComponent<UIContainer>();
            nextButton.onClick.AddListener(_OnClickNextButton);
        }

        public void ScaleText(float scale)
        {
            dialogueText.fontSize = (int)(textSize * scale);
        }

        public void Show(string character, string text, Action onComplete = null)
        {
            if (_container.isHidden)
                _container.Show();

            if (_textAnim != null) _textAnim.Kill();

            _onComplete = onComplete;
            characterText.text = character;
            dialogueText.text = "";
            _textAnim = dialogueText
                .DOText(text, text.Length * textSpeed)
                .OnComplete(() =>
                {
                    stateIcon.sprite = nextSprite;
                    _textAnimationState = TextAnimationState.Complete;
                    if (isAutoNext)
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
            _container.Hide();
        }

        public void SetCharacterAndText(string character, string text)
        {
            if (_container.isHidden)
                return;

            if (_textAnim != null) _textAnim.Kill(true);
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
    }
}