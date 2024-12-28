using System;
using System.IO;
using System.Text;
using Match3Test.Board.Model;
using Match3Test.Game.Settings;
using Match3Test.Saves;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardSaveProvider : BaseSaveProvider<BoardSaveModel>
    {
        private Array _colorEnumValues;
        private readonly GameSettings _gameSettings;

        private const int DefaultBoardWidth = 7;
        private const int DefaultBoardHeight = 7;
        private const string SavePath = "Assets/Models/model.txt";

        protected override string Key => "Board";

        public BoardSaveProvider(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }
        
        protected override BoardSaveModel DefaultSave() => new BoardSaveModel
        {
            Width = DefaultBoardWidth,
            Height = DefaultBoardHeight,
            Board = new Gem[DefaultBoardWidth, DefaultBoardHeight]
        };

        private BoardSaveModel GetSaveModel(int width, int height)
        {
            BoardSaveModel model = new BoardSaveModel
            {
                Width = width,
                Height = height,
                Board = new Gem[width, height]
            };

            return model;
        }

        public void SaveAsTextFile(BoardSaveModel board)
        {
            File.WriteAllText(SavePath, GetModelAsString(board));
            Debug.Log("File saved at: " + SavePath);
        }

        public void SaveEmptyBoardAsTextFile()
        {
            string emptyBoardString = GetEmptyModelAsString();
            File.WriteAllText(SavePath, emptyBoardString);
            Debug.Log("File saved at: " + SavePath);
        }

        public BoardSaveModel GetFromTextFile(TextAsset textFile)
        {
            string[] lines = textFile.text.Split('\n');
            int width = lines[0].Length;
            int height = lines.Length;
            var board = GetSaveModel(width, height);
            for (int y = 0; y < board.Height; y++)
            {
                string line = lines[lines.Length - y - 1];
                for (int x = 0; x < board.Width; x++)
                {
                    char c = line[x];
                    Gem gem;
                    if (c == '.')
                    {
                        GemView gemPrefab = _gameSettings.EmptyPrefab;
                        gem = new Gem(gemPrefab, x, y);
                    }
                    else
                    {
                        GemColor gemColor = GetGemColorByLetter(c);
                        if (char.IsLower(c))
                        {
                            GemView gemPrefab = _gameSettings.GetRegularGemPrefab(gemColor);
                            gem = new Gem(gemPrefab, x, y);
                        }
                        else
                        {
                            GemView gemPrefab = _gameSettings.GetBombPrefab(gemColor);
                            gem = new Gem(gemPrefab, x, y);
                        }
                    }

                    board[x, y] = gem;
                }
            }

            return board;
        }

        private string GetEmptyModelAsString()
        {
            var emptyBoard = DefaultSave();
            return GetModelAsString(emptyBoard);
        }

        private string GetModelAsString(BoardSaveModel board)
        {
            var strBuilder = new StringBuilder();
            for (int y = 0; y < board.Height; y++)
            {
                if (y > 0) strBuilder.Append("\n");
                for (int x = 0; x < board.Width; x++)
                {
                    Gem gem = board.GetGem(x, y);
                    if (gem == null || gem.GemClass == GemClass.Special && gem.GemSpecialType == GemSpecialType.Empty)
                    {
                        strBuilder.Append(".");
                        continue;
                    }

                    string colorLetter = gem.GemColor.ToString().Substring(0, 1);
                    if (gem.GemClass == GemClass.Common)
                        colorLetter = colorLetter.ToLower();

                    strBuilder.Append(colorLetter);
                }
            }

            return strBuilder.ToString();
        }

        private GemColor GetGemColorByLetter(char letter)
        {
            if (_colorEnumValues == null)
                _colorEnumValues = Enum.GetValues(typeof(GemColor));

            foreach (GemColor gemColor in _colorEnumValues)
            {
                if (gemColor == GemColor.Any) continue;

                string l = gemColor.ToString().Substring(0, 1).ToUpper();
                if (l == letter.ToString().ToUpper()) return gemColor;
            }

            return GemColor.Any;
        }
        
    }
}