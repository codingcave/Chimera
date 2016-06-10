using System;

namespace Chimera
{
	public partial class Chimera:IDisposable
	{
		private SettingsHandler _settings;
		private static bool _init = false;
		private static bool _initLanguage = false;
		private static Chimera _instance = null;

		private Chimera ()
		{
			_settings = new SettingsHandler ();
			InitSystems ();
		}

		public static Chimera Instance {
			get {
				if (_instance == null)
					_instance = new Chimera ();
				return _instance;
			}
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			//throw new NotImplementedException ();
		}

		#endregion
	}
}

