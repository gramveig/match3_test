using System.Collections;
using Match3Test.Game;
using TMPro;
using UnityEngine;

namespace Match3Test.Views.UI
{
    public class ScoreViewUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreField;

        private float _displayScore;
        private float _newScore;

        private void OnEnable()
        {
            StartCoroutine(Subscribe());
        }

        private IEnumerator Subscribe()
        {
            yield return null;
            GameController.Instance.OnScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            GameController.Instance.OnScoreChanged -= OnScoreChanged;
        }

        private void Update()
        {
            float scoreSpeed = GameController.Instance.GameSettings.ScoreSpeed;
            _displayScore = Mathf.Lerp(_displayScore, _newScore, scoreSpeed * Time.deltaTime);
            _scoreField.text = Mathf.RoundToInt(_displayScore).ToString("0");
        }
        
        private void OnScoreChanged(int newScore)
        {
            _newScore = newScore;
        }
    }
}