using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public struct SHeader
    {

        public byte[] name;  //4
        public byte prgRomChunks;
        public byte chrRomChunks;
        public byte mapper1;
        public byte mapper2;
        public byte prgRamSize;
        public byte tvSystem1;
        public byte tvSystem2;
        public byte[] unused; //5
        public int size;


        public SHeader(byte[] data)
        {
            int indx = 4;
            name = data.SubArray(0, 4);
            prgRomChunks = data[indx++];
            chrRomChunks = data[indx++];
            mapper1 = data[indx++];
            mapper2 = data[indx++];
            prgRamSize = data[indx++];
            tvSystem1 = data[indx++];
            tvSystem2 = data[indx++];
            unused = data.SubArray(indx, 5);
            indx += 5;

            size = indx;

        }
    }
}
