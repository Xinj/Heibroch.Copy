using Heibroch.WindowsInteractivity;

namespace Heibroch.Copy.Events
{
    public class GlobalKeyPressed
    {
        private KeyPressed keyPressed;

        public GlobalKeyPressed(KeyPressed keyPressed) => this.keyPressed = keyPressed;

        public int Key 
        {
            get => keyPressed.Key;
            set => keyPressed.Key = value;
        }

        public bool ProcessKey
        {
            get => keyPressed.ProcessKey;
            set => keyPressed.ProcessKey = value;
        }
    }
}
