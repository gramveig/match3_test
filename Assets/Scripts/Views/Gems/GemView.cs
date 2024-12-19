using Match3Test.Board.Model;
using UnityEngine;

namespace Match3Test.Views.Gems
{
    public class GemView : MonoBehaviour
    {
        [SerializeField] private GemClass gemClass;
        [SerializeField] private GemColor gemColor;
        [SerializeField] private GemSpecialType gemSpecialType;

        public GemClass GemClass => gemClass;
        public GemColor GemColor => gemColor;
        public GemSpecialType GemSpecialType => gemSpecialType;
    }
}