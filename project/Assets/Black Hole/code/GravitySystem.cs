using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class GravitySystem : MonoBehaviour
{
    [Header("Stats:")] [Space]
    [SerializeField] private Vector2 powerMultiplierRange;
    [SerializeField] private float standardGravity;
    [SerializeField] private float standardInfluenceRadius;

    [Space]
    private Vector2 _centerOfGravity;
    [SerializeField] private float _gravityForce;
    [SerializeField] private CircleCollider2D _gravityInfluenceCircle;
    [SerializeField] private List<Rigidbody2D> attractedObjects;

    private void Awake() {
        _centerOfGravity = transform.position;
        _gravityInfluenceCircle = GetComponent<CircleCollider2D>();
    }

    private void OnEnable() {
        float multiplier = Random.Range(powerMultiplierRange.x, powerMultiplierRange.y);
        _gravityForce = standardGravity * multiplier;
        _gravityInfluenceCircle.radius = standardInfluenceRadius * multiplier;
    }

    private void FixedUpdate() {
        foreach (Rigidbody2D attractedObject in attractedObjects) ApplyGravityForceTo(attractedObject);
    }

    private void ApplyGravityForceTo(Rigidbody2D attractedObject) {
        Vector2 distanceVectorToCenterOfGravity = ComputeDistanceVectorFromCenterOfGravityTo(attractedObject.position);
        float attractionForce = GravityForceAt(distanceVectorToCenterOfGravity.magnitude);
        attractedObject.AddForce(distanceVectorToCenterOfGravity * attractionForce * Time.deltaTime);
    }
    private Vector2 ComputeDistanceVectorFromCenterOfGravityTo(Vector2 attractedObjectPosition) => attractedObjectPosition - _centerOfGravity;
    private float GravityForceAt(float distanceFromCenterOfGravity) => -_gravityForce / Mathf.Pow(distanceFromCenterOfGravity, 2f);
    
    private void OnTriggerEnter2D(Collider2D collision) => attractedObjects.Add(collision.gameObject.GetComponent<Rigidbody2D>());
    private void OnTriggerExit2D(Collider2D collision) => attractedObjects.Remove(collision.gameObject.GetComponent<Rigidbody2D>());

}
