using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using Match3Test.Game.Settings;
using UnityEngine;
using Zenject;

namespace Match3Test.Board
{
    public class BoardStateRefillBoard : BoardState
    {
        private GameSettings _gameSettings;

        public override BoardStateId Id => BoardStateId.RefillBoard;

        [Inject]
        public void Construct(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        public override void Start()
        {
            RefillBoard();
        }
        
        private void RefillBoard()
        {
            _boardAnimator.StartNewAnimationSequence(AnimationType.MoveGems);
            BoardSaveModel board = _boardController.Board;
            float dropHeight = _gameSettings.GemDropHeight;
            _boardController.RefillGems.Clear();
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    Gem gem = board[x, y];
                    if (gem == null)
                    {
                        _boardController.TrySetGem(x, y);
                        //_boardController.SetGemOfSpecifiedColor(x, y, GemColor.Red);

                        gem = board[x, y];
                        gem.GemView.transform.position = new Vector2(gem.Pos.x, gem.Pos.y + dropHeight);
                        _boardAnimator.AddGemToAnimationSequence(gem, x, AnimationType.MoveGems);
                        _boardController.RefillGems.Add(gem);
                    }
                }
            }

            if (_boardAnimator.IsGemsInAnimation(AnimationType.MoveGems))
                _boardAnimator.AnimateGemsInSequence(OnAnimationEnd, AnimationType.MoveGems);
            else
            {
                Debug.LogWarning("No gems for refill found");
                _boardController.SetState(BoardStateId.CheckMatchesAfterRefill);
            }
        }

        private void OnAnimationEnd()
        {
            _boardController.SetState(BoardStateId.ShakeAfterRefill);
        }
    }
}