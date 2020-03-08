using UnityEngine;
using Amheklerior.Gravity.Util;

namespace Amheklerior.Gravity.UI {

    public class EngineHeatUIScript : MonoBehaviour {

        [SerializeField] private SpriteRenderer heatFillSprite;
        [SerializeField] private FloatVariable currentEngineHeat;

        private float _maxFillValue = 425f;
        private float _engineMaxHeat = 30f;
        private float Ratio => _maxFillValue / _engineMaxHeat;

        private void Update() {
            heatFillSprite.size = new Vector2(currentEngineHeat.CurrentValue * Ratio, heatFillSprite.size.y);
        }
        
    }
}