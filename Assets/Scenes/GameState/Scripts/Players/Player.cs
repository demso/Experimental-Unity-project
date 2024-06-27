using System;
using System.Collections.Generic;
using GlobalNamespace;
using JetBrains.Annotations;
using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Items;
using Scenes.GameState.Scripts.Objects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scenes.GameState.Scripts.Players {
    public class Player : Entity, IStorage {
        public float normalSpeed = 3f;
        [CanBeNull] internal IInteractable closestObject;

        List<Item> inventoryItems = new List<Item>();

        public Item equipedItem;

        public PlayerHandler playerHandler;

        internal float normalSpeedMultiplier = 1;
        internal float currentSpeedMultiplier = 1;
        internal float runMultiplier = 1.5f;
        internal float sneakMultiplier = 0.5f;

        public float itemRotation;

        public bool needsReload;
        public bool isReloading;

        public float reloadProgress = 1;
        public float reloadFactor = 1f;

        [CanBeNull]
        public IInteractable GetClosestObject() {
            return closestObject;
        }

        public void TakeItem(Item item) {
            item.OnTaking(this);
            item.RemoveFromWorld();
            inventoryItems.Add(item);
            //GameState.instance.hud.updateInvHUDContent();
        }

        public void RemoveItem(Item item) {
            item.OnDrop();
            if (equipedItem == item)
                UneqipItem();
            inventoryItems.Remove(item);
            //GameState.instance.hud.updateInvHUDContent();
        }

        // public boolean throwGrenade(long time){
        //     Grenade item = getItemOfType(Grenade.class);
        //     if (item != null) {
        //         item.fire(time);
        //         return true;
        //     }
        //
        //     return false;
        // }

        //         public void reload(){
        //         if (equipedItem instanceof Gun gun) {
        //
        //                 needsReload = true;
        // //            if (magaz != null) {
        // //                gun.reload(magaz);
        // //            } else {
        // //                gun.reload(null);
        // //            }
        //         }
        //     }

        public void Heal(float hp) {
            if (hp <= 0)
                return;
            Hp += hp;
            if (Hp > MaxHp)
                Hp = MaxHp;
        }

        // public void AutoHeal(){
        //     if (Math.Abs(Hp - MaxHp) < 0.05f)
        //         return;
        //     Meds meds = GetItemOfType(Meds.class);
        //     if (meds != null) {
        //         meds.use();
        //     }
        //     else
        //         HandyHelper.instance.log("[ClientPlayer:autoHeal] No Meds found");
        // }

        public List<Item> GetInventoryItems() {
            return inventoryItems;
        }

        public void SetInventoryItems(params Item[] items) {
            inventoryItems.Clear();
            if (items != null && items.Length > 0)
                inventoryItems.AddRange(items);
        }

        public bool InventoryContains(Item item) {
            return GetInventoryItems().Contains(item);
        }

        public void EquipItem(Item item) {
            if (item == null)
                return;
            if (!InventoryContains(item))
                TakeItem(item);
            if (equipedItem != null)
                equipedItem.OnUnequip();
            equipedItem = item;
            item.OnEquip(this);
            CLogger.Instance.Log(item.uid + " equipped by " + Name);
        }

        public Item UneqipItem() {
            if (equipedItem == null)
                return null;
            equipedItem.OnUnequip();
            Item tmpItem = equipedItem;
            equipedItem = null;
            return tmpItem;
        }

        public override void Kill() {
            base.Kill();
            CLogger.Instance.Log("Oh no im killed!");
        }

        public void Revive() {
            Hp = MaxHp;
            IsAlive = true;
            CLogger.Instance.Log("Player revived");
        }

        // public bool Fire() {
        //     if (equipedItem != null && equipedItem instanceof Gun gun) {
        //         return gun.fireBullet();
        //     }
        //     return false;
        // }

        public bool Interact() {
            if (closestObject != null) {
                var obj = (IInteractable)closestObject;
                obj.Interact(this);
                return true;
            }

            return false;
        }

        public override string ToString() {
            return Name;
        }

        private void OnDestroy() {
            if (equipedItem != null) {
                UneqipItem();
            }
            
            foreach (var item in inventoryItems.ToArray()) {
                item.Dispose();
            }
        }
    }
}

