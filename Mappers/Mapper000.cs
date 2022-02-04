using System;
using System.Collections.Generic;
using System.Text;

namespace Components.Mappers
{
    internal class Mapper000 : Mapper
    {

        #region Constructor

        internal Mapper000(int prgBanks, int chrBanks): 
            base(prgBanks, chrBanks)
        {

        }

        #endregion

        #region Methods

        internal override bool CpuMapRead(int addr,ref  int mappedAddr)
        {
            if(addr >= 0x8000 && addr <= 0xFFFF)
            {
                mappedAddr = addr & (nPrgBanks > 1 ? 0x7FFF : 0x3FFF);
                return true;
            }

            return false;
        }

        internal override bool CpuMapWrite(int addr,ref int mappedAddr)
        {
            if (addr >= 0x8000 && addr <= 0xFFFF)
            {
                mappedAddr = addr & (nPrgBanks > 1 ? 0x7FFF : 0x3FFF);
                return true;
            }

            return false;
        }

        internal override bool CpuMapWrite(int addr, ref int mappedAddr, byte data)
        {
            if (addr >= 0x8000 && addr <= 0xFFFF)
            {
                mappedAddr = addr & (nPrgBanks > 1 ? 0x7FFF : 0x3FFF);
                return true;
            }

            return false;
        }

        internal override bool PpuMapRead(int addr,ref int mappedAddr)
        {
            if (addr <= 0x1FFF)
            {
                mappedAddr = addr;
                return true;
            }

            return false;
        }

        internal override bool PpuMapWrite(int addr,ref int mappedAddr)
        {
            if (addr >= 0x0000 && addr <= 0x1FFF && nChrBanks == 0)
            {
                mappedAddr = addr;
                return true;
            }

            return false;
        }

        #endregion
    }
}
