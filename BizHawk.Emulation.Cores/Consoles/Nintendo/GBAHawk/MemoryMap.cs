using System;

using BizHawk.Common.BufferExtensions;
using BizHawk.Emulation.Common;


/*
	(Values not listed are not Used)
	$0E000000-$0E00FFFF 	SRAM
	$0C000000-$0DFFFFFF 	ROM (wait state 2)
	$0A000000-$0BFFFFFF 	ROM (wait state 1)
	$08000000-$09FFFFFF 	ROM (wait state 0)
	$07000000-$070003FF 	OAM
	$06000000-$06017FFF 	VRAM
	$05000000-$050003FF 	BG/OBJ Palette RAM
	$04000000-$040003FF 	IO Registers
	$03000000-$03007FFF 	WRAM (On Chip)
	$02000000-$0203FFFF 	WRAM (On Board)
	$00000000-$00003FFF 	BIOS
*/

namespace BizHawk.Emulation.Cores.Nintendo.GBAHawk
{
	public partial class GBAHawk
	{
		public byte ReadMemory(uint addr)
		{
			MemoryCallbacks.CallReads(addr, "System Bus");

			if (addr < 0x2000000)
			{
				if (addr < 0x4000)
				{
					return _bios[addr];
				}
				return 0xFF;
			}
			else if (addr < 0x3000000)
			{
				if (addr < 0x2040000)
				{
					return RAM[addr];
				}
				return 0xFF;
			}
			else if (addr < 0x4000000)
			{
				if (addr < 0x3008000)
				{
					return RAM[addr];
				}
				return 0xFF;
			}
			else if (addr < 0x5000000)
			{
				if (addr < 0x4000400)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0x6000000)
			{
				if (addr < 0x5000400)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0x7000000)
			{
				if (addr < 0x6018000)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0x8000000)
			{
				if (addr < 0x7000400)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0xA000000)
			{
				return _rom[addr - 0x8000000];
			}
			else if (addr < 0xC000000)
			{
				return _rom[addr - 0xA000000];
			}
			else if (addr < 0xE000000)
			{
				return _rom[addr - 0xC000000];
			}
			else if (addr < 0xF000000)
			{
				if (addr < 0xE0010000)
				{
					return _rom[addr - 0xC000000];
				}
				return 0xFF;
			}
			else
			{
				return 0xFF;
			}
		}

		public void WriteMemory(uint addr, byte value)
		{
			MemoryCallbacks.CallWrites(addr, "System Bus");
			if (addr < 0x2000000)
			{

			}
			else if (addr < 0x3000000)
			{
				if (addr < 0x2040000)
				{
					RAM[addr] = value;
				}
			}
			else if (addr < 0x4000000)
			{
				if (addr < 0x3008000)
				{
					RAM[addr] = value;
				}
			}
			else if (addr < 0x5000000)
			{
				if (addr < 0x4000400)
				{
					RAM[addr] = value;
				}
			}
			else if (addr < 0x6000000)
			{
				if (addr < 0x5000400)
				{
					Write_Registers(addr, value);
				}
			}
			else if (addr < 0x7000000)
			{
				if (addr < 0x6018000)
				{
					Write_Registers(addr, value);
				}
			}
			else if (addr < 0x8000000)
			{
				if (addr < 0x7000400)
				{
					Write_Registers(addr, value);
				}
			}
			else if (addr < 0xA000000)
			{

			}
			else if (addr < 0xC000000)
			{

			}
			else if (addr < 0xE000000)
			{

			}
			else if (addr < 0xF000000)
			{
				if (addr < 0xE0010000)
				{

				}
			}
		}

		public byte PeekMemory(uint addr)
		{
			if (addr < 0x2000000)
			{
				if (addr < 0x4000)
				{
					return _bios[addr];
				}
				return 0xFF;
			}
			else if (addr < 0x3000000)
			{
				if (addr < 0x2040000)
				{
					return RAM[addr];
				}
				return 0xFF;
			}
			else if (addr < 0x4000000)
			{
				if (addr < 0x3008000)
				{
					return RAM[addr];
				}
				return 0xFF;
			}
			else if (addr < 0x5000000)
			{
				if (addr < 0x4000400)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0x6000000)
			{
				if (addr < 0x5000400)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0x7000000)
			{
				if (addr < 0x6018000)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0x8000000)
			{
				if (addr < 0x7000400)
				{
					return Read_Registers(addr);
				}
				return 0xFF;
			}
			else if (addr < 0xA000000)
			{
				return _rom[addr - 0x8000000];
			}
			else if (addr < 0xC000000)
			{
				return _rom[addr - 0xA000000];
			}
			else if (addr < 0xE000000)
			{
				return _rom[addr - 0xC000000];
			}
			else if (addr < 0xF000000)
			{
				if (addr < 0xE0010000)
				{
					return _rom[addr - 0xC000000];
				}
				return 0xFF;
			}
			else
			{
				return 0xFF;
			}
		}
	}
}
