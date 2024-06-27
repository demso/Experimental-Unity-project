using System.Collections.Generic;
using Scenes.GameState.Scripts.Objects;
using UnityEngine;

namespace Scenes.GameState.Scripts.Players {
    public partial class PlayerHandler {
        private List<MonoBehaviour> closeObjects = new List<MonoBehaviour>();

        private void OnTriggerEnter2D(Collider2D other) {
            var interactable = (MonoBehaviour) other.gameObject.GetComponent<IInteractable>();
            if (interactable != null)
                closeObjects.Add(interactable);
        }

        private void OnTriggerExit2D(Collider2D other) {
            closeObjects.Remove(other.gameObject.GetComponent<MonoBehaviour>());
        }
    }
}