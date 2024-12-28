using System.IO;
using System.Text;
using Match3Test.Board.Model;
using Match3Test.Saves;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardSaveProvider : BaseSaveProvider<BoardSaveModel>
    {
        private const int DefaultBoardWidth = 7;
        private const int DefaultBoardHeight = 7;
        private const string SavePath = "Assets/Models/model.txt";

        protected override string Key => "Board";

        protected override BoardSaveModel DefaultSave() => new BoardSaveModel
        {
            Width = DefaultBoardWidth,
            Height = DefaultBoardHeight,
            Board = new Gem[DefaultBoardWidth, DefaultBoardHeight]
        };

        public void SaveAsTextFile()
        {
            File.WriteAllText(SavePath, GetModelAsString());
            Debug.Log("File saved at: " + SavePath);
        }

        private string GetModelAsString()
        {
            var board = Read();
            var strBuilder = new StringBuilder();
            for (int y = 0; y < board.Height; y++)
            {
                if (y > 0) strBuilder.Append("\n");
                for (int x = 0; x < board.Width; x++)
                {
                    Gem gem = board.GetGem(x, y);
                    if (gem == null)
                    {
                        strBuilder.Append(".");
                        continue;
                    }

                    string colorLetter = gem.GemColor.ToString().Substring(0, 1);
                    if (gem.GemClass == GemClass.Common)
                        colorLetter = colorLetter.ToLower();

                    strBuilder.Append(".");
                }
            }

            return strBuilder.ToString();
        }
    }
}