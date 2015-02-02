using System;
using cashbook.contracts;
using cashbook.contracts.data;

namespace cashbook.console.tests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var body = new MockBody ();
			var head = new Head (body);

			head.Run (args);
		}
	}

	class MockBody : IBody {
		#region IBody implementation

		public BalanceSheet Load_monthly_balance_sheet (DateTime month)
		{
			return new BalanceSheet{ 
				Month = month,
				Items = new[] {
					new BalanceSheet.Item{
						TransactionDate = month,
						Description = "Deposit",
						Value = 0.0,
						RunningTotalValue = 100.0
					},
					new BalanceSheet.Item{
						TransactionDate = month.AddDays(1),
						Description = "Taxi",
						Value = -10,
						RunningTotalValue = 90.0
					},
					new BalanceSheet.Item{
						TransactionDate = month.AddDays(2),
						Description = "Final",
						Value = 0.0,
						RunningTotalValue = 90.0
					},				
				}
			};
		}

		public void Deposit (DateTime transactionDate, double amount, string description, bool force, Action<Balance> onSuccess, Action<string> onError)
		{
			Console.WriteLine ("Deposit received: {0}", description);
			if (amount < 0)
				onError ("No deposit < 0!");
			else
				onSuccess (new Balance{ Month = transactionDate, Value = amount });
		}

		public void Withdraw (DateTime transactionDate, double amount, string description, bool force, Action<Balance> onSuccess, Action<string> onError)
		{
			Console.WriteLine ("Withdrawal received: {0}", description);
			if (amount > 0)
				onError ("No withdrawal > 0!");
			else
				onSuccess (new Balance{ Month = transactionDate, Value = amount });
		}

		#endregion
	}
}
