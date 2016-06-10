using System;
using Mono.Unix;
using Mono.Addins;
using Chimera.Extension;
using System.IO;

namespace Chimera
{
	public partial class Chimera
	{
		public static void InitLanguage ()
		{
			if (!_initLanguage) {
				Catalog.Init ("main", AppDomain.CurrentDomain.BaseDirectory + "locale");
				_initLanguage = true;
			}
		}

		private void InitSystems ()
		{
			if (!_init) {
				_init = true;
				InitLanguage ();

				if (!Directory.Exists (System.IO.Path.Combine (_settings.ApplicationDataLocation, "plugins"))) {
					Directory.CreateDirectory (System.IO.Path.Combine (_settings.ApplicationDataLocation, "plugins"));
				}

				File.WriteAllLines (System.IO.Path.Combine (_settings.ApplicationDataLocation, "plugins", "common.addins"), 
					new string[] {
						"<Addins>",
						"<Directory>" + System.IO.Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "plugins") + "</Directory>",
						"</Addins>"
					}
				);

				AddinManager.Initialize (
					_settings.ConfigurationLocation,
					System.IO.Path.Combine (_settings.ApplicationDataLocation, "plugins"),
					_settings.ConfigurationLocation
				);

				AddinManager.Registry.ResetConfiguration ();
				AddinManager.Registry.Rebuild (null);
				AddinManager.Registry.Update (null);
			}
		}
	}
}

