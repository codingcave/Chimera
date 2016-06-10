using System;
using System.IO;
using System.Xml.Linq;
using Chimera.Extension;
using Mono.Unix;
using System.Reflection;

namespace Chimera
{
	public class SettingsHandler
	{
		private string _appData;
		private string _config;

		private XDocument _configFile;

		public SettingsHandler ()
		{
			_appData = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments), ".chimera");
			_config = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), "chimera");

			if (File.Exists (Path.Combine (_appData, "settings.xml"))) {
				using (FileStream fs = File.OpenRead (Path.Combine (_appData, "settings.xml"))) {
					_configFile = XDocument.Load (fs);
				}
			} else {
				_configFile = new XDocument (new XElement ("ChimeraSettings"));
			}
		}

		public T LoadSettings<T> (string section)
		{
			XElement xsec = _configFile.Root.Element (section);
			return (T)ExtractSection (typeof(T), xsec);
		}

		private object ExtractSection (Type type, XElement item)
		{
			var secAttr = type.GetCustomAttributes (typeof(SettingsSectionAttribute), true);
			if (secAttr.Length > 0) {
				object obj = Activator.CreateInstance (type);

				var properties = type.GetProperties ();
				foreach (var p in properties) {
					try {
						var attr = p.GetCustomAttribute<SettingsItemAttribute> (true);
						if (attr != null && p.CanWrite) {
							if (item != null) {
								try {
									string name = attr.Name != null ? attr.Name : p.Name;
									var entry = item.Element (name);
									if (entry != null) {
										object val = ParseValue (p.PropertyType, entry);
										p.SetValue (obj, val);
									} else {
										if (attr.DefaultValue != null)
											p.SetValue (obj, attr.DefaultValue);
									}
								} catch (FormatException fex) {
									if (attr.DefaultValue != null)
										p.SetValue (obj, attr.DefaultValue);
								}
							} else {
								if (attr.DefaultValue != null)
									p.SetValue (obj, attr.DefaultValue);
							}
						}
					} catch (Exception ex) {
						// TODO
					}
				}
				return obj;
			} else {
				throw new SettingsParseException (string.Format (Catalog.GetString ("Type {0} cannot be used as settings container."), type.Name));
			}
		}

		private object ParseValue (Type type, XElement x)
		{
			if (type == typeof(string)) {
				return x.Value;
			}
			if (type == typeof(int)) {
				return int.Parse (x.Value);
			}
			if (type == typeof(double)) {
				return double.Parse (x.Value);
			}
			if (type == typeof(bool)) {
				return bool.Parse (x.Value);
			}
			return ExtractSection (type, x);
		}

		public void SaveSettings<T> (string section, T item)
		{
			XElement xsec = _configFile.Root.Element (section);
			XElement newSec = CreateSection (item, section);
			if (newSec != null) {
				if (xsec != null) {
					xsec.ReplaceWith (newSec);
				} else {
					_configFile.Root.Add (newSec);
				}
				_configFile.Save (Path.Combine (_appData, "settings.xml"));
			}
		}

		private XElement CreateSection (object obj, string name)
		{
			var secAttr = obj.GetType ().GetCustomAttributes (typeof(SettingsSectionAttribute), true);
			if (secAttr.Length > 0) {
				XElement item = new XElement (name);
				var properties = obj.GetType ().GetProperties ();
				foreach (var p in properties) {
					try {
						var attr = p.GetCustomAttribute<SettingsItemAttribute> (true);
						if (attr != null && p.CanRead) {
							string entry = attr.Name != null ? attr.Name : p.Name;
							XElement val = ToValue (entry, p.GetValue (obj));
							item.Add (val);
						}
					} catch (Exception ex) {
						// TODO
					}
				}
				return item;
			} else {
				throw new SettingsParseException (string.Format (Catalog.GetString ("Type {0} cannot be used as settings container."), obj.GetType ().Name));
			}
		}

		private XElement ToValue (string name, object value)
		{
			if (
				value.GetType () == typeof(string) ||
				value.GetType () == typeof(int) ||
				value.GetType () == typeof(double) ||
				value.GetType () == typeof(bool)) {
				return new XElement (name, value.ToString ());
			}
			return CreateSection (value, name);
		}

		public string ApplicationDataLocation {
			get {
				return _appData;
			}
		}

		public string ConfigurationLocation {
			get {
				return _config;
			}
		}
	}
}