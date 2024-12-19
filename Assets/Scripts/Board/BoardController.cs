using UnityEngine;

namespace Match3Test.Board
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private Vector2Int boardDimensions;
        
        public static BoardController Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}