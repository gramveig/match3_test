using System.Collections.Generic;
using Match3Test.Board.Model;

namespace Match3Test.Board.MatchLogic
{
    public class VerticalMatchDetector : MatchDetector
    {
        public VerticalMatchDetector(BoardController boardController) : base(boardController)
        {
        }
        
        public override bool IsMatchesInLine(int lineIdx, ref List<Match> matches)
        {
            int x = lineIdx;

            bool isMatches = false;
            for (var y = 0; y < _boardHeight;)
            {
                Gem startGem = _boardController.GetGem(x, y);
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

                    matches.Add(match);
                    y += matchingGems.Count;
                }
                else
                    y += 1;
            }

            return isMatches;
        }
        
        public override bool IsMatches(ref List<Match> matches)
        {
            bool isMatches = false;
            for (int x = 0; x < _boardWidth; x++)
            {
                bool isMatchesInColumn = IsMatchesInLine(x, ref matches);
                if (isMatchesInColumn) isMatches = true;
            }

            return isMatches;
        }
    }
}