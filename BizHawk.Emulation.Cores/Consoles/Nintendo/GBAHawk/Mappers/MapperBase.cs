using BizHawk.Common;
using System;

namespace BizHawk.Emulation.Cores.Nintendo.GBAHawk
{
	public class MapperBase
	{
		public GBAHawk Core { get; set; }

		public virtual byte ReadMemory(uint addr)
		{
			return 0;
		}

		public virtual byte PeekMemory(uint addr)
		{
			return 0;
		}

		public virtual void WriteMemory(uint addr, byte value)
		{
		}

		public virtual void PokeMemory(uint addr, byte value)
		{
		}

		public virtual void SyncState(Serializer ser)
		{
		}

		public virtual void Dispose()
		{
		}

		public virtual void Initialize()
		{
		}

		public virtual void Mapper_Tick()
		{
		}

		public virtual void RTC_Get(byte value, int index)
		{
		}
	}
}
