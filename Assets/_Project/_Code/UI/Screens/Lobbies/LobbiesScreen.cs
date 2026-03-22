using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Zenject;

namespace TankPlayground.UI
{
    public class LobbiesScreen : Screen
    {
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private LobbySelectionItem _selectionItemPrefab;
        [SerializeField] private CustomButton _refreshButton;
        
        private DiContainer _container;
        private bool _isRefreshing;

        [Inject]
        public void Construct(DiContainer container) => _container = container;

        protected override void Start()
        {
            base.Start();
            
            _refreshButton.ClickedObservable
                .Subscribe(_ => RefreshLobbyList().Forget())
                .AddTo(this);

            RefreshLobbyList().Forget();
        }
        
        private async UniTaskVoid RefreshLobbyList()
        {
            if (_isRefreshing)
                return;
            
            _isRefreshing = true;
            _refreshButton.IsInteractable = false;
            _loadingScreen.SetActive(true);
            
            QueryLobbiesOptions options = new()
            {
                Count = 25,
                Filters = new()
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new QueryFilter(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ),
                }
            };
            
            QueryResponse queryResult = await Lobbies.Instance.QueryLobbiesAsync(options);
            _isRefreshing = false;
            _refreshButton.IsInteractable = true;
            _loadingScreen.SetActive(false);
            CreateLobbySelectionItems(queryResult.Results);
        }
        
        private void CreateLobbySelectionItems(List<Lobby> lobbies)
        {
            foreach (Transform child in _parentTransform)
                Destroy(child.gameObject);
            
            foreach (Lobby lobby in lobbies)
            {
                LobbySelectionItem itemInstance = _container.InstantiatePrefabForComponent<LobbySelectionItem>(
                    _selectionItemPrefab, _parentTransform);
                itemInstance.Initialize(lobby);
            }
        }
    }
}