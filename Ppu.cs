using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Components
{
    public class Ppu
    {
        #region fields

        private Bus bus;
        private Cartridge cartridge;
        private byte[,] tblName = new byte[2, 1024];
        private byte[] tblPalette = new byte[32];
        private byte[,] tblPattern = new byte[2, 4096]; // TO PLAY
        private int cycle = 0;

        private static int screenHeight = 240;
        private static int screenWidth = 256;
        private uint[] palScreen;

        private uint[] sprScreen = new uint[screenWidth * screenHeight];
        private int scanLine = 0;
        Random rand = new Random(255);

        private LoopyRegister vram_addr;
        private LoopyRegister tram_addr;

        private PpuCtrl control;
        private Mask mask;
        private Status status;

        byte fine_x = 0x00;
        byte address_latch = 0x00;
        byte ppu_data_buffer = 0x00;

        byte bg_next_tile_id = 0x00;
        byte bg_next_tile_attrib = 0x00;
        byte bg_next_tile_lsb = 0x00;
        byte bg_next_tile_msb = 0x00;
        uint bg_shifter_pattern_lo = 0x0000;
        uint bg_shifter_pattern_hi = 0x0000;
        uint bg_shifter_attrib_lo = 0x0000;
        uint bg_shifter_attrib_hi = 0x0000;
        byte[] oam = new byte[64 * 4];
        byte[] spriteScanline = new byte[8 * 4];

        byte oam_addr = 0x00;
        byte[] sprite_shifter_pattern_lo = new byte[8];

        internal bool nmi = false;

        #endregion

        #region Constructor

        public Ppu(Bus bus)
        {
            this.bus = bus;
            InitScreenPalette();
        }

        #endregion

        #region Properties

        public int ScreenHeight { get => screenHeight; }

        public int ScreenWidth { get => screenWidth; }

        #endregion

        #region Methods

        internal void Reset()
        {
            fine_x = 0x00;
            address_latch = 0x00;
            ppu_data_buffer = 0x00;
            scanLine = 0;
            cycle = 0;
            bg_next_tile_id = 0x00;
            bg_next_tile_attrib = 0x00;
            bg_next_tile_lsb = 0x00;
            bg_next_tile_msb = 0x00;
            bg_shifter_pattern_lo = 0x0000;
            bg_shifter_pattern_hi = 0x0000;
            bg_shifter_attrib_lo = 0x0000;
            bg_shifter_attrib_hi = 0x0000;
            status = new Status(0x00);
            mask = new Mask(0x00);
            control = new PpuCtrl(0x00);
            vram_addr = new LoopyRegister(0x0000);
            tram_addr = new LoopyRegister(0x0000);
        }

        private void InitScreenPalette()
        {
            palScreen = new uint[0x40];

            palScreen[0x00] = new Pixel(84, 84, 84).Raw;
            palScreen[0x01] = new Pixel(0, 30, 116).Raw;
            palScreen[0x02] = new Pixel(8, 16, 144).Raw;
            palScreen[0x03] = new Pixel(48, 0, 136).Raw;
            palScreen[0x04] = new Pixel(68, 0, 100).Raw;
            palScreen[0x05] = new Pixel(92, 0, 48).Raw;
            palScreen[0x06] = new Pixel(84, 4, 0).Raw;
            palScreen[0x07] = new Pixel(60, 24, 0).Raw;
            palScreen[0x08] = new Pixel(32, 42, 0).Raw;
            palScreen[0x09] = new Pixel(8, 58, 0).Raw;
            palScreen[0x0A] = new Pixel(0, 64, 0).Raw;
            palScreen[0x0B] = new Pixel(0, 60, 0).Raw;
            palScreen[0x0C] = new Pixel(0, 50, 60).Raw;
            palScreen[0x0D] = new Pixel(0, 0, 0).Raw;
            palScreen[0x0E] = new Pixel(0, 0, 0).Raw;
            palScreen[0x0F] = new Pixel(0, 0, 0).Raw;

            palScreen[0x10] = new Pixel(152, 150, 152).Raw;
            palScreen[0x11] = new Pixel(8, 76, 196).Raw;
            palScreen[0x12] = new Pixel(48, 50, 236).Raw;
            palScreen[0x13] = new Pixel(92, 30, 228).Raw;
            palScreen[0x14] = new Pixel(136, 20, 176).Raw;
            palScreen[0x15] = new Pixel(160, 20, 100).Raw;
            palScreen[0x16] = new Pixel(152, 34, 32).Raw;
            palScreen[0x17] = new Pixel(120, 60, 0).Raw;
            palScreen[0x18] = new Pixel(84, 90, 0).Raw;
            palScreen[0x19] = new Pixel(40, 114, 0).Raw;
            palScreen[0x1A] = new Pixel(8, 124, 0).Raw;
            palScreen[0x1B] = new Pixel(0, 118, 40).Raw;
            palScreen[0x1C] = new Pixel(0, 102, 120).Raw;
            palScreen[0x1D] = new Pixel(0, 0, 0).Raw;
            palScreen[0x1E] = new Pixel(0, 0, 0).Raw;
            palScreen[0x1F] = new Pixel(0, 0, 0).Raw;

            palScreen[0x20] = new Pixel(236, 238, 236).Raw;
            palScreen[0x21] = new Pixel(76, 154, 236).Raw;
            palScreen[0x22] = new Pixel(120, 124, 236).Raw;
            palScreen[0x23] = new Pixel(176, 98, 236).Raw;
            palScreen[0x24] = new Pixel(228, 84, 236).Raw;
            palScreen[0x25] = new Pixel(236, 88, 180).Raw;
            palScreen[0x26] = new Pixel(236, 106, 100).Raw;
            palScreen[0x27] = new Pixel(212, 136, 32).Raw;
            palScreen[0x28] = new Pixel(160, 170, 0).Raw;
            palScreen[0x29] = new Pixel(116, 196, 0).Raw;
            palScreen[0x2A] = new Pixel(76, 208, 32).Raw;
            palScreen[0x2B] = new Pixel(56, 204, 108).Raw;
            palScreen[0x2C] = new Pixel(56, 180, 204).Raw;
            palScreen[0x2D] = new Pixel(60, 60, 60).Raw;
            palScreen[0x2E] = new Pixel(0, 0, 0).Raw;
            palScreen[0x2F] = new Pixel(0, 0, 0).Raw;

            palScreen[0x30] = new Pixel(236, 238, 236).Raw;
            palScreen[0x31] = new Pixel(168, 204, 236).Raw;
            palScreen[0x32] = new Pixel(188, 188, 236).Raw;
            palScreen[0x33] = new Pixel(212, 178, 236).Raw;
            palScreen[0x34] = new Pixel(236, 174, 236).Raw;
            palScreen[0x35] = new Pixel(236, 174, 212).Raw;
            palScreen[0x36] = new Pixel(236, 180, 176).Raw;
            palScreen[0x37] = new Pixel(228, 196, 144).Raw;
            palScreen[0x38] = new Pixel(204, 210, 120).Raw;
            palScreen[0x39] = new Pixel(180, 222, 120).Raw;
            palScreen[0x3A] = new Pixel(168, 226, 144).Raw;
            palScreen[0x3B] = new Pixel(152, 226, 180).Raw;
            palScreen[0x3C] = new Pixel(160, 214, 228).Raw;
            palScreen[0x3D] = new Pixel(160, 162, 160).Raw;
            palScreen[0x3E] = new Pixel(0, 0, 0).Raw;
            palScreen[0x3F] = new Pixel(0, 0, 0).Raw;
        }

        public byte CpuRead(int addr, bool readOnly = false)
        {
            byte data = 0x00;

            if (readOnly)
            {
                switch (addr)
                {
                    case 0x0000: // Control
                        data = control.reg;
                        break;
                    case 0x0001: // Mask
                        data = mask.reg;
                        break;
                    case 0x0002: // Status
                        data = status.reg;
                        break;
                    case 0x0003: // OAM Address
                        break;
                    case 0x0004: // OAM Data
                        break;
                    case 0x0005: // Scroll
                        break;
                    case 0x0006: // PPU Address
                        break;
                    case 0x0007: // PPU Data
                        break;
                }
            }
            else
            {
                switch (addr)
                {
                    // Control - Not readable
                    case 0x0000: break;

                    // Mask - Not Readable
                    case 0x0001: break;

                    // Status
                    case 0x0002:
                        data = (byte)((status.reg & 0xE0) | (ppu_data_buffer & 0x1F));

                        status.vertical_blank = false;
                        status.reg &= (byte)(~0x0080 & 0xFF);

                        address_latch = 0;
                        break;

                    // OAM Address
                    case 0x0003: break;

                    // OAM Data
                    case 0x0004:
                        data = oam[oam_addr];
                        break;
                    // Scroll - Not Readable
                    case 0x0005: break;

                    // PPU Address - Not Readable
                    case 0x0006: break;

                    // PPU Data
                    case 0x0007:
                        data = ppu_data_buffer;
                        ppu_data_buffer = PpuRead(vram_addr.reg);
                        if (vram_addr.reg >= 0x3F00) data = ppu_data_buffer;
                        vram_addr = new LoopyRegister(vram_addr.reg + (control.increment_mode ? 32 : 1));
                        break;
                }
            }

            return data;
        }

        public void CpuWrite(int addr, byte data)
        {
            switch (addr)
            {
                case 0x0000: // Control
                    control = new PpuCtrl(data);
                    tram_addr.AddNametableX(control.nametable_x);
                    tram_addr.AddNametableY(control.nametable_y);
                    break;
                case 0x0001: // Mask
                    mask = new Mask(data);
                    break;
                case 0x0002: // Status
                    break;
                case 0x0003: // OAM Address
                    oam_addr = data;
                    break;
                case 0x0004: // OAM Data
                    oam[oam_addr] = data;
                    break;
                case 0x0005: // Scroll
                    if (address_latch == 0)
                    {
                        fine_x = (byte)(data & 0x07);
                        tram_addr.AddCoarseX((byte)(data >> 3));
                        address_latch = 1;
                    }
                    else
                    {
                        tram_addr.AddFineY((byte)(data & 0x07));
                        tram_addr.AddCoarseY((byte)(data >> 3));
                        address_latch = 0;
                    }
                    break;
                case 0x0006: // PPU Address
                    if (address_latch == 0)
                    {
                        tram_addr = new LoopyRegister(((data & 0x3F) << 8) | (tram_addr.reg & 0x00FF));
                        address_latch = 1;
                    }
                    else
                    {
                        tram_addr = new LoopyRegister((tram_addr.reg & 0xFF00) | data);
                        vram_addr = tram_addr;
                        address_latch = 0;
                    }
                    break;
                case 0x0007: // PPU Data
                    PpuWrite(vram_addr.reg, data);
                    vram_addr = new LoopyRegister(vram_addr.reg + (control.increment_mode ? 32 : 1));
                    break;
            }
        }

        public byte PpuRead(int addr, bool readOnly = false)
        {
            byte data = 0x00;
            addr &= 0x3FFF;

            if (cartridge.PpuRead(addr, ref data))
            {
                //return data;
            }
            else if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                return tblPattern[(addr & 0x1000) >> 12, addr & 0x0FFF];
            }
            else if (addr >= 0x2000 && addr <= 0x3EFF)
            {
                addr &= 0x0FFF;

                if (cartridge.Mirror == Mirror.VERTICAL)
                {
                    // Vertical
                    if (addr >= 0x0000 && addr <= 0x03FF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0400 && addr <= 0x07FF)
                        data = tblName[1, addr & 0x03FF];
                    if (addr >= 0x0800 && addr <= 0x0BFF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0C00 && addr <= 0x0FFF)
                        data = tblName[1, addr & 0x03FF];
                }
                else if (cartridge.Mirror == Mirror.HORIZONTAL)
                {
                    // Horizontal
                    if (addr >= 0x0000 && addr <= 0x03FF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0400 && addr <= 0x07FF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0800 && addr <= 0x0BFF)
                        data = tblName[1, addr & 0x03FF];
                    if (addr >= 0x0C00 && addr <= 0x0FFF)
                        data = tblName[1, addr & 0x03FF];
                }
            }
            else if (addr >= 0x3F00 && addr <= 0x3FFF)
            {
                addr &= 0x001F;
                if (addr == 0x0010) addr = 0x0000;
                if (addr == 0x0014) addr = 0x0004;
                if (addr == 0x0018) addr = 0x0008;
                if (addr == 0x001C) addr = 0x000C;
                data = (byte)(tblPalette[addr] & (mask.grayscale ? 0x30 : 0x3F));
            }

            return data;
        }

        public void PpuWrite(int addr, byte data)
        {
            addr &= 0x3FFF;

            if (cartridge.PpuWrite(addr, ref data))
            {

            }
            else if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                tblPattern[(addr & 0x1000) >> 12, addr & 0x0FFF] = data;
            }
            else if (addr >= 0x2000 && addr <= 0x3EFF)
            {
                addr &= 0x0FFF;
                if (cartridge.Mirror == Mirror.VERTICAL)
                {
                    // Vertical
                    if (addr >= 0x0000 && addr <= 0x03FF)
                        tblName[0, addr & 0x03FF] = data;
                    if (addr >= 0x0400 && addr <= 0x07FF)
                        tblName[1, addr & 0x03FF] = data;
                    if (addr >= 0x0800 && addr <= 0x0BFF)
                        tblName[0, addr & 0x03FF] = data;
                    if (addr >= 0x0C00 && addr <= 0x0FFF)
                        tblName[1, addr & 0x03FF] = data;
                }
                else if (cartridge.Mirror == Mirror.HORIZONTAL)
                {
                    // Horizontal
                    if (addr >= 0x0000 && addr <= 0x03FF)
                        tblName[0, addr & 0x03FF] = data;
                    if (addr >= 0x0400 && addr <= 0x07FF)
                        tblName[0, addr & 0x03FF] = data;
                    if (addr >= 0x0800 && addr <= 0x0BFF)
                        tblName[1, addr & 0x03FF] = data;
                    if (addr >= 0x0C00 && addr <= 0x0FFF)
                        tblName[1, addr & 0x03FF] = data;
                }
            }
            else if (addr >= 0x3F00 && addr <= 0x3FFF)
            {
                addr &= 0x001F;
                if (addr == 0x0010) addr = 0x0000;
                if (addr == 0x0014) addr = 0x0004;
                if (addr == 0x0018) addr = 0x0008;
                if (addr == 0x001C) addr = 0x000C;
                tblPalette[addr] = data;
            }
        }

        public uint[] GetPatternTable(byte i, byte palette)
        {
            return null;
        }

        public uint GetColourFromPaletteRam(byte palette, byte pixel)
        {
            return palScreen[PpuRead(0x3F00 + (palette << 2) + pixel) & 0x3F];
        }

        internal void ConnectCartridge(Cartridge cartridge)
        {
            this.cartridge = cartridge;
        }

        private void IncrementScrollX()
        {
            if (mask.render_background || mask.render_sprites)
            {
                if (vram_addr.coarse_x == 31)
                {
                    vram_addr.coarse_x = 0;
                    vram_addr.nametable_x = !vram_addr.nametable_x;
                }
                else
                {
                    // Staying in current nametable, so just increment
                    vram_addr.AddCoarseX(++vram_addr.coarse_x);
                }
            }
        }

        private void IncrementScrollY()
        {
            if (mask.render_background || mask.render_sprites)
            {
                if (vram_addr.fine_y < 7)
                {
                    vram_addr.AddFineY(++vram_addr.fine_y);
                }
                else
                {
                    vram_addr.fine_y = 0;

                    if (vram_addr.coarse_y == 29)
                    {
                        vram_addr.coarse_y = 0;
                        vram_addr.nametable_y = !vram_addr.nametable_y;
                    }
                    else if (vram_addr.coarse_y == 31)
                    {
                        vram_addr.coarse_y = 0;
                    }
                    else
                    {
                        vram_addr.AddCoarseY(++vram_addr.coarse_y);
                    }
                }
            }
        }

        private void TransferAddressX()
        {
            if (mask.render_background || mask.render_sprites)
            {
                vram_addr.AddNametableX(tram_addr.nametable_x);
                vram_addr.AddCoarseX(tram_addr.coarse_x);
            }
        }

        private void TransferAddressY ()
        {
            // Ony if rendering is enabled
            if (mask.render_background || mask.render_sprites)
            {
                vram_addr.fine_y = tram_addr.fine_y;
                vram_addr.nametable_y = tram_addr.nametable_y;
                vram_addr.coarse_y = tram_addr.coarse_y;

                vram_addr.reg |= tram_addr.fine_y;
                vram_addr.reg |= tram_addr.nametable_y ? 0x1000 : 0x0000;
                vram_addr.reg |= tram_addr.fine_y;
            }
        }

        private void LoadBackgroundShifters()
        {
            bg_shifter_pattern_lo = (bg_shifter_pattern_lo & 0xFF00) | bg_next_tile_lsb;
            bg_shifter_pattern_hi = (bg_shifter_pattern_hi & 0xFF00) | bg_next_tile_msb;

            bg_shifter_attrib_lo = (uint)((bg_shifter_attrib_lo & 0xFF00) | (uint)((bg_next_tile_attrib & 0b01) != 0 ? 0xFF : 0x00));
            bg_shifter_attrib_hi = (uint)((bg_shifter_attrib_hi & 0xFF00) | (uint)((bg_next_tile_attrib & 0b10) != 0 ? 0xFF : 0x00));
        }

        private void UpdateShifters()
        {
            if (mask.render_background)
            {
                // Shifting background tile pattern row
                bg_shifter_pattern_lo <<= 1;
                bg_shifter_pattern_hi <<= 1;

                // Shifting palette attributes by 1
                bg_shifter_attrib_lo <<= 1;
                bg_shifter_attrib_hi <<= 1;
            }
        }

        public void Clock()
        {
            if (scanLine >= -1 && scanLine < 240)
            {
                if (scanLine == 0 && cycle == 0)
                {
                    // "Odd Frame" cycle skip
                    cycle = 1;
                }

                if (scanLine == -1 && cycle == 1)
                {
                    status.vertical_blank = false;
                    status.reg &= (byte)(~0x0080 & 0xFF);
                }


                if ((cycle >= 2 && cycle < 258) || (cycle >= 321 && cycle < 338))
                {
                    UpdateShifters();

                    switch ((cycle - 1) % 8)
                    {
                        case 0:
                            LoadBackgroundShifters();

                            bg_next_tile_id = PpuRead(0x2000 | (vram_addr.reg & 0x0FFF));
                            break;
                        case 2:			
                            bg_next_tile_attrib = PpuRead(0x23C0 | (vram_addr.nametable_y ? 1 : 0 << 11)
                                                                 | (vram_addr.nametable_x ? 1 : 0 << 10)
                                                                 | ((vram_addr.coarse_y >> 2) << 3)
                                                                 | (vram_addr.coarse_x >> 2));
                            
                            if ((vram_addr.coarse_y & 0x02) != 0) bg_next_tile_attrib >>= 4;
                            if ((vram_addr.coarse_x & 0x02) != 0) bg_next_tile_attrib >>= 2;
                            bg_next_tile_attrib &= 0x03;
                            break;

                        // Compared to the last two, the next two are the easy ones... :P

                        case 4:
                            bg_next_tile_lsb = PpuRead((control.pattern_background ? 1 : 0 << 12)
                                                       + (bg_next_tile_id << 4)
                                                       + (vram_addr.fine_y) + 0);

                            break;
                        case 6:
                            bg_next_tile_msb = PpuRead((control.pattern_background ? 1 : 0 << 12)
                                                       + (bg_next_tile_id << 4)
                                                       + (vram_addr.fine_y) + 8);
                            break;
                        case 7:
                            IncrementScrollX();
                            break;
                    }
                }

                // End of a visible scanline, so increment downwards...
                if (cycle == 256)
                {
                    IncrementScrollY();
                }

                //...and reset the x position
                if (cycle == 257)
                {
                    LoadBackgroundShifters();
                    TransferAddressX();
                }

                // Superfluous reads of tile id at end of scanline
                if (cycle == 338 || cycle == 340)
                {
                    bg_next_tile_id = PpuRead(0x2000 | (vram_addr.reg & 0x0FFF));
                }

                if (scanLine == -1 && cycle >= 280 && cycle < 305)
                {
                    // End of vertical blank period so reset the Y address ready for rendering
                    TransferAddressY();
                }
            }            

            if (scanLine >= 241 && scanLine < 261)
            {
                if (scanLine == 241 && cycle == 1)
                {
                    // Effectively end of frame, so set vertical blank flag
                    status.vertical_blank = true;
                    status.reg |= 0x80;

                    if (control.enable_nmi)
                        nmi = true;
                }
            }

            byte bg_pixel = 0x00;   // The 2-bit pixel to be rendered
            byte bg_palette = 0x00; // The 3-bit index of the palette the pixel indexes

            if (mask.render_background)
            {
                // Handle Pixel Selection by selecting the relevant bit
                // depending upon fine x scolling. This has the effect of
                // offsetting ALL background rendering by a set number
                // of pixels, permitting smooth scrolling
                int bit_mux = 0x8000 >> fine_x;

                // Select Plane pixels by extracting from the shifter 
                // at the required location. 
                byte p0_pixel = (byte)((bg_shifter_pattern_lo & bit_mux) > 0 ? 0x01 : 0x00);
                byte p1_pixel = (byte)((bg_shifter_pattern_hi & bit_mux) > 0 ? 1: 0);

                // Combine to form pixel index
                bg_pixel = (byte)((p1_pixel << 1) | p0_pixel);

                // Get palette
                byte bg_pal0 = (byte)((bg_shifter_attrib_lo & bit_mux) > 0 ? 1 : 0);
                byte bg_pal1 = (byte)((bg_shifter_attrib_hi & bit_mux) > 0 ? 1 : 0);
                bg_palette = (byte)((bg_pal1 << 1) | bg_pal0);
            }


            // Now we have a final pixel colour, and a palette for this cycle
            // of the current scanline. Let's at long last, draw that ^&%*er :P

            //sprScreen[(cycle - 1) + scanLine] = GetColourFromPaletteRam(bg_palette, bg_pixel);

            if (cycle < screenWidth && scanLine >= 0 && scanLine < screenHeight)
            {
                sprScreen[(scanLine * screenWidth) + (cycle)] = (uint)(GetColourFromPaletteRam(bg_palette, bg_pixel) >> 8); 
            }

            cycle++;
            if (cycle >= 341)
            {
                cycle = 0;
                scanLine++;
                if (scanLine >= 261)
                {
                    scanLine = -1;
                    OnFrameCompleted?.Invoke(sprScreen);
                }
            }
        }

        #endregion

        #region Events

        public event FrameCompleted OnFrameCompleted;

        #endregion

    }

    [StructLayout(LayoutKind.Sequential)]
    struct Mask
    {
        internal bool grayscale;
        internal bool render_background_left;
        internal bool render_sprites_left;
        internal bool render_background;
        internal bool render_sprites;
        internal bool enhance_red;
        internal bool enhance_green;
        internal bool enhance_blue;

        public byte reg;

        public Mask(byte mask)
        {
            int indx = 1;
            grayscale = (mask & indx) == indx; indx <<= 1;
            render_background_left = (mask & indx) == indx; indx <<= 1;
            render_sprites_left = (mask & indx) == indx; indx <<= 1;
            render_background = (mask & indx) == indx; indx <<= 1;
            render_sprites = (mask & indx) == indx; indx <<= 1;
            enhance_red = (mask & indx) == indx; indx <<= 1;
            enhance_green = (mask & indx) == indx; indx <<= 1;
            enhance_blue = (mask & indx) == indx;

            reg = mask;
        }
    }

    struct Status
    {
        internal byte unused;
        internal bool sprite_overflow;
        internal bool sprite_zero_hit;
        internal bool vertical_blank;
        internal byte reg;

        public Status(byte status)
        {
            int indx = 1 << 5;
            unused = (byte)(status & 0x1F);
            sprite_overflow = (status & indx) == indx; indx <<= 1;
            sprite_zero_hit = (status & indx) == indx; indx <<= 1;
            vertical_blank = (status & indx) == indx; indx <<= 1;

            reg = status;
        }
    }

    struct PpuCtrl
    {
        internal bool nametable_x;
        internal bool nametable_y;
        internal bool increment_mode;
        internal bool pattern_sprite;
        internal bool pattern_background;
        internal bool sprite_size;
        internal bool slave_mode; // unused
        internal bool enable_nmi;

        internal byte reg;

        public PpuCtrl(byte ppuCtrl)
        {
            int indx = 1;
            nametable_x = (ppuCtrl & indx) == indx; indx <<= 1;
            nametable_y = (ppuCtrl & indx) == indx; indx <<= 1;
            increment_mode = (ppuCtrl & indx) == indx; indx <<= 1;
            pattern_sprite = (ppuCtrl & indx) == indx; indx <<= 1;
            pattern_background = (ppuCtrl & indx) == indx; indx <<= 1;
            sprite_size = (ppuCtrl & indx) == indx; indx <<= 1;
            slave_mode = (ppuCtrl & indx) == indx; indx <<= 1;
            enable_nmi = (ppuCtrl & indx) == indx;

            reg = ppuCtrl;
        }
    }

    struct LoopyRegister
    {
        internal byte coarse_x;
        internal byte coarse_y;
        internal bool nametable_x;
        internal bool nametable_y;
        internal byte fine_y;
        internal bool unused;

        internal int reg;

        public LoopyRegister(int loopyRegister)
        {
            coarse_x = (byte)(loopyRegister & 0x1F);
            coarse_y = (byte)((loopyRegister >> 5) & 0x1F);
            nametable_x = ((loopyRegister >> 10) & 0x01) == 1;
            nametable_y = ((loopyRegister >> 11) & 0x01) == 1;
            fine_y = (byte)((loopyRegister >> 12) & 0x07);
            unused = false;

            reg = loopyRegister & 0x0000FFFF;
        }

        internal LoopyRegister AddCoarseX(byte coarse_x)
        {
            this.coarse_x = coarse_x;
            reg = (reg & ~0x1F) | coarse_x;

            return this;
        }

        internal LoopyRegister AddCoarseY(byte coarse_y)
        {
            this.coarse_y = coarse_y;
            reg = (reg & ~(0x1F << 5)) | ((coarse_y & 0x1F) << 5);

            return this;
        }

        internal LoopyRegister AddFineY(byte fine_y)
        {
            if(fine_y > 7)
            {
                unused = true;
                this.fine_y = 0;
            }
            this.fine_y = fine_y;
            reg = reg | ((fine_y & 0xFFFF) << 12);

            return this;
        }

        internal LoopyRegister AddNametableX(bool nametable_x)
        {
            this.nametable_x = nametable_x;

            if (nametable_x)
            {
                reg = reg | (1 << 10);
            }
            else
            {
                reg = reg & ~(1 << 10);
            }

            return this;
        }

        internal LoopyRegister AddNametableY(bool nametable_y)
        {
            this.nametable_y = nametable_y;

            if (nametable_y)
            {
                reg = reg | (1 << 10);
            }
            else
            {
                reg = reg & ~(1 << 10);
            }

            return this;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct ObjectAttributeEntry
    {
        [FieldOffset(0)]
        internal byte y;          // Y position of sprite
        [FieldOffset(1)]
        internal byte id;         // ID of tile from pattern memory
        [FieldOffset(2)]
        internal byte attribute;  // Flags define how sprite should be rendered
        [FieldOffset(3)]
        internal byte x;          // X position of sprite
        [FieldOffset(0)]
        internal uint oae;


        public ObjectAttributeEntry(uint oae) : this()
        {
            y = (byte)(oae & 0x000000FFu);
            id = (byte)((oae >> 8) & 0x000000FFu);
            attribute = (byte)((oae >> 16) & 0x000000FFu);
            x = (byte)((oae >> 24) & 0x000000FFu);
            this.oae = oae;
        }
    }

    public delegate void FrameCompleted(uint[] frame);
}
