using System;
using Scenes.GameState.Scripts.Entities;
using SuperTiled2Unity;
using UnityEngine;

namespace Scenes.GameState.Scripts.Items.Bullets {
    public class Bullet : MonoBehaviour {
        public GameObject tracerObject;
        public Rigidbody2D body;
        public Vector2 moveVec;
        public int damage = 4;
        public float bulletSpeed = 200f;

        private void OnCollisionEnter2D(Collision2D other) {
            var entity = other.gameObject.GetComponent<Entity>();
            switch (entity.Name) {
                case "zombie": 
                    if (entity is Zombie zombie){
                        zombie.Hurt(damage);
                    }
                    break;
            }
            tracerObject.transform.position = other.contacts[0].point;
            Destroy(gameObject);
        }

        public static void FireBullet(SuperTile tile, Vector2 position, Vector2 target) {
            var body = GameHandler.Instance.BodyFactory.BulletBody(position.x, position.y);
            var go = body.gameObject;
            var tracerObject = new GameObject("SpriteObject") {
                transform = {
                    parent = go.transform,
                    localPosition = target * -1,
                    eulerAngles = (Quaternion.FromToRotation(Vector3.zero, target) * Quaternion.Euler(0, 0, 90)).eulerAngles
                }
            };
            var bullet = go.AddComponent<Bullet>();
            var bulletRenderer = go.AddComponent<SpriteRenderer>();
            var tracerRenderer = tracerObject.AddComponent<SpriteRenderer>();
            var bulletSprite = tile.m_Sprite;
            var tracerSprite = Sprite.Create(Resources.Load<Texture2D>("Visual/Textures/bullet_tracer_yellow"), new Rect(0, 0, 10, 174), new Vector2(0.5f, 0.5f), pixelsPerUnit: 32);

            bulletRenderer.sprite = bulletSprite;
            tracerRenderer.sprite = tracerSprite;
            
            tracerRenderer.drawMode = SpriteDrawMode.Sliced;

            bullet.moveVec = target.normalized * bullet.bulletSpeed;
            bullet.tracerObject = tracerObject;
            bullet.body = body;

            body.inertia = 1;
            body.linearVelocity = bullet.moveVec;
        }
    }
}