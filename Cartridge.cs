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

        private byte[] vPRGMemory;
        private byte[] vCHRMemory;

        private int nMapperId = 0;
        private int nPRGBanks = 0;
        private int nCHRBanks = 0;

        private SHeader header;
        private Mapper mapper;

        private Mirror mirror;

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
            mirror = (header.mapper1 & 0x01) == 0x01 ? Mirror.VERTICAL : Mirror.HORIZONTAL;

            fileType = 1;

            switch (fileType)
            {
                case 0:
                case 1:
                    nPRGBanks = header.prgRomChunks;
                    vPRGMemory = data.ToList().Skip(indx).Take(indx + nPRGBanks * 16384).ToList().ToArray();
                    indx += nPRGBanks * 16384;
                    nCHRBanks = header.chrRomChunks;
                    vCHRMemory = data.ToList().Skip(indx).Take(indx + (nCHRBanks * 8194)).ToList().ToArray();
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

            if (mapper.CpuMapRead(addr, ref mappedAddr))
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
            int mappedAddr = 0;
            if (mapper.PpuMapWrite(addr,ref mappedAddr))
            {
                vCHRMemory[mappedAddr] = data;
                return true;
            }
            else
                return false;
        }

        internal void Reset()
        {
            if(mapper != null)
            {
                mapper.Reset();
            }
        }

        #endregion

        #region Properties

        public Mirror Mirror { get => mirror; set => mirror = value; }

        #endregion

    }
}
