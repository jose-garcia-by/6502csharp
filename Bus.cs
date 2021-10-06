using System;
using System.Threading;
using System.Threading.Tasks;

namespace Components
{
    public class Bus
    {

        #region Fields

        private byte[] systemRam = new byte[2048];

        private Ppu ppu;
        private Cartridge cartridge;

        private int nSystemClockCounter = 0;
        private Task mainThread;
        private bool isRuning = false;
        internal byte[] controller = new byte[2];
        private byte[] controllerState = new byte[2];

        byte dma_page = 0x00;
        byte dma_addr = 0x00;
        byte dma_data = 0x00;

        bool dma_dummy = true;
        bool dma_transfer = false;

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
            else if (addr >= 0x4016 && addr <= 0x4017)
            {
                controllerState[addr & 0x0001] = controller[addr & 0x0001];
            }
        }

        public byte CpuRead(int addr, bool readOnly = false)
        {
            byte data = 0x00;

            if (cartridge != null && cartridge.CpuRead(addr, ref data))
            {
                return data;
            }
            else if (addr <= 0x1FFF)
            {
                return systemRam[addr & 0x07FF];
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {
                return ppu.CpuRead(addr & 0x0007, readOnly);
            }
            else if (addr >= 0x4016 && addr <= 0x4017)
            {
                data = (byte)((controllerState[addr & 0x0001] & 0x80) > 0 ? 1 : 0);
                controllerState[addr & 0x0001] <<= 1;
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
            cartridge.Reset();
            Cpu.Reset();
            Ppu.Reset();
            nSystemClockCounter = 0;

            if (!(mainThread is null)) {
                mainThread.Dispose();
                mainThread = null;
                isRuning = false;
            }

            Run();
        }

        public void Clock()
        {
            ppu.Clock();

            if (nSystemClockCounter % 3 == 0)
            {
                Cpu.Clock();
            }

            if (ppu.nmi)
            {
                Ppu.nmi = false;
                Cpu.Nmi();
            }

            nSystemClockCounter++;
        }

        public void Run()
        {
            if (!isRuning)
            {
                mainThread = Task.Run(() =>
                {
                    while (true)
                    {
                        Clock();
                    }
                });

                isRuning = true;
            }
        }

        internal byte[] SystemRam { get => systemRam; }

        #endregion
    }
}
