using System.Collections.Generic;
using Match3Test.Board.Model;
using UnityEngine;

namespace Match3Test.Board.MatchLogic
{
    public class Match
    {
        public List<Gem> MatchingGems;
        public GemColor MatchColor;

        public bool IsBomb(Gem swipedGem, Gem otherGem, out Vector2Int pos)
        {
            const int MinPiecesForBomb = 4;

            pos = default;
            if (MatchingGems.Count < MinPiecesForBomb) return false;

            foreach (Gem matchingGem in MatchingGems)
            {
                if (matchingGem == swipedGem)
                {
                    pos = swipedGem.Pos;
                    return true;
                }

                if (matchingGem == otherGem)
                {
                    pos = otherGem.Pos;
                    return true;
                }
            }

            return false;
        }
    }
}