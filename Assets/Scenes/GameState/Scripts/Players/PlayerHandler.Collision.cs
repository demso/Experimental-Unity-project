using System.Collections.Generic;
using Scenes.GameState.Scripts.Objects;
using UnityEngine;

namespace Scenes.GameState.Scripts.Players {
    public partial class PlayerHandler {
        private List<CloseObject> _closeObjects = new();

        private struct CloseObject {
            public MonoBehaviour Interactable;
            public Collider2D Collider;
            public Vector2 Position;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            var interactable = (MonoBehaviour) other.gameObject.GetComponent<IInteractable>();
            if (interactable != null)
                _closeObjects.Add(new CloseObject{ Collider = other, Interactable = interactable, Position = other.transform.position });
        }

        private void OnTriggerExit2D(Collider2D other) {
            var index = _closeObjects.FindIndex(x => x.Collider == other);
            if (index >= 0) 
                _closeObjects.RemoveAt(index);
        }
    }
}