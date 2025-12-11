using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading;

namespace TinyCoreCPU
{
    public class CPU
    {
        // registers
        public byte A, B, C, D, E, F, G, H;
        public byte PXX, PXY; // graphics position registers
        public byte COL;       // graphics color register
        public ushort PC;      // program counter
        public bool FLAGZ, FLAGN, FLAGAO; // zero, negative, awaiting operand

        // memory and instruction set
        public Memory memory;
        public InstructionSet instructions;

        // CPU clock
        public double CPUhz = 1_000_000; // default 1 MHz
        private byte awaitingOpcode;

        // Expansion port devices
        private Dictionary<byte, IDevice> expansionDevices = new Dictionary<byte, IDevice>();

        public CPU(Memory memory, InstructionSet instructions, double hz = 1_000_000)
        {
            this.CPUhz = hz;
            this.memory = memory;
            this.instructions = instructions;
            PC = 0;
        }

        // attach a device to an expansion port
        public void AttachDevice(byte port, IDevice device)
        {
            expansionDevices[port] = device;
        }

        // read from an expansion port (used by the IN/0x09 instruction)
        public byte ReadPort(byte port)
        {
            if (expansionDevices.TryGetValue(port, out var device))
                return device.Read();
            return 0;
        }

        // write to an expansion port (used by the OUT/0x08 instruction)
        public void WritePort(byte port, byte value)
        {
            if (expansionDevices.TryGetValue(port, out var device))
                device.Write(value);
        }

        // execution
        public void Run()
        {
            Stopwatch sw = Stopwatch.StartNew();
            double secondsPerCycle = 1.0 / CPUhz;

            while (true)
            {
                double cycleStartTime = sw.Elapsed.TotalSeconds;

                if (!FLAGAO)
                {
                    byte opcode = memory.Read(PC++);
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
                    byte operand = memory.Read(PC++);
                    instructions.Execute(awaitingOpcode, this, operand);
                    FLAGAO = false;
                }

                // throttle to approximate clock speed
                double elapsed = sw.Elapsed.TotalSeconds - cycleStartTime;
                double targetCycleTime = secondsPerCycle;

                int sleepMs = (int)((targetCycleTime - elapsed) * 1000);
                if (sleepMs > 0)
                    Thread.Sleep(sleepMs);
            }
        }
    }

    // interface for our expansion port
    public interface IDevice
    {
        byte Read();          // read from device (IN)
        void Write(byte val); // write to device (OUT)
    }
}