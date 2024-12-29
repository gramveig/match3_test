using Match3Test.Board.BoardAnimationClasses;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateShakeAfterRefill : BoardState
    {
        public override BoardStateId Id => BoardStateId.ShakeAfterRefill;

        public override void Start()
        {
            _boardAnimator.StartNewAnimationSequence(_boardAnimator.GetAnimationSequence(AnimationType.MoveGems),
                AnimationType.ShakeGems);
            _boardAnimator.AnimateGemsInSequence(OnAnimationEnd, AnimationType.ShakeGems);
        }

        private void OnAnimationEnd()
        {
            _boardController.SetState(BoardStateId.CheckMatchesAfterRefill);
        }
    }
}