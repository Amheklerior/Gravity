using System.Collections.Generic;
using UnityEngine;

namespace Amheklerior.Gravity.Blackhole {

    [RequireComponent(typeof(CircleCollider2D))]
    public class GravitySystem : MonoBehaviour {

        [Header("Stats:")]
        [Space]
        [SerializeField] private Vector2 powerMultiplierRange;
        [SerializeField] private float standardGravity;
        [SerializeField] private float standardInfluenceRadius;
        [Space]

        [SerializeField] private Vector2 _centerOfGravity;
        private float _gravityForce;
        private CircleCollider2D _gravityInfluenceCircle;
        private List<Rigidbody2D> _attractedObjects;

        public float InfluenceRadius => _gravityInfluenceCircle.radius;
        public Vector2 CenterOfGravity => _centerOfGravity;

        private void Awake() {
            _gravityInfluenceCircle = GetComponent<CircleCollider2D>();
            _attractedObjects = new List<Rigidbody2D>();
        }

        private void OnEnable() {
            float multiplier = Random.Range(powerMultiplierRange.x, powerMultiplierRange.y);
            _gravityForce = standardGravity * multiplier;
            _gravityInfluenceCircle.radius = standardInfluenceRadius * multiplier;
            _centerOfGravity = transform.position;
        }

        private void FixedUpdate() {
            foreach (Rigidbody2D attractedObject in _attractedObjects) ApplyGravityForceTo(attractedObject);
        }

        private void ApplyGravityForceTo(Rigidbody2D attractedObject) {
            Vector2 distanceVectorToCenterOfGravity = ComputeDistanceVectorFromCenterOfGravityTo(attractedObject.position);
            float attractionForce = GravityForceAt(distanceVectorToCenterOfGravity.magnitude);
            attractedObject.AddForce(distanceVectorToCenterOfGravity * attractionForce * Time.deltaTime);
        }
        private Vector2 ComputeDistanceVectorFromCenterOfGravityTo(Vector2 attractedObjectPosition) => attractedObjectPosition - _centerOfGravity;
        private float GravityForceAt(float distanceFromCenterOfGravity) => -_gravityForce / Mathf.Pow(distanceFromCenterOfGravity, 2f);

        private void OnTriggerEnter2D(Collider2D collision) => _attractedObjects.Add(collision.gameObject.GetComponent<Rigidbody2D>());
        private void OnTriggerExit2D(Collider2D collision) => _attractedObjects.Remove(collision.gameObject.GetComponent<Rigidbody2D>());
        
    }
}