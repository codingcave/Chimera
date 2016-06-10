using System;

namespace Chimera.Extension
{
	[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public sealed class SettingsItemAttribute : Attribute
	{
		private SettingsValue _value = SettingsValue.Default;
		private string _name = null;
		private object _default = null;

		public SettingsValue ValueExtract { 
			get {
				return _value;
			}
			set {
				_value = value;
			} 
		}

		public string Name {
			get {
				return _name;
			}
			set {
				this._name = value;
			}
		}

		public object DefaultValue { 
			get {
				return _default;
			}
		}

		public SettingsItemAttribute (object defaultValue = null)
		{
			_default = defaultValue;
		}
	}
}

