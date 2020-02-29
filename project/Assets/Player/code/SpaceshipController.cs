using UnityEngine;
using UnityCommonLibrary.Util;

namespace Amheklerior.Gravity.Player {

    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceshipController : MonoBehaviour {
        
        #region Setup

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = transform;
            
            _inputControls = new InputControls();
            _inputControls.Gameplay.Move.performed += ctx => _isMovingInputProvided = true;
            _inputControls.Gameplay.Move.canceled += ctx => _isMovingInputProvided = false;
            _inputControls.Gameplay.Rotate.performed += ctx => _rotationalDirection = -ctx.ReadValue<float>();
            _inputControls.Gameplay.Rotate.canceled += ctx => _rotationalDirection = 0f;
        }

        private void OnEnable() => _inputControls.Gameplay.Enable();
        private void OnDisable() => _inputControls.Gameplay.Disable();

        #endregion

        #region Movement

        [Header("Movement stats:")]
        [SerializeField] [Range(10f, 50f)] private float movementSpeed = 25f;
        [SerializeField] [Range(1f, 5f)] private float rotationalSpeed = 1f;

        private InputControls _inputControls;
        private Rigidbody2D _rigidbody;
        private Transform _transform;
        private bool _isMovingInputProvided;
        private float _rotationalDirection;

        private void Move() => _rigidbody.AddForce(transform.up * movementSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
        private void Rotate() => _rigidbody.AddTorque(_rotationalDirection * rotationalSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);

        #endregion
        
        #region Engine Heating management
        [Space]
        [Header("Engine heating settings:")]
        [SerializeField] private float engineMaxOverheatTreshold;
        [SerializeField] private float heatupRate;
        [SerializeField] private float cooldownRate;
        [SerializeField] private float _engineHeat = 0f; // TODO hide from inspector and convert to a serialized global accessible variable...
        [SerializeField] private bool IsEngineOverheated => _engineHeat >= engineMaxOverheatTreshold; // TODO hide from inspector 
        private ITimer _overheatedTimer = new Timer(1d);

        public void OnEngineUsage() {
            _engineHeat = Mathf.Clamp(_engineHeat + heatupRate * Time.fixedDeltaTime, 0f, engineMaxOverheatTreshold);
            if(IsEngineOverheated) _overheatedTimer.ResetTimer();
        }
        public void OnEngineRest() {
            if (_overheatedTimer.IsCountDownOver()) {
                _engineHeat = Mathf.Clamp(_engineHeat - cooldownRate * Time.fixedDeltaTime, 0f, engineMaxOverheatTreshold);
            }
        }

        #endregion

        private void FixedUpdate() {
            if (_isMovingInputProvided && !IsEngineOverheated) {
                Move();
                OnEngineUsage();
            } else if (_overheatedTimer.IsCountDownOver()) {
                OnEngineRest();
            } else {
                _overheatedTimer.CountdownBy(Time.fixedDeltaTime);
            }
            if (_rotationalDirection != 0f) Rotate();
        }
        
    }
}