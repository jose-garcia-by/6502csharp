using System;
using System.Threading;
using System.Threading.Tasks;

namespace Components
{
    public class Bus
    {

        #region Fields

        private byte[] systemRam = new byte[0xFFFF];

        private Ppu ppu;
        private Cartridge cartridge;

        private int nSystemClockCounter = 0;
        private Task mainThread;        

        #endregion

        #region Properties

        public Olc6502 Cpu { get; set; }

        public Ppu Ppu { get => ppu; set => ppu = value; }

        #endregion

        #region Constructor
        public Bus()
        {
            Parallel.ForEach(systemRam, r => r = 0x00);
            Cpu = new Olc6502();
            Cpu.ConnectBus(this);
            ppu = new Ppu(this);
        }

        #endregion

        #region Methods

        public void CpuWrite(int addr, ref byte data)
        {
            if (cartridge != null && cartridge.CpuWrite(addr, ref data))
            {

            }
            else if (addr <= 0x01FFF)
            {
                systemRam[addr & 0x07FF] = data;
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                ppu.CpuWrite(addr & 0x0007, data);
            }
        }

        public byte CpuRead(int addr, bool readOnly = false)
        {
            byte data = 0x00;

            if (cartridge != null && cartridge.CpuRead(addr, ref data))
            {

            }
            if (addr <= 0x1FFF)
            {
                return systemRam[addr & 0x07FF];
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                return ppu.CpuRead(addr & 0x0007, readOnly);
            }

            return data;
        }

        public void InsertCartridge(Cartridge cartridge)
        {
            this.cartridge = cartridge;
            ppu.ConnectCartridge(cartridge);
        }

        public void Reset()
        {
            Cpu.Reset();
            nSystemClockCounter = 0;
        }

        public void Clock()
        {
            ppu.Clock();

            if (nSystemClockCounter % 3 == 0)
            {
                Cpu.Clock();
            }

            nSystemClockCounter++;
        }

        public void Run()
        {
            if (mainThread == null)
            {
                mainThread = Task.Run(() =>
                {
                    while (true)
                    {
                        for (int i = 0; i <= 53700; i++)
                        {
                            Clock();
                        }
                    }
                });
            }
        }

        internal byte[] SystemRam { get => systemRam; }

        #endregion
    }
}
