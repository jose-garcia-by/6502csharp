using System;
using System.Collections.Generic;
using System.Text;

namespace Components.Mappers
{
    internal abstract class Mapper
    {

        #region Fields

        protected int nPrgBanks;
        protected int nChrBanks;

        #endregion

        #region Constructor

        internal Mapper(int prgBanks, int chrBanks)
        {
            this.nPrgBanks = prgBanks;
            this.nChrBanks = chrBanks;
        }

        #endregion

        #region Properties

        internal int NPrgBanks { get => nPrgBanks; }

        internal int NChrBanks { get => nChrBanks; }

        #endregion

        #region Methods 

        internal virtual bool CpuMapRead(int addr,ref int mappedAddr) => false;

        internal virtual bool CpuMapWrite(int addr,ref int mappedAddr) => false;

        internal virtual bool PpuMapRead(int addr,ref int mappedAddr) => false;

        internal virtual bool PpuMapWrite(int addr,ref int mappedAddr) => false;

        #endregion

    }
}
