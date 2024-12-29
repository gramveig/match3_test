using System.Collections;
using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.Model;
using Match3Test.Game;
using Match3Test.Game.Settings;
using UnityEngine;
using Zenject;

namespace Match3Test.Board
{
    public class BoardStateExplodeGemsAroundBombs : BoardState
    {
        private GameController _gameController;
        private GameSettings _gameSettings;

        [Inject]
        public void Construct(GameController gameController, GameSettings gameSettings)
        {
            _gameController = gameController;
            _gameSettings = gameSettings;
        }
        
        public override BoardStateId Id => BoardStateId.ExplodeGemsAroundBombs;

        public override void Start()
        {
            if (_boardController.Matches.IsBombs())
                _boardController.StartCoroutine(WaitAndExplodeGemsAroundBombs());
            else
                _boardController.SetState(BoardStateId.SpawnNewBombs);
        }

        private IEnumerator WaitAndExplodeGemsAroundBombs()
        {
            Debug.Log("Waiting to explode gems around bombs...");
            yield return new WaitForSeconds(_gameSettings.BombExplosionDelay);

            _boardController.Bombs = _boardController.Matches.GetBombs();
            foreach (Gem bomb in _boardController.Bombs)
                ExplodeGemsAroundBomb(bomb);

            _boardAnimator.AnimateGemsInAnimation(OnAnimationEnd, AnimationType.DestroyGems);
        }
        
        private void ExplodeGemsAroundBomb(Gem gem)
        {
            BoardSaveModel board = _boardController.Board;
            int x = gem.Pos.x;
            int y = gem.Pos.y;
            DestroyGemAroundBomb(board.GetGem(x, y + 2));
            DestroyGemAroundBomb(board.GetGem(x - 1 , y + 1));
            DestroyGemAroundBomb(board.GetGem(x, y + 1));
            DestroyGemAroundBomb(board.GetGem(x + 1 , y + 1));
            DestroyGemAroundBomb(board.GetGem(x - 2, y));
            DestroyGemAroundBomb(board.GetGem(x - 1, y));
            DestroyGemAroundBomb(board.GetGem(x + 1, y));
            DestroyGemAroundBomb(board.GetGem(x + 2, y));
            DestroyGemAroundBomb(board.GetGem(x - 1 , y - 1));
            DestroyGemAroundBomb(board.GetGem(x, y - 1));
            DestroyGemAroundBomb(board.GetGem(x + 1 , y - 1));
            DestroyGemAroundBomb(board.GetGem(x, y - 2));
        }
        
        private void DestroyGemAroundBomb(Gem gem)
        {
            if (   gem == null
                   || gem.GemClass == GemClass.Special && gem.GemSpecialType == GemSpecialType.Bomb //only common gems are destroyed, bombs are destroyed after delay
               )
                return;

            int scoreValue = gem.GemView.ScoreValue;
            _boardController.Board[gem.Pos.x, gem.Pos.y] = null;
            _boardAnimator.AddGemToAnimation(gem, AnimationType.DestroyGems);
            _gameController.AddScore(scoreValue);
        }

        private void OnAnimationEnd()
        {
            _boardController.SetState(BoardStateId.DestroyBombs);
        }
    }
}