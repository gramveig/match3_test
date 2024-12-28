using System.Collections.Generic;
using Match3Test.Board.Model;

namespace Match3Test.Board.MatchLogic
{
    public class VerticalMatchDetector : MatchDetector
    {
        public VerticalMatchDetector(BoardSaveModel boardSaveModel) : base(boardSaveModel)
        {
        }
        
        public override bool IsMatchesInLine(int lineIdx, ref Matches matches)
        {
            int x = lineIdx;

            bool isMatches = false;
            int boardHeight = _boardModel.Height;
            for (var y = 0; y < boardHeight;)
            {
                Gem startGem = _boardModel.GetGem(x, y);
                if (startGem == null)
                {
                    y += 1;
                    continue;
                }

                bool isMatch = IsMatch(x, y + 1, 0, 1, startGem, out List<Gem> matchingGems);
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
                    y += matchingGems.Count;
                }
                else
                    y += 1;
            }

            return isMatches;
        }
        
        public override bool IsMatches(ref Matches matches)
        {
            bool isMatches = false;
            int boardWidth = _boardModel.Width;
            for (int x = 0; x < boardWidth; x++)
            {
                bool isMatchesInColumn = IsMatchesInLine(x, ref matches);
                if (isMatchesInColumn) isMatches = true;
            }

            return isMatches;
        }
    }
}