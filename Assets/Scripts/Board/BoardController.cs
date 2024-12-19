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
        private GameController _gameController;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _gameController = GameController.Instance;
            BoardSaveProvider = new BoardSaveProvider(boardWidth, boardHeight);
        }
    }
}