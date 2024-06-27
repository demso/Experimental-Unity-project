using Scenes.GameState.Scripts.Factories;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.GameState.Scripts {
    public partial class GameHandler : MonoBehaviour
    {
        public BodyFactory bodyFactory;
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
