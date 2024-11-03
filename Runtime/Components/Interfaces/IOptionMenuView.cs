using System;
using System.Collections.Generic;

namespace Again.Runtime.Components.Interfaces
{
    public interface IOptionMenuView
    {
        void Reset();
        void SetVisible(bool isVisible);
        void ShowOptions(List<string> options, Action<int> callback);
        void UpdateOptionTexts(List<string> options);
    }
}