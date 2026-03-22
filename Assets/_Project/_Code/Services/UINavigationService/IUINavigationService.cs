using System.Collections.Generic;

namespace TankPlayground.UI
{
    public interface IUINavigationService
    {
        void Initialize(List<Screen> screens);
    }
}