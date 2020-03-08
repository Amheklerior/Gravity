using UnityEngine;
using UnityConnectionLayer.Pooling;
using Amheklerior.Gravity.Util;

public class CollectibleScript : MonoBehaviour {

    [SerializeField] private IntVariable currentlyCollectedItems;
    [SerializeField] private UnityConnectionLayer.EventSystem.Event itemCollectedEvent;

    private GameObjectPool _pool;

    public GameObjectPool Pool { set => _pool = value; }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            currentlyCollectedItems.CurrentValue++;
            itemCollectedEvent.Raise();
            _pool.Release(gameObject);
        }
    }
}
