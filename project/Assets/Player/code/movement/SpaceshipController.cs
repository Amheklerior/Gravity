using UnityEngine;

namespace Amheklerior.Gravity.Player {

    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceshipController : MonoBehaviour {

        [SerializeField] [Range(10f, 50f)] private float movementSpeed = 25f;
        [SerializeField] [Range(1f, 5f)] private float rotationalSpeed = 1f;

        private InputControls _inputControls;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private bool _isMoving;
        private float _rotationalDirection;

        #region Setup

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = transform;
            
            _inputControls = new InputControls();
            _inputControls.Gameplay.Move.performed += ctx => _isMoving = true;
            _inputControls.Gameplay.Move.canceled += ctx => _isMoving = false;
            _inputControls.Gameplay.Rotate.performed += ctx => _rotationalDirection = -ctx.ReadValue<float>();
            _inputControls.Gameplay.Rotate.canceled += ctx => _rotationalDirection = 0f;
        }

        private void OnEnable() => _inputControls.Gameplay.Enable();
        private void OnDisable() => _inputControls.Gameplay.Disable();

        #endregion

        private void FixedUpdate() {
            if (_isMoving) Move();
            if (_rotationalDirection != 0f) Rotate();
        }
        
        private void Move() => _rigidbody.AddForce(transform.up * movementSpeed * Time.deltaTime, ForceMode2D.Impulse);

        private void Rotate() => _rigidbody.AddTorque(_rotationalDirection * rotationalSpeed * Time.deltaTime, ForceMode2D.Impulse);
        
    }
}