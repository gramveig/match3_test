using System.Collections.Generic;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;

namespace Match3Test.Board.MatchLogic
{
    public abstract class MatchDetector
    {
        protected BoardController _boardController;
        protected int _boardWidth;
        protected int _boardHeight;

        public MatchDetector(BoardController boardController)
        {
            _boardController = boardController;
            _boardWidth = boardController.BoardWidth;
            _boardHeight = boardController.BoardHeight;
        }

        public abstract bool IsMatchesInLine(int lineIndex, ref List<Match> matches);

        public bool IsMatchesInLine(int lineIndex)
        {
            List<Match> matches = null;
            return IsMatchesInLine(lineIndex, ref matches);
        }
        
        protected bool IsMatch(int startX, int startY, int incX, int incY, Gem startGem, out List<Gem> matchingGems)
        {
            matchingGems = null;
            
            if (startGem == null) return false;

            for (int x = startX, y = startY;
                 incX != 0 && (incX > 0 && x < _boardWidth || incX < 0 && x >= 0) ||
                 incY != 0 && (incY > 0 && y < _boardHeight || incY < 0 && y >= 0);
                 x += incX, y += incY
            )
            {
                Gem gem = _boardController.GetGem(x, y);
                if (   gem == null
                    || gem.GemColor != startGem.GemColor
                )
                    break;

                if (matchingGems == null)
                    matchingGems = new List<Gem>();

                matchingGems.Add(gem);
            }

            return matchingGems != null && matchingGems.Count >= GameSettings.MinMatchingRegularTiles - 1;
        }
    }
}