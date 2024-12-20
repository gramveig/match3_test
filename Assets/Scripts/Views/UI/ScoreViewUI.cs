using System.Collections;
using Match3Test.Game;
using TMPro;
using UnityEngine;

namespace Match3Test.Views.UI
{
    public class ScoreViewUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreField;

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

        private void OnScoreChanged(int newScore)
        {
            _scoreField.text = newScore.ToString();
        }
    }
}