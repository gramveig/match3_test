using Match3Test.Board.BoardAnimationClasses;
using Match3Test.Board.MatchLogic;
using Match3Test.Board.Model;
using Match3Test.Game;
using UnityEngine;
using Zenject;

namespace Match3Test.Board
{
    public class BoardStateDestroyMatchingNonBombGems : BoardState
    {
        private GameController _gameController;
        
        public override BoardStateId Id => BoardStateId.DestroyMatchingNonBombGems;

        [Inject]
        public void Construct(GameController gameController)
        {
            _gameController = gameController;
        }

        public override void Start()
        {
            DestroyMatchingNonBombGems();
        }

        private void DestroyMatchingNonBombGems()
        {
            Matches matches = _boardController.Matches;
            BoardSaveModel board = _boardController.Board;
            foreach (Match match in matches.GemMatches)
            {
                foreach (Gem gem in match.MatchingGems)
                    if (gem.GemClass == GemClass.Common)
                    {
                        Gem curGem = board[gem.Pos.x, gem.Pos.y];
                        if (curGem != gem) continue;

                        int scoreValue = gem.GemView.ScoreValue;
                        board[gem.Pos.x, gem.Pos.y] = null;
                        _boardAnimator.AddGemToAnimation(gem, AnimationType.DestroyGems);
                        _gameController.AddScore(scoreValue);
                    }
            }

            _boardAnimator.AnimateGemsInAnimation(OnAnimationEnd, AnimationType.DestroyGems);
        }

        private void OnAnimationEnd()
        {
            _boardController.SetState(BoardStateId.ExplodeGemsAroundBombs);
        }
    }
}