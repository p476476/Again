using System;
using Again.Scripts.Runtime.Enums;

namespace Again.Scripts.Runtime.Components
{
    public interface IDialogueView
    {
        void Reset();
        void ShowText(string character, string text, Action callback);
        void Hide();
        void SetVisible(bool isVisible);
        void SetCharacterAndText(string getCharacterString, string getTextString);
        void ScaleText(float commandScale);

        void Shake(float commandDuration, float commandStrength, int commandVibrato, float commandRandomness,
            bool commandSnapping, bool commandFadeOut, ShakeType commandShakeType, Action onComplete);
    }
}