using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateSwipeBack : BoardState
    {
        private BoardSaveModel _board;
        private SwipeData _swipeData;
        
        public override BoardStateId Id => BoardStateId.SwipeBack;

        public override void Start()
        {
            _swipeData = _boardController.SwipeData;
            if (_swipeData == null)
            {
                Debug.LogError("Swipe data must be set before calling the state");
                return;
            }

            _board = _boardController.Board;
            
            SwipeBack();
        }
        
        private void SwipeBack()
        {
            Gem swipedGem = _swipeData.SwipedGem;
            Gem otherGem = _swipeData.OtherGem;
            
            Vector2Int swipedGemPos = swipedGem.Pos;
            Vector2Int otherGemPos = otherGem.Pos;
            swipedGem.Pos = otherGemPos;
            otherGem.Pos = swipedGemPos;
            _board.SetGemAtNewPos(swipedGem);
            _board.SetGemAtNewPos(otherGem);

            _boardAnimator.AddGemToAnimation(swipedGem, AnimationType.MoveGems);
            _boardAnimator.AddGemToAnimation(otherGem, AnimationType.MoveGems);
            _boardAnimator.AnimateGemsInAnimation(OnAnimationEnd, AnimationType.MoveGems);
        }

        private void OnAnimationEnd()
        {
            _boardController.SetState(BoardStateId.WaitForSwipe);
        }
    }
}