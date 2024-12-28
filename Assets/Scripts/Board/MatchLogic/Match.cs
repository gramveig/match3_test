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
        public bool IsNewBomb(Gem swipedGem, Gem otherGem, out Vector2Int pos)
        {
            pos = default;
            if (MatchingGems.Count < MinPiecesForBombCount) return false;

            foreach (Gem matchingGem in MatchingGems)
            {
                //the bomb should form at the coordinates of one of gems that the player has swiped
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

        //detects the bomb formation in the automatic mode when the board is refilled
        public bool IsNewBomb(out Vector2Int pos)
        {
            pos = default;
            if (MatchingGems.Count < MinPiecesForBombCount) return false;

            //this will place the bomb either at the left or the bottom of the match,
            //as we loop through the gems in this order when looking for a match
            pos = MatchingGems[0].Pos;
            return true;
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