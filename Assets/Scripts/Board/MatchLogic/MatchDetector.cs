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

        public abstract bool IsMatches(ref List<Match> matches);

        protected bool IsMatch(int startX, int startY, int incX, int incY, Gem startGem, out List<Gem> matchingGems)
        {
            matchingGems = null;
            
            if (startGem == null) return false;

            int tilesCounter = 0;
            bool isBombMatch = false;
            GemColor bombMatchColor = GemColor.Any;
            for (int x = startX, y = startY;
                 incX != 0 && (incX > 0 && x < _boardWidth || incX < 0 && x >= 0) ||
                 incY != 0 && (incY > 0 && y < _boardHeight || incY < 0 && y >= 0);
                 x += incX, y += incY
            )
            {
                Gem gem = _boardController.GetGem(x, y);
                if (gem == null) break;

                if (   gem.GemClass == GemClass.Special
                    && gem.GemSpecialType == GemSpecialType.Bomb
                )
                {
                    if (   startGem.GemClass == GemClass.Special
                        && startGem.GemSpecialType == GemSpecialType.Bomb
                        && tilesCounter == 0
                       )
                    {
                        isBombMatch = true;
                        if (matchingGems == null)
                            matchingGems = new List<Gem>();

                        matchingGems.Add(gem);
                        tilesCounter++;
                        bombMatchColor = gem.GemColor;
                        continue;
                    }

                    if (isBombMatch && bombMatchColor != gem.GemColor) break;
                }

                if (gem.GemColor != startGem.GemColor) break;

                if (matchingGems == null)
                    matchingGems = new List<Gem>();

                matchingGems.Add(gem);
                tilesCounter++;
            }

            return isBombMatch //can match 1 bomb with 1 bomb
                   || matchingGems != null && matchingGems.Count >= GameSettings.MinMatchingRegularTiles - 1; //-1 because we're counting the start gem
        }
    }
}