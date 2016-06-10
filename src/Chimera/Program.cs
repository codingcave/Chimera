using System;

namespace Chimera
{
	public class MainClass
	{
		private static Chimera chimera;
		private static string[] startArgs;

		public static void Main (string[] args)
		{
			startArgs = args;
			chimera = Chimera.Instance;

			chimera.Dispose ();
		}
	}
}
