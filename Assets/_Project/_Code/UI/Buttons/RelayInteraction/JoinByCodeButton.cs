using System;
using Cysharp.Threading.Tasks;
using TankPlayground.Core.Network;
using TMPro;
using UnityEngine;
using Zenject;

namespace TankPlayground.UI
{
    public class JoinByCodeButton : CustomButton
    {
        [SerializeField] private TMP_InputField _joinCodeInput;
        [SerializeField] private TextMeshProUGUI _errorText;
        
        private INetworkClient _networkClient;
        private string _joinCode;

        [Inject]
        public void Construct(INetworkClient networkClient) => _networkClient = networkClient;
        
        private void Start()
        {
            IsInteractable = false;
            _joinCodeInput.onValueChanged.AddListener(value =>
            {
                _joinCode = value;
                IsInteractable = value.Length > 0;
            });
        }

        public override void OnClicked()
        {
            base.OnClicked();
            JoinClickAsync().Forget();
        }

        private async UniTaskVoid JoinClickAsync()
        {
            try
            {
                IsInteractable = false;
                bool joined = await _networkClient.TryJoinGameAsync(_joinCode);

                if (!joined)
                {
                    IsInteractable = true;
                    _errorText.gameObject.SetActive(true);
                }
            }
            catch (Exception e)
            {
                IsInteractable = true;
                _errorText.gameObject.SetActive(true);
                Debug.LogWarning($"Error while joining game: {e}");
            }
        }
    }
}