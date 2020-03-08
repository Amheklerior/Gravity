using System.Collections.Generic;
using UnityEngine;
using UnityConnectionLayer.Pooling;

namespace Amheklerior.Gravity.Blackhole {

    public class BlackholesManager : MonoBehaviour {

        [Header("Settings:")]
        [Space]
        [SerializeField] private GameObjectPool _blackholesPool;
        [SerializeField] private GameObjectPool _collectiblesPool;
        [SerializeField] private float spawnAreaRadius;
        [SerializeField] private float immutableAreaRadius;
        [SerializeField] private int maxNumberOfActiveBlackholes;
        [SerializeField] private int maxNumberOfAttempts;
        [SerializeField] private int collectibleMinDistance;
        [SerializeField] private int maxCollectiblesToSpawnAtOnce;
        [Space]

        private List<GravitySystem> _activeBlackholesGravitySystems;
        private List<GameObject> _activeCollectiblesInTheScene;
        private Transform _transform;

        private void Awake() {
            _transform = transform;
            _activeBlackholesGravitySystems = new List<GravitySystem>(maxNumberOfActiveBlackholes);
            _activeCollectiblesInTheScene = new List<GameObject>();

            _blackholesPool.OnGet += (GameObject instance) => {
                instance.transform.position = GetRandomPosition();
                instance.SetActive(true);
            };
            _blackholesPool.OnRelease += (GameObject instance) => {
                instance.transform.position = Vector3.zero;
                instance.SetActive(false);
            };
            
            _collectiblesPool.OnGet += (GameObject instance) => {
                instance.GetComponent<CollectibleScript>().Pool = _collectiblesPool;
                instance.transform.position = GetRandomPosition();
                instance.SetActive(true);
            };
            _collectiblesPool.OnRelease += (GameObject instance) => {
                instance.transform.position = Vector3.zero;
                instance.SetActive(false);
            };
        }

        private void Start() => FillSpace();

        public void FillSpace() {
            FillAllSpaceWithBlackholes();
            SpawnCollectibles();
        }

        /*
        public void EmptySpace() {
            foreach (GravitySystem blackhole in _activeBlackholesGravitySystems) {
                _activeBlackholesGravitySystems.Remove(blackhole);
                _blackholesPool.Release(blackhole.gameObject);
            }
            foreach (GameObject collectible in _activeCollectiblesInTheScene) {
                _activeCollectiblesInTheScene.Remove(collectible);
                _collectiblesPool.Release(collectible);
            }
        }
        */

        private void OnTriggerExit2D(Collider2D collision) {
            if (collision.gameObject.CompareTag("Blackhole")) {
                _activeBlackholesGravitySystems.Remove(collision.GetComponent<GravitySystem>());
                _blackholesPool.Release(collision.gameObject);
                FillAllSpaceWithBlackholes();
            } else if (collision.gameObject.CompareTag("Collectible") && collision.gameObject.activeInHierarchy) {
                _collectiblesPool.Release(collision.gameObject);
                SpawnCollectibles();
            }
        }

        public void FillAllSpaceWithBlackholes() {
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
            (newBlackhole.CenterOfGravity - (Vector2) _transform.position).magnitude < immutableAreaRadius;


        
        private void SpawnCollectibles() {
            for (int i = 0; i < maxCollectiblesToSpawnAtOnce; i++) {
                Transform collectible = _collectiblesPool.Get()?.transform;
                if (collectible == null) return;
                if (IsTooCloseToABlackhole(collectible) || IsInVisibleArea(collectible)) {
                    _blackholesPool.Release(collectible.gameObject);
                } else {
                    _activeCollectiblesInTheScene.Add(collectible.gameObject);
                }
            }
        }

        private bool IsInVisibleArea(Transform collectible) => (collectible.position - _transform.position).magnitude < immutableAreaRadius;

        private bool IsTooCloseToABlackhole(Transform collectible) {
            Vector2 position = collectible.position;
            foreach (GravitySystem blackhole in _activeBlackholesGravitySystems) {
                if (collectibleMinDistance > (position - blackhole.CenterOfGravity).magnitude) return true;
            }
            return false;
        }
        
    }
}
