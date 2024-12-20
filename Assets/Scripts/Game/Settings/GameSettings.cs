using Match3Test.Board.Model;
using Match3Test.Utility;
using Match3Test.Utility.Pooling;
using Match3Test.Views.Gems;
using UnityEngine;

namespace Match3Test.Game.Settings
{
    [CreateAssetMenu(menuName = "Match3Test/GameSettings", fileName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public const int MinMatchingRegularTiles = 3;

        [SerializeField] private GemView[] regularGemPrefabs;
        [SerializeField] private GemView[] bombPrefabs;
        [SerializeField] private GameObject bgTilePrefab;
        [SerializeField] private float gemSpeed;
        [SerializeField] private float gemDropHeight;
        [SerializeField] private float scoreSpeed;
        [SerializeField] private int commonGemPrefetchCount = 20;
        [SerializeField] private int bombPrefetchCount = 10;
        [SerializeField] private float bombExplosionDelay = 0.2f;
        [SerializeField] private float bombDestructionDelay = 0.2f;

        public GameObject BgTilePrefab => bgTilePrefab;
        public float GemSpeed => gemSpeed;
        public float GemDropHeight => gemDropHeight;
        public float ScoreSpeed => scoreSpeed;
        public float BombExplosionDelay => bombExplosionDelay;
        public float BombDestructionDelay => bombDestructionDelay;

        public GemView GetRegularGemPrefabOfType(GemColor gemColor)
        {
            foreach (GemView regularGemPrefab in regularGemPrefabs)
                if (regularGemPrefab.GemColor == gemColor)
                    return regularGemPrefab;

            Debug.LogError($"Cannot find regular gem of color {gemColor}");
            return null;
        }

        public GemView GetRandomRegularGemPrefab() => regularGemPrefabs[Random.Range(0, regularGemPrefabs.Length)];
        public GemView[] GetRandomizedGemPrefabs()
        {
            GemView[] prefabsCopy = (GemView[])regularGemPrefabs.Clone();
            ArrayHelper.ShuffleArray(prefabsCopy);

            return prefabsCopy;
        }

        public void IniPrefabPool()
        {
            foreach (GemView regularGemPrefab in regularGemPrefabs)
            {
                IPooledPrefab pooledPrefab = regularGemPrefab.GetComponent<IPooledPrefab>();
                if (pooledPrefab == null)
                {
                    Debug.LogError($"Prefab {regularGemPrefab} contains no components implementing IPooledPrefab interface");
                    continue;
                }

                pooledPrefab.InitPool(commonGemPrefetchCount);
            }
            
            foreach (GemView bombPrefab in bombPrefabs)
            {
                IPooledPrefab pooledPrefab = bombPrefab.GetComponent<IPooledPrefab>();
                if (pooledPrefab == null)
                {
                    Debug.LogError($"Prefab {bombPrefab} contains no components implementing IPooledPrefab interface");
                    continue;
                }

                pooledPrefab.InitPool(bombPrefetchCount);
            }
        }

        public GemView GetBombPrefabOfColor(GemColor gemColor)
        {
            foreach (GemView bombPrefab in bombPrefabs)
                if (bombPrefab.GemColor == gemColor) return bombPrefab;

            Debug.LogError($"Cannot find bomb of color {gemColor}");
            return null;
        }
    }
}