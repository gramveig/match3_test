using System;
using System.Collections;
using Match3Test.Board;
using Match3Test.Game.Settings;
using UnityEngine;

namespace Match3Test.Game
{
    public enum GameState
    {
        None,
        Starting,
        WaitForMove,
        Moving
    }
    
    public class GameController : MonoBehaviour
    {
        public GameState GameState { get; set; }

        public event Action<int> OnScoreChanged;

        private int _score;

        private void Start()
        {
            GameState = GameState.Starting;
            StartCoroutine(SetWaitForMoveState());
        }

        public void AddScore(int score)
        {
            if (score <= 0) return;

            _score += score;
            OnScoreChanged?.Invoke(_score);
        }

        private IEnumerator SetWaitForMoveState()
        {
            yield return null;
            GameState = GameState.WaitForMove;
        }
    }
}