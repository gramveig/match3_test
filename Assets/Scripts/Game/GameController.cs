using System.Collections;
using Match3Test.Board;
using Match3Test.Game.Settings;
using UnityEngine;

namespace Match3Test.Game
{
    public enum GameState
    {
        None,
        Starting,
        WaitForMove,
        Moving
    }
    
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;

        public static GameController Instance;
        public GameSettings GameSettings => gameSettings;
        public GameState GameState { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameState = GameState.Starting;
            StartCoroutine(SetWaitForMoveState());
        }

        private IEnumerator SetWaitForMoveState()
        {
            yield return null;
            GameState = GameState.WaitForMove;
        }
    }
}