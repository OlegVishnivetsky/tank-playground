using System;
using TankPlayground.UI;
using UnityEngine.AddressableAssets;

namespace TankPlayground.Configs
{
    [Serializable]
    public class ScreenEntry
    {
        public UIScreenType ScreenType;
        public AssetReferenceGameObject Reference;
    }
}