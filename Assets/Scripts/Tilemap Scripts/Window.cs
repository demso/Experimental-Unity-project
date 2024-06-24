using Entities;
using SuperTiled2Unity;
using Tilemap_Scripts.Interfaces;
using UnityEngine;

namespace DefaultNamespace.Tilemap_Scripts {
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