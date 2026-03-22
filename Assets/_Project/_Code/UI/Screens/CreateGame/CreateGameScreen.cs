using R3;
using TankPlayground.Core.Network;
using TMPro;
using UnityEngine;
using Zenject;

namespace TankPlayground.UI
{
    public class CreateGameScreen : Screen
    {
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private ToggleButton _isPublicToggle;
        [SerializeField] private TextMeshProUGUI _maxPlayersText;
        [SerializeField] private CustomButton _increaseMaxPlayersButton;
        [SerializeField] private CustomButton _decreaseMaxPlayersButton;
        [SerializeField] private CustomButton _createGameButton;

        private INetworkHost _networkHost;
        
        private int _maxPlayers = 5;
        private bool _isPrivate;
        
        private const int MinMaxPlayers = 2;
        private const int MaxMaxPlayers = 10;

        [Inject]
        public void Construct(INetworkHost networkHost) => _networkHost = networkHost;

        protected override void Start()
        {
            base.Start();

            UpdatePlayersCountText();
            
            _isPublicToggle.StateChangedObservable
                .Subscribe(isPrivate => _isPrivate = isPrivate)
                .AddTo(this);

            _increaseMaxPlayersButton.ClickedObservable
                .Subscribe(_ =>
                {
                    _maxPlayers = Mathf.Min(_maxPlayers + 1, MaxMaxPlayers);
                    UpdatePlayersCountText();
                })
                .AddTo(this);
            
            _decreaseMaxPlayersButton.ClickedObservable
                .Subscribe(_ =>
                {
                    _maxPlayers = Mathf.Max(_maxPlayers - 1, MinMaxPlayers);
                    UpdatePlayersCountText();
                })
                .AddTo(this);
            
            _createGameButton.ClickedObservable
                .Subscribe(_ => CreateGame())
                .AddTo(this);
        }

        private void CreateGame() => _networkHost.StartHostAsync(_nameInputField.text, _maxPlayers, _isPrivate);

        private void UpdatePlayersCountText() => _maxPlayersText.text = _maxPlayers.ToString();
    }
}