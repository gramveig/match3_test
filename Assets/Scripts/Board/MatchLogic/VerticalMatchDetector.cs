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
                    matchingGems.Add(startGem);
                    Match match = new Match
                    {
                        MatchingGems = matchingGems
                    };

                    matches.Add(match);
                    y += matchingGems.Count;
                }
                else
                    y += 1;
            }

            return isMatches;
        }
    }
}