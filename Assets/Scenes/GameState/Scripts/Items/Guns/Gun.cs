// using System;
// using GlobalNamespace;
// using Scenes.GameState.Scripts.Players;
// using Scenes.GameState.Scripts.Tilemap_Scripts;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.Rendering;
//
// namespace Scenes.GameState.Scripts.Items.Guns {
//     public class Gun : Item{
//          public enum EFireType {
//             AUTO,
//             SEMI_AUTO
//         }
//         //protected GunSpriteBehaviour gunSpriteBehaviour;
//         protected internal Vector2 bulletTempRotationVec = new Vector2(1,1);
//         protected internal GunMagazine insertedMagazine;
//         private float _reloadTime = 2;
//         public float ReloadTime {
//             get => _reloadTime;
//             set => _reloadTime = value;
//         }
//
//         public EFireType FireType {
//             get => _fireType;
//             set => _fireType = value;
//         }
//         private EFireType _fireType = EFireType.SEMI_AUTO;
//
//         public Gun(int uid, String tileName, String itemName) : base(uid, tileName, itemName) {
//             spriteWidth = 0.4f;
//             spiteHeight = 0.4f;
//         }
//
//         public void Reload(GunMagazine mag){
//             if (insertedMagazine != null){
//                 insertedMagazine.OnUnInsert();
//             }
//             if (mag == null) {
//                 insertedMagazine = null;
//                 return;
//             }
//             mag.OnInsert(this);
//             insertedMagazine = mag;
//         }
//
//         public bool HasMagazine(){
//             return insertedMagazine != null;
//         }
//
//         public GunMagazine GetMagazine(){
//             return insertedMagazine;
//         }
//
//         public bool FireBullet(){
//             if (IsOwnedByPlayer()) {
//                 if (!IsEquipped()) return false;
//                 if (insertedMagazine == null || insertedMagazine.CurrentAmount == 0) {
//                     CLogger.Instance.Log("[Player(" + Owner.ToString() + "):fire] Not enough ammo (" + ((insertedMagazine == null) ? "no magazine in gun" : insertedMagazine.CurrentAmount) + ")");
//                     return false;
//                 }
//                 insertedMagazine.OnFire();
//             }
//             bulletTempRotationVec = Quaternion.Euler(0, 0, ((Player)Owner).itemRotation) * Vector2.one;
//             //gunSpriteBehaviour.onFire();
//             new Bullet(TileResolver.getTile("bullet"), owner.getPosition(), bulletTempRotationVec);
//             return true;
//         }
//         
//         public override Rigidbody2D Allocate(Vector2 position){
//             onDrop();
//             prepareForRendering();
//
//             physicalBody = bodyResolver.itemBody(position.x, position.y, this);
//             new Box2dBehaviour(physicalBody, gameObject);
//
//             gameObject.setEnabled(true);
//             if (mouseHandler != null)
//                 mouseHandler.setPosition(getPosition().x - mouseHandler.getWidth()/2f, getPosition().y - mouseHandler.getHeight()/2f);
//             
//             return physicalBody;
//         }
//
//         @Override
//         public void prepareForRendering() {
//             if (gameObject == null)
//                 gameObject = new GameObject(itemName, false, unBox);
//
//             if (hud == null)
//                 return;
//
//             if (gunSpriteBehaviour == null || gunSpriteBehaviour.getState().equals(BehaviourState.DESTROYED))
//                 createSpriteBehaviour();
//                     //gunSpriteBehaviour = new GunSpriteBehaviour(GO, this, spriteWidth, spiteHeight, tile.getTextureRegion(), Globals.DEFAULT_RENDER_ORDER);
//
//                 gunSpriteBehaviour.setRenderOrder(Globals.PLAYER_RENDER_ORDER);
//
//             if (isEquipped()) {}
//             else {
//                 if (hud != null && mouseHandler == null) {
//                     mouseHandler = new DebugUI.Table();
//                     mouseHandler.setSize(spriteWidth - 0.1f, spiteHeight - 0.1f);
//                     mouseHandler.setTouchable(Touchable.enabled);
//                     mouseHandler.addListener(new ClickListener() {
//                         @Override
//                         public void enter(InputEvent event, float x, float y, int pointer, @Null Actor fromActor) {
//                             super.enter(event, x, y, pointer, fromActor);
//                             hud.debugEntries.put(stringID + "_ClickListener", "Pointing at " + stringID + " at " + getPosition());
//                             hud.showItemInfoWindow(Gun.this);
//                         }
//
//                         @Override
//                         public void exit(InputEvent event, float x, float y, int pointer, @Null Actor toActor) {
//                             super.exit(event, x, y, pointer, toActor);
//                             hud.debugEntries.removeKey(stringID + "_ClickListener");
//                             hud.hideItemInfoWindow(Gun.this);
//                         }
//                     });
//                 }
//                 if (hud != null)
//                     gameStage.addActor(mouseHandler);
//             }
//         }
//
//         protected void createSpriteBehaviour(){
//             gunSpriteBehaviour = new GunSpriteBehaviour(gameObject, this, spriteWidth, spiteHeight, tile.getTextureRegion(), Globals.DEFAULT_RENDER_ORDER);
//         }
//
//         public boolean isOwnedByClientPlayer(){
//             return owner instanceof Player;
//         }
//
//         @Override
//         public void onEquip(Player player){
//             super.onEquip(player);
//             prepareForRendering();
//             gameObject.setEnabled(true);
//         }
//
//         @Override
//         public void onUnequip() {
//             isEquipped = false;
//             if (gameObject != null)
//                 gameObject.setEnabled(false);
//         }
//     }
// }