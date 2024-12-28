using System.Collections.Generic;
using Match3Test.Board.Model;

namespace Match3Test.Board.MatchLogic
{
    public class HorizontalMatchDetector : MatchDetector
    {
        public HorizontalMatchDetector(BoardSaveModel boardSaveModel) : base(boardSaveModel)
        {
        }

        public override bool IsMatchesInLine(int lineIdx, ref Matches matches)
        {
            int y = lineIdx;

            bool isMatches = false;
            int boardWidth = _boardModel.Width;
            for (var x = 0; x < boardWidth;)
            {
                Gem startGem = _boardModel.GetGem(x, y);
                if (startGem == null)
                {
                    x += 1;
                    continue;
                }

                bool isMatch = IsMatch(x + 1, y, 1, 0, startGem, out List<Gem> matchingGems);
                if (isMatch)
                {
                    if (matches == null) return true;

                    isMatches = true;
                    matchingGems.Insert(0, startGem); //have to add the start gem too
                    Match match = new Match
                    {
                        MatchingGems = matchingGems,
                        MatchColor = startGem.GemColor
                    };

                    matches.AddMatch(match);
                    x += matchingGems.Count;
                }
                else
                    x += 1;
            }

            return isMatches;
        }

        public override bool IsMatches(ref Matches matches)
        {
            bool isMatches = false;
            int boardHeight = _boardModel.Height;
            for (int y = 0; y < boardHeight; y++)
            {
                bool isMatchesInRow = IsMatchesInLine(y, ref matches);
                if (isMatchesInRow) isMatches = true;
            }

            return isMatches;
        }
    }
}