using UnityEngine;
using UnityEngine.UI;
using Amheklerior.Gravity.Util;

namespace Amheklerior.Gravity.UI {

    public class CollectedEnergyUIScript : MonoBehaviour {

        [SerializeField] private IntVariable currentlyCollected;
        [SerializeField] private IntVariable requiredToCollect;
        [SerializeField] private Text element;

        private void Update() {
            element.text = currentlyCollected.CurrentValue + " / " + requiredToCollect.CurrentValue;
        }

    }
}