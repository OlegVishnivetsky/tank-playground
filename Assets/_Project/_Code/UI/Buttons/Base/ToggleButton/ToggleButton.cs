using System.Collections.Generic;
using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TankPlayground.UI
{
    public class ToggleButton : CustomButton
    {
        [FoldoutGroup("Toggle Settings")]
        [SerializeField] protected bool StartingStatus = true;
        
        [FoldoutGroup("Toggle Settings")]
        [SerializeReference]
        private List<BaseToggleStrategy> _toggleStrategies = new();
        
        public bool IsEnabled { get; protected set; }
        
        private readonly Subject<bool> _stateChangedSubject = new();
        private readonly Subject<bool> _startingStateInitializedSubject = new();
        
        public Observable<bool> StateChangedObservable => _stateChangedSubject;
        public Observable<bool> StartingStateInitializedObservable => _stateChangedSubject;

        protected virtual void Start()
        {
            foreach (BaseToggleStrategy toggleStrategy in _toggleStrategies)
                toggleStrategy.Initialize(this);

            HandleStartingState();
            UpdateVisualState(IsEnabled);
            _startingStateInitializedSubject.OnNext(true);
        }

        public override void OnClicked()
        {
            base.OnClicked();
            
            SetState(!IsEnabled);
            _stateChangedSubject.OnNext(IsEnabled);
        }
        
        public void SetState(bool enabled)
        {
            IsEnabled = enabled;
            UpdateVisualState(IsEnabled);
            OnToggleStateChanged(IsEnabled);
        }

        public void SetStateWithVisualOnly(bool enabled)
        {
            IsEnabled = enabled;
            UpdateVisualState(IsEnabled);
        }
        
        protected virtual void HandleStartingState() => IsEnabled = StartingStatus;

        protected void OnToggleStateChanged(bool enabled) { }

        private void UpdateVisualState(bool enabled)
        {
            foreach (BaseToggleStrategy toggleStrategy in _toggleStrategies)
                toggleStrategy.UpdateVisualState(enabled);
        }
    }
}