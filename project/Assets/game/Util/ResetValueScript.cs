using UnityEngine;

namespace Amheklerior.Gravity.Util {

    public class ResetValueScript : MonoBehaviour {

        [SerializeField] private IntVariable reference;
        public void ResetValue() => reference.ResetValue();
    }
}
