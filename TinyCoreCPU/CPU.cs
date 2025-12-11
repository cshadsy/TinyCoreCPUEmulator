using System;
using System.Diagnostics;
using System.Threading;

namespace TinyCoreCPU
{
    public class CPU
    {
        public byte A, B; // registers
        public byte PXX, PXY; // position registers for graphics
        public byte COL; // color register for graphics
        public ushort PC; // program counter
        public bool FLAGZ, FLAGN, FLAGAO; // flags: zero, negative, awaiting operand
        public Memory memory; // ram
        public InstructionSet instructions; // instruction set

        public double CPUhz = 1_000; // 1 kilohertz
        private byte awaitingOpcode;

        public CPU(Memory memory, InstructionSet instructions, double hz)
        {
            this.CPUhz = hz;
            this.memory = memory;
            this.instructions = instructions;
            PC = 0;
        }

        public void Run()
        {
            Stopwatch sw = Stopwatch.StartNew();
            double secondsPerCycle = 1.0 / CPUhz;

            while (true)
            {
                double cycleStartTime = sw.Elapsed.TotalSeconds;

                if (!FLAGAO)
                {
                    byte opcode = memory.Read(PC);
                    PC++;

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
                    byte operand = memory.Read(PC);
                    PC++;
                    instructions.Execute(awaitingOpcode, this, operand);
                    FLAGAO = false;
                }

                // throttle to approximate clock speed
                double elapsed = sw.Elapsed.TotalSeconds - cycleStartTime;
                double targetCycleTime = secondsPerCycle; // 1 us per cycle at hz

                // sleep if CPU is running faster than hz
                int sleepMs = (int)((targetCycleTime - elapsed) * 1000);
                if (sleepMs > 0)
                    Thread.Sleep(sleepMs);
            }
        }
    }
}