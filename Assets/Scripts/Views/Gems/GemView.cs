using System;
using Match3Test.Board;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Utility;
using UnityEngine;

namespace Match3Test.Views.Gems
{
    public class GemView : MonoBehaviour
    {
        [SerializeField] private GemClass gemClass;
        [SerializeField] private GemColor gemColor;
        [SerializeField] private GemSpecialType gemSpecialType;

        public GemClass GemClass => gemClass;
        public GemColor GemColor => gemColor;
        public GemSpecialType GemSpecialType => gemSpecialType;

        private GameController _gameController;
        private BoardController _boardController;
        private Gem _gem;
        private bool _mousePressed;
        private Vector2 _firstTouchPosition;

        public void Init(Gem gem)
        {
            _gem = gem;
        }
        
        private void Start()
        {
            _gameController = GameController.Instance;
            _boardController = BoardController.Instance;
        }

        private void Update()
        {
            if (_mousePressed && Input.GetMouseButtonUp(0))
            {
                _mousePressed = false;
                if (_gameController.GameState == GameState.WaitForMove) ProcessSwipe();
            }
        }

        private void OnMouseDown()
        {
            if (_gameController.GameState == GameState.WaitForMove)
            {
                _firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _mousePressed = true;
            }
        }

        private void ProcessSwipe()
        {
            Vector2 finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float swipeAngle = CalculateAngle(_firstTouchPosition, finalTouchPosition);
            Direction swipeDirection = AngleHelper.AngleToDirection(swipeAngle);

            _boardController.ProcessSwipe(_gem, swipeDirection);
        }

        private float CalculateAngle(Vector2 firstTouchPosition, Vector2 finalTouchPosition)
        {
            float swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
            swipeAngle = swipeAngle * 180 / Mathf.PI;

            return swipeAngle;
        }
    }
}