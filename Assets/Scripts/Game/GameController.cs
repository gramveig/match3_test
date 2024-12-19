using UnityEngine;

namespace Match3Test.Game
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        private void Awake()
        {
            Instance = this;
        }
    }
}