using Match3Test.Board.Model;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Game.Settings
{
    [CreateAssetMenu(menuName = "Match3Test/GameSettings", fileName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private GemView[] regularGemPrefabs;
        [SerializeField] private GemView[] specialGemPrefabs;

        public GemView GetRegularGemPrefabOfType(GemColor gemColor)
        {
            foreach (GemView regularGemPrefab in regularGemPrefabs)
                if (regularGemPrefab.GemColor == gemColor)
                    return regularGemPrefab;

            Debug.LogError($"Cannot find regular gem of color {gemColor}");
            return null;
        }

        public GemView GetSpecialGemPrefabOfType(GemSpecialType gemSpecialType)
        {
            foreach (GemView specialGemPrefab in specialGemPrefabs)
                if (specialGemPrefab.GemSpecialType == gemSpecialType)
                    return specialGemPrefab;

            Debug.LogError($"Cannot find special gem of type {gemSpecialType}");
            return null;
        }
    }
}