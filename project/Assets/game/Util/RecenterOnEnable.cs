using UnityEngine;

namespace Amheklerior.Gravity.Util {

    public class RecenterOnEnable : MonoBehaviour {

        private Transform _transform;

        private void Awake() => _transform = transform; 
        private void OnEnable() => _transform.position = Vector3.zero;

    }
}
