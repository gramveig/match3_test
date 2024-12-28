using System;
using System.Collections;
using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Game;
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

        private GameController _gameController;
        private GameSettings _gameSettings;
        private BoardAnimator _boardAnimator;
        private BoardSaveModel _board;
        private BoardSaveProvider _boardSaveProvider;
        private HorizontalMatchDetector _horizontalMatchDetector;
        private VerticalMatchDetector _verticalMatchDetector;
        private Matches _matches;
        private GemView[] _randomizedGemPrefabs;
        private Gem _swipedGem;
        private Gem _otherGem;
        private Direction _swipeDirection;
        private Gem[] _bombs;

        [Inject]
        public void Construct(GameController gameController, GameSettings gameSettings, BoardAnimator boardAnimator,
            BoardSaveProvider boardSaveProvider)
        {
            _gameController = gameController;
            _gameSettings = gameSettings;
            _boardAnimator = boardAnimator;
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
        }

        //public

        public void ProcessSwipe(Gem gem, Direction swipeDirection)
        {
            if (gem == null)
            {
                Debug.LogError("Gem is null");
                return;
            }
            
            Gem otherGem = GetOtherGem(gem, swipeDirection);
            if (otherGem == null) return;

            SwipeGems(gem, otherGem, swipeDirection, CheckMatchesAfterSwipe);
            //save the swipe data to find bombs and in case we need to swipe back
            _swipedGem = gem;
            _otherGem = otherGem;
            _swipeDirection = swipeDirection;
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

        private void TrySetGem(int x, int y)
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

        //debug
        private void SetGemOfSpecifiedColor(int x, int y, GemColor gemColor)
        {
            GemView gemPrefab = _gameSettings.GetRegularGemPrefab(gemColor);
            Gem gem = new Gem(gemPrefab, x, y);
            _board[x, y] = gem;
            InstantiateGemView(gem);
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

        private void InstantiateGemView(Gem gem)
        {
            GemView gemView = InitGemViewInstance(gem.GemPrefab,gem.Pos);
            gemView.Init(gem);
            gem.GemView = gemView;
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

        private Gem GetOtherGem(Gem gem, Direction swipeDirection)
        {
            Vector2Int swappedCoord = GetNewGemPos(gem, swipeDirection);
            int x = swappedCoord.x;
            int y = swappedCoord.y;
            if (x < 0 || x >= _board.Width || y < 0 || y >= _board.Height) return null;
            
            Gem otherGem = _board[x, y];
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

        private void CheckMatchesAfterSwipe()
        {
            bool isMatches = CheckForMatches();
            if (!isMatches) SwipeBack(OnSwipeBackEnd);
        }

        private bool CheckForMatches()
        {
            _matches.Clear();
            if (AngleHelper.IsHorizontal(_swipeDirection))
            {
                bool isHorizontalMatches = _horizontalMatchDetector.IsMatchesInLine(_swipedGem.Pos.y, ref _matches);
                bool isVerticalMatches1 = _verticalMatchDetector.IsMatchesInLine(_swipedGem.Pos.x, ref _matches);
                bool isVerticalMatches2 = _verticalMatchDetector.IsMatchesInLine(_otherGem.Pos.x, ref _matches);
                if (isHorizontalMatches || isVerticalMatches1 || isVerticalMatches2)
                {
                    _matches.GetBombs(_swipedGem, _otherGem);
                    ProcessMatches(_matches);
                    return true;
                }
            }
            else
            {
                bool isVerticalMatches = _verticalMatchDetector.IsMatchesInLine(_swipedGem.Pos.x, ref _matches);
                bool isHorizontalMatches1 = _horizontalMatchDetector.IsMatchesInLine(_swipedGem.Pos.y, ref _matches);
                bool isHorizontalMatches2 = _horizontalMatchDetector.IsMatchesInLine(_otherGem.Pos.y, ref _matches);
                if (isVerticalMatches || isHorizontalMatches1 || isHorizontalMatches2)
                {
                    _matches.GetBombs(_swipedGem, _otherGem);
                    ProcessMatches(_matches);
                    return true;
                }
            }

            return false;
        }

        private void SwipeGems(Gem gem, Gem otherGem, Direction swipeDirection, Action callback)
        {
            Debug.Log($"Swiping {gem.GemColor} gem with coordinate ({gem.Pos.x}, {gem.Pos.y}) {swipeDirection}");

            gem.Pos = GetNewGemPos(gem, swipeDirection);
            _board.SetGemAtNewPos(gem);
            otherGem.Pos = GetNewOtherGemPos(otherGem, swipeDirection);
            _board.SetGemAtNewPos(otherGem);

            _boardAnimator.AddGemToAnimation(gem, AnimationType.MoveGems);
            _boardAnimator.AddGemToAnimation(otherGem, AnimationType.MoveGems);
            _boardAnimator.AnimateGemsInAnimation(callback, AnimationType.MoveGems);

            _gameController.GameState = GameState.Moving;
        }

        private void SwipeBack(Action callback)
        {
            Vector2Int swipedGemPos = _swipedGem.Pos;
            Vector2Int otherGemPos = _otherGem.Pos;
            _swipedGem.Pos = otherGemPos;
            _otherGem.Pos = swipedGemPos;
            _board.SetGemAtNewPos(_swipedGem);
            _board.SetGemAtNewPos(_otherGem);

            _boardAnimator.AddGemToAnimation(_swipedGem, AnimationType.MoveGems);
            _boardAnimator.AddGemToAnimation(_otherGem, AnimationType.MoveGems);
            _boardAnimator.AnimateGemsInAnimation(callback, AnimationType.MoveGems);
        }

        private void OnSwipeBackEnd()
        {
            _gameController.GameState = GameState.WaitForMove;
        }

        private void ProcessMatches(Matches matches)
        {
            Debug.Log($"Matches detected: {matches.MatchesCount}");
            if (matches.IsNonBombMatchingGems())
                DestroyMatchingNonBombGems(matches);
            else
                ExplodeBombs();
        }

        private void DestroyMatchingNonBombGems(Matches matches)
        {
            Debug.Log("Destroying matching non-bomb gems");
            
            foreach (Match match in matches.GemMatches)
            {
                foreach (Gem gem in match.MatchingGems)
                    if (gem.GemClass == GemClass.Common)
                    {
                        Gem curGem = _board[gem.Pos.x, gem.Pos.y];
                        if (curGem != gem) continue;

                        int scoreValue = gem.GemView.ScoreValue;
                        _board[gem.Pos.x, gem.Pos.y] = null;
                        _boardAnimator.AddGemToAnimation(gem, AnimationType.DestroyGems);
                        _gameController.AddScore(scoreValue);
                    }
            }

            _boardAnimator.AnimateGemsInAnimation(ExplodeBombs, AnimationType.DestroyGems);
        }

        private void ExplodeBombs()
        {
            Debug.Log("Checking for matched bombs to explode...");
            if (_matches.IsBombs())
                StartCoroutine(WaitAndExplodeGemsAroundBombs());
            else
            {
                Debug.Log("No matching bombs found.");
                SpawnNewBombs();
            }
        }

        private IEnumerator WaitAndExplodeGemsAroundBombs()
        {
            Debug.Log("Waiting to explode gems around bombs...");
            yield return new WaitForSeconds(_gameSettings.BombExplosionDelay);

            _bombs = _matches.GetBombs();
            foreach (Gem bomb in _bombs)
                ExplodeGemsAroundBomb(bomb);

            _boardAnimator.AnimateGemsInAnimation(DestroyBombs, AnimationType.DestroyGems);
        }

        private void ExplodeGemsAroundBomb(Gem gem)
        {
            int x = gem.Pos.x;
            int y = gem.Pos.y;
            DestroyGemAroundBomb(_board.GetGem(x, y + 2));
            DestroyGemAroundBomb(_board.GetGem(x - 1 , y + 1));
            DestroyGemAroundBomb(_board.GetGem(x + 1 , y + 1));
            DestroyGemAroundBomb(_board.GetGem(x - 2, y));
            DestroyGemAroundBomb(_board.GetGem(x - 1, y));
            DestroyGemAroundBomb(_board.GetGem(x + 1, y));
            DestroyGemAroundBomb(_board.GetGem(x + 2, y));
            DestroyGemAroundBomb(_board.GetGem(x - 1 , y - 1));
            DestroyGemAroundBomb(_board.GetGem(x + 1 , y - 1));
            DestroyGemAroundBomb(_board.GetGem(x, y - 2));
        }

        private void DestroyGemAroundBomb(Gem gem)
        {
            if (gem == null) return;
            
            int scoreValue = gem.GemView.ScoreValue;
            _board[gem.Pos.x, gem.Pos.y] = null;
            _boardAnimator.AddGemToAnimation(gem, AnimationType.DestroyGems);
            _gameController.AddScore(scoreValue);
        }
        
        private void DestroyBombs()
        {
            StartCoroutine(WaitAndDestroyBombs());
        }

        private IEnumerator WaitAndDestroyBombs()
        {
            Debug.Log("Waiting to destroy the exploded bombs...");
            yield return new WaitForSeconds(_gameSettings.BombDestructionDelay);

            foreach (Gem bomb in _bombs)
            {
                int scoreValue = bomb.GemView.ScoreValue;
                _board[bomb.Pos.x, bomb.Pos.y] = null;
                _boardAnimator.AddGemToAnimation(bomb, AnimationType.DestroyGems);
                _gameController.AddScore(scoreValue);
            }

            Debug.Log("Destroying the exploded bombs.");
            _boardAnimator.AnimateGemsInAnimation(SpawnNewBombs, AnimationType.DestroyGems);
        }

        private void SpawnNewBombs()
        {
            Debug.Log("Spawning new bombs if any.");
            foreach (Gem newBomb in _matches.NewBombs)
            {
                _board[newBomb.Pos.x, newBomb.Pos.y] = newBomb;
                InstantiateGemView(newBomb);
            }

            CompactGems();
        }

        private void CompactGems()
        {
            Debug.Log("Compacting gems");
            _boardAnimator.StartNewAnimationSequence(AnimationType.MoveGems);
            for (int x = 0; x < _board.Width; x++)
            {
                int nullCounter = 0;
                for (int y = 0; y < _board.Height; y++)
                {
                    Gem gem = _board[x, y];
                    if (gem == null)
                    {
                        nullCounter++;
                    }
                    else if (nullCounter > 0)
                    {
                        _board[x, y] = null;
                        int newYPos = gem.Pos.y - nullCounter;
                        gem.Pos.y = newYPos;
                        _board[x, newYPos] = gem;
                        _boardAnimator.AddGemToAnimationSequence(gem, x, AnimationType.MoveGems);
                    }
                }
            }

            if (_boardAnimator.IsGemsInAnimation(AnimationType.MoveGems))
            {
                _boardAnimator.AnimateGemsInSequence(ShakeAfterCompact, AnimationType.MoveGems);
            }
            else
            {
                Debug.Log("No gems to compact found");
                RefillBoard();
            }
        }

        private void ShakeAfterCompact()
        {
            Debug.Log("Shaking gems after compacting");
            _boardAnimator.StartNewAnimationSequence(_boardAnimator.GetAnimationSequence(AnimationType.MoveGems),
                AnimationType.ShakeGems);
            _boardAnimator.AnimateGemsInSequence(RefillBoard, AnimationType.ShakeGems);
        }

        private int _debugCount = 0;
        private void RefillBoard()
        {
            _boardAnimator.StartNewAnimationSequence(AnimationType.MoveGems);
            Debug.Log("Refilling the board");
            float dropHeight = _gameSettings.GemDropHeight;
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    Gem gem = _board[x, y];
                    if (gem == null)
                    {
                        if (_debugCount == 0)
                            SetGemOfSpecifiedColor(x, y, GemColor.Yellow);
                        else
                            TrySetGem(x, y);

                        gem = _board[x, y];
                        gem.GemView.transform.position = new Vector2(gem.Pos.x, gem.Pos.y + dropHeight);
                        _boardAnimator.AddGemToAnimationSequence(gem, x, AnimationType.MoveGems);
                    }
                }

                _debugCount++;
            }

            if (_boardAnimator.IsGemsInAnimation(AnimationType.MoveGems))
            {
                _boardAnimator.AnimateGemsInSequence(ShakeAfterRefill, AnimationType.MoveGems);
            }
            else
            {
                Debug.LogWarning("No gems for refill found");
                CheckMatchesAfterRefill();
            }
        }

        private void ShakeAfterRefill()
        {
            Debug.Log("Shake after refill");
            _boardAnimator.StartNewAnimationSequence(_boardAnimator.GetAnimationSequence(AnimationType.MoveGems),
                AnimationType.ShakeGems);
            _boardAnimator.AnimateGemsInSequence(CheckMatchesAfterRefill, AnimationType.ShakeGems);
        }

        private void CheckMatchesAfterRefill()
        {
            Debug.Log("Looking for matches after refill...");
            _matches.Clear();
            bool isHorizontalMatches = _horizontalMatchDetector.IsMatches(ref _matches);
            bool isVerticalMatches = _verticalMatchDetector.IsMatches(ref _matches);
            if (isHorizontalMatches || isVerticalMatches)
            {
                Debug.Log("Matches count: " + _matches.MatchesCount);
                _matches.GetAutoBombs();
                ProcessMatches(_matches);
            }
            else
            {
                Debug.Log("No matches found.");
                _gameController.GameState = GameState.WaitForMove;
            }
        }
    }
}