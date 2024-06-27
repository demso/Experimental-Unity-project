namespace GlobalNamespace {
    public class CLogger {
        private CLogger() { }

        public static CLogger Instance { get; } = new();

        public void Log(string message) {
            UnityEngine.Debug.Log(message);
        }
    }
}