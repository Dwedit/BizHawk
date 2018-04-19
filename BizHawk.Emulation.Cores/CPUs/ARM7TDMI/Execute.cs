using System;

namespace BizHawk.Emulation.Common.Components.ARM7TDMI
{
	public partial class ARM7TDMI
	{
		private int totalExecutedCycles;
		public int TotalExecutedCycles { get { return totalExecutedCycles; } set { totalExecutedCycles = value; } }

		private int EI_pending;
		private bool interrupts_enabled;

		// variables for executing instructions
		public int instr_pntr = 0;
		public ushort[] cur_instr;
		public int opcode;
		public bool jammed;
		public int LY;

		public void FetchInstruction(uint opcode)
		{
			switch (opcode & 0xE000000)
			{
				case 0x0000000:
					break;
				case 0x2000000:
					break;
				case 0x4000000:
					// single data transfer (immdeiate offset)
					break;
				case 0x6000000:
					// 
					break;
				case 0x8000000:
					// block data transfer

					break;
				case 0xA000000:
					// branch

					break;
				case 0xC000000:
					// coprocessor data transfer

					break;
				case 0xE000000:
					if ((opcode & 0x1000000) == 0)
					{						
						if ((opcode & 0x10) == 0)
						{
							// coprocessor data operation
						}
						else
						{
							// coprocessor register transfer
						}
					}
					else
					{
						// software interrupt
					}
					break;
				default:
					break;

			}
		}
	}
}