using System;
using System.Threading.Tasks;

namespace Components
{
    public class Bus
    {

        #region Fields

        byte[] systemRam = new byte[0xFFFF];

        #endregion

        #region Properties

        public Olc6502 Cpu { get; set; }
        public Ppu Ppu { get; set; }

        #endregion


        #region Constructor
        public Bus()
        {
            Parallel.ForEach(systemRam, r => r = 0x00);
            Cpu = new Olc6502();
            Cpu.ConnectBus(this);
            Ppu = new Ppu();
        }

        #endregion

        #region Methods

        public void CpuWrite(uint addr, byte data)
        {
            if (addr <= 0x01FFF)
            {
                systemRam[addr & 0x07FF] = data;
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {

            }
        }

        public byte CpuRead(uint addr, bool readOnly = false)
        {
            if (addr <= 0x1FFF)
            {
                return systemRam[addr & 0x07FF];
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {

            }

            return 0x00;
        }

        internal byte[] SystemRam { get => systemRam; }

        #endregion
    }
}
