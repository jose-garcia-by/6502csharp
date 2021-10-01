using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public class Ppu
    {
        #region fields

        private Bus bus;
        private Cartridge cartridge;
        private byte[,] tblName = new byte[2, 1024];
        private byte[] tblPallet = new byte[32];
        private byte[,] tblPattern = new byte[2, 4096]; // TO PLAY
        private int cycle = 0;

        private static int screenHeight = 240;
        private static int screenWidth = 256;

        private uint[] sprScreen = new uint[screenWidth * screenHeight];
        private int scanLine = 0;
        private bool frameComplete = false;
        Random rand = new Random(255);

        #endregion

        #region Constructor

        public Ppu(Bus bus)
        {
            this.bus = bus;
        }

        #endregion

        #region Properties

        public int ScreenHeight { get => screenHeight; }

        public int ScreenWidth { get => screenWidth; }

        #endregion

        #region Methods

        public byte CpuRead(int addr, bool readOnly = false)
        {
            byte data = 0x00;

            switch (addr)
            {
                case 0x0000: // Control
                    break;
                case 0x0001: // Mask
                    break;
                case 0x0002: // Status
                    break;
                case 0x0003: // OAM Address
                    break;
                case 0x0004: // OAM Data
                    break;
                case 0x0005: // Scroll
                    break;
                case 0x0006: // PPU Address
                    break;
                case 0x0007: // PPU Data
                    break;
            }

            return data;
        }

        public void CpuWrite(int addr, byte data)
        {
            switch (addr)
            {
                case 0x0000: // Control
                    break;
                case 0x0001: // Mask
                    break;
                case 0x0002: // Status
                    break;
                case 0x0003: // OAM Address
                    break;
                case 0x0004: // OAM Data
                    break;
                case 0x0005: // Scroll
                    break;
                case 0x0006: // PPU Address
                    break;
                case 0x0007: // PPU Data
                    break;
            }
        }

        public byte PpuRead(int addr, byte readOnly)
        {
            byte data = 0x00;
            addr &= 0x3FFF;

            if (cartridge.PpuRead(addr, ref data))
            {

            }

            return data;
        }

        internal void ConnectCartridge(Cartridge cartridge)
        {
            this.cartridge = cartridge;
        }

        public void Clock()
        {
            if (cycle < screenWidth && scanLine >= 0 && scanLine < screenHeight)
            {
                sprScreen[(scanLine * screenWidth) + (cycle)] = (rand.Next(0, 255) % 2) == 0 ? 0xFFFFFFu : 0x000000u;
            }

            // Advance renderer - it never stops, it's relentless
            cycle++;
            if (cycle >= 341)
            {
                cycle = 0;
                scanLine++;
                if (scanLine >= 261)
                {
                    scanLine = -1;
                    frameComplete = true;
                    OnFrameCompleted?.Invoke(sprScreen);
                }
            }
        }

        #endregion

        #region Events

        public event FrameCompleted OnFrameCompleted;

        #endregion

    }

    public delegate void FrameCompleted(uint[] frame);
}
