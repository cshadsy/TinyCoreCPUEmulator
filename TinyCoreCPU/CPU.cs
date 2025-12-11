using System;

namespace TinyCoreCPU
{
    public class CPU
    {
        public byte A, B; // registers
        public ushort PC; // program counter
        public bool FLAGZ, FLAGN, FLAGAO; // flags: zero, negative, awaiting operand
        public Memory memory; // ram
        public InstructionSet instructions; // instruction set

        private byte awaitingOpcode;

        public CPU(Memory memory, InstructionSet instructions)
        {
            this.memory = memory;
            this.instructions = instructions;
            PC = 0;
        }

        public void Run()
        {
            while (true)
            {
                if (!FLAGAO)
                {
                    byte opcode = memory.Read(PC);
                    PC++;

                    // check if this opcode requires an operand, if not, we will execute it immediately.
                    if (instructions.RequiresOperand(opcode))
                    {
                        FLAGAO = true;
                        awaitingOpcode = opcode;
                    }
                    else
                    {
                        instructions.Execute(opcode, this, 0);
                    }
                }
                else
                {
                    // operand fetch
                    byte operand = memory.Read(PC);
                    PC++;
                    instructions.Execute(awaitingOpcode, this, operand);
                    FLAGAO = false;
                }
            }
        }
    }
}