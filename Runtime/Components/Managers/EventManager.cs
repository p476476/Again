using System;
using System.Collections.Generic;

namespace Again.Runtime.Components.Managers
{
    public class EventManager
    {
        private readonly List<Delegate> _anyEventList = new();
        private readonly Dictionary<string, Delegate> _eventDict = new();

        public void OnAny(Action<string> action)
        {
            _anyEventList.Add(action);
        }

        public void OffAny(Action<string> action)
        {
            _anyEventList.Remove(action);
        }

        public void OnAny<T>(Action<string, T> action)
        {
            _anyEventList.Add(action);
        }

        public void OffAny<T>(Action<string, T> action)
        {
            _anyEventList.Remove(action);
        }

        public void On(string eventName, Action action)
        {
            if (_eventDict.ContainsKey(eventName))
                _eventDict[eventName] = Delegate.Combine(_eventDict[eventName], action);
            else
                _eventDict[eventName] = action;
        }

        public void On<T>(string eventName, Action<T> action)
        {
            if (_eventDict.ContainsKey(eventName))
                _eventDict[eventName] = Delegate.Combine(_eventDict[eventName], action);
            else
                _eventDict[eventName] = action;
        }

        public void Off(string eventName, Action action)
        {
            if (_eventDict.ContainsKey(eventName))
            {
                _eventDict[eventName] = Delegate.Remove(_eventDict[eventName], action);
                if (_eventDict[eventName] == null)
                    _eventDict.Remove(eventName);
            }
        }

        public void Off<T>(string eventName, Action<T> action)
        {
            if (_eventDict.ContainsKey(eventName))
            {
                _eventDict[eventName] = Delegate.Remove(_eventDict[eventName], action);
                if (_eventDict[eventName] == null)
                    _eventDict.Remove(eventName);
            }
        }

        public void Emit(string eventName)
        {
            if (_eventDict.TryGetValue(eventName, out var action))
                (action as Action)?.Invoke();

            foreach (var anyEvent in _anyEventList)
                (anyEvent as Action<string>)?.Invoke(eventName);
        }

        public void Emit<T>(string eventName, T param)
        {
            if (_eventDict.TryGetValue(eventName, out var action))
                (action as Action<T>)?.Invoke(param);

            foreach (var anyEvent in _anyEventList)
                (anyEvent as Action<string, T>)?.Invoke(eventName, param);
        }

        public void Reset()
        {
            _eventDict.Clear();
        }

        public bool HasEvent(string eventName)
        {
            return _eventDict.ContainsKey(eventName);
        }
    }
}