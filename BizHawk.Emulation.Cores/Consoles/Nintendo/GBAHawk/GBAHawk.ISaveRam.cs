using System;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.GBAHawk
{
	public partial class GBAHawk : ISaveRam
	{
		public byte[] CloneSaveRam()
		{
			return (byte[])cart_RAM.Clone();
		}

		public void StoreSaveRam(byte[] data)
		{
			Buffer.BlockCopy(data, 0, cart_RAM, 0, data.Length);
			Console.WriteLine("loading SRAM here");
		}

		public bool SaveRamModified
		{
			get 
			{
				return has_bat & _syncSettings.Use_SRAM;
			}	
		}
	}
}
