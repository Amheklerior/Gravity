using UnityEngine;

namespace Amheklerior.Gravity.Level {

    public class Parallax : MonoBehaviour {

        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float parallaxEffect;

        private Vector2 _singleTileDimensions;
        private Vector2 _overallDimensions;
        private Vector2 _position;
        private Transform _transform;

        private void Start() {
            _position = transform.position;
            _singleTileDimensions = GetComponent<ParticleSystem>().shape.scale;
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

            float newX = relativeCoveredDistance.x == _position.x + _singleTileDimensions.x ? _position.x + _overallDimensions.x
                : relativeCoveredDistance.x > _position.x + _singleTileDimensions.x ? _position.x + _overallDimensions.x
                : relativeCoveredDistance.x < _position.x - _singleTileDimensions.x ? _position.x - _overallDimensions.x
                : _position.x;
            float newY = relativeCoveredDistance.y > _position.y + _singleTileDimensions.y ? _position.y + _overallDimensions.y
                : relativeCoveredDistance.y < _position.y - _overallDimensions.y ? _position.y - _overallDimensions.y
                : _position.y;
            _position = new Vector2(newX, newY);
        }
    }
}