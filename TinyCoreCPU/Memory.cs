namespace TinyCoreCPU
{
    public class Memory
    {
        public byte[] CPU_RAM = new byte[128];       // 0-127
        public byte[] Program_RAM = new byte[256];   // configurable by the emulator, should definitely be larger than this, if you want to run more advanced programs. this is 128 bytes of RAM.

        public byte Read(ushort address)
        {
            if (address < CPU_RAM.Length) return CPU_RAM[address];
            address -= (ushort)CPU_RAM.Length;
            return Program_RAM[address];
        }

        public void Write(ushort address, byte value)
        {
            if (address < CPU_RAM.Length) CPU_RAM[address] = value;
            else
            {
                address -= (ushort)CPU_RAM.Length;
                Program_RAM[address] = value;
            }
        }
    }
}