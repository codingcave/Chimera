using System;

namespace Chimera.Extension
{
	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
	public sealed class SettingsSectionAttribute : Attribute
	{
		public SettingsSectionAttribute ()
		{

		}
	}
}

