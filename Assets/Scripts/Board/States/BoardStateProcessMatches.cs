using Match3Test.Board.MatchLogic;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateProcessMatches : BoardState
    {
        public override BoardStateId Id => BoardStateId.ProcessMatches;

        public override void Start()
        {
            ProcessMatches();
        }
        
        private void ProcessMatches()
        {
            Matches matches = _boardController.Matches;
            
            Debug.Log($"Matches detected: {matches.MatchesCount}");
            if (matches.IsNonBombMatchingGems())
                _boardController.SetState(BoardStateId.DestroyMatchingNonBombGems);
            else
                _boardController.SetState(BoardStateId.ExplodeGemsAroundBombs);
        }
    }
}