using System;
using UnityEngine;

namespace Game.Scripts.Foyer.Ui
{
    public abstract class Form : MonoBehaviour
    {
        protected virtual void Awake()
        {
            AddListeners();
        }
        
        protected abstract void AddListeners();

        protected abstract void RemoveListeners();

        protected virtual void OnDestroy()
        {
            RemoveListeners();
        }

        public abstract void SetInteractable(bool isInteractable);
    }
}