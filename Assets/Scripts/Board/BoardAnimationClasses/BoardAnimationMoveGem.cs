using Match3Test.Board.Model;

namespace Match3Test.Board.BoardAnimationClasses
{
    public class BoardAnimationMoveGem : BoardAnimation
    {
        protected override void Animate(Gem gem)
        {
            gem.GemView.Move(gem.Pos);
        }
    }
}