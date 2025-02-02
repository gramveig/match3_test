using System.Collections.Generic;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;

namespace Match3Test.Board.MatchLogic
{
    public abstract class MatchDetector
    {
        protected BoardSaveModel _boardModel;

        public MatchDetector(BoardSaveModel boardModel)
        {
            _boardModel = boardModel;
        }

        public abstract bool IsMatchesInLine(int lineIndex, ref Matches matches);

        public bool IsMatchesInLine(int lineIndex)
        {
            Matches matches = null;
            return IsMatchesInLine(lineIndex, ref matches);
        }

        public abstract bool IsMatches(ref Matches matches);

        protected bool IsMatch(int startX, int startY, int incX, int incY, Gem startGem, out List<Gem> matchingGems)
        {
            matchingGems = null;
            
            if (startGem == null
                || startGem.GemClass == GemClass.Special && startGem.GemSpecialType == GemSpecialType.Empty
            )
                return false;

            int tilesCounter = 0;
            bool isBombMatch = false;
            int boardWidth = _boardModel.Width;
            int boarHeight = _boardModel.Height;
            GemColor bombMatchColor = GemColor.Any;
            for (int x = startX, y = startY;
                 incX != 0 && (incX > 0 && x < boardWidth || incX < 0 && x >= 0) ||
                 incY != 0 && (incY > 0 && y < boarHeight || incY < 0 && y >= 0);
                 x += incX, y += incY
            )
            {
                Gem gem = _boardModel.GetGem(x, y);
                if (gem == null || gem.GemClass == GemClass.Special && gem.GemSpecialType == GemSpecialType.Empty) break;

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