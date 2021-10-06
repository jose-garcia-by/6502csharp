using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public enum Flags6502 : byte
    {
        C = 1, // Carry bit
        Z = 2, // Zero
        I = 4, // Disable Interrup
        D = 8, // Decimal mode
        B = 16, // Brak
        U = 32, // Unused
        V = 64, // Overflow
        N = 128  // Negative
    }
}
