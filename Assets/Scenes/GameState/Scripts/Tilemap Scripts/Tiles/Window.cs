using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Objects;
using Scenes.GameState.Scripts.Tilemap_Scripts.Interfaces;
using SuperTiled2Unity;
using UnityEngine;

namespace Scenes.GameState.Scripts.Tilemap_Scripts {
    public class Window : TileHandlerBase, IInteractable, IOpenable, ICloseable{
        private bool isCLosed = true;
        public SuperTile openTile;
        public SuperTile closedTile;
        public void Interact(Entity entity)
        {
            Toggle();
        }

        public void Toggle() {
            if (isCLosed) {
                Open();
            } else {
                Close();  
            }
        }

        public void Open() {
            isCLosed = false;
            Tile = openTile;
            gameObject.name = "Open window";
            gameObject.layer = LayerMask.NameToLayer("Ignore Light And Default");
        }

        public void Close() {
            isCLosed = true;
            Tile = closedTile;
            gameObject.name = "Closed window";
            gameObject.layer = LayerMask.NameToLayer("Ignore Light");
        }
    }
}