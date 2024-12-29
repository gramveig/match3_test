using Match3Test.Board.Model;
using UnityEngine;

namespace Match3Test.Board
{
    public class SwipeData
    {
        public Gem SwipedGem;
        public Gem OtherGem;
        public Direction SwipeDirection;
        public Vector2Int NewGemPos;
        public Vector2Int NewOtherGemPos;
    }
}