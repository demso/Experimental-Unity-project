using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Tilemap_Scripts;
using Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public float moveSpeed = 2.5f;
    Vector2 moveImpulse = Vector2.zero;
    private Rigidbody2D body;
    private Texture2D selection;
    private SpriteRenderer selectionRenderer;
    private GameObject selectionGO;
    private Player _player;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        _player = GetComponent<Player>();
        selection = Resources.Load<Texture2D>("Visual/selection");
        selectionGO = new GameObject();
        selectionGO.SetActive(false);
        selectionGO.name = "Selection Sprite";
        selectionGO.hideFlags = HideFlags.HideInHierarchy;
        selectionRenderer = selectionGO.AddComponent<SpriteRenderer>();
        Sprite sprite = Sprite.Create(selection, new Rect(0, 0, selection.width, selection.height),
            new Vector2(0.5f, 0.5f), 32);
        selectionRenderer.sprite = sprite;
        selectionRenderer.sortingOrder = 3;
    }

    void FixedUpdate() {
        moveImpulse.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveImpulse = ((moveImpulse * moveSpeed * body.mass) / (1 - Time.fixedDeltaTime * body.drag)) *
                      Time.fixedDeltaTime * 10f;
        body.AddForce(moveImpulse, ForceMode2D.Impulse);
    }

    private void Update() {
        TileHandlerBase closest = getClosestObject();
        if (closest != null) {
            selectionGO.transform.position = closest.transform.position;
            selectionGO.SetActive(true);
            if (Keyboard.current.eKey.wasPressedThisFrame) {
                ((IInteractable)closest).Interact(_player);
            }
        } else {
            selectionGO.SetActive(false);  
        }
    }

    private List<TileHandlerBase> closeObjects = new List<TileHandlerBase>();

    private void OnTriggerEnter2D(Collider2D other) {
        TileHandlerBase tileHandler = other.gameObject.GetComponent<TileHandlerBase>();
        if (tileHandler is IInteractable interactable)
        {
            closeObjects.Add(tileHandler);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        closeObjects.Remove(other.gameObject.GetComponent<TileHandlerBase>());
    }

    public TileHandlerBase getClosestObject() {
        if (closeObjects.Count == 0)
            return null;
        
        closeObjects.Sort((a, b) => {
            Vector2 plPos = gameObject.transform.position;
            Vector2 aPos = a.gameObject.transform.position;
            Vector2 bPos = b.gameObject.transform.position;

            if ((plPos - aPos).magnitude > (plPos - bPos).magnitude) {
                return 1;
            }

            if ((plPos - aPos).magnitude < (plPos - bPos).magnitude) {
                return -1;
            }

            return 0;
        });

        return closeObjects[0];
    }
}