using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Game;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private int boardWidth;
        [SerializeField] private int boardHeight;

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
        }

        public Gem GetGem(int x, int y)
        {
            return Board[x, y];
        }

        
    }
}