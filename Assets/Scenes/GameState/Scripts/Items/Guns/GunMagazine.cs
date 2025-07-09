// using System;
// using System.Collections.Generic;
//
// namespace Scenes.GameState.Scripts.Items.Guns {
//     public class GunMagazine : Item {
//         public int Capacity { get; set; } = 10;
//         public int CurrentAmount { get; set; }
//         public List<string> GunTypes { get; } = new List<string>();
//         protected internal Gun insertedIn; // null if not inserted in
//
//         public GunMagazine(int uid, string iId, string itemName) : base(uid, iId, itemName) {
//             CurrentAmount = Capacity;
//         }
//
//         public void OnInsert(Gun gun){
//             insertedIn = gun;
//             Owner.RemoveItem(this);
//         }
//
//         public void OnUnInsert(){
//             if (CurrentAmount == 0) {
//                 Dispose();
//             } else {
//                 GetGun().Owner.TakeItem(this);
//             }
//             insertedIn = null;
//         }
//
//         public bool IsInserted(){
//             return insertedIn != null;
//         }
//
//         public Gun GetGun(){
//             return insertedIn;
//         }
//
//         public void AddGunTypes(params string[] gunTypes){
//             GunTypes.AddRange(GunTypes);
//         }
//
//         public int OnFire(){
//             if (CurrentAmount > 0)
//                 CurrentAmount--;
//             return CurrentAmount;
//         }
//     }
// }