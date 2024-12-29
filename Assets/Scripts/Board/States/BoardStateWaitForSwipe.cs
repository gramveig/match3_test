using System;
using Match3Test.Board.Model;
using Match3Test.Game;
using UnityEngine;
using Zenject;

namespace Match3Test.Board
{
    public class BoardStateWaitForSwipe : BoardState
    {
        private GameController _gameController;

        [Inject]
        public void Construct(GameController gameController)
        {
            _gameController = gameController;
        }
        
        public override BoardStateId Id => BoardStateId.WaitForSwipe;

        public override void Start()
        {
            _gameController.GameState = GameState.WaitForMove;
            _boardController.OnSwipe += OnSwipe;
        }

        public override void Finish()
        {
            _gameController.GameState = GameState.Moving;
            _boardController.OnSwipe -= OnSwipe;
        }

        private void OnSwipe(Gem swipedGem, Direction swipeDirection)
        {
            if (swipedGem == null)
            {
                Debug.LogError("Gem is null");
                return;
            }
            
            Gem otherGem = GetOtherGem(swipedGem, swipeDirection);
            if (otherGem == null) return;
            
            _boardController.SwipeData = new SwipeData
            {
                SwipedGem = swipedGem,
                OtherGem = otherGem,
                SwipeDirection = swipeDirection,
                NewGemPos = GetNewGemPos(swipedGem, swipeDirection),
                NewOtherGemPos = swipedGem.Pos
            };

            _boardController.SetState(BoardStateId.Swipe);
        }
        
        private Gem GetOtherGem(Gem swipedGem, Direction swipeDirection)
        {
            Vector2Int swipedCoord = GetNewGemPos(swipedGem, swipeDirection);
            int x = swipedCoord.x;
            int y = swipedCoord.y;
            BoardSaveModel board = _boardController.Board;
            if (x < 0 || x >= board.Width || y < 0 || y >= board.Height) return null;
            
            Gem otherGem = board[x, y];
            return otherGem;
        }

        private Vector2Int GetNewGemPos(Gem swipedGem, Direction swipeDirection)
        {
            int x = swipedGem.Pos.x;
            int y = swipedGem.Pos.y;
            if (swipeDirection == Direction.Up) y++;
            else if (swipeDirection == Direction.Right) x++;
            else if (swipeDirection == Direction.Down) y--;
            else if (swipeDirection == Direction.Left) x--;
            else throw new Exception("Unknown swipe direction");

            return new Vector2Int(x, y);
        }
    }
}