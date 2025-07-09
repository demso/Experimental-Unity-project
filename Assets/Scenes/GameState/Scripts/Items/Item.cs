using Scenes.GameState.Scripts.Factories;
using Scenes.GameState.Scripts.Objects;
using Scenes.GameState.Scripts.Players;
using Scenes.GameState.Scripts.Tilemap_Scripts;
using SuperTiled2Unity;
using UnityEngine;

namespace Scenes.GameState.Scripts.Items {
    public class Item : ScriptableObject {
        public SuperTile tile;

        public Rigidbody2D PhysicalBody { get; private set; }

        //public Table MouseHandler { get; private set; }
        protected internal bool isEquipped = false;
        protected internal IStorage Owner { get; private set; }

        //public UnBox unBox;

        //public HUD hud;
        public int uid;

        //public Stage gameStage;
        public string StringID { get; private set; } = "{No tile name}"; //string item identifier
        public string itemName = "{No name item}";

        public string description =
            "First you must develop a Skin that implements all the widgets you plan to use in your layout. You can't use a widget if it doesn't have a valid style. Do this how you would usually develop a Skin in Scene Composer.";

        public float spriteWidth = 0.4f;
        public float spriteHeight = 0.4f;

        protected internal GameObject gameObject;
        //protected SpriteBehaviour spriteBehaviour;
        protected internal ItemsFactory factory;
        public bool IsDisposed { get; private set; }

        public void Init(int uid, SuperTile tile, string itemName) {
            this.uid = uid;
            this.tile = tile;
            this.StringID = tile.GetPropertyOrDefault("name", "no_name");
            this.itemName = itemName;
        }

        public void Init(int uid, string iId, string itemName) {
            Init(uid, TileResolver.GetTile(iId), itemName);
        }

        public Rigidbody2D Allocate(Vector2 position) {
            if (gameObject == null || !gameObject) {
                OnDrop();
                PrepareForRendering();
                PhysicalBody = factory.bodyFactory.ItemBody(position.x, position.y, ref gameObject);
                gameObject.AddComponent<ItemComponent>().Item = this;
                gameObject.name = ToString();
                var spriteObject = new GameObject("SpriteObject");
                spriteObject.transform.parent = gameObject.transform;
                var spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
                var sprite = tile.m_Sprite;
                spriteRenderer.sprite = sprite;
                spriteRenderer.drawMode = SpriteDrawMode.Sliced;
                spriteRenderer.size = new Vector2(spriteWidth, spriteHeight);
                spriteObject.transform.localPosition = -spriteRenderer.size / 2;
            } else {
                throw new System.Exception("Item already allocated");
                //gameObject.transform.position = position;
            }
            
            // if (MouseHandler != null)
            //     MouseHandler.SetPosition(GetPosition().X - MouseHandler.GetWidth() / 2f,
            //         GetPosition().Y - MouseHandler.GetHeight() / 2f);
            return PhysicalBody;
        }

        public void PrepareForRendering() {
            // if (unBox != null && gameObject == null)
            //     gameObject = new GameObject(itemName, false, unBox);
            //
            // if (hud != null && spriteBehaviour == null)
            //     spriteBehaviour = new SpriteBehaviour(gameObject, spriteWidth, spriteHeight, tile.TextureRegion, Globals.ITEMS_RENDER_ORDER);
            //
            // if (hud != null && MouseHandler == null)
            // {
            //     MouseHandler = new DebugUI.Table();
            //     MouseHandler.SetSize(spriteWidth - 0.1f, spriteHeight - 0.1f);
            //     MouseHandler.SetTouchable(Touchable.Enabled);
            //     MouseHandler.AddListener(new ClickListener()
            //     {
            //         public override void Enter(InputEvent evt, float x, float y, int pointer, Actor fromActor)
            //         {
            //             base.Enter(evt, x, y, pointer, fromActor);
            //             hud.DebugEntries[StringID + "_ClickListener"] = "Pointing at " + StringID + " at " + GetPosition();
            //             hud.ShowItemInfoWindow(this);
            //         }
            //
            //         public override void Exit(InputEvent evt, float x, float y, int pointer, Actor toActor)
            //         {
            //             base.Exit(evt, x, y, pointer, toActor);
            //             hud.DebugEntries.Remove(StringID + "_ClickListener");
            //             hud.HideItemInfoWindow(this);
            //         }
            //     });
            // }
            // if (hud != null)
            //     gameStage.AddActor(MouseHandler);
        }

        // protected void CreateSpriteBehaviour() {
        //     spriteBehaviour = new SpriteBehaviour(gameObject, spriteWidth, spriteHeight, tile.TextureRegion,
        //         Globals.ITEMS_RENDER_ORDER);
        // }

        public void RemoveFromWorld() {
            // if (MouseHandler != null) {
            //     gameStage.GetActors().RemoveValue(MouseHandler, true);
            // }

            if (PhysicalBody != null) {
                ClearPhysicalBody();
            }
        }

        public Vector2? GetPosition() {
            return gameObject?.transform?.position;
        }

        public void ClearPhysicalBody() {
            Object.Destroy(gameObject);
            PhysicalBody = null;
            gameObject = null;
        }

        public string GetName() {
            return itemName ?? "";
        }

        public void OnInteraction(Player player) {
            player.TakeItem(this);
        }

        public void OnTaking(IStorage storage) {
            Owner = storage;
        }

        public void OnDrop() {
            OnUnequip();
            Owner = null;
        }

        public void OnEquip(Player player) {
            isEquipped = true;
            Owner = player;

            RemoveFromWorld();
        }

        public void OnUnequip() {
            if (!IsEquipped())
                return;
            isEquipped = false;
            // if (gameObject != null)
            //     gameObject.Enabled = false;
        }

        public bool IsEquipped() {
            return isEquipped;
        }

        public bool IsOwnedByPlayer() {
            return Owner is Player;
        }

        public void Dispose() {
            if (Owner != null)
                Owner.RemoveItem(this);
            OnDrop();
            if (gameObject != null && gameObject)
                Object.Destroy(gameObject);
            factory.OnItemDispose(this);
            RemoveFromWorld();
            IsDisposed = true;
        }

        public override string ToString() {
            return $"({uid}) {itemName} [{Owner}] ";
        }

        public override bool Equals(object obj) {
            if (obj is Item item) {
                return item.uid == uid;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}