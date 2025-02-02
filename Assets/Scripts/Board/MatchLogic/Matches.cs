using System.Collections.Generic;
using Match3Test.Board.Model;
using Match3Test.Game.Settings;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Board.MatchLogic
{
    public class Matches
    {
        public List<Match> GemMatches = new ();
        public List<Gem> NewBombs = new ();
        private List<Gem> _matchingBombs = new();
        private GameSettings _gameSettings;

        public Matches(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        public void Clear()
        {
            GemMatches.Clear();
            NewBombs.Clear();
            _matchingBombs.Clear();
        }

        public void AddMatch(Match match)
        {
            GemMatches.Add(match);
        }

        public int MatchesCount => GemMatches.Count;

        //find bombs after the gems were manually swiped
        public void GetNewBombs(List<Gem> checkedGems)
        {
            foreach (Match match in GemMatches)
            {
                if (match.IsNewBomb(checkedGems, out Vector2Int bombPos))
                {
                    GemView bombPrefab = _gameSettings.GetBombPrefab(match.MatchColor);
                    Gem bomb = new Gem(bombPrefab, bombPos.x, bombPos.y);
                    AddBombIfNotPresent(bomb);
                }
            }
        }

        public bool IsNonBombMatchingGems()
        {
            foreach (Match match in GemMatches)
                foreach (Gem gem in match.MatchingGems)
                    if (gem.GemClass == GemClass.Common)
                        return true;

            return false;
        }

        public bool IsBombs()
        {
            foreach (Match match in GemMatches)
                if (match.IsBombs())
                    return true;

            return false;
        }

        public Gem[] GetBombs()
        {
            foreach (Match match in GemMatches)
                foreach (Gem gem in match.MatchingGems)
                    if (gem.GemClass == GemClass.Special && gem.GemSpecialType == GemSpecialType.Bomb)
                        _matchingBombs.Add(gem);

            return _matchingBombs.ToArray();
        }

        //private

        private void AddBombIfNotPresent(Gem bomb)
        {
            if (IsSameBomb(bomb)) return;
            
            NewBombs.Add(bomb);
        }

        private bool IsSameBomb(Gem bombToAdd)
        {
            foreach (Gem bomb in NewBombs)
                if (bomb.Pos == bombToAdd.Pos)
                    return true;

            return false;
        }
    }
}