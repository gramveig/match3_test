using Match3Test.Board.Model;
using Match3Test.Utility;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Game.Settings
{
    [CreateAssetMenu(menuName = "Match3Test/GameSettings", fileName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public const int MinMatchingRegularTiles = 3;

        [SerializeField] private GemView[] regularGemPrefabs;
        [SerializeField] private GemView[] specialGemPrefabs;
        [SerializeField] private GameObject bgTilePrefab;
        [SerializeField] private float gemSpeed;
        [SerializeField] private float gemDropHeight;
        [SerializeField] private float scoreSpeed;

        public GameObject BgTilePrefab => bgTilePrefab;
        public float GemSpeed => gemSpeed;
        public float GemDropHeight => gemDropHeight;
        public float ScoreSpeed => scoreSpeed;

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

        public GemView GetRandomRegularGemPrefab() => regularGemPrefabs[Random.Range(0, regularGemPrefabs.Length)];
        public GemView[] GetRandomizedGemPrefabs()
        {
            GemView[] prefabsCopy = (GemView[])regularGemPrefabs.Clone();
            ArrayHelper.ShuffleArray(prefabsCopy);

            return prefabsCopy;
        }
    }
}