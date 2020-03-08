using UnityEngine;
using UnityCommonLibrary.Util;
using Amheklerior.Gravity.Util;

namespace Amheklerior.Gravity.Player {

    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceshipController : MonoBehaviour {

        private Vector3 _initialScale;

        #region Animations & sounds

        private static readonly string IS_SPRINTING = "isSprinting";
        private static readonly string DEAD = "dead";
        private static readonly string VICTORY = "victory";
        private Animator _anim;
        
        [SerializeField] private AudioClip turboEngineSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip victorySound;
        private AudioSource _audio;

        #endregion

        #region Setup

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _transform = transform;
            _initialScale = _transform.localScale;
            _anim = GetComponent<Animator>();
            _audio = GetComponent<AudioSource>();

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

            _rigidbody.drag = 0.8f;
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
                _anim.SetBool(IS_SPRINTING, true);
                Move();
                OnEngineUsage();
            } else if (_overheatedTimer.IsCountDownOver()) {
                _anim.SetBool(IS_SPRINTING, false);
                OnEngineRest();
            } else {
                _anim.SetBool(IS_SPRINTING, false);
                _overheatedTimer.CountdownBy(Time.fixedDeltaTime);
            }
            if (_rotationalDirection != 0f) Rotate();
        }
        
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Blackhole center")) {
                _rigidbody.drag = 50f;
                _anim.SetTrigger(DEAD);
            }
        }

        public void PlayIdleSound() {
            _audio.Stop();
        }
        public void PlayEngineSound() {
            _audio.loop = true;
            _audio.clip = turboEngineSound;
            _audio.Play();
        }
        public void PlayDeathSound() {
            _audio.loop = false;
            _audio.clip = deathSound;
            _audio.Play();
        }
        public void PlayVictorySound() {
            _audio.loop = false;
            _audio.clip = victorySound;
            _audio.Play();
        }


        #region Game over / Victory

        [SerializeField] private UnityConnectionLayer.EventSystem.Event gameOverEvent;
        [SerializeField] private UnityConnectionLayer.EventSystem.Event victoryEvent;

        public void GameOver() => gameOverEvent.Raise();
        public void Victory() => victoryEvent.Raise();

        public void TriggerVictoryAnimation() => _anim.SetTrigger(VICTORY);

        #endregion


    }
}