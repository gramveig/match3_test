using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;
using Match3Test.Utility;
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
        private GemView[] _randomizedGemPrefabs;

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

        //private
        
        private void InitRandomBoard()
        {
            //Random.seed = 42;

            Debug.Log("Generating random board");
            for (int x = 0; x < boardWidth; x++)
                for (int y = 0; y < boardHeight; y++)
                    TrySetGem(x, y);
        }

        private void TrySetGem(int x, int y)
        {
            GemView gemPrefab = _gameController.GameSettings.GetRandomRegularGemPrefab();
            Gem gem = new Gem(gemPrefab, x, y);
            Board[x, y] = gem;
            if (!(_horizontalMatchDetector.IsMatchesInLine(y) || _verticalMatchDetector.IsMatchesInLine(x)))
                InstantiateGem(gem);
            else
            {
                Board[x, y] = null;
                TrySetDifferentGems(x, y);
            }
        }

        private void TrySetDifferentGems(int x, int y)
        {
            if (_randomizedGemPrefabs == null)
                _randomizedGemPrefabs = _gameController.GameSettings.GetRandomizedGemPrefabs();
            else
                ArrayHelper.ShuffleArray(_randomizedGemPrefabs);

            foreach (GemView gemPrefab in _randomizedGemPrefabs)
            {
                Gem gem = new Gem(gemPrefab, x, y);
                Board[x, y] = gem;
                if (!(_horizontalMatchDetector.IsMatchesInLine(y) || _verticalMatchDetector.IsMatchesInLine(x)))
                {
                    InstantiateGem(gem);
                    return;
                }
            }

            Debug.LogError($"Unable to find non-matching gem for position {x}, {y}");
        }

        private void InstantiateGem(Gem gem)
        {
            InstantiateBgTile(gem.Pos);
            GemView gemView = InstantiateGem(gem.GemPrefab,gem.Pos);
            gem.GemView = gemView;
        }

        private void InstantiateBgTile(Vector2Int pos)
        {
            Instantiate(_gameController.GameSettings.BgTilePrefab, (Vector2)pos, Quaternion.identity,
                bgTilesContainer);
        }

        private GemView InstantiateGem(GemView gemPrefab, Vector2Int pos)
        {
            return Instantiate(gemPrefab, (Vector2)pos, Quaternion.identity,
                gemsContainer).GetComponent<GemView>();
        }
    }
}