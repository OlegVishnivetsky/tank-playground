using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TankPlayground.UI
{
    public class Screen : AnimatableUI
    {
        [FoldoutGroup("Screen Settings")]
        [SerializeField] private UIScreenType _screenType;
        [FoldoutGroup("Screen Settings")]
        [SerializeField] private float _itemAppearInterval = 0.1f;
        [FoldoutGroup("Screen Settings")]
        [SerializeField] private float _initialDelay;
        [FoldoutGroup("Screen Settings")]
        [SerializeField] private bool _showAtStart;
        [FoldoutGroup("Screen Settings")]
        [SerializeField] private bool _setItemsManually = true;
        [FoldoutGroup("Screen Settings")]
        [SerializeField] private List<ScreenItem> _screenItems = new();
        [FoldoutGroup("Screen Settings")]
        [SerializeField] private List<ScreenNavigationButton> _navigationButtons = new();
        
        public UIScreenType ScreenType => _screenType;
        public bool ShowAtStart => _showAtStart;
        public IReadOnlyList<ScreenNavigationButton> NavigationButtons => _navigationButtons;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (_setItemsManually)
                return;

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out ScreenItem item))
                    _screenItems.Add(item);
            }
        }

        [Button]
        public override void Show(float delay = 0f)
        {
            base.Show(delay);
            
            Sequence itemsSequence = DOTween.Sequence();
            itemsSequence.SetDelay(delay);
            
            for (int i = 0; i < _screenItems.Count; i++)
            {
                if (_screenItems[i] == null) 
                    continue;
                
                int index = i;
                
                itemsSequence.AppendCallback(() => _screenItems[index].Show());
                
                if (i < _screenItems.Count - 1)
                    itemsSequence.AppendInterval(_itemAppearInterval);
            }
            
            itemsSequence.Play();
        }
        
        [Button]
        public override void Hide(float delay = 0f)
        {
            base.Hide(delay);
            
            Sequence itemsSequence = DOTween.Sequence();
            itemsSequence.SetDelay(delay);
            
            for (int i = 0; i < _screenItems.Count; i++)
            {
                if (_screenItems[i] == null)
                    continue;
                
                int index = i; 
                itemsSequence.AppendCallback(() => _screenItems[index].Hide());
                
                if (i < _screenItems.Count - 1)
                    itemsSequence.AppendInterval(_itemAppearInterval);
            }
            
            itemsSequence.Play();
        }
    }
}