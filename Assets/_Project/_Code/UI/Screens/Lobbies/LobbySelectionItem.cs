using R3;
using TankPlayground.Core.Network;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Zenject;

namespace TankPlayground.UI
{
    public class LobbySelectionItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _playerCountText;
        [SerializeField] private CustomButton _joinButton;

        private INetworkClient _networkClient;
        private Lobby _lobby;
        
        [Inject]
        public void Construct(INetworkClient networkClient) => _networkClient = networkClient;
        
        public void Initialize(Lobby lobby)
        {
            _lobby = lobby;
            _nameText.text = lobby.Name;
            _playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            _joinButton.ClickedObservable
                .Subscribe(_ => Join())
                .AddTo(this);
        }

        private void Join()
        {
            _networkClient.TryJoinPublicLobbyAsync(_lobby);
        }
    }
}