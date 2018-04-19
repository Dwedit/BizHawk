using System;

using BizHawk.Common.BufferExtensions;
using BizHawk.Emulation.Common;
using BizHawk.Emulation.Common.Components.ARM7TDMI;
using BizHawk.Common.NumberExtensions;

using BizHawk.Emulation.Cores.Consoles.Nintendo.Gameboy;
using System.Runtime.InteropServices;

namespace BizHawk.Emulation.Cores.Nintendo.GBAHawk
{
	[Core(
		"GBAHawk",
		"",
		isPorted: false,
		isReleased: false)]
	[ServiceNotApplicable(typeof(IDriveLight))]
	public partial class GBAHawk : IEmulator, ISaveRam, IDebuggable, IStatable, IInputPollable, IRegionable,
	ISettable<GBAHawk.GBASettings, GBAHawk.GBASyncSettings>
	{
		public byte input_register;

		// The unused bits in this register are still read/writable
		public byte REG_FFFF;
		// The unused bits in this register (interrupt flags) are always set
		public byte REG_FF0F = 0xE0;
		public bool enable_VBL;
		public bool enable_STAT;
		public bool enable_TIMO;
		public bool enable_SER;
		public bool enable_PRS;


		// memory domains
		public byte[] RAM = new byte[0x8000]; 
		public byte[] ZP_RAM = new byte[0x80];
		/* 
		 * VRAM is arranged as: 

		 */
		public byte[] VRAM = new byte[0x4000];
		public byte[] OAM = new byte[0xA0];

		public int RAM_Bank;
		public byte VRAM_Bank;

		public byte[] _bios;
		public readonly byte[] _rom;		
		public readonly byte[] header = new byte[0x50];

		public byte[] cart_RAM;
		public bool has_bat;

		private int _frame = 0;

		public bool Use_MT;

		public MapperBase mapper;

		private readonly ITraceable _tracer;

		public ARM7TDMI cpu;
		public PPU ppu;
		public Timer timer;
		public Audio audio;
		public SerialPort serialport;

		[CoreConstructor("GBA")]
		public GBAHawk(CoreComm comm, GameInfo game, byte[] rom, /*string gameDbFn,*/ object settings, object syncSettings)
		{
			var ser = new BasicServiceProvider(this);
			
			cpu = new ARM7TDMI
			{
				ReadMemory = ReadMemory,
				WriteMemory = WriteMemory,
				PeekMemory = PeekMemory,
				DummyReadMemory = ReadMemory,
				OnExecFetch = ExecFetch,
			};
			
			timer = new Timer();
			audio = new Audio();
			serialport = new SerialPort();

			CoreComm = comm;

			_settings = (GBASettings)settings ?? new GBASettings();
			_syncSettings = (GBASyncSettings)syncSettings ?? new GBASyncSettings();
			_controllerDeck = new GBAHawkControllerDeck(_syncSettings.Port1);

			byte[] Bios = null;

			// Load up a BIOS and initialize the correct PPU
			Bios = comm.CoreFileProvider.GetFirmware("GBA", "World", true, "BIOS Not Found, Cannot Load");
			ppu = new PPU();	

			if (Bios == null)
			{
				throw new MissingFirmwareException("Missing Gamboy Bios");
			}

			_bios = Bios;

			// CPU needs to know about GBC status too
			cpu.is_GBC = false;

			Buffer.BlockCopy(rom, 0x100, header, 0, 0x50);

			string hash_md5 = null;
			hash_md5 = "md5:" + rom.HashMD5(0, rom.Length);
			Console.WriteLine(hash_md5);

			_rom = rom;
			Setup_Mapper();

			_frameHz = 60;

			timer.Core = this;
			audio.Core = this;
			ppu.Core = this;
			serialport.Core = this;

			ser.Register<IVideoProvider>(this);
			ser.Register<ISoundProvider>(audio);
			ServiceProvider = ser;

			_settings = (GBASettings)settings ?? new GBASettings();
			_syncSettings = (GBASyncSettings)syncSettings ?? new GBASyncSettings();

			_tracer = new TraceBuffer { Header = cpu.TraceHeader };
			ser.Register<ITraceable>(_tracer);

			SetupMemoryDomains();
			HardReset();
		}

		public DisplayType Region => DisplayType.NTSC;

		private readonly GBAHawkControllerDeck _controllerDeck;

		private void HardReset()
		{
			in_vblank = true; // we start off in vblank since the LCD is off
			in_vblank_old = true;

			RAM_Bank = 1; // RAM bank always starts as 1 (even writing zero still sets 1)

			Register_Reset();
			timer.Reset();
			ppu.Reset();
			audio.Reset();
			serialport.Reset();

			cpu.SetCallbacks(ReadMemory, PeekMemory, PeekMemory, WriteMemory);

			_vidbuffer = new int[VirtualWidth * VirtualHeight];
		}

		private void ExecFetch(uint addr)
		{
			MemoryCallbacks.CallExecutes(addr, "System Bus");
		}

		private void Setup_Mapper()
		{
			// setup up mapper based on header entry
			string mppr;

			switch (header[0x47])
			{
				case 0x0: mapper = new MapperDefault();		mppr = "NROM";							break;
				case 0x8: mapper = new MapperDefault();		mppr = "NROM";							break;
				case 0x9: mapper = new MapperDefault();		mppr = "NROM";		has_bat = true;		break;

				case 0x4:
				case 0x7:
				case 0xA:
				case 0xE:
				case 0x14:
				case 0x15:
				case 0x16:
				case 0x17:
				case 0x18:
				case 0x1F:
				case 0x21:
				default:
					// mapper not implemented
					Console.WriteLine(header[0x47]);
					throw new Exception("Mapper not implemented");
			}

			Console.Write("Mapper: ");
			Console.WriteLine(mppr);

			cart_RAM = null;

			switch (header[0x49])
			{
				case 1:
					cart_RAM = new byte[0x800];
					break;
				case 2:
					cart_RAM = new byte[0x2000];
					break;
				case 3:
					cart_RAM = new byte[0x8000];
					break;
				case 4:
					cart_RAM = new byte[0x20000];
					break;
				case 5:
					cart_RAM = new byte[0x10000];
					break;
				case 0:
					Console.WriteLine("Mapper Number indicates Battery Backed RAM but none present.");
					Console.WriteLine("Disabling Battery Setting.");
					has_bat = false;
					break;
			}

			mapper.Core = this;
			mapper.Initialize();
			
			// Extra RTC initialization for mbc3
			if (mppr == "MBC3")
			{
				Use_MT = true;
				int days = (int)Math.Floor(_syncSettings.RTCInitialTime / 86400.0);

				int days_upper = ((days & 0x100) >> 8) | ((days & 0x200) >> 2);

				mapper.RTC_Get((byte)days_upper, 4);
				mapper.RTC_Get((byte)(days & 0xFF), 3);

				int remaining = _syncSettings.RTCInitialTime - (days * 86400);

				int hours = (int)Math.Floor(remaining / 3600.0);

				mapper.RTC_Get((byte)(hours & 0xFF), 2);

				remaining = remaining - (hours * 3600);

				int minutes = (int)Math.Floor(remaining / 60.0);

				mapper.RTC_Get((byte)(minutes & 0xFF), 1);

				remaining = remaining - (minutes * 60);

				mapper.RTC_Get((byte)(remaining & 0xFF), 0);
			}
		}
	}
}
