using Artics.Physics.UnityPhisicsVisualizers;
using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Tilemap_Scripts;
using UnityEngine;

namespace Scenes.GameState.Scripts.Entities {
    public class MobsFactory {
        private int _mobId = 0;
        private int MobId {
            get => ++_mobId;
            set => _mobId = value;
        }

        public Zombie SpawnZombie(Vector2 position) {
            var zombieObject = new GameObject();
            zombieObject.transform.position = position;
            zombieObject.layer = LayerMask.NameToLayer("Ignore Light");
            var body = zombieObject.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.mass = 50;
            var collider = zombieObject.AddComponent<CircleCollider2D>();
            collider.radius = 0.3f;
            collider.sharedMaterial = Resources.Load<PhysicsMaterial2D>("Materials/Zombie");

            var zombieSpriteObject = new GameObject();
            zombieSpriteObject.transform.parent = zombieObject.transform;
            zombieSpriteObject.name = "SpriteObject";
            var zombieSprite = TileResolver.GetTile("zombie").m_Sprite;
            var zombieRenderer = zombieSpriteObject.AddComponent<SpriteRenderer>();
            
            //zombieObject.AddComponent<Collider2dRenderer>();
            zombieRenderer.sprite = zombieSprite;
            zombieRenderer.drawMode = SpriteDrawMode.Sliced;
            zombieRenderer.size = new Vector2(1, 1);
           
            zombieSpriteObject.transform.localPosition = new Vector2(-0.5f, -0.5f);
            
            var zombie = zombieObject.AddComponent<Zombie>();
            zombieObject.name = zombie.Name;

            zombie.ID = MobId;
            body.linearDamping = 10f;
            body.freezeRotation = true;
            
            return zombie;
        }
    }
}