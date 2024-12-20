using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private int boardWidth;
        [SerializeField] private int boardHeight;
        [SerializeField] private Transform gemsContainer;
        [SerializeField] private Transform bgTilesContainer;

        public static BoardController Instance;
        public BoardSaveProvider BoardSaveProvider { get; private set; }
        public BoardSaveModel Board { get; private set; }
        public int BoardWidth => boardWidth;
        public int BoardHeight => boardHeight;
        private GameController _gameController;
        private HorizontalMatchDetector _horizontalMatchDetector;
        private VerticalMatchDetector _verticalMatchDetector;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _gameController = GameController.Instance;
            BoardSaveProvider = new BoardSaveProvider(boardWidth, boardHeight);
            Board = BoardSaveProvider.Read();
            _horizontalMatchDetector = new HorizontalMatchDetector(this);
            _verticalMatchDetector = new VerticalMatchDetector(this);
            InitRandomBoard();
        }

        public Gem GetGem(int x, int y)
        {
            return Board[x, y];
        }

        public void InitRandomBoard()
        {
            for (int x = 0; x < boardWidth; x++)
                for (int y = 0; y < boardHeight; y++)
                    TrySetGem(x, y);
        }

        private void TrySetGem(int x, int y)
        {
            InstantiateBgTile(x, y);
            GemView gemPrefab = _gameController.GameSettings.GetRandomRegularGemPrefab();
            GemView gemView = InstantiateGem(gemPrefab, x, y);
            Board[x, y] = new Gem
            {
                GemClass = gemPrefab.GemClass,
                GemColor = gemPrefab.GemColor,
                GemSpecialType = gemPrefab.GemSpecialType,
                Pos = new Vector2Int(x, y),
                GemView = gemView
            };
        }

        private void InstantiateBgTile(int x, int y)
        {
            Instantiate(_gameController.GameSettings.BgTilePrefab, new Vector2(x, y), Quaternion.identity,
                bgTilesContainer);
        }

        private GemView InstantiateGem(GemView gemPrefab, int x, int y)
        {
            return Instantiate(gemPrefab, new Vector2(x, y), Quaternion.identity,
                gemsContainer).GetComponent<GemView>();
        }
    }
}