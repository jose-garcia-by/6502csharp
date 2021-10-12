using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public class Pixel
    {

        #region Fields

        private uint raw;

        #endregion

        #region Constructor

        public Pixel(byte r, byte g, byte b)
        {
            raw = (uint)((r << 24) | (g << 16) | (b << 8) | 0xFF);
        }

        public Pixel(byte r, byte g, byte b, byte a)
        {
            raw = (0xFF000000u & a) + r << 16 + g << 8 + b;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the value for a RGB components + Alpha
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        internal void SetRGB(byte r, byte g, byte b)
        {
            raw = (uint)((r << 16) | (g << 8) | b);
        }

        /// <summary>
        /// Set the value for a RGB components + Alpha
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        internal void SetARGB(byte r, byte g, byte b, byte a)
        {
            raw = (0xFF000000u & a) + r << 16 + g << 8 + b;
        }

        #endregion

        #region Properties

        public uint Raw { get => raw; }

        #endregion
    }
}
