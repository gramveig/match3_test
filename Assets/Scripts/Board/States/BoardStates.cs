using Match3Test.Untility;

namespace Match3Test.Board
{
    public class BoardStates
    {
        public BoardState GetBoardState(BoardStateId boardStateId)
        {
            BoardState boardState = boardStateId switch
            {
                BoardStateId.WaitForSwap => new BoardStateWaitForSwap()
            };
            
            SceneDiContainer.Container.Inject(boardState);

            return boardState;
        }
    }
}