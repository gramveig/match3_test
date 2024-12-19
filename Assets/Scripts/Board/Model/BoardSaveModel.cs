using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Match3Test.Board.Model
{
    [Serializable]
    public class BoardSaveModel
    {
        [JsonProperty("Board")] private Gem[,] board;
        [JsonProperty("Width")] public int Width;
        [JsonProperty("Height")] public int Height;

        public BoardSaveModel(int width, int height)
        {
            Width = width;
            Height = height;
            board = new Gem[width, height];
        }
        
        public Gem this[int x, int y]
        {
            get => board[x, y];
            set => board[x, y] = value;
        }
    }
}