using System;

namespace Match3Test.Board.Model
{
    [Serializable]
    public class Gem
    {
        public GemClass GemClass;
        public GemColor GemColor;
        public GemSpecialType GemSpecialType;
    }
}