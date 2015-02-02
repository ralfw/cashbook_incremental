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
		public void Calculate_monthly_balances() {
			var body = new Body (null);

			var transactions = new[]{ 
				new Transaction{Type = TransactionTypes.Deposit, TransactionDate = new DateTime(2014, 12, 2), Amount = 100},
				new Transaction{Type = TransactionTypes.Withdrawal, TransactionDate = new DateTime(2014, 12, 10), Amount = 10},
				new Transaction{Type = TransactionTypes.Withdrawal, TransactionDate = new DateTime(2015, 1, 2), Amount = 10},
				new Transaction{Type = TransactionTypes.Deposit, TransactionDate = new DateTime(2015, 1, 15), Amount = 50},
			};

			var balances = body.Calculate_monthly_balances (transactions).ToArray ();

			Assert.AreEqual (2, balances.Count());
			Assert.AreEqual (new DateTime(2014, 12, 1), balances[0].CuttoffDate);
			Assert.AreEqual (90, balances[0].Amount);
			Assert.AreEqual (new DateTime(2015, 1, 1), balances[1].CuttoffDate);
			Assert.AreEqual (130, balances[1].Amount);
		}


		[Test]
		public void Calculate_end_of_month_balance() {
			var es = new eventstore.InMemoryEventStore ();
			var repo = new Repository (es);
			var body = new Body (repo);

			repo.Make_deposit (new DateTime (2014, 12, 2), 100, "");
			repo.Make_deposit (new DateTime (2014, 12, 10), 50, "");
			repo.Make_deposit (new DateTime (2015, 1, 2), 200, "");
			repo.Make_deposit (new DateTime (2015, 1, 31), 10, "");

			Assert.AreEqual (new DateTime(2014,12,1), body.Calculate_end_of_month_balance (new DateTime (2014, 12, 15)).CuttoffDate);
			Assert.AreEqual (150, body.Calculate_end_of_month_balance (new DateTime (2014, 12, 15)).Amount);
			Assert.AreEqual (360, body.Calculate_end_of_month_balance (new DateTime (2015, 1, 31)).Amount);
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

