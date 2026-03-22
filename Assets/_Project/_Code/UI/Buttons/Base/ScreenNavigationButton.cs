using UnityEngine;

namespace TankPlayground.UI
{
    public class ScreenNavigationButton : CustomButton
    {
        [SerializeField] private UIScreenType _showScreenType;
        [SerializeField] private UIScreenType _hideScreenType;
        
        private Screen _screenToShow;
        private Screen _screenToHide;
        
        public UIScreenType ScreenTypeToShow => _showScreenType;
        public UIScreenType ScreenTypeToHide => _hideScreenType;

        public void Initialize(Screen screenToShow, Screen screenToHide)
        {
            _screenToShow = screenToShow;
            _screenToHide = screenToHide;
        }

        public override void OnClicked()
        {
            base.OnClicked();
            
            _screenToShow?.Show();
            _screenToHide?.Hide();
        }
    }
}