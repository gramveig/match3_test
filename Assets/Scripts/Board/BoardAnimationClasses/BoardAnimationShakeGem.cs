using Match3Test.Board.Model;

namespace Match3Test.Board.BoardAnimationClasses
{
    public class BoardAnimationShakeGem : BoardAnimation
    {
        protected override void Animate(Gem gem)
        {
            gem.GemView.Shake();
        }
    }
}