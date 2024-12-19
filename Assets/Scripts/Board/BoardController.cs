using UnityEngine;

namespace Match3Test.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private int boardWidth;
        [SerializeField] private int boardHeight;
        
        public static BoardController Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}