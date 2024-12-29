using System.Collections.Generic;
using Match3Test.Board.Model;
using UnityEngine;

namespace Match3Test.Board.MatchLogic
{
    public class Match
    {
        public List<Gem> MatchingGems;
        public GemColor MatchColor;

        const int MinPiecesForBombCount = 4;

        //detects the bomb formation when the gems are swiped manually
        public bool IsNewBomb(List<Gem> checkGems, out Vector2Int pos)
        {
            pos = default;
            if (MatchingGems.Count < MinPiecesForBombCount) return false;

            foreach (Gem matchingGem in MatchingGems)
            {
                if (checkGems.Contains(matchingGem))
                {
                    pos = matchingGem.Pos;
                    return true;
                }
            }

            return false;
        }

        public bool IsBombs()
        {
            foreach (Gem matchingGem in MatchingGems)
                if (matchingGem.GemClass == GemClass.Special && matchingGem.GemSpecialType == GemSpecialType.Bomb)
                    return true;

            return false;
        }
    }
}