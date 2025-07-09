using System;
using UnityEngine;

namespace Scenes.GameState.Scripts.Entities {
    public abstract class Entity : MonoBehaviour {
        public enum KindType {
            Zombie,
            Player
        }
        public int ID { get; internal set; } = 0;
        private float _hp = 1;
        public float Hp {
            get => _hp;
            set {
                _hp = Math.Max(0, value);
                if (_hp == 0)
                    IsAlive = false;
            }
        }
        private float _maxHp = 1;
        public float MaxHp {
            get => _maxHp;
            set {
                _maxHp = Math.Max(0, value);
                if (_maxHp == 0)
                    IsAlive = false;
            }
        }
        public bool IsAlive { get; protected set; } = true;
        public Rigidbody2D Body { get; private set; }
        public FriendlinessType Friendliness { get; internal set; } = FriendlinessType.Neutral;
        public KindType Kind { get; internal set; }
        public string Name { get; internal set; } = "{No name}";

        public virtual float Hurt(float damage) {
            Hp = Math.Max(0, Hp - damage);
            if (Hp == 0)
                Kill();

            return Hp;
        }

        public virtual void Kill() {
            IsAlive = false;
        }

        public virtual void SetPosition(float x, float y) {
            gameObject.transform.position = new Vector3(x, y);
        }

        public virtual Vector2 GetPosition() {
            return gameObject.transform.position;
        }

        public virtual Vector2 GetVelocity() {
            return Body.linearVelocity;
        }

        public void Init() {
            Body = gameObject.GetComponent<Rigidbody2D>();
        }

        public override string ToString() {
            return "(" + ID + ")" + Name;
        }
    }
}