using System;
using System.Runtime.CompilerServices;

namespace TinyCoreCPU
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. create our memory instance; see Memory.cs
            Memory memory = new Memory();

            // 2. built in program for now since we dont have an assembler yet
            byte[] testProgram = new byte[]
                {
                    0x01, 0x05,       // LOAD_A 5
                    0x02, 0x03,       // LOAD_B 5
                    0x08, 0x40,       // OUT 64
                    0x23, 0x00,       // JMPA 0
                    0x01, 0x00,       // LOAD_A 0
                    0xFF              // HLT
                };

            // load it
            for (int i = 0; i < testProgram.Length; i++)
            {
                memory.Program_RAM[i] = testProgram[i];
            }

            // fill the rest with HLTs to stay safe for now
            for (int i = testProgram.Length; i < memory.Program_RAM.Length; i++)
                memory.Program_RAM[i] = 0xFF; // HLT

            // 3. create the instruction set; see InstructionSet.cs
            InstructionSet instructions = new InstructionSet();

            // 4. create the cpu; see CPU.cs
            CPU cpu = new CPU(memory, instructions, 1_000);

            // 5. attach keyboard to port 0
            var keyboard = new KeyboardDriver();
            cpu.AttachDevice(0, keyboard);

            // 6. run the cpu in another thread and do an input loop for the host
            var cpuThread = new System.Threading.Thread(() => cpu.Run());
            cpuThread.IsBackground = true;
            cpuThread.Start();

            while (true)
            {
                while (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.Escape)
                        return;

                    keyboard.KeyDown(keyInfo.Key);
                }

                System.Threading.Thread.Sleep(1);
            }
        }
    }
}