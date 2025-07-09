using Artics.Physics.UnityPhisicsVisualizers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scenes.GameState.Scripts.Factories {
    public class BodyFactory {
        public enum Direction {
            North,
            South,
            West,
            East
        }

        public Rigidbody2D GetTileBody(string bodyType, Direction direction, float x, float y) {
            Rigidbody2D body = null;
            switch (bodyType) {
                case "FULL_BODY":
                    var go = new GameObject();

                    body = go.AddComponent<Rigidbody2D>();
                    body.bodyType = RigidbodyType2D.Static;
                    var collider = go.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(1, 1);
                    var colliderSize = collider.size;
                    go.transform.position = new Vector3(x + colliderSize.x / 2f, y - colliderSize.y / 2f);
                    go.name = bodyType;
                    break;
                case "WINDOW":
                    body = Window(direction, x, y);
                    break;
            }

            return body;
        }

        public Rigidbody2D Window(Direction direction, float x, float y) {
            var go = new GameObject();
            var body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            var collider = go.AddComponent<BoxCollider2D>();

            switch (direction) {
                case Direction.North:
                    collider.size = new Vector2(1f, 0.1f);
                    var colliderSize = collider.size;
                    go.transform.position = new Vector3(x + colliderSize.x / 2f, y - colliderSize.y / 2f);
                    break;
                case Direction.South:
                    collider.size = new Vector2(1f, 0.1f);
                    colliderSize = collider.size;
                    go.transform.position = new Vector3(x + colliderSize.x / 2f, y - (1 - colliderSize.y / 2f));
                    break;
                case Direction.West:
                    collider.size = new Vector2(0.1f, 1f);
                    colliderSize = collider.size;
                    go.transform.position = new Vector3(x, y - colliderSize.y / 2f);
                    break;
                case Direction.East:
                    collider.size = new Vector2(0.1f, 1f);
                    colliderSize = collider.size;
                    go.transform.position = new Vector3(x + (1 - colliderSize.x / 2f), y - colliderSize.y / 2f);
                    break;
            }

            go.name = "WINDOW";
            return body;
        }

        public Rigidbody2D ItemBody(float x, float y, ref GameObject go) {
            if (go == null || !go)
                go = new GameObject();
            var body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            var collider = go.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.4f, 0.4f);
            var colliderSize = collider.size;
            go.transform.position = new Vector3(x, y);
            go.name = "ITEM";
            go.layer = LayerMask.NameToLayer("Ignore Light And Default");
            //var colRen = go.AddComponent<Collider2dRenderer>();
            return body;
        }

        public Rigidbody2D BulletBody(float x, float y) {
            var go = new GameObject();
            var body = go.AddComponent<Rigidbody2D>();
            var collider = go.AddComponent<CircleCollider2D>();
            body.bodyType = RigidbodyType2D.Dynamic;
            body.mass = 0.7f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            collider.radius = 0.04f;
            collider.density = 1;
            collider.sharedMaterial = Resources.Load<PhysicsMaterial2D>("Materials/Bullet");

            go.layer = LayerMask.NameToLayer("Ignore Light");
            go.transform.position.Set(x, y, 0);
            return body;
        }

        public static Direction GetDirection(Tilemap tilemap, int x, int y) {
            var pos = new Vector3Int(x, y);
            var transform = tilemap.GetTransformMatrix(pos);
            var direction = Direction.North;
            if (IsFlippedVertically(transform) && IsFlippedHorizontally(transform))
                direction = Direction.South;
            else if (IsFlippedVertically(transform) && IsFlippedDiagonally(transform))
                direction = Direction.West;
            else if (IsFlippedHorizontally(transform) && IsFlippedDiagonally(transform)) direction = Direction.East;

            return direction;
        }

        private static bool IsFlippedHorizontally(Matrix4x4 mat) {
            return mat.m00 == -1 || (IsFlippedDiagonally(mat) && mat.m01 == 1);
        }

        private static bool IsFlippedVertically(Matrix4x4 mat) {
            return mat.m11 == -1 || (IsFlippedDiagonally(mat) && mat.m10 == 1);
        }

        private static bool IsFlippedDiagonally(Matrix4x4 mat) {
            return (mat.m01 == -1 && mat.m10 == 1) || (mat.m10 == -1 && mat.m01 == 1) || (mat.m01 == 1 && mat.m10 == 1);
        }
    }
}