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
        [NonSerialized] public GemView GemPrefab; 
        [NonSerialized] public GemView GemView;

        public Gem(GemView gemPrefab, int x, int y)
        {
            GemClass = gemPrefab.GemClass;
            GemColor = gemPrefab.GemColor;
            GemSpecialType = gemPrefab.GemSpecialType;
            Pos = new Vector2Int(x, y);
            GemPrefab = gemPrefab;
        }
    }
}