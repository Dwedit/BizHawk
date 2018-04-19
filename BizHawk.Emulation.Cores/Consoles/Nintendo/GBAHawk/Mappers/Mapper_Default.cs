using BizHawk.Common;
using BizHawk.Common.NumberExtensions;
using System;

namespace BizHawk.Emulation.Cores.Nintendo.GBAHawk
{
	// Default mapper with no bank switching
	public class MapperDefault : MapperBase
	{
		public override void Initialize()
		{
			// nothing to initialize
		}

		public override byte ReadMemory(uint addr)
		{
			if (addr < 0x8000)
			{
				return Core._rom[addr];
			}
			else
			{
				if (Core.cart_RAM != null)
				{
					return Core.cart_RAM[addr - 0xA000];
				}
				else
				{
					return 0;
				}
			}
		}

		public override byte PeekMemory(uint addr)
		{
			return ReadMemory(addr);
		}

		public override void WriteMemory(uint addr, byte value)
		{
			if (addr < 0x8000)
			{
				// no mapping hardware available
			}
			else
			{
				if (Core.cart_RAM != null)
				{
					Core.cart_RAM[addr - 0xA000] = value;
				}
			}
		}

		public override void PokeMemory(uint addr, byte value)
		{
			WriteMemory(addr, value);
		}
	}
}
