using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using NaughtyAttributes;
using SuperTiled2Unity;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

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
