using System;
using System.Collections;
using System.Collections.Generic;
using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;
using Match3Test.Utility;
using Match3Test.Views.Gems;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Match3Test.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private int boardWidth;
        [SerializeField] private int boardHeight;
        [SerializeField] private Transform gemsContainer;
        [SerializeField] private Transform bgTilesContainer;

        public BoardSaveProvider BoardSaveProvider { get; private set; }
        public BoardSaveModel Board { get; private set; }
        public int BoardWidth => boardWidth;
        public int BoardHeight => boardHeight;

        public event Action OnMoveGemsCompleteEvent;
        public event Action OnBurstGemsCompleteEvent;
        public event Action OnShakeGemsCompleteEvent;

        private GameController _gameController;
        private GameSettings _gameSettings;
        private HorizontalMatchDetector _horizontalMatchDetector;
        private VerticalMatchDetector _verticalMatchDetector;
        private GemView[] _randomizedGemPrefabs;
        private int _movingGemsCounter;
        private Gem _swipedGem;
        private Gem _otherGem;
        private Direction _swipeDirection;
        private List<Match> _matches = new List<Match>();
        private List<Gem> _bombs = new List<Gem>();
        private int _burstingGemsCounter;
        private MoveSequence _moveSequence;
        private int _shakedGemsCounter;

        private class MoveSequence
        {
            private BoardController _boardController;
            private GameController _gameController;
            private GameSettings _gameSettings;
            private List<List<Gem>> sequence = new();
            public int Count;

            public MoveSequence(BoardController boardController, GameController gameController, GameSettings gameSettings)
            {
                _boardController = boardController;
                _gameController = gameController;
                _gameSettings = gameSettings;
            }

            private void AddIndex(int idx)
            {
                for (int i = 0; i <= idx; i++)
                    if (sequence.Count - 1 < i)
                        sequence.Add(new List<Gem>());
            }

            public void AddToIndex(int idx, Gem gem)
            {
                if (gem == null) return;

                if (idx >= sequence.Count) AddIndex(idx);

                sequence[idx].Add(gem);
                Count++;
            }

            public void Clear()
            {
                sequence.Clear();
                Count = 0;
            }

            private int GetMaxNestedListCount()
            {
                int maxCount = 0;
                for (int i = 0; i < sequence.Count; i++)
                {
                    int count = sequence[i].Count;
                    if (count > maxCount) maxCount = count;
                }

                return maxCount;
            }

            private int GetGemsCount()
            {
                int count = 0;
                for (int i = 0; i < sequence.Count; i++)
                {
                    count += sequence[i].Count;
                }

                return count;
            }

            public void MoveInSequence()
            {
                _boardController.StartCoroutine(SeqEnumerator());
            }

            private IEnumerator SeqEnumerator()
            {
                float delayStep = _gameSettings.DelayStep;
                int maxJ = GetMaxNestedListCount();
                int gemsCount = GetGemsCount();
                _boardController._movingGemsCounter += gemsCount;
                for (int j = 0; j < maxJ; j++)
                {
                    yield return new WaitForSeconds(delayStep * j);

                    for (int i = 0; i < sequence.Count; i++)
                    {
                        var list = sequence[i];
                        if (j >= list.Count) continue;

                        Gem gem = list[j];
                        _boardController.MoveGemToNewPos(gem, false);
                    }
                }
            }

            public void ShakeInSequence()
            {
                _boardController.StartCoroutine(ShakeSeqEnumerator());
            }

            private IEnumerator ShakeSeqEnumerator()
            {
                float delayStep = _gameSettings.DelayStep;
                int maxJ = GetMaxNestedListCount();
                int gemsCount = GetGemsCount();
                _boardController._shakedGemsCounter += gemsCount;
                for (int j = 0; j < maxJ; j++)
                {
                    yield return new WaitForSeconds(delayStep * j);

                    for (int i = 0; i < sequence.Count; i++)
                    {
                        var list = sequence[i];
                        if (j >= list.Count) continue;

                        Gem gem = list[j];
                        gem.GemView.Shake();
                    }
                }
            }
        }

        [Inject]
        public void Construct(GameController gameController, GameSettings gameSettings)
        {
            _gameController = gameController;
            _gameSettings = gameSettings;
        }

        private void Start()
        {
            _gameSettings.IniPrefabPool();
            BoardSaveProvider = new BoardSaveProvider(boardWidth, boardHeight);
            Board = BoardSaveProvider.Read();
            _horizontalMatchDetector = new HorizontalMatchDetector(this);
            _verticalMatchDetector = new VerticalMatchDetector(this);
            _moveSequence = new MoveSequence(this, _gameController, _gameSettings);
            InitRandomBoard();
        }

        public Gem GetGem(int x, int y)
        {
            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight) return null;

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
            }
        }

        public void OnShakeGemsComplete()
        {
            if (_shakedGemsCounter <= 0)
                Debug.LogError("Shake gem counter is 0 while still playing animation");

            _shakedGemsCounter--;
            if (_shakedGemsCounter < 0)
                _shakedGemsCounter = 0;

            if (_shakedGemsCounter == 0)
            {
                Debug.Log("Shake complete");
                OnShakeGemsCompleteEvent?.Invoke();
            }
        }

    //private

        private void InitRandomBoard()
        {
            Debug.Log("Generating random board");
            for (int x = 0; x < boardWidth; x++)
                for (int y = 0; y < boardHeight; y++)
                    TrySetGem(x, y);
        }

        private void TrySetGem(int x, int y)
        {
            GemView gemPrefab = _gameSettings.GetRandomRegularGemPrefab();
            Gem gem = new Gem(gemPrefab, x, y);
            Board[x, y] = gem;
            if (!(_horizontalMatchDetector.IsMatchesInLine(y) || _verticalMatchDetector.IsMatchesInLine(x)))
            {
                InstantiateGem(gem);
                return;
            }

            TrySetDifferentGems(x, y);
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
                Board[x, y] = gem;
                if (!(_horizontalMatchDetector.IsMatchesInLine(y) || _verticalMatchDetector.IsMatchesInLine(x)))
                {
                    InstantiateGem(gem);
                    return;
                }
            }

            Debug.LogWarning($"Unable to find non-matching gem for position {x}, {y}");
            GemView randomGemPrefab = _gameSettings.GetRandomRegularGemPrefab();
            Gem randomGem = new Gem(randomGemPrefab, x, y);
            Board[x, y] = randomGem;
            InstantiateGem(randomGem);
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
            Instantiate(_gameSettings.BgTilePrefab, (Vector2)pos, Quaternion.identity,
                bgTilesContainer);
        }

        private GemView InstantiateGem(GemView gemPrefab, Vector2Int pos)
        {
            GemView gemView = gemPrefab.GetInstance();
            Transform t = gemView.transform;
            t.position = (Vector2)pos;
            t.SetParent(gemsContainer, true);

            return gemView;
        }
        
        private void MoveGemToNewPos(Gem gem, bool increaseCounter = true)
        {
            if (gem == null || gem.GemView == null) return;

            if (increaseCounter)
                _movingGemsCounter++;

            gem.GemView.Move(gem.Pos);
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
                if (_horizontalMatchDetector.IsMatchesInLine(_swipedGem.Pos.y, ref _matches)
                    || _verticalMatchDetector.IsMatchesInLine(_swipedGem.Pos.x, ref _matches)
                    || _verticalMatchDetector.IsMatchesInLine(_otherGem.Pos.x, ref _matches)
                   )
                {
                    ProcessMatches(_matches);
                    return;
                }
            }
            else
            {
                if (_verticalMatchDetector.IsMatchesInLine(_swipedGem.Pos.x, ref _matches)
                    || _horizontalMatchDetector.IsMatchesInLine(_swipedGem.Pos.y, ref _matches)
                    || _horizontalMatchDetector.IsMatchesInLine(_otherGem.Pos.y, ref _matches)
                   )
                {
                    ProcessMatches(_matches);
                    return;
                }
            }

            SwipeBack();
        }

        private void SwipeBack()
        {
            Vector2Int swipedGemPos = _swipedGem.Pos;
            Vector2Int otherGemPos = _otherGem.Pos;
            _swipedGem.Pos = otherGemPos;
            _otherGem.Pos = swipedGemPos;
            MoveGemToNewPos(_swipedGem);
            MoveGemToNewPos(_otherGem);
            OnMoveGemsCompleteEvent += OnSwipeBackEnd;
        }

        private void OnSwipeBackEnd()
        {
            OnMoveGemsCompleteEvent -= OnSwipeBackEnd;
            
            _gameController.GameState = GameState.WaitForMove;
        }

        private void ProcessMatches(List<Match> matches)
        {
            Debug.Log($"Matches detected: {matches.Count}");

            GetBombs(matches);
            DestroyMatchingNonBombGems(matches);
        }

        private void GetBombs(List<Match> matches)
        {
            _bombs.Clear();
            foreach (Match match in matches)
            {
                if (match.IsNewBomb(_swipedGem, _otherGem, out Vector2Int bombPos))
                {
                    GemView bombPrefab = _gameSettings.GetBombPrefabOfColor(match.MatchColor);
                    Gem bomb = new Gem(bombPrefab, bombPos.x, bombPos.y);
                    _bombs.Add(bomb);
                }
            }
        }
        
        private void DestroyMatchingNonBombGems(List<Match> matches)
        {
            bool isGemsToDestory = false;
            foreach (Match match in matches)
            {
                List<Gem> matchingGems = match.MatchingGems;
                foreach (Gem gem in matchingGems)
                    if (gem.GemClass == GemClass.Common)
                    {
                        DestroyGem(gem);
                        isGemsToDestory = true;
                    }
            }

            if (isGemsToDestory)
                OnBurstGemsCompleteEvent += ExplodeBombs;
            else
                ExplodeBombs();
        }

        private int DestroyGem(Gem gem, bool notBombs = false)
        {
            if (   gem == null
                || notBombs && gem.GemClass == GemClass.Special && gem.GemSpecialType == GemSpecialType.Bomb
            )
                return 0;

            Gem curGem = Board[gem.Pos.x, gem.Pos.y];
            if (curGem != gem) return 0;

            _burstingGemsCounter++;
            int scoreValue = gem.GemView.ScoreValue;
            gem.GemView.Destroy();
            Board[gem.Pos.x, gem.Pos.y] = null;
            _gameController.AddScore(scoreValue);
            return 1;
        }

        private void ExplodeBombs()
        {
            OnBurstGemsCompleteEvent -= ExplodeBombs;

            Debug.Log("Checking for matched bombs to explode...");
            if (IsBombs())
                StartCoroutine(WaitAndExplodeBombs());
            else
            {
                Debug.Log("No matching bombs found.");
                SpawnNewBombs();
            }
        }

        private bool IsBombs()
        {
            foreach (Match match in _matches)
                if (match.IsBombs())
                    return true;

            return false;
        }
        
        private IEnumerator WaitAndExplodeBombs()
        {
            Debug.Log("Waiting to explode bombs...");
            yield return new WaitForSeconds(_gameSettings.BombExplosionDelay);

            bool isExplodingGems = false;
            foreach (Match match in _matches)
            {
                List<Gem> matchingGems = match.MatchingGems;
                foreach (Gem gem in matchingGems)
                    if (gem.GemClass == GemClass.Special && gem.GemSpecialType == GemSpecialType.Bomb)
                        isExplodingGems = ExplodeBomb(gem);
            }

            if (isExplodingGems)
            {
                Debug.Log("Exploding bombs");
                OnBurstGemsCompleteEvent += DestroyBombs;
            }
            else
            {
                Debug.Log("No gems to destroy with bomb found");
                SpawnNewBombs();
            }
        }

        private bool ExplodeBomb(Gem gem)
        {
            int x = gem.Pos.x;
            int y = gem.Pos.y;
            int c = 0;
            c += DestroyGem(GetGem(x, y + 2), true);
            c += DestroyGem(GetGem(x - 1 , y + 1), true);
            c += DestroyGem(GetGem(x + 1 , y + 1), true);
            c += DestroyGem(GetGem(x - 2, y), true);
            c += DestroyGem(GetGem(x - 1, y), true);
            c += DestroyGem(GetGem(x + 1, y), true);
            c += DestroyGem(GetGem(x + 2, y), true);
            c += DestroyGem(GetGem(x - 1 , y - 1), true);
            c += DestroyGem(GetGem(x + 1 , y - 1), true);
            c += DestroyGem(GetGem(x, y - 2), true);

            return c > 0;
        }

        private void DestroyBombs()
        {
            OnBurstGemsCompleteEvent -= DestroyBombs;

            StartCoroutine(WaitAndDestroyBombs());
        }

        private IEnumerator WaitAndDestroyBombs()
        {
            Debug.Log("Waiting to destroy the exploded bombs...");
            yield return new WaitForSeconds(_gameSettings.BombDestructionDelay);
            
            foreach (Match match in _matches)
            {
                List<Gem> matchingGems = match.MatchingGems;
                foreach (Gem gem in matchingGems)
                    if (gem.GemClass == GemClass.Special && gem.GemSpecialType == GemSpecialType.Bomb)
                        DestroyGem(gem);
            }

            Debug.Log("Destroying the exploded bombs.");
            OnBurstGemsCompleteEvent += SpawnNewBombs;
        }

        private void SpawnNewBombs()
        {
            OnBurstGemsCompleteEvent -= SpawnNewBombs;

            Debug.Log("Spawning new bombs if any.");
            foreach (Gem bomb in _bombs)
            {
                Board[bomb.Pos.x, bomb.Pos.y] = bomb;
                InstantiateGem(bomb);
            }

            CompactGems();
        }

        private void CompactGems()
        {
            Debug.Log("Compacting gems");
            _moveSequence.Clear();
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
                    else if (nullCounter > 0)
                    {
                        Board[x, y] = null;
                        gem.Pos.y -= nullCounter;
                        _moveSequence.AddToIndex(x, gem);
                    }
                }
            }

            if (_moveSequence.Count > 0)
            {
                _moveSequence.MoveInSequence();
                OnMoveGemsCompleteEvent += ShakeAfterCompact;
            }
            else
            {
                Debug.Log("No gems to compact found");
                RefillBoard();
            }
        }

        private void ShakeAfterCompact()
        {
            OnMoveGemsCompleteEvent -= ShakeAfterCompact;

            _moveSequence.ShakeInSequence();
            OnShakeGemsCompleteEvent += RefillBoard;
        }
        
        private void RefillBoard()
        {
            OnShakeGemsCompleteEvent -= RefillBoard;

            Debug.Log("Refilling the board");
            _moveSequence.Clear();
            float dropHeight = _gameSettings.GemDropHeight;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    Gem gem = Board[x, y];
                    if (gem == null)
                    {
                        TrySetGem(x, y);
                        gem = Board[x, y];
                        gem.GemView.transform.position = new Vector2(gem.Pos.x, gem.Pos.y + dropHeight);
                        _moveSequence.AddToIndex(x, gem);
                    }
                }
            }

            if (_moveSequence.Count > 0)
            {
                _moveSequence.MoveInSequence();
                OnMoveGemsCompleteEvent += ShakeAfterRefill;
            }
            else
            {
                Debug.LogWarning("No gems for refill found");
                FindMatchesAfterRefill();
            }
        }

        private void ShakeAfterRefill()
        {
            OnMoveGemsCompleteEvent -= ShakeAfterRefill;

            _moveSequence.ShakeInSequence();
            OnShakeGemsCompleteEvent += FindMatchesAfterRefill;
        }

        private void FindMatchesAfterRefill()
        {
            OnShakeGemsCompleteEvent -= FindMatchesAfterRefill;

            Debug.Log("Looking for matches after refill...");
            _matches.Clear();
            if (_horizontalMatchDetector.IsMatches(ref _matches) || _verticalMatchDetector.IsMatches(ref _matches))
            {
                Debug.Log("Matches count: " + _matches.Count);
                DestroyMatchingNonBombGems(_matches);
            }
            else
            {
                Debug.Log("No matches found.");
                _gameController.GameState = GameState.WaitForMove;
            }
        }
    }
}