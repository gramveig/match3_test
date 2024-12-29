using System.Collections;
using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;
using UnityEngine;
using Zenject;

namespace Match3Test.Board
{
    public class BoardStateDestroyBombs : BoardState
    {
        private GameController _gameController;
        private GameSettings _gameSettings;
        
        public override BoardStateId Id => BoardStateId.DestroyBombs;

        [Inject]
        public void Construct(GameController gameController, GameSettings gameSettings)
        {
            _gameController = gameController;
            _gameSettings = gameSettings;
        }
        
        public override void Start()
        {
            _boardController.StartCoroutine(WaitAndDestroyBombs());
        }

        private IEnumerator WaitAndDestroyBombs()
        {
            Debug.Log("Waiting to destroy the exploded bombs...");
            yield return new WaitForSeconds(_gameSettings.BombDestructionDelay);

            foreach (Gem bomb in _boardController.Bombs)
            {
                int scoreValue = bomb.GemView.ScoreValue;
                _boardController.Board[bomb.Pos.x, bomb.Pos.y] = null;
                _boardAnimator.AddGemToAnimation(bomb, AnimationType.DestroyGems);
                _gameController.AddScore(scoreValue);
            }

            Debug.Log("Destroying the exploded bombs.");
            _boardAnimator.AnimateGemsInAnimation(OnAnimationComplete, AnimationType.DestroyGems);
        }

        private void OnAnimationComplete()
        {
            _boardController.SetState(BoardStateId.SpawnNewBombs);
        }
    }
}