using System;
using Match3Test.Untility;

namespace Match3Test.Board
{
    public class BoardStates
    {
        public BoardState GetBoardState(BoardStateId boardStateId)
        {
            BoardState boardState = boardStateId switch
            {
                BoardStateId.WaitForSwipe => new BoardStateWaitForSwipe(),
                BoardStateId.Swipe => new BoardStateSwipe(),
                BoardStateId.SwipeBack => new BoardStateSwipeBack(),
                BoardStateId.CheckMatchesAfterSwipe => new BoardStateCheckMatchesAfterSwipe(),
                BoardStateId.ProcessMatches => new BoardStateProcessMatches(),
                BoardStateId.DestroyMatchingNonBombGems => new BoardStateDestroyMatchingNonBombGems(),
                BoardStateId.ExplodeGemsAroundBombs => new BoardStateExplodeGemsAroundBombs(),
                BoardStateId.DestroyBombs => new BoardStateDestroyBombs(),
                BoardStateId.SpawnNewBombs => new BoardStateSpawnNewBombs(),
                BoardStateId.CompactGems => new BoardStateCompactGems(),
                BoardStateId.ShakeAfterCompact => new BoardStateShakeAfterCompact(),
                BoardStateId.RefillBoard => new BoardStateRefillBoard(),
                BoardStateId.ShakeAfterRefill => new BoardStateShakeAfterRefill(),
                BoardStateId.CheckMatchesAfterRefill => new BoardStateCheckMatchesAfterRefill(),
                _ => throw new Exception($"Unknown board state {boardStateId}")
            };
            
            SceneDiContainer.Container.Inject(boardState);

            return boardState;
        }
    }
}