using System;
using System.Threading.Tasks;

namespace Components
{
    public class Bus
    {

        #region Fields

        byte[] systemRam = new byte[2048];

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

            //LDA $60
            //ADC $61
            //STA $62
            //var program = new byte[] { 0xA9, 0x60, 0x69, 0x61, 0x8D, 0x62, 0x00, 0x69, 0x62, 0x8D, 0x63, 0x00 };

            //for(int i = 0; i < program.Length; i++)
            //{
            //    systemRam[i] = program[i];
            //}

            //systemRam[0x60] = 0x0f;
            //systemRam[0x61] = 0x55;
        }

        #endregion

        #region Methods

        public void CpuWrite(uint addr, byte data)
        {
            if ((addr & 0xffffe000) == 0)
            {
                systemRam[addr & 0x07ff] = data;
            }
            else if(addr >= 0x2000 && addr <= 0x3FFF)
            {

            }
        }

        public byte CpuRead(uint addr, bool readOnly = false)
        {
            if ((addr & 0xffffe000) == 0)
            {
                return systemRam[addr & 0x07ff];
            }
            else if (addr >= 0x2000 && addr <= 0x3FFF)
            {

            }

            return 0x00;
        }

        #endregion
    }
}
