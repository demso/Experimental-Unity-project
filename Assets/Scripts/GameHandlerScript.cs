using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameHandlerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Globals.player = GameObject.Find("Player");
        Globals.camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
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
