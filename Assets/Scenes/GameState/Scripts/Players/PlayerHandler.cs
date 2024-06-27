using Scenes.GameState.Scripts.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scenes.GameState.Scripts.Players {
    public partial class PlayerHandler : MonoBehaviour {
        Vector2 moveImpulse = Vector2.zero;
        private Rigidbody2D body;
        private Texture2D selection;
        private SpriteRenderer selectionRenderer;
        private GameObject selectionGO;
        private Players.Player _player;

        private void Awake() {
            body = GetComponent<Rigidbody2D>();
            _player = GetComponent<Player>();
            selection = Resources.Load<Texture2D>("Visual/selection");
            selectionGO = new GameObject();
            selectionGO.SetActive(false);
            selectionGO.name = "Selection Sprite";
            selectionGO.hideFlags = HideFlags.HideInHierarchy;
            selectionRenderer = selectionGO.AddComponent<SpriteRenderer>();
            selectionRenderer.drawMode = SpriteDrawMode.Sliced;
            Sprite sprite = Sprite.Create(selection, new Rect(0, 0, selection.width, selection.height),
                new Vector2(0.5f, 0.5f), 32);
            selectionRenderer.sprite = sprite;
            selectionRenderer.sortingOrder = 3;
        }

        void FixedUpdate() {
            moveImpulse.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            float speedMult = 1;
            if (Input.GetKey(KeyCode.LeftShift))
                speedMult = _player.runMultiplier;
            else if (Input.GetKey(KeyCode.C))
                speedMult = _player.sneakMultiplier;
            _player.currentSpeedMultiplier = speedMult;
            moveImpulse = ((moveImpulse * _player.normalSpeed * speedMult * body.mass) / (1 - Time.fixedDeltaTime * body.drag)) *
                          Time.fixedDeltaTime * 10f;
            body.AddForce(moveImpulse, ForceMode2D.Impulse);
        }

        private void Update() {
            CloseObject? closest = GetClosestObject();
            
            _player.closestObject = (IInteractable) closest?.Interactable ;
            if (closest != null) {
                selectionGO.transform.position = closest.Value.Position;
                Vector2 spriteSize = closest.Value.Collider.bounds.size;
                spriteSize.x = Mathf.Max(spriteSize.x, 0.5f);
                spriteSize.y = Mathf.Max(spriteSize.y, 0.5f);
                selectionRenderer.size = spriteSize;
                selectionGO.SetActive(true);
                if (Keyboard.current.eKey.wasPressedThisFrame) {
                    ((IInteractable)closest.Value.Interactable).Interact(_player);
                }
            } else {
                selectionGO.SetActive(false);  
            }
        }

        private CloseObject? GetClosestObject() {
            if (_closeObjects.Count == 0)
                return null;
            _closeObjects.Sort((a, b) => {
                Vector2 plPos = gameObject.transform.position;

                if ((plPos - a.Position).magnitude > (plPos - b.Position).magnitude) {
                    return 1;
                }

                if ((plPos - a.Position).magnitude < (plPos - b.Position).magnitude) {
                    return -1;
                }

                return 0;
            });
            return _closeObjects[0];
        }
    }
}