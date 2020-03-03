using System.Collections.Generic;
using UnityEngine;
using UnityConnectionLayer.Pooling;

namespace Amheklerior.Gravity.Blackhole {

    public class BlackholesManager : MonoBehaviour {

        [Header("Settings:")]
        [Space]
        [SerializeField] private float spawnAreaRadius;
        [SerializeField] private float immutableAreaRadius;
        [SerializeField] private int maxNumberOfActiveBlackholes;
        [SerializeField] private int maxNumberOfAttempts;
        [Space]

        private GameObjectPool _blackholesPool;
        private List<GravitySystem> _activeBlackholesGravitySystems;
        private Transform _transform;

        private void Awake() {
            _transform = transform;
            _activeBlackholesGravitySystems = new List<GravitySystem>(maxNumberOfActiveBlackholes);
            _blackholesPool = GetComponentInParent<GameObjectPool>();
            _blackholesPool.OnGet += (GameObject instance) => {
                instance.transform.position = GetRandomPosition();
                instance.SetActive(true);
            };
            _blackholesPool.OnRelease += (GameObject instance) => {
                instance.transform.position = Vector3.zero;
                instance.SetActive(false);
            };
        }

        private void Start() => FillAllSpaceWithBlackholes();
        private void OnTriggerExit2D(Collider2D collision) {
            _activeBlackholesGravitySystems.Remove(collision.GetComponent<GravitySystem>());
            _blackholesPool.Release(collision.gameObject);
            FillNonVisibleSpaceWithBlackholes();
        }

        public void FillAllSpaceWithBlackholes() {
            int currentAttempts = 0;
            while (currentAttempts < maxNumberOfAttempts && _activeBlackholesGravitySystems.Count < _activeBlackholesGravitySystems.Capacity) {
                GravitySystem newBlackhole = _blackholesPool.Get().GetComponent<GravitySystem>();
                if (!IsTooCloseToAnotherBlackhole(newBlackhole)) {
                    _activeBlackholesGravitySystems.Add(newBlackhole);
                    currentAttempts = 0;
                } else {
                    currentAttempts++;
                    _blackholesPool.Release(newBlackhole.gameObject);
                }
            }
        }

        public void FillNonVisibleSpaceWithBlackholes() {
            int currentAttempts = 0;
            while (currentAttempts < maxNumberOfAttempts && _activeBlackholesGravitySystems.Count < _activeBlackholesGravitySystems.Capacity) {
                GravitySystem newBlackhole = _blackholesPool.Get()?.GetComponent<GravitySystem>();
                if (newBlackhole == null) return;
                if (!IsTooCloseToAnotherBlackhole(newBlackhole) && !IsInImmutableArea(newBlackhole)) {
                    _activeBlackholesGravitySystems.Add(newBlackhole);
                    currentAttempts = 0;
                } else {
                    currentAttempts++;
                    _blackholesPool.Release(newBlackhole.gameObject);
                }
            }
        }

        private Vector2 GetRandomPosition() => Random.insideUnitCircle * spawnAreaRadius + (Vector2)_transform.position;

        private bool IsTooCloseToAnotherBlackhole(GravitySystem newBlackhole) {
            foreach (GravitySystem blackhole in _activeBlackholesGravitySystems) {
                if (AreTooClose(newBlackhole, blackhole)) return true;
            }
            return false;
        }

        private bool AreTooClose(GravitySystem newBlackhole, GravitySystem blackhole) => 
            (newBlackhole.InfluenceRadius + blackhole.InfluenceRadius) > (newBlackhole.CenterOfGravity - blackhole.CenterOfGravity).magnitude;

        private bool IsInImmutableArea(GravitySystem newBlackhole) =>
            (newBlackhole.CenterOfGravity - (Vector2) transform.position).magnitude < immutableAreaRadius;

        /* DEBUG
        private void OnDrawGizmos() {
            UnityEditor.Handles.DrawWireDisc(_transform.position, Vector3.back, immutableAreaRadius);
            UnityEditor.Handles.DrawWireDisc(_transform.position, Vector3.back, spawnAreaRadius);
        }
        */
    }
}
