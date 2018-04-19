using System;
using BizHawk.Emulation.Common;
using BizHawk.Common.NumberExtensions;
using BizHawk.Common;

namespace BizHawk.Emulation.Cores.Nintendo.GBAHawk
{
	public partial class GBAHawk
	{
		public byte Read_Registers(uint addr)
		{
			byte ret = 0;

			if (addr < 0x04000060)
			{
				ret = ppu.ReadReg(addr);
			}
			else if (addr < 0x040000B0)
			{
				ret = audio.ReadReg(addr);
			}
			else if (addr < 0x04000100)
			{
				ret = 0;
			}
			else if (addr < 0x04000120)
			{
				ret = 0;
			}
			else if (addr < 0x04000130)
			{
				ret = 0;
			}
			else if (addr < 0x04000134)
			{
				ret = 0;
			}
			else if (addr < 0x04000200)
			{
				ret = 0;
			}
			else
			{
				ret = 0;
			}

			return ret;
		}

		public void Write_Registers(uint addr, byte value)
		{
			byte ret = 0;

			if (addr < 0x04000060)
			{
				ret = ppu.ReadReg(addr);
			}
			else if (addr < 0x040000B0)
			{
				ret = audio.ReadReg(addr);
			}
			else if (addr < 0x04000100)
			{
				ret = 0;
			}
			else if (addr < 0x04000120)
			{
				ret = 0;
			}
			else if (addr < 0x04000130)
			{
				ret = 0;
			}
			else if (addr < 0x04000134)
			{
				ret = 0;
			}
			else if (addr < 0x04000200)
			{
				ret = 0;
			}
			else
			{
				ret = 0;
			}
		}

		public void Register_Reset()
		{
			input_register = 0xCF; // not reading any input
		}
	}
}
