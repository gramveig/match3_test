using Match3Test.Board.Model;
using UnityEngine;

namespace Match3Test.Board
{
    public class BoardStateSpawnNewBombs : BoardState
    {
        public override BoardStateId Id => BoardStateId.SpawnNewBombs;

        public override void Start()
        {
            foreach (Gem newBomb in _boardController.Matches.NewBombs)
            {
                _boardController.Board[newBomb.Pos.x, newBomb.Pos.y] = newBomb;
                _boardController.InstantiateGemView(newBomb);
            }

            _boardController.SetState(BoardStateId.CompactGems);
        }
    }
}