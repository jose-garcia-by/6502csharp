using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
    public enum Flags6502 : byte
    {
        C = 1, // Carry bit
        Z = (1 << 1), // Zero
        I = (1 << 2), // Disable Interrup
        D = (1 << 3), // Decimal mode
        B = (1 << 4), // Brak
        U = (1 << 5), // Unused
        V = (1 << 6), // Overflow
        N = (1 << 7)  // Negative
    }
}
