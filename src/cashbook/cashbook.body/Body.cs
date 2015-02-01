using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.body.data.contract;
using System.Collections.Generic;
using cashbook.body.data;
using System.Linq;

namespace cashbook.body
{
	public class Body
	{
		Repository repo;

		public Body(Repository repo) {
			this.repo = repo;
		}


		public void Deposit(DateTime transactionDate, double amount, string description,
							Action<Balance> onSuccess, Action<string> onError
		) {
			Validate_transaction_date (transactionDate,
				() => {
					this.repo.Make_deposit(transactionDate, Math.Abs(amount), description);
					var newBalance = Calculate_end_of_month_balance(transactionDate);
					onSuccess(newBalance);
				},
				onError);
		}


		private void Validate_transaction_date(DateTime txDate, 
											   Action onValid, Action<string> onInvalid) {
			if (txDate > DateTime.Now)
				onInvalid ("Cannot execute transactions in the future!");
			else if (txDate.Year < DateTime.Now.Year || txDate.Month < DateTime.Now.Month)
				onInvalid ("Cannot execute transactions before current month. Use -force to override.");
			else
				onValid ();
		}


		internal Balance Calculate_end_of_month_balance(DateTime date) {
			var allTx = this.repo.Load_all_transactions ();
			var monthlyBalances = Calculate_monthly_balances (allTx);
			// in frage stehenden monat herauspicken
			var month = new DateTime (date.Year, date.Month, 1);
			return monthlyBalances.First (b => b.CuttoffDate == month);
		}


		internal IEnumerable<Balance> Calculate_monthly_balances(IEnumerable<Transaction> transactions) {
			// pro monat die tx summieren
			var monthlySums = transactions.Select(tx => new { Month = new DateTime(tx.TransactionDate.Year, tx.TransactionDate.Month, 1), Amount = tx.SignedAmount})
										  .GroupBy(tx => tx.Month)
										  .Select(g => new {Month = g.Key, Sum = g.Sum(tx => tx.Amount)});										  
			// monatsendstände akkumulieren
			var balances = new List<Balance> ();
			foreach (var s in monthlySums) {
				var b = new Balance{CuttoffDate = s.Month, Amount = s.Sum};
				if (balances.Count > 0)
					b.Amount += balances [balances.Count - 1].Amount;
				balances.Add (b);
			}
			return balances;
		}
	}
}