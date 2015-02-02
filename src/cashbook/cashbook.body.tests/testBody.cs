using NUnit.Framework;
using System;
using cashbook.body.data;
using System.Linq;
using cashbook.body.data.contract;

namespace cashbook.body.tests
{
	[TestFixture ()]
	public class testBody
	{
		[SetUp]
		public void Setup() {
			TimeProvider.Now = () => DateTime.Now;
		}
			
		[Test]
		public void Deposit_in_current_month() {
			TimeProvider.Now = () => new DateTime(2014,12,31);

			var es = new eventstore.InMemoryEventStore ();
			var repo = new Repository (es);
			var body = new Body (repo);

			Balance result = null;
			body.Deposit (new DateTime (2014, 12, 2), 100, "", false,
				_ => result = _,
				null);
			Assert.AreEqual (new DateTime (2014, 12, 1), result.CuttoffDate);
			Assert.AreEqual (100, result.Amount);
			body.Deposit (new DateTime (2014, 12, 10), 50, "", false,
				_ => result = _,
				null);
			Assert.AreEqual (new DateTime (2014, 12, 1), result.CuttoffDate);
			Assert.AreEqual (150, result.Amount);
		}

		[Test]
		public void Deposit_in_previous_month_fails() {
			var es = new eventstore.InMemoryEventStore ();
			var repo = new Repository (es);
			var body = new Body (repo);

			string errormsg = "";
			body.Deposit (new DateTime (2014, 12, 2), 100, "", false,
				null,
				_ => errormsg = _);
			Assert.IsTrue (errormsg.IndexOf ("-force") > 0);
		}

		[Test]
		public void Deposit_in_previous_month_forced() {
			var es = new eventstore.InMemoryEventStore ();
			var repo = new Repository (es);
			var body = new Body (repo);

			Balance result = null;
			body.Deposit (new DateTime (2014, 12, 2), 100, "", true,
				_ => result = _,
				null);
			Assert.AreEqual (100, result.Amount);
		}

		[Test]
		public void Deposit_in_future_month_fails() {
			var es = new eventstore.InMemoryEventStore ();
			var repo = new Repository (es);
			var body = new Body (repo);

			string errormsg = "";
			body.Deposit (new DateTime (2100, 12, 2), 100, "", false,
				null,
				_ => errormsg = _);
			Assert.IsTrue (errormsg.IndexOf ("future") > 0);
		}
	}
}

