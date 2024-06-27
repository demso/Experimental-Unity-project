namespace Scenes.GameState.Scripts.Entities {
    public class FriendlinessType {
        public static readonly FriendlinessType Neutral = new FriendlinessType("Neutral");
        public static readonly FriendlinessType Friend = new FriendlinessType("Friend");
        public static readonly FriendlinessType Enemy = new FriendlinessType("Enemy");

        public static FriendlinessType FromString(string name) {
            return name switch {
                "Friend" => Friend,
                "Enemy" => Enemy,
                _ => Neutral
            };
        }

        private readonly string name;

        private FriendlinessType(string n) {
            name = n;
        }
        public override string ToString() {
            return name;
        }
    }
}