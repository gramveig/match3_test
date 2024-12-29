using System;
using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateSwipe : BoardState
    {
        private BoardSaveModel _board;

        public override BoardStateId Id => BoardStateId.Swipe;

        public override void Start()
        {
            _board = _boardController.Board;
            SwipeData swipeData = _boardController.SwipeData;
            if (swipeData == null)
            {
                Debug.LogError("Swipe data has to be saved before calling the state");
                _boardController.SetState(BoardStateId.WaitForSwipe);
            }
            
            SwipeGems(swipeData, OnAnimationComplete);
        }

        //private

        private void OnAnimationComplete()
        {
            _boardController.SetState(BoardStateId.CheckMatchesAfterSwipe);
        }

        private void SwipeGems(SwipeData swipeData, Action callback)
        {
            Gem swipedGem = swipeData.SwipedGem;
            Gem otherGem = swipeData.OtherGem;
            Direction swipeDirection = swipeData.SwipeDirection;

            Debug.Log($"Swiping {swipedGem.GemColor} gem with coordinate ({swipedGem.Pos.x}, {swipedGem.Pos.y}) {swipeDirection}");

            swipedGem.Pos = swipeData.NewGemPos;
            _board.SetGemAtNewPos(swipedGem);
            otherGem.Pos = swipeData.NewOtherGemPos;
            _board.SetGemAtNewPos(otherGem);

            _boardAnimator.AddGemToAnimation(swipedGem, AnimationType.MoveGems);
            _boardAnimator.AddGemToAnimation(otherGem, AnimationType.MoveGems);
            _boardAnimator.AnimateGemsInAnimation(callback, AnimationType.MoveGems);
        }
    }
}