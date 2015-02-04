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
		public ValidationReport Validate_candidate_transaction (DateTime transactionDate, string description, decimal amount, bool force) {
			throw new NotImplementedException ();
		}


		public BalanceSheet Load_monthly_balance_sheet (DateTime month)
		{
			return new BalanceSheet{ 
				Month = month,
				Items = new[] {
					new BalanceSheet.Item{
						TransactionDate = month,
						Description = "Deposit",
						Value = 0.0m,
						RunningTotalValue = 100.0m
					},
					new BalanceSheet.Item{
						TransactionDate = month.AddDays(1),
						Description = "Taxi",
						Value = -10,
						RunningTotalValue = 90.0m
					},
					new BalanceSheet.Item{
						TransactionDate = month.AddDays(2),
						Description = "Final",
						Value = 0.0m,
						RunningTotalValue = 90.0m
					},				
				}
			};
		}

		public void Deposit (DateTime transactionDate, decimal amount, string description, bool force, Action<Balance> onSuccess, Action<string> onError)
		{
			Console.WriteLine ("Deposit received: {0}", description);
			if (amount < 0)
				onError ("No deposit < 0!");
			else
				onSuccess (new Balance{ Month = transactionDate, Value = amount });
		}

		public void Withdraw (DateTime transactionDate, decimal amount, string description, bool force, Action<Balance> onSuccess, Action<string> onError)
		{
			Console.WriteLine ("Withdrawal received: {0}", description);
			if (amount > 0)
				onError ("No withdrawal > 0!");
			else
				onSuccess (new Balance{ Month = transactionDate, Value = amount });
		}


		public ExportReport Export(DateTime fromDate, DateTime toDate) {
			return new ExportReport{ Filename = string.Format("{0}---{1}.csv", fromDate, toDate), 
									 NumberOfTransactions = 42 };
		}
		#endregion
	}
}
