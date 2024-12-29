using System;
using System.Collections.Generic;
using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Game.Settings;
using Match3Test.Utility;
using Match3Test.Views.Gems;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Match3Test.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private Transform gemsContainer;
        [SerializeField] private Transform bgTilesContainer;
        [SerializeField] private TextAsset startBoard;
        [SerializeField] private bool useStartBoard;

        private GameSettings _gameSettings;
        private BoardSaveModel _board;
        private BoardSaveProvider _boardSaveProvider;
        private HorizontalMatchDetector _horizontalMatchDetector;
        private VerticalMatchDetector _verticalMatchDetector;
        private Matches _matches;
        private GemView[] _randomizedGemPrefabs;
        private Gem _swipedGem;
        private Gem _otherGem;
        private Direction _swipeDirection;
        private readonly BoardStates _boardStates = new ();
        private BoardState _state;
        private List<Gem> _refillGems = new ();

        public event Action<Gem, Direction> OnSwipe;
        public SwipeData SwipeData;
        public Matches Matches => _matches;
        public HorizontalMatchDetector HorizontalMatchDetector => _horizontalMatchDetector;
        public VerticalMatchDetector VerticalMatchDetector => _verticalMatchDetector;
        public Gem[] Bombs { get; set; }
        public BoardSaveModel Board => _board;
        public List<Gem> RefillGems => _refillGems;

        [Inject]
        public void Construct(GameSettings gameSettings, BoardSaveProvider boardSaveProvider)
        {
            _gameSettings = gameSettings;
            _boardSaveProvider = boardSaveProvider;
        }

        //event functions

        private void Awake()
        {
            _boardSaveProvider.Reset();
            _board = _boardSaveProvider.Read();
            _horizontalMatchDetector = new HorizontalMatchDetector(_board);
            _verticalMatchDetector = new VerticalMatchDetector(_board);
            _matches = new Matches(_gameSettings);
            _gameSettings.IniPrefabPool();
        }

        private void Start()
        {
            if (!useStartBoard || startBoard == null)
                InitRandomBoard();
            else
                InitBoardFromTextFile(startBoard);

            SetState(BoardStateId.WaitForSwipe);
        }

        //public

        public void SetState(BoardStateId newStateId)
        {
            if (_state != null && _state.Id == newStateId) return;

            _state?.Finish();
            Debug.Log("Setting board controller state: " + newStateId);
            _state = _boardStates.GetBoardState(newStateId);
            _state.Start();
        }

        public void CallOnSwipe(Gem gem, Direction swipeDirection)
        {
            OnSwipe?.Invoke(gem, swipeDirection);
        }

        public void InstantiateGemView(Gem gem)
        {
            GemView gemView = InitGemViewInstance(gem.GemPrefab,gem.Pos);
            gemView.Init(gem);
            gem.GemView = gemView;
        }

        public void TrySetGem(int x, int y)
        {
            GemView gemPrefab = _gameSettings.GetRandomRegularGemPrefab();
            Gem gem = new Gem(gemPrefab, x, y);
            _board[x, y] = gem;
            if (!(_horizontalMatchDetector.IsMatchesInLine(y) || _verticalMatchDetector.IsMatchesInLine(x)))
            {
                InstantiateGemView(gem);
                return;
            }

            TrySetDifferentGems(x, y);
        }

        public void SetGemOfSpecifiedColor(int x, int y, GemColor gemColor) //debug
        {
            GemView gemPrefab = _gameSettings.GetRegularGemPrefab(gemColor);
            Gem gem = new Gem(gemPrefab, x, y);
            _board[x, y] = gem;
            InstantiateGemView(gem);
        }
        
        [Button("Save Model To Text File")]
        public void SaveModelToTextFile()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Game must be running");
                return;
            }
            
            _boardSaveProvider.SaveAsTextFile(_board);
        }

        [Button("Save Empty Model To Text File")]
        public void SaveEmptyModelToTextFile()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Game must be running");
                return;
            }

            _boardSaveProvider.SaveEmptyBoardAsTextFile();
        }
        
        //private

        private void InitRandomBoard()
        {
            Debug.Log("Generating random board");
            for (int x = 0; x < _board.Width; x++)
                for (int y = 0; y < _board.Height; y++)
                {
                    InstantiateBgTile(new Vector2Int(x, y));
                    TrySetGem(x, y);
                }
        }
        
        private void InitBoardFromTextFile(TextAsset startBoardAsset)
        {
            _board = _boardSaveProvider.GetFromTextFile(startBoardAsset);
            for (int x = 0; x < _board.Width; x++)
                for (int y = 0; y < _board.Height; y++)
                {
                    Gem gem = _board[x, y];
                    if (gem != null)
                    {
                        InstantiateBgTile(gem.Pos);
                        InstantiateGemView(gem);
                    }
                    else
                        InstantiateBgTile(new Vector2Int(x, y));
                }

            _horizontalMatchDetector = new HorizontalMatchDetector(_board);
            _verticalMatchDetector = new VerticalMatchDetector(_board);
        }

        private void TrySetDifferentGems(int x, int y)
        {
            if (_randomizedGemPrefabs == null)
                _randomizedGemPrefabs = _gameSettings.GetRandomizedGemPrefabs();
            else
                ArrayHelper.ShuffleArray(_randomizedGemPrefabs);

            foreach (GemView gemPrefab in _randomizedGemPrefabs)
            {
                Gem gem = new Gem(gemPrefab, x, y);
                _board[x, y] = gem;
                if (!(_horizontalMatchDetector.IsMatchesInLine(y) || _verticalMatchDetector.IsMatchesInLine(x)))
                {
                    InstantiateBgTile(gem.Pos);
                    InstantiateGemView(gem);
                    return;
                }
            }

            Debug.LogWarning($"Unable to find non-matching gem for position {x}, {y}");
            GemView randomGemPrefab = _gameSettings.GetRandomRegularGemPrefab();
            Gem randomGem = new Gem(randomGemPrefab, x, y);
            _board[x, y] = randomGem;
            InstantiateBgTile(randomGem.Pos);
            InstantiateGemView(randomGem);
        }

        private GemView InitGemViewInstance(GemView gemPrefab, Vector2Int pos)
        {
            GemView gemView = gemPrefab.GetInstance();
            Transform t = gemView.transform;
            t.position = (Vector2)pos;
            t.SetParent(gemsContainer, true);

            return gemView;
        }
        
        private void InstantiateBgTile(Vector2Int pos)
        {
            Instantiate(_gameSettings.BgTilePrefab, (Vector2)pos, Quaternion.identity,
                bgTilesContainer);
        }
    }
}