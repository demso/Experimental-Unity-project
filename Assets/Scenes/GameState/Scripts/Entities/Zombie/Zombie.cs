using System;
using GlobalNamespace;
using Scenes.GameState.Scripts.Players;

namespace Scenes.GameState.Scripts.Entities {
    public class Zombie : Entity{
        public float Damage { get; } = Globals.ZOMBIE_DAMAGE;
        public float MaxSpeed { get; } = 3f;
        public float MaxAttackCoolDown { get; } = 1f;
        public float AttackCoolDown { get; private set; } = 0;
        public Player Target { get; }

        private void Awake() {
            Name = "Zombie";
            Friendliness = FriendlinessType.Enemy;
            Hp = Globals.ZOMBIE_HEALTH;
            MaxHp = Hp;
        }

        public void Attack(Entity entity) {
            if (AttackCoolDown <= 0 && entity.Kind == KindType.Player && entity.IsAlive) {
                entity.Hurt(Damage); AttackCoolDown = MaxAttackCoolDown;
                CLogger.Instance.Log("["+Name+ "] Hurted "+ entity.Name + ", entity hp: "+ entity.Hp);
            }
        }
    }
}