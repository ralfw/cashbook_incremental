using System;
using cashbook.body;
using CLAP;
using eventstore;

namespace cashbook.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TimeProvider.Now = () => DateTime.Now;

			var es = new FileEventStore ("events");
			var repo = new Repository (es);
			var body = new Body (repo);
			var head = new Head (body);

			head.Run (args);
		}
	}
}
