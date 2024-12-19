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
    }
}