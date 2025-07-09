using System.Collections.Generic;
using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Factories;
using Scenes.GameState.Scripts.Items;
using Scenes.GameState.Scripts.Players;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Scenes.GameState.Scripts {
    public partial class GameHandler : MonoBehaviour {
        public static GameHandler Instance;
        public ItemsFactory ItemsFactory;
        public BodyFactory BodyFactory;
        public MobsFactory MobsFactory;
        public Dictionary<int, Item> Items;
        void Update()
        {
            if (Keyboard.current.equalsKey.wasPressedThisFrame)
            {
                Globals.camera.orthographicSize -= 1;
            }
            if (Keyboard.current.minusKey.wasPressedThisFrame)
            {
                Globals.camera.orthographicSize += 1;
            }
        }
    }
}
