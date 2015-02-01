using NUnit.Framework;
using System;
using cashbook.body.data;
using System.Linq;

namespace cashbook.body.tests
{
	[TestFixture ()]
	public class testBody
	{
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
	}
}

