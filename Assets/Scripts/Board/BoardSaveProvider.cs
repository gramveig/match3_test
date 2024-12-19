using Match3Test.Board.Model;
using Match3Test.Saves;

namespace Match3Test.Board
{
    public class BoardSaveProvider : BaseSaveProvider<BoardSaveModel>
    {
        private int _boardWidth;
        private int _boardHeight;
        
        public BoardSaveProvider(int boardWidth, int boardHeight) : base()
        {
            _boardWidth = boardWidth;
            _boardHeight = boardHeight;
        }
        
        protected override string Key => "Board";

        protected override BoardSaveModel DefaultSave() => new BoardSaveModel
        {
            Width = _boardWidth,
            Height = _boardHeight,
            Board = new Gem[_boardWidth, _boardHeight]
        };
    }
}