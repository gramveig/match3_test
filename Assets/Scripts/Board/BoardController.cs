using System;
using System.Collections.Generic;
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

        public event Action OnMoveGemsCompleteEvent;
        public event Action OnBurstGemsCompleteEvent;

        private GameController _gameController;
        private HorizontalMatchDetector _horizontalMatchDetector;
        private VerticalMatchDetector _verticalMatchDetector;
        private GemView[] _randomizedGemPrefabs;
        private int _movingGemsCounter;
        private Gem _swipedGem;
        private Gem _otherGem;
        private Direction _swipeDirection;
        private List<Match> _matches = new List<Match>();
        private int _burstingGemsCounter;

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
            //BoardSaveProvider.Save(Board);
        }

        public Gem GetGem(int x, int y)
        {
            return Board[x, y];
        }

        public void ProcessSwipe(Gem gem, Direction swipeDirection)
        {
            _otherGem = GetOtherGem(gem, swipeDirection);
            if (_otherGem == null) return;

            _swipedGem = gem;
            _swipeDirection = swipeDirection;
            Debug.Log($"Swiping {gem.GemColor} gem with coordinate ({gem.Pos.x}, {gem.Pos.y}) {swipeDirection}");

            gem.Pos = GetNewGemPos(gem, swipeDirection);
            MoveGemToNewPos(gem);
            _otherGem.Pos = GetNewOtherGemPos(_otherGem, swipeDirection);
            MoveGemToNewPos(_otherGem);
            OnMoveGemsCompleteEvent += CheckForMatches;
            _gameController.GameState = GameState.Moving;
        }

        public void OnMoveGemComplete()
        {
            if (_movingGemsCounter <= 0)
                Debug.LogError("Moving gem counter is 0 while still moving a gem");

            _movingGemsCounter--;
            if (_movingGemsCounter < 0)
                _movingGemsCounter = 0;

            if (_movingGemsCounter == 0)
                OnMoveGemsCompleteEvent?.Invoke();
        }

        public void OnBurstGemComplete()
        {
            if (_burstingGemsCounter <= 0)
                Debug.LogError("Bursting gem counter is 0 while still playing animation");

            _burstingGemsCounter--;
            if (_burstingGemsCounter < 0)
                _burstingGemsCounter = 0;

            if (_burstingGemsCounter == 0)
            {
                Debug.Log("Burst complete");
                OnBurstGemsCompleteEvent?.Invoke();
                _gameController.GameState = GameState.WaitForMove;
            }
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

        private bool TrySetGem(int x, int y)
        {
            GemView gemPrefab = _gameController.GameSettings.GetRandomRegularGemPrefab();
            Gem gem = new Gem(gemPrefab, x, y);
            Board[x, y] = gem;
            if (!(_horizontalMatchDetector.IsMatchesInLine(y) || _verticalMatchDetector.IsMatchesInLine(x)))
            {
                InstantiateGem(gem);
                return true;
            }

            Board[x, y] = null;
            bool result = TrySetDifferentGems(x, y);
            return result;
        }

        private bool TrySetDifferentGems(int x, int y)
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
                    return true;
                }
            }

            Debug.LogError($"Unable to find non-matching gem for position {x}, {y}");
            return false;
        }

        private void InstantiateGem(Gem gem)
        {
            InstantiateBgTile(gem.Pos);
            GemView gemView = InstantiateGem(gem.GemPrefab,gem.Pos);
            gemView.Init(gem);
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
        
        private void MoveGemToNewPos(Gem gem)
        {
            gem.GemView.Move(gem.Pos);
            _movingGemsCounter++;
            Board[gem.Pos.x, gem.Pos.y] = gem;
        }

        private Gem GetOtherGem(Gem gem, Direction swipeDirection)
        {
            Vector2Int swappedCoord = GetNewGemPos(gem, swipeDirection);
            int x = swappedCoord.x;
            int y = swappedCoord.y;
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight) return null;
            
            Gem otherGem = Board[x, y];
            return otherGem;
        }

        private Vector2Int GetNewGemPos(Gem gem, Direction swipeDirection)
        {
            int x = gem.Pos.x;
            int y = gem.Pos.y;
            if (swipeDirection == Direction.Up) y++;
            else if (swipeDirection == Direction.Right) x++;
            else if (swipeDirection == Direction.Down) y--;
            else if (swipeDirection == Direction.Left) x--;
            else throw new Exception("Unknown swipe direction");

            return new Vector2Int(x, y);
        }

        private Vector2Int GetNewOtherGemPos(Gem gem, Direction swipeDirection)
        {
            int x = gem.Pos.x;
            int y = gem.Pos.y;
            if (swipeDirection == Direction.Up) y--;
            else if (swipeDirection == Direction.Right) x--;
            else if (swipeDirection == Direction.Down) y++;
            else if (swipeDirection == Direction.Left) x++;
            else throw new Exception("Unknown swipe direction");

            return new Vector2Int(x, y);
        }
        
        private void CheckForMatches()
        {
            OnMoveGemsCompleteEvent -= CheckForMatches;
            _matches.Clear();
            if (AngleHelper.IsHorizontal(_swipeDirection))
            {
                if (   _horizontalMatchDetector.IsMatchesInLine(_swipedGem.Pos.y, ref _matches)
                       || _verticalMatchDetector.IsMatchesInLine(_swipedGem.Pos.x, ref _matches)
                       || _verticalMatchDetector.IsMatchesInLine(_otherGem.Pos.x, ref _matches)
                   )
                    ProcessMatches(_matches);
            }
            else
            {
                if (   _verticalMatchDetector.IsMatchesInLine(_swipedGem.Pos.x, ref _matches)
                       || _horizontalMatchDetector.IsMatchesInLine(_swipedGem.Pos.y, ref _matches)
                       || _horizontalMatchDetector.IsMatchesInLine(_otherGem.Pos.y, ref _matches)
                   )
                    ProcessMatches(_matches);
            }
        }

        private void ProcessMatches(List<Match> matches)
        {
            Debug.Log($"Matches detected: {matches.Count}");

            DestroyMatchingGems(matches);
        }

        private void DestroyMatchingGems(List<Match> matches)
        {
            foreach (Match match in matches)
            {
                List<Gem> matchingGems = match.MatchingGems;
                foreach (Gem gem in matchingGems)
                    DestroyGem(gem);
            }

            OnBurstGemsCompleteEvent += CompactGems;
        }

        private void DestroyGem(Gem gem)
        {
            _burstingGemsCounter++;
            gem.GemView.Destroy();
            Board[gem.Pos.x, gem.Pos.y] = null;
        }

        private void CompactGems()
        {
            OnBurstGemsCompleteEvent -= CompactGems;
            for (int x = 0; x < boardWidth; x++)
            {
                int nullCounter = 0;
                for (int y = 0; y < boardHeight; y++)
                {
                    Gem gem = Board[x, y];
                    if (gem == null)
                    {
                        nullCounter++;
                    }
                    else
                    {
                        Board[x, y] = null;
                        gem.Pos.y -= nullCounter;
                        MoveGemToNewPos(gem);
                    }
                }
            }

            OnMoveGemsCompleteEvent += RefillBoard;
        }

        private void RefillBoard()
        {
            OnMoveGemsCompleteEvent -= RefillBoard;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Gem gem = Board[x, y];
                    if (gem == null)
                    {
                        bool result = TrySetGem(x, y);
                        if (!result) continue;
                    }
                }
            }
        }
    }
}