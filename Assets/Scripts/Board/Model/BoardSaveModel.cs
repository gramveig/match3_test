using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Match3Test.Board.Model
{
    [Serializable]
    public class BoardSaveModel
    {
        [JsonProperty("Board")] public Gem[,] Board;
        [JsonProperty("Width")] public int Width;
        [JsonProperty("Height")] public int Height;

        public Gem this[int x, int y]
        {
            get => Board[x, y];
            set => Board[x, y] = value;
        }

        public Gem GetGem(int x, int y)
        {
            if (x < 0 || x >=  Width || y < 0 || y >= Height) return null;
            
            return Board[x, y];
        }

        public void SetGemAtNewPos(Gem gem)
        {
            int x = gem.Pos.x;
            int y = gem.Pos.y;
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                Debug.LogError($"Position {x}, {y} is out of the board's dimensions");
                return;
            }

            Board[x, y] = gem;
        }
    }
}