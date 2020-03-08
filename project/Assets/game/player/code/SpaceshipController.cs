using UnityEngine;
using UnityCommonLibrary.Util;
using Amheklerior.Gravity.Util;

namespace Amheklerior.Gravity.Player {

    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceshipController : MonoBehaviour {

        private Vector3 _initialScale;

        #region Setup

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = transform;
            _initialScale = _transform.localScale;

            _inputControls = new InputControls();
            _inputControls.Gameplay.Move.performed += ctx => _isMovingInputProvided = true;
            _inputControls.Gameplay.Move.canceled += ctx => _isMovingInputProvided = false;
            _inputControls.Gameplay.Rotate.performed += ctx => _rotationalDirection = -ctx.ReadValue<float>();
            _inputControls.Gameplay.Rotate.canceled += ctx => _rotationalDirection = 0f;
        }

        private void OnEnable() {
            _transform.rotation = Quaternion.Euler(Vector3.zero);
            _transform.localScale = _initialScale; 
            _inputControls.Gameplay.Enable();
            engineCurrentHeat.CurrentValue = 0f;
        }
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
        [SerializeField] private FloatVariable engineCurrentHeat;
        [SerializeField] private bool IsEngineOverheated => engineCurrentHeat.CurrentValue >= engineMaxOverheatTreshold; 
        private ITimer _overheatedTimer = new Timer(1d);

        public void OnEngineUsage() {
            engineCurrentHeat.CurrentValue = Mathf.Clamp(engineCurrentHeat.CurrentValue + heatupRate * Time.fixedDeltaTime, 0f, engineMaxOverheatTreshold);
            if(IsEngineOverheated) _overheatedTimer.ResetTimer();
        }
        public void OnEngineRest() {
            if (_overheatedTimer.IsCountDownOver()) {
                engineCurrentHeat.CurrentValue = Mathf.Clamp(engineCurrentHeat.CurrentValue - cooldownRate * Time.fixedDeltaTime, 0f, engineMaxOverheatTreshold);
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