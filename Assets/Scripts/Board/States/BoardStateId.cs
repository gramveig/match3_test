namespace Match3Test.Board
{
    public enum BoardStateId
    {
        None,
        WaitForSwap,
        Swap,
        BackSwap,
        CheckForMatchesAfterSwap,
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