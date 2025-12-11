using System;

// any Console.WriteLine will be replaced later by our own serial class

namespace TinyCoreCPU
{
    public class InstructionSet
    {
        public bool RequiresOperand(byte opcode)
        {
            return opcode switch
            {
                0x01 => true, // LOAD_A
                0x02 => true, // LOAD_B
                0x05 => true, // JMP
                0x06 => true, // JZ
                0x07 => true, // JNZ
                0x08 => true, // OUT
                0x10 => true, // PXX
                0x11 => true, // PXY
                0x12 => true, // COL
                _ => false
            };
        }

        public void Execute(byte opcode, CPU cpu, byte operand)
        {
            switch (opcode)
            {
                case 0x01: // LOAD_A
                    cpu.A = operand;
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;
                case 0x02: // LOAD_B
                    cpu.B = operand;
                    cpu.FLAGZ = cpu.B == 0;
                    cpu.FLAGN = cpu.B >= 128;
                    break;
                case 0x03: // ADD
                    cpu.A = (byte)((cpu.A + cpu.B) % 256);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;
                case 0x08: // OUT 
                    cpu.memory.Write(operand, cpu.A);
                    Console.WriteLine($"OUT[{operand}] <= {cpu.A}");
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;
                case 0xFF: // HLT
                    Environment.Exit(0);
                    Console.WriteLine($"HLT");
                    break;
                case 0x00: break; // NOP
                default:
                    throw new Exception($"Unknown opcode {opcode:X2}");
            }
        }
    }
}