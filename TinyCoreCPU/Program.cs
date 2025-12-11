using System;

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
                    0x01, 0x01,    // LOAD_A 1
                    0x08, 0x40,    // OUT 64
                    0x02, 0x01,    // LOAD_B 1
                    0x08, 0x40,    // OUT 64
                    0x03,          // ADD (A = A + B)
                    0x08, 0x40,    // OUT 64
                    0x20, 0x00,    // JNZ 0
                    0xFF           // HLT
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

            // 5. run the cpu
            cpu.Run();
        }
    }
}