using System;
using cashbook.body;
using CLAP;
using eventstore;
using cashbook.body.data;

namespace cashbook.console
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			TimeProvider.Now = () => DateTime.Now;

			var es = new FileEventStore ("events");
			var repo = new Repository (es);
			var cashbookFactory = new Func<Transaction[], Cashbook>(transactions => new Cashbook (transactions));
			var body = new Body (repo, cashbookFactory);
			var head = new Head (body);

			head.Run (args);
		}
	}
}
