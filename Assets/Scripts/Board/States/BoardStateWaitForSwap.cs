using System;
using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using Match3Test.Game;
using UnityEngine;
using Zenject;

namespace Match3Test.Board
{
    public class BoardStateWaitForSwap : BoardState
    {
        private GameController _gameController;
        private BoardSaveModel _board;

        [Inject]
        public void Construct(GameController gameController)
        {
            _gameController = gameController;
        }
        
        public override BoardStateId Id => BoardStateId.WaitForSwap;

        public override void Start()
        {
            _board = _boardController.Board;
            _gameController.GameState = GameState.WaitForMove;
            _boardController.OnSwipe += OnSwap;
        }

        public override void Update()
        {
            
        }

        public override void Finish()
        {
            _boardController.OnSwipe -= OnSwap;
        }

        //private

        private void OnSwap(Gem gem, Direction swipeDirection)
        {
            if (gem == null)
            {
                Debug.LogError("Gem is null");
                return;
            }
            
            Gem otherGem = GetOtherGem(gem, swipeDirection);
            if (otherGem == null) return;

            SwipeGems(gem, otherGem, swipeDirection, OnAnimationComplete);
            //save the swipe data to find bombs and in case we need to swipe back
            _boardController.SwipeData = new SwipeData
            {
                SwipedGem = gem,
                OtherGem = otherGem,
                SwipeDirection = swipeDirection
            };
        }

        private void OnAnimationComplete()
        {
            //CheckMatchesAfterSwipe
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
    }
}