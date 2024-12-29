using Match3Test.Board.BoardAnimationClasses;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateShakeAfterCompact : BoardState
    {
        public override BoardStateId Id => BoardStateId.ShakeAfterCompact;

        public override void Start()
        {
            Debug.Log("Shaking gems after compacting");
            _boardAnimator.StartNewAnimationSequence(_boardAnimator.GetAnimationSequence(AnimationType.MoveGems),
                AnimationType.ShakeGems);
            _boardAnimator.AnimateGemsInSequence(OnAnimationComplete, AnimationType.ShakeGems);
        }

        private void OnAnimationComplete()
        {
            _boardController.SetState(BoardStateId.RefillBoard);
        }
    }
}