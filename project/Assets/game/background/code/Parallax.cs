using UnityEngine;

namespace Amheklerior.Gravity.Level {

    public class Parallax : MonoBehaviour {

        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float parallaxEffect;
        [SerializeField] private bool dimensionsFromSprite = true;

        private Vector2 _singleTileDimensions;
        private Vector2 _overallDimensions;
        private Vector2 _position;
        private Transform _transform;

        private void Start() {
            _position = transform.position;
            _singleTileDimensions = dimensionsFromSprite
                ? GetComponent<SpriteRenderer>().bounds.size 
                : GetComponent<ParticleSystem>().shape.scale;
            _overallDimensions = _singleTileDimensions * 3;
            _transform = transform;
        }

        private void Update() {
            MoveRelativeToTheCamera();
            RepositionIfNecessary();
        }

        private void MoveRelativeToTheCamera() {
            Vector2 coveredDistance = new Vector2(
                _cameraTransform.position.x * parallaxEffect,
                _cameraTransform.position.y * parallaxEffect);

            _transform.position = new Vector3(
                _position.x + coveredDistance.x,
                _position.y + coveredDistance.y,
                _transform.position.z);
        }

        private void RepositionIfNecessary() {
            Vector2 relativeCoveredDistance =  new Vector2(
            _cameraTransform.position.x * (1 - parallaxEffect),
            _cameraTransform.position.y * (1 - parallaxEffect));

            float newX = _position.x;
            if (relativeCoveredDistance.x > newX + _singleTileDimensions.x) {
                newX += _singleTileDimensions.x;
            } else if (relativeCoveredDistance.x < newX - _singleTileDimensions.x) {
                newX -= _singleTileDimensions.x;
            }

            float newY = _position.y;
            if (relativeCoveredDistance.y > newY + _singleTileDimensions.y) {
                newY += _singleTileDimensions.y;
            } else if (relativeCoveredDistance.y < newY - _singleTileDimensions.y) {
                newY -= _singleTileDimensions.y;
            }

            _position = new Vector2(newX, newY);
        }
    }
}