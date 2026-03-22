using System.Collections.Generic;
using TankPlayground.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZLinq;

namespace TankPlayground.Configs
{
    [CreateAssetMenu(fileName = "UIScreen Config", menuName = "Configs/UI Screen Config")]
    public class UIScreenConfig : ScriptableObject
    {
        [SerializeField] private List<ScreenEntry> _screens = new();

        public List<AssetReferenceGameObject> GetAll(params UIScreenType[] types) =>
            types
                .AsValueEnumerable()
                .Select(Get)
                .Where(r => r != null)
                .ToList();

        public AssetReferenceGameObject Get(UIScreenType screenType) =>
            _screens
                .AsValueEnumerable()
                .FirstOrDefault(s => s.ScreenType == screenType)
                .Reference;
    }
}