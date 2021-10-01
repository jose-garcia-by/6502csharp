using Components.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    public class Cartridge
    {

        #region Fields

        private List<byte> vPRGMemory;
        private List<byte> vCHRMemory;

        private int nMapperId = 0;
        private int nPRGBanks = 0;
        private int nCHRBanks = 0;

        private SHeader header;
        private Mapper mapper;

        #endregion

        #region Constructor

        public Cartridge(byte[] data)
        {
            header = new SHeader(data);
            int fileType = 0;
            int indx = header.size;

            if ((header.mapper1 & 0x04) != 0)
            {
                indx += 512;
            }

            nMapperId = ((header.mapper2 >> 4) << 4) | (header.mapper1 >> 4);

            fileType = 1;

            switch (fileType)
            {
                case 0:
                case 1:
                    nPRGBanks = header.prgRomChunks;
                    vPRGMemory = data.ToList().Skip(indx).Take(indx = nPRGBanks * 16384).ToList();

                    nCHRBanks = header.chrRomChunks;
                    vCHRMemory = data.ToList().Skip(indx).Take(indx = nCHRBanks * 8194).ToList();
                    break;
            }

            switch (nMapperId)
            {
                case 0:
                    mapper = new Mapper000(nPRGBanks, nCHRBanks);
                    break;
            }
        }

        #endregion

        #region Methods

        internal bool CpuRead(int addr, ref byte data)
        {
            int mappedAddr = 0x00;

            if(mapper.CpuMapRead(addr,ref mappedAddr))
            {
                data = vPRGMemory[mappedAddr];
                return true;
            }

            return false;
        }

        internal bool CpuWrite(int addr, ref byte data)
        {
            return false;
        }

        internal bool PpuRead(int addr, ref byte data)
        {
            int mappedAddr = 0x00;
            if (mapper.PpuMapRead(addr, ref mappedAddr))
            {
                data = vCHRMemory[mappedAddr];
                return true;
            }
            else
                return false;
        }

        internal bool PpuWrite(int addr, ref byte data)
        {
            return false;
        }

        #endregion

    }
}
