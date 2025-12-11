using System;
// keyboard driver for TinyCoreCPU, handling Z and X keys.
namespace TinyCoreCPU
{
    public class KeyboardDriver : IDevice
    {
        private byte keyState = 0;

        // bit 0 is Z, bit 1 is X
        public byte Read()
        {
            // return the current state and optionally reset one-shot
            byte state = keyState;
            keyState = 0; 
            return state;
        }

        public void Write(byte val)
        {
            // keyboard is read-only
        }

        // call from the host input loop
        public void KeyDown(ConsoleKey key)
        {
            if (key == ConsoleKey.Z)
                keyState |= 0b00000001;
            else if (key == ConsoleKey.X)
                keyState |= 0b00000010;
        }

        public void KeyUp(ConsoleKey key)
        {
            if (key == ConsoleKey.Z)
                keyState &= 0b11111110;
            else if (key == ConsoleKey.X)
                keyState &= 0b11111101;
        }
    }
}