using System.Collections.Generic;
using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Utility;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateCheckMatchesAfterSwipe : BoardState
    {
        public override BoardStateId Id => BoardStateId.CheckMatchesAfterSwipe;

        public override void Start()
        {
            SwipeData swipeData = _boardController.SwipeData;
            if (swipeData == null)
            {
                Debug.LogError("Swipe data must be set before calling the state");
                return;
            }
            
            bool isMatches = CheckForMatches();
            if (isMatches)
            {
                _boardController.Matches.GetNewBombs(new List<Gem> { swipeData.SwipedGem, swipeData.OtherGem });
                _boardController.SetState(BoardStateId.ProcessMatches);
            }
            else
                _boardController.SetState(BoardStateId.SwipeBack);
        }
        
        private bool CheckForMatches()
        {
            Matches matches = _boardController.Matches;
            matches.Clear();

            SwipeData swipeData = _boardController.SwipeData;
            Gem swipedGem = swipeData.SwipedGem;
            Gem otherGem = swipeData.OtherGem;

            if (AngleHelper.IsHorizontal(swipeData.SwipeDirection))
            {
                bool isHorizontalMatches = _boardController.HorizontalMatchDetector.IsMatchesInLine(swipedGem.Pos.y, ref matches);
                bool isVerticalMatches1 = _boardController.VerticalMatchDetector.IsMatchesInLine(swipedGem.Pos.x, ref matches);
                bool isVerticalMatches2 = _boardController.VerticalMatchDetector.IsMatchesInLine(otherGem.Pos.x, ref matches);
                if (isHorizontalMatches || isVerticalMatches1 || isVerticalMatches2) return true;
            }
            else
            {
                bool isVerticalMatches = _boardController.VerticalMatchDetector.IsMatchesInLine(swipedGem.Pos.x, ref matches);
                bool isHorizontalMatches1 = _boardController.HorizontalMatchDetector.IsMatchesInLine(swipedGem.Pos.y, ref matches);
                bool isHorizontalMatches2 = _boardController.HorizontalMatchDetector.IsMatchesInLine(otherGem.Pos.y, ref matches);
                if (isVerticalMatches || isHorizontalMatches1 || isHorizontalMatches2) return true;
            }

            return false;
        }
    }
}