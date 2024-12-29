namespace Match3Test.Board
{
    public enum BoardStateId
    {
        None,
        WaitForSwipe,
        Swipe,
        SwipeBack,
        CheckMatchesAfterSwipe,
        ProcessMatches,
        DestroyMatchingNonBombGems,
        ExplodeGemsAroundBombs,
        DestroyBombs,
        SpawnNewBombs,
        CompactGems,
        ShakeAfterCompact,
        RefillBoard,
        ShakeAfterRefill,
        CheckMatchesAfterRefill
    }
}