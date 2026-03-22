using System.Collections.Generic;
using UnityEngine;
using ZLinq;

namespace TankPlayground.UI
{
    public class UINavigationService : IUINavigationService
    {
        public void Initialize(List<Screen> screens)
        {
            foreach (Screen screen in screens)
            {
                foreach (ScreenNavigationButton navButton in screen.NavigationButtons)
                {
                    Screen screenToShow = screens
                        .AsValueEnumerable()
                        .FirstOrDefault(s => s.ScreenType == navButton.ScreenTypeToShow);
                    Screen screenToHide = screens
                        .AsValueEnumerable()
                        .FirstOrDefault(s => s.ScreenType == navButton.ScreenTypeToHide);
                    
                    navButton.Initialize(screenToShow, screenToHide);
                }
            }
            
            foreach (Screen screen in screens)
            {
                if (!screen.ShowAtStart)
                    continue;
                
                screen.Show();
            }
        }
    }
}