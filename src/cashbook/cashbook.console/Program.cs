using System;
using cashbook.body;
using CLAP;

namespace cashbook.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var body = new Body ();
			var head = new Head (body);

			head.Run (args);
		}
	}
}
