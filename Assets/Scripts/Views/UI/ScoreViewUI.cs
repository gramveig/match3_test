using System.Collections;
using Match3Test.Game;
using Match3Test.Game.Settings;
using TMPro;
using UnityEngine;
using Zenject;

namespace Match3Test.Views.UI
{
    public class ScoreViewUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreField;

        private GameController _gameController;
        private GameSettings _gameSettings;
        private float _displayScore;
        private float _newScore;

        [Inject]
        public void Constructor(GameController gameController, GameSettings gameSettings)
        {
            _gameController = gameController;
            _gameSettings = gameSettings;
        }
        
        private void OnEnable()
        {
            StartCoroutine(Subscribe());
        }

        private IEnumerator Subscribe()
        {
            yield return null;
            _gameController.OnScoreChanged += OnScoreChanged;
        }

        private void OnDisable()
        {
            _gameController.OnScoreChanged -= OnScoreChanged;
        }

        private void Update()
        {
            float scoreSpeed = _gameSettings.ScoreSpeed;
            _displayScore = Mathf.Lerp(_displayScore, _newScore, scoreSpeed * Time.deltaTime);
            scoreField.text = Mathf.RoundToInt(_displayScore).ToString("0");
        }
        
        private void OnScoreChanged(int newScore)
        {
            _newScore = newScore;
        }
    }
}