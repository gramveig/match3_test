using System.Collections.Generic;
using Match3Test.Board.Model;

namespace Match3Test.Board.MatchLogic
{
    public class HorizontalMatchDetector : MatchDetector
    {
        public HorizontalMatchDetector(BoardController boardController) : base(boardController)
        {
        }

        public override bool IsMatchesInLine(int lineIdx, ref List<Match> matches)
        {
            int y = lineIdx;

            bool isMatches = false;
            for (var x = 0; x < _boardWidth;)
            {
                Gem startGem = _boardController.GetGem(x, y);
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
                        MatchingGems = matchingGems
                    };

                    matches.Add(match);
                    x += matchingGems.Count;
                }
                else
                    x += 1;
            }

            return isMatches;
        }
    }
}