using System;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Board.Model
{
    [Serializable]
    public class Gem
    {
        public GemClass GemClass;
        public GemColor GemColor;
        public GemSpecialType GemSpecialType;
        public Vector2Int Pos;
        public GemView GemView;
    }
}