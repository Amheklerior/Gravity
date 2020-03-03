using UnityEngine;

namespace Amheklerior.Gravity.Blackhole {
    
    public class AutoRotator : MonoBehaviour {

        [SerializeField] private float rotationaSpeed = 5f;
        
        private Transform _transform;
        private Vector3 _currentRotation;

        private void Awake() {
            _transform = transform;
            _currentRotation = _transform.rotation.eulerAngles;
        }
        
        private void Update() {
            _currentRotation.z += rotationaSpeed * Time.deltaTime;
            _transform.rotation = Quaternion.Euler(_currentRotation);
        }

    }
}