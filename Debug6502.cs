using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public class Debug6502
    {
        Bus bus;
        StringBuilder dump;


        public Debug6502(Bus bus)
        {
            this.bus = bus;
            dump = new StringBuilder();            

            //Test 6502 program

            var program = new byte[] { 0xA2, 0x0A, 0x8E, 0x00, 0x00, 0xA2, 0x03, 0x8E, 0x01, 0x00, 0xAC, 0x00, 0x00, 0xA9, 0x00, 0x18, 0x6D, 0x01, 0x00,
                0x88, 0xD0, 0xFA, 0x8D, 0x02, 0x00, 0xEA, 0xEA, 0xEA };

            for (int i = 0; i < program.Length; i++)
            {
                bus.SystemRam[i | 0x8000] = program[i];
            }

            bus.SystemRam[0xFFFC] = 0x00;
            bus.SystemRam[0xFFFD] = 0x80;

            bus.Cpu.Reset();
        }

        public void PrintState()
        {
            dump.Clear();
            for (int i = 0; i <= 0x000f; i++)
            {
                PrintRam(i);
            }

            dump.AppendLine();
            dump.AppendLine();

            for (int i = 0; i <= 0x000f; i++)
            {
                PrintRom(i);
            }
        }

        private void PrintRam(int rowNo)
        {
            int offset = 0;
            int rowAddr = 0x0010 * rowNo;
            var row = $"${rowAddr:X4}: {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} " +
                        $"{GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} " +
                        $"{GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)}";

            switch (rowNo)
            {
                case 0:
                    row += GetStatus();
                    break;
                case 1:
                    row += $"  PC: {bus.Cpu.Pc.ToString("X4")}";
                    break;
                case 2:
                    row += $"  A: {bus.Cpu.A.ToString("X4")}";
                    break;
                case 3:
                    row += $"  X: {bus.Cpu.X.ToString("X4")}";
                    break;
                case 4:
                    row += $"  Y: {bus.Cpu.Y.ToString("X4")}";
                    break;
                case 5:
                    row += $"  StackPt: {bus.Cpu.Stkp.ToString("X4")}";
                    break;
            }

            dump.AppendLine(row);
        }

        private void PrintRom(int rowNo)
        {
            int offset = 0;
            int rowAddr = (0x0010 * rowNo) | 0x8000;
            var row = $"${rowAddr:X4}: {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} " +
                        $"{GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} " +
                        $"{GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)} {GetMemAt(rowAddr | offset++)}";
            dump.AppendLine(row);
        }

        private string GetStatus()
        {
            return $"  STATUS: N V - B{bus.Cpu.GetFlag(Flags6502.B)} D{bus.Cpu.GetFlag(Flags6502.D)} I{bus.Cpu.GetFlag(Flags6502.I)} Z{bus.Cpu.GetFlag(Flags6502.Z)} C{bus.Cpu.GetFlag(Flags6502.C)}";
        }

        private string GetMemAt(int addr)
        {
            return bus.SystemRam[addr].ToString("X2");
        }

        public string Dump { get => dump.ToString(); }

    }
}
