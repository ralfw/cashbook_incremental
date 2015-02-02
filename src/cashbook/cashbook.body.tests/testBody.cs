using NUnit.Framework;
using System;
using cashbook.body.data;
using System.Linq;
using cashbook.contracts.data;

namespace cashbook.body.tests
{
	[TestFixture ()]
	public class testBody
	{
		Body body;

		[SetUp]
		public void Setup() {
			TimeProvider.Now = () => DateTime.Now;

			var es = new eventstore.InMemoryEventStore ();
			var repo = new Repository (es);
			this.body = new Body (repo, txs => new Cashbook(txs));
		}
			

		[Test]
		public void Make_withdrawal_in_curr_month() {
			TimeProvider.Now = () => new DateTime(2014,12,31);

			this.body.Deposit (new DateTime (2014, 11, 2), 100, "", true,
				_ => {},
				null);

			Balance result = null;

			this.body.Withdraw (new DateTime (2014, 12, 2), 42, "", false,
				_ => result = _,
				null);

			Assert.AreEqual (new DateTime (2014, 12, 1), result.Month);
			Assert.AreEqual (58, result.Value);

			this.body.Withdraw (new DateTime (2014, 12, 10), 10, "", false,
				_ => result = _,
				null);
			Assert.AreEqual (48, result.Value);
		}


		[Test]
		public void Deposit_in_current_month() {
			TimeProvider.Now = () => new DateTime(2014,12,31);

			Balance result = null;

			this.body.Deposit (new DateTime (2014, 12, 2), 100, "", false,
				_ => result = _,
				null);
			Assert.AreEqual (new DateTime (2014, 12, 1), result.Month);
			Assert.AreEqual (100, result.Value);

			body.Deposit (new DateTime (2014, 12, 10), 50, "", false,
				_ => result = _,
				null);
			Assert.AreEqual (new DateTime (2014, 12, 1), result.Month);
			Assert.AreEqual (150, result.Value);
		}


		[Test]
		public void Deposit_in_previous_month_fails() {
			string errormsg = "";

			this.body.Deposit (new DateTime (2014, 12, 2), 100, "", false,
				null,
				_ => errormsg = _);

			Assert.IsTrue (errormsg.IndexOf ("-force") > 0);
		}


		[Test]
		public void Deposit_in_previous_month_forced() {
			Balance result = null;

			this.body.Deposit (new DateTime (2014, 12, 2), 100, "", true,
				_ => result = _,
				null);

			Assert.AreEqual (100, result.Value);
		}


		[Test]
		public void Deposit_in_future_month_fails() {
			string errormsg = "";

			body.Deposit (new DateTime (2100, 12, 2), 100, "", false,
				null,
				_ => errormsg = _);

			Assert.IsTrue (errormsg.IndexOf ("future") > 0);
		}


		[Test]
		public void Load_monthly_balance_sheet() {
			TimeProvider.Now = () => new DateTime(2014,12,31);

			this.body.Deposit (new DateTime (2014, 11, 2), 100, "", true, _ => {}, null);
			this.body.Withdraw (new DateTime (2014, 11, 10), 10, "", true, _ => {}, null);
			this.body.Withdraw (new DateTime (2014, 12, 3), 10, "", true, _ => {}, null);
			this.body.Withdraw (new DateTime (2014, 12, 11), 5, "", true, _ => {}, null);
			this.body.Withdraw (new DateTime (2014, 12, 22), 5, "", true, _ => {}, null);

			var bs = this.body.Load_monthly_balance_sheet (new DateTime (2014, 11, 10));
			Assert.AreEqual(new DateTime(2014, 11, 1), bs.Month);
			Assert.AreEqual (4, bs.Items.Count());

			Assert.AreEqual (0.0, bs.Items [0].Value);
			Assert.AreEqual (0.0, bs.Items [0].RunningTotalValue);
			Assert.AreEqual(new DateTime(2014, 11, 1), bs.Items[0].TransactionDate);

			Assert.AreEqual (100.0, bs.Items [1].Value);
			Assert.AreEqual (100.0, bs.Items [1].RunningTotalValue);

			Assert.AreEqual (-10.0, bs.Items [2].Value);
			Assert.AreEqual (90.0, bs.Items [2].RunningTotalValue);

			Assert.AreEqual (0.0, bs.Items [3].Value);
			Assert.AreEqual (90, bs.Items [3].RunningTotalValue);
			Assert.AreEqual(new DateTime(2014, 11, 30), bs.Items[3].TransactionDate);


			bs = this.body.Load_monthly_balance_sheet(new DateTime(2014, 12, 24));
			Assert.AreEqual(new DateTime(2014, 12, 1), bs.Month);
			Assert.AreEqual(5, bs.Items.Count());

			Assert.AreEqual(90.0, bs.Items[0].RunningTotalValue);
			Assert.AreEqual(new DateTime(2014, 12, 1), bs.Items[0].TransactionDate);

			Assert.AreEqual(-10.0, bs.Items[1].Value);
			Assert.AreEqual(80.0, bs.Items[1].RunningTotalValue);

			Assert.AreEqual(0.0, bs.Items[4].Value);
			Assert.AreEqual(70, bs.Items[4].RunningTotalValue);
			Assert.AreEqual(new DateTime(2014, 12, 31), bs.Items[4].TransactionDate);
		}
	}
}

