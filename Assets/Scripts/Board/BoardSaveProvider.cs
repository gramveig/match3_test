using Match3Test.Board.Model;
using Match3Test.Saves;

namespace Match3Test.Board
{
    public class BoardSaveProvider : BaseSaveProvider<BoardSaveModel>
    {
        private const int DefaultBoardWidth = 7;
        private const int DefaultBoardHeight = 7;
        
        protected override string Key => "Board";

        protected override BoardSaveModel DefaultSave() => new BoardSaveModel
        {
            Width = DefaultBoardWidth,
            Height = DefaultBoardHeight,
            Board = new Gem[DefaultBoardWidth, DefaultBoardHeight]
        };
    }
}