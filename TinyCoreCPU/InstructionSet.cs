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
                0x01 => true,  // LOAD_A
                0x02 => true,  // LOAD_B
                0x03 => false, // ADD
                0x04 => false, // SUB
                0x05 => false, // INC_A
                0x06 => false, // DEC_A
                0x07 => true,  // LOAD_MEM
                0x08 => true,  // OUT
                0x09 => true,  // IN
                0x10 => true,  // PXX
                0x11 => true,  // PXY
                0x12 => true,  // COL
                0x13 => false, // DRW
                0x14 => false, // AND
                0x15 => false, // OR
                0x16 => false, // XOR
                0x17 => false, // NOT
                0x18 => true,  // INC_B
                0x19 => true,  // DEC_B
                0x20 => true,  // JMP
                0x21 => true,  // JZ
                0x22 => true,  // JNZ
                0x23 => true,  // JMPA
                0x24 => false, // MVA
                0x25 => false, // MVB
                0x26 => true,  // LOADARB
                0x27 => true,  // MOV
                0xFF => false, // HLT
                0x00 => false, // NOP
                _ => throw new Exception($"Unknown opcode {opcode:X2}")
            };
        }

        public void Execute(byte opcode, CPU cpu, byte operand)
        {
            switch (opcode)
            {
                case 0x00: // NOP
                    break;

                case 0x01: // LOAD_A
                    cpu.A = operand;
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = (cpu.A & 0x80) != 0;
                    break;

                case 0x02: // LOAD_B
                    cpu.B = operand;
                    cpu.FLAGZ = cpu.B == 0;
                    cpu.FLAGN = (cpu.B & 0x80) != 0;
                    break;

                case 0x03: // ADD
                    cpu.A = (byte)((cpu.A + cpu.B) & 0xFF);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;

                case 0x04: // SUB
                    cpu.A = (byte)((cpu.A - cpu.B) & 0xFF);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;

                case 0x05: // INC_A
                    cpu.A = (byte)((cpu.A + 1) & 0xFF);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;

                case 0x06: // DEC_A
                    cpu.A = (byte)((cpu.A - 1) & 0xFF);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;

                case 0x07: // LOAD_MEM
                    cpu.A = cpu.memory.Read(operand);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;

                case 0x08: // OUT
                    cpu.memory.Write(operand, cpu.A);
                    Console.WriteLine($"OUT[{operand}] <= {cpu.A}");
                    break;

                case 0x09: // IN
                    cpu.A = cpu.ReadPort(operand);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = cpu.A >= 128;
                    break;

                case 0x10: // PXX
                    cpu.PXX = operand;
                    break;

                case 0x11: // PXY
                    cpu.PXY = operand;
                    break;

                case 0x12: // COL
                    cpu.COL = operand;
                    break;

                case 0x13: // DRW
                    // stub
                    break;

                case 0x14: // AND
                    cpu.A = (byte)(cpu.A & cpu.B);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = (cpu.A & 0x80) != 0;
                    break;

                case 0x15: // OR
                    cpu.A = (byte)(cpu.A | cpu.B);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = (cpu.A & 0x80) != 0;
                    break;

                case 0x16: // XOR
                    cpu.A = (byte)(cpu.A ^ cpu.B);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = (cpu.A & 0x80) != 0;
                    break;

                case 0x17: // NOT
                    cpu.A = (byte)(~cpu.A);
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = (cpu.A & 0x80) != 0;
                    break;

                case 0x18: // INC_B
                    cpu.B++;
                    cpu.FLAGZ = cpu.B == 0;
                    cpu.FLAGN = (cpu.B & 0x80) != 0;
                    break;

                case 0x19: // DEC_B
                    cpu.B--;
                    cpu.FLAGZ = cpu.B == 0;
                    cpu.FLAGN = (cpu.B & 0x80) != 0;
                    break;

                case 0x20: // JMP
                    cpu.PC = operand;
                    break;

                case 0x21: // JZ
                    if (cpu.FLAGZ)
                        cpu.PC = operand;
                    break;

                case 0x22: // JNZ
                    if (!cpu.FLAGZ)
                        cpu.PC = operand;
                    break;

                case 0x23: // JMPA
                    if (cpu.A == cpu.B)
                    {
                        cpu.PC = operand;
                    }
                    break;

                case 0x24: // MVA
                    cpu.A = cpu.B;
                    cpu.FLAGZ = cpu.A == 0;
                    cpu.FLAGN = (cpu.A & 0x80) != 0;
                    break;

                case 0x25: // MVB
                    cpu.B = cpu.A;
                    cpu.FLAGZ = cpu.B == 0;
                    cpu.FLAGN = (cpu.B & 0x80) != 0;
                    break;

                case 0x26: // LOADARB
                    {
                        // operand here is the register ID
                        byte regId = operand;
                        byte value = cpu.memory.Read(cpu.PC++); // fetch second operand, this is bad handling but whatever lmao
                        switch (regId)
                        {
                            case 0x00: cpu.A = value; break;
                            case 0x01: cpu.B = value; break;
                            case 0x02: cpu.C = value; break;
                            case 0x03: cpu.D = value; break;
                            case 0x04: cpu.E = value; break;
                            case 0x05: cpu.F = value; break;
                            case 0x06: cpu.G = value; break;
                            case 0x07: cpu.H = value; break;
                            default: throw new Exception($"Unknown register ID {regId:X2}");
                        }

                        if (regId == 0x00) // A
                        {
                            cpu.FLAGZ = cpu.A == 0;
                            cpu.FLAGN = (cpu.A & 0x80) != 0;
                        }
                        else if (regId == 0x01) // B
                        {
                            cpu.FLAGZ = cpu.B == 0;
                            cpu.FLAGN = (cpu.B & 0x80) != 0;
                        }
                    }
                    break;

                case 0x27: // MOV
                    {
                        byte srcId = operand;
                        byte destId = cpu.memory.Read(cpu.PC++); // fetch second operand, again is very very Bad

                        byte value = srcId switch
                        {
                            0x00 => cpu.A,
                            0x01 => cpu.B,
                            0x02 => cpu.C,
                            0x03 => cpu.D,
                            0x04 => cpu.E,
                            0x05 => cpu.F,
                            0x06 => cpu.G,
                            0x07 => cpu.H,
                            _ => throw new Exception($"Unknown source register ID {srcId:X2}")
                        };

                        switch (destId)
                        {
                            case 0x00: cpu.A = value; break;
                            case 0x01: cpu.B = value; break;
                            case 0x02: cpu.C = value; break;
                            case 0x03: cpu.D = value; break;
                            case 0x04: cpu.E = value; break;
                            case 0x05: cpu.F = value; break;
                            case 0x06: cpu.G = value; break;
                            case 0x07: cpu.H = value; break;
                            default: throw new Exception($"Unknown destination register ID {destId:X2}");
                        }

                        if (destId == 0x00) // A
                        {
                            cpu.FLAGZ = cpu.A == 0;
                            cpu.FLAGN = (cpu.A & 0x80) != 0;
                        }
                        else if (destId == 0x01) // B
                        {
                            cpu.FLAGZ = cpu.B == 0;
                            cpu.FLAGN = (cpu.B & 0x80) != 0;
                        }
                    }
                    break;

                case 0xFF: // HLT
                    Console.WriteLine("HALT");
                    Environment.Exit(0);
                    break;

                default:
                    throw new Exception($"Unknown opcode {opcode:X2}");
            }
        }
    }
}