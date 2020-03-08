using UnityEngine;
using UnityConnectionLayer.Pooling;
using Amheklerior.Gravity.Util;
using Amheklerior.Gravity.Player;

public class CollectibleScript : MonoBehaviour {

    [SerializeField] private IntVariable currentlyCollectedItems;
    [SerializeField] private IntVariable requiredEnergyGlobes;
    [SerializeField] private UnityConnectionLayer.EventSystem.Event pickupEvent;

    private GameObjectPool _pool;
    private AudioSource _audio;

    private void Awake() => _audio = GetComponent<AudioSource>();

    public GameObjectPool Pool { set => _pool = value; }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            currentlyCollectedItems.CurrentValue++;
            pickupEvent.Raise();
            if (currentlyCollectedItems.CurrentValue >= requiredEnergyGlobes.CurrentValue) {
                collision.gameObject.GetComponent<SpaceshipController>().TriggerVictoryAnimation();
            }
            _pool.Release(gameObject);
        }
    }
}
