using Match3Test.Board;
using Match3Test.Game.Settings;
using UnityEngine;

namespace Match3Test.Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;

        public static GameController Instance;
        public GameSettings GameSettings => gameSettings;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
        }
    }
}