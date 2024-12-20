using Match3Test.Board;

namespace Match3Test.Utility
{
    public static class AngleHelper
    {
        public static Direction AngleToDirection(float angle)
        {
            if (angle < 45 && angle > -45) return Direction.Right;

            if (angle > 45 && angle <= 135) return Direction.Up;

            if (angle < -45 && angle >= -135) return Direction.Down;

            return Direction.Left;
        }
    }
}