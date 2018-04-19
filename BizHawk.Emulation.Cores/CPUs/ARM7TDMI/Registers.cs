using System.Runtime.InteropServices;
using System;

namespace BizHawk.Emulation.Common.Components.ARM7TDMI
{
	public partial class ARM7TDMI
	{
		// Operation: On mode switch, Regs is populated with the register set of the appropriate mode

		public const uint PC = 15;
		public const uint CPSR = 16;
		public const uint SPSR = 17;

		public const int User_c = 0;
		public const int FIQ_c = 1;
		public const int Sup_c = 2;
		public const int Abort_c = 3;
		public const int IRQ_c = 4;
		public const int Und_c = 5;

		// registers
		public uint[] Regs = new uint[18];
		public uint[] Regs_User = new uint[18];
		public uint[] Regs_FIQ = new uint[8];
		public uint[] Regs_Sup = new uint[3];
		public uint[] Regs_Abort = new uint[3];
		public uint[] Regs_IRQ = new uint[3];
		public uint[] Regs_Und = new uint[3];

		public void context_switch(int new_c, int old_c)
		{
			// regs 0-7 never change regardless of context (even into thumb mode)
			// save all the old regs
			if (old_c != FIQ_c)
			{
				for (int i = 8; i < 13; i++)
				{
					Regs_User[i] = Regs[i];
				}
			}
			else
			{
				for (int i = 0; i < 7; i++)
				{
					Regs_FIQ[i] = Regs[i + 8];
				}
				Regs_FIQ[7] = Regs[17];
			}

			switch (old_c)
			{
				case User_c:
					Regs_User[0] = Regs[13];
					Regs_User[1] = Regs[14];
					Regs_User[2] = Regs[17];
					break;
				case Sup_c:
					Regs_Sup[0] = Regs[13];
					Regs_Sup[1] = Regs[14];
					Regs_Sup[2] = Regs[17];
					break;
				case Abort_c:
					Regs_Abort[0] = Regs[13];
					Regs_Abort[1] = Regs[14];
					Regs_Abort[2] = Regs[17];
					break;
				case IRQ_c:
					Regs_IRQ[0] = Regs[13];
					Regs_IRQ[1] = Regs[14];
					Regs_IRQ[2] = Regs[17];
					break;
				case Und_c:
					Regs_Und[0] = Regs[13];
					Regs_Und[1] = Regs[14];
					Regs_Und[2] = Regs[17];
					break;
			}

			// shift in the new regs
			if (old_c == FIQ_c)
			{
				for (int i = 8; i < 13; i++)
				{
					Regs[i] = Regs_User[i];
				}
			}

			switch (new_c)
			{
				case User_c:
					Regs[13] = Regs_User[13];
					Regs[14] = Regs_User[14];
					Regs[17] = Regs_User[17];
					break;
				case FIQ_c:
					Regs[8] = Regs_Sup[0];
					Regs[9] = Regs_Sup[1];
					Regs[10] = Regs_Sup[2];
					Regs[11] = Regs_Sup[3];
					Regs[12] = Regs_Sup[4];
					Regs[13] = Regs_Sup[5];
					Regs[14] = Regs_Sup[6];
					Regs[17] = Regs_Sup[7];
					break;
				case Sup_c:
					Regs[13] = Regs_Sup[0];
					Regs[14] = Regs_Sup[1];
					Regs[17] = Regs_Sup[2];
					break;
				case Abort_c:
					Regs[13] = Regs_Abort[0];
					Regs[14] = Regs_Abort[1];
					Regs[17] = Regs_Abort[2];
					break;
				case IRQ_c:
					Regs[13] = Regs_IRQ[0];
					Regs[14] = Regs_IRQ[1];
					Regs[17] = Regs_IRQ[2];
					break;
				case Und_c:
					Regs[13] = Regs_Und[0];
					Regs[14] = Regs_Und[1];
					Regs[17] = Regs_Und[2];
					break;
			}
		}

		public const uint FlagN = 0x80000000;
		public const uint FlagZ = 0x40000000;
		public const uint FlagC = 0x20000000;
		public const uint FlagV = 0x10000000;

		public const uint FlagIRQ = 0x80;
		public const uint FlagFIQ = 0x40;
		public const uint FlagT = 0x20;


		private void ResetRegisters()
		{
			Regs = new uint[18];
			Regs_User = new uint[18];
			Regs_FIQ = new uint[8];
			Regs_Sup = new uint[3];
			Regs_Abort = new uint[3];
			Regs_IRQ = new uint[3];
			Regs_Und = new uint[3];
		}
	}
}