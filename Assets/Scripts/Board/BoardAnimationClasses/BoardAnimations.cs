using System;
using Match3Test.Untility;

namespace Match3Test.Board.BoardAnimationClasses
{
    public class BoardAnimations
    {
        private BoardAnimationMoveGem _moveGemAnimation;
        private BoardAnimationDestroyGem _animationDestroyGem;
        private BoardAnimationShakeGem _animationShakeGem;

        public BoardAnimation GetAnimation(GemAnimationType gemAnimationType)
        {
            BoardAnimation result = gemAnimationType switch
            {
                GemAnimationType.Move => _moveGemAnimation ??= new BoardAnimationMoveGem(),
                GemAnimationType.Destroy => _animationDestroyGem ??= new BoardAnimationDestroyGem(),
                GemAnimationType.Shake => _animationShakeGem ??= new BoardAnimationShakeGem(),
                _ => throw new Exception($"Unknown animation type {gemAnimationType}")
            };

            SceneDiContainer.Container.Inject(result);
            return result;
        }
    }
}