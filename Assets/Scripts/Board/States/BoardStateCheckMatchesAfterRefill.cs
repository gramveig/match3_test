using Match3Test.Board.MatchLogic;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateCheckMatchesAfterRefill : BoardState
    {
        public override BoardStateId Id => BoardStateId.CheckMatchesAfterRefill;

        public override void Start()
        {
            CheckMatchesAfterRefill();
        }
        
        private void CheckMatchesAfterRefill()
        {
            Debug.Log("Looking for matches after refill...");
            Matches matches = _boardController.Matches;
            matches.Clear();
            bool isHorizontalMatches = _boardController.HorizontalMatchDetector.IsMatches(ref matches);
            bool isVerticalMatches = _boardController.VerticalMatchDetector.IsMatches(ref matches);
            if (isHorizontalMatches || isVerticalMatches)
            {
                Debug.Log("Matches count: " + matches.MatchesCount);
                matches.GetAutoBombs();
                _boardController.SetState(BoardStateId.ProcessMatches);
            }
            else
            {
                Debug.Log("No matches found.");
                _boardController.SetState(BoardStateId.WaitForSwipe);
            }
        }
    }
}