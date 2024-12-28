using System;
using Match3Test.Untility;

namespace Match3Test.Board.BoardAnimationClasses
{
    public class BoardAnimations
    {
        private BoardAnimationMoveGem _moveGemAnimation;
        private BoardAnimationDestroyGem _animationDestroyGem;
        private BoardAnimationShakeGem _animationShakeGem;

        public BoardAnimation GetAnimation(AnimationType animationType)
        {
            BoardAnimation result = animationType switch
            {
                AnimationType.MoveGems => _moveGemAnimation ??= new BoardAnimationMoveGem(),
                AnimationType.DestroyGems => _animationDestroyGem ??= new BoardAnimationDestroyGem(),
                AnimationType.ShakeGems => _animationShakeGem ??= new BoardAnimationShakeGem(),
                _ => throw new Exception($"Unknown animation type {animationType}")
            };

            SceneDiContainer.Container.Inject(result);
            return result;
        }
    }
}