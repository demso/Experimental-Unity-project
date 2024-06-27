using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Objects;
using Scenes.GameState.Scripts.Players;
using Scenes.GameState.Scripts.Tilemap_Scripts;
using UnityEngine;

namespace Scenes.GameState.Scripts.Items {
    public class ItemComponent : MonoBehaviour, IInteractable
    {
        public Item Item;
        public void Interact(Entity entity) {
            if (entity is Player player)
                Item.OnInteraction(player);
        }
    }
}