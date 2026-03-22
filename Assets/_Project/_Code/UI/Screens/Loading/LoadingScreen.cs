using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankPlayground.UI
{
    public class LoadingScreen : Screen
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _progressText;
        
        private float _targetProgress;
        private float _currentProgress;
        
        public float FillAmount => _fillImage.fillAmount;
        
        private const float SmoothSpeed = 3f;

        private void Update()
        {
            if (Mathf.Approximately(_currentProgress, _targetProgress))
                return;
        
            _currentProgress = Mathf.MoveTowards(_currentProgress, _targetProgress, SmoothSpeed * Time.deltaTime);
            _fillImage.fillAmount = _currentProgress;
            _progressText.text = $"{Mathf.RoundToInt(_currentProgress * 100)}%";
        }

        public void SetProgress(float normalized) => _targetProgress = Mathf.Clamp01(normalized);

        public void SetStatus(string status) => _statusText.text = status;

        public void ResetAll()
        {
            _targetProgress = 0f;
            _currentProgress = 0f;
            _fillImage.fillAmount = 0f;
            _statusText.text = "";
            _progressText.text = "";
        }
    }
}