using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public class Ppu
    {
        #region fields

        #endregion

        #region Constructor

        public Ppu()
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public byte Read(uint addr, bool rdonly = false)
        {
            return 0x00;
        }

        public void Write(uint addr, byte data)
        {

        }

        #endregion

    }
}
