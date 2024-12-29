using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateCompactGems : BoardState
    {
        public override BoardStateId Id => BoardStateId.CompactGems;

        public override void Start()
        {
            CompactGems();
        }
        
        private void CompactGems()
        {
            BoardSaveModel board = _boardController.Board;
            _boardAnimator.StartNewAnimationSequence(AnimationType.MoveGems);
            for (int x = 0; x < board.Width; x++)
            {
                int nullCounter = 0;
                for (int y = 0; y < board.Height; y++)
                {
                    Gem gem = board[x, y];
                    if (gem == null)
                    {
                        nullCounter++;
                    }
                    else if (nullCounter > 0)
                    {
                        board[x, y] = null;
                        int newYPos = gem.Pos.y - nullCounter;
                        gem.Pos.y = newYPos;
                        board[x, newYPos] = gem;
                        _boardAnimator.AddGemToAnimationSequence(gem, x, AnimationType.MoveGems);
                    }
                }
            }

            if (_boardAnimator.IsGemsInAnimation(AnimationType.MoveGems))
                _boardAnimator.AnimateGemsInSequence(OnAnimationComplete, AnimationType.MoveGems);
            else
            {
                Debug.Log("No gems to compact found");
                _boardController.SetState(BoardStateId.RefillBoard);
            }
        }

        private void OnAnimationComplete()
        {
            _boardController.SetState(BoardStateId.ShakeAfterCompact);
        }
    }
}