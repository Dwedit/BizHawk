using System;
using System.ComponentModel;

using Newtonsoft.Json;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Nintendo.GBAHawk
{
	public partial class GBAHawk : IEmulator, IStatable, ISettable<GBAHawk.GBASettings, GBAHawk.GBASyncSettings>
	{
		public GBASettings GetSettings()
		{
			return _settings.Clone();
		}

		public GBASyncSettings GetSyncSettings()
		{
			return _syncSettings.Clone();
		}

		public bool PutSettings(GBASettings o)
		{
			_settings = o;
			return false;
		}

		public bool PutSyncSettings(GBASyncSettings o)
		{
			bool ret = GBASyncSettings.NeedsReboot(_syncSettings, o);
			_syncSettings = o;
			return ret;
		}

		private GBASettings _settings = new GBASettings();
		public GBASyncSettings _syncSettings = new GBASyncSettings();

		public class GBASettings
		{
			public enum PaletteType
			{
				BW,
				Gr
			}

			[DisplayName("Color Mode")]
			[Description("Pick Between Green scale and Grey scale colors")]
			[DefaultValue(PaletteType.BW)]
			public PaletteType Palette { get; set; }


			public GBASettings Clone()
			{
				return (GBASettings)MemberwiseClone();
			}
		}

		public class GBASyncSettings
		{
			[JsonIgnore]
			public string Port1 = GBAHawkControllerDeck.DefaultControllerName;

			public enum ControllerType
			{
				Default,
				Kirby
			}

			[JsonIgnore]
			private ControllerType _GBController;

			[DisplayName("Controller")]
			[Description("Select Controller Type")]
			[DefaultValue(ControllerType.Default)]
			public ControllerType GBController
			{
				get { return _GBController; }
				set
				{
					if (value == ControllerType.Default) { Port1 = GBAHawkControllerDeck.DefaultControllerName; }
					else { Port1 = "Gameboy Controller + Kirby"; }

					_GBController = value;
				}
			}

			public enum ConsoleModeType
			{
				Auto,
				GB,
				GBC
			}

			[DisplayName("Console Mode")]
			[Description("Pick which console to run, 'Auto' chooses from ROM extension, 'GB' and 'GBC' chooses the respective system")]
			[DefaultValue(ConsoleModeType.Auto)]
			public ConsoleModeType ConsoleMode { get; set; }

			[DisplayName("CGB in GBA")]
			[Description("Emulate GBA hardware running a CGB game, instead of CGB hardware.  Relevant only for titles that detect the presense of a GBA, such as Shantae.")]
			[DefaultValue(false)]
			public bool GBACGB { get; set; }

			[DisplayName("RTC Initial Time")]
			[Description("Set the initial RTC time in terms of elapsed seconds.")]
			[DefaultValue(0)]
			public int RTCInitialTime
			{
				get { return _RTCInitialTime; }
				set { _RTCInitialTime = Math.Max(0, Math.Min(1024 * 24 * 60 * 60, value)); }
			}

			[DisplayName("Timer Div Initial Time")]
			[Description("Don't change from 0 unless it's hardware accurate. GBA GBC mode is known to be 8.")]
			[DefaultValue(0)]
			public int DivInitialTime
			{
				get { return _DivInitialTime; }
				set { _DivInitialTime = Math.Min((ushort)65535, (ushort)value); }
			}

			[DisplayName("Use Existing SaveRAM")]
			[Description("When true, existing SaveRAM will be loaded at boot up")]
			[DefaultValue(false)]
			public bool Use_SRAM { get; set; }


			[JsonIgnore]
			private int _RTCInitialTime;
			[JsonIgnore]
			public ushort _DivInitialTime;


			public GBASyncSettings Clone()
			{
				return (GBASyncSettings)MemberwiseClone();
			}

			public static bool NeedsReboot(GBASyncSettings x, GBASyncSettings y)
			{
				return !DeepEquality.DeepEquals(x, y);
			}
		}
	}
}
