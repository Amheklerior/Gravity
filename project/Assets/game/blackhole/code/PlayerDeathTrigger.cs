using UnityEngine;

namespace Amheklerior.Gravity.Blackhole {

    public class PlayerDeathTrigger : MonoBehaviour {

        [SerializeField] private UnityConnectionLayer.EventSystem.Event gameOverEvent;
        private void OnTriggerEnter2D(Collider2D collision) => gameOverEvent.Raise();

    }
}