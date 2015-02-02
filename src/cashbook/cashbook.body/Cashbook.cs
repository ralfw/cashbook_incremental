using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.body.data.contract;
using System.Collections.Generic;
using cashbook.body.data;
using System.Linq;

namespace cashbook.body
{

	public class Cashbook {
	    readonly Transaction[] transactions;

		public Cashbook(Transaction[] transactions) {
			this.transactions = transactions;
		}


		public BalanceSheet this[DateTime month] {
			get {
				var nmonth = month.Normalize ();

				// get items for month
				var items = this.transactions
					.Where(tx => tx.TransactionDate.Normalize() == nmonth)
									.Select (tx => new Item{
									   	TransactionDate = tx.TransactionDate,
										Description = tx.Description,
										Value = tx.Value
									});

				// calculate monthly balanace at start of month
				var monthlyBalances = Calculate_monthly_balances (this.transactions);
				var previousBalance = monthlyBalances
										.Where (b => b < month)
										.LastOrDefault ();



				return new BalanceSheet{ Month = month, Items = items };
			}
		}


		public static void Validate_transaction_date(DateTime txDate, bool force, 
			Action onValid, Action<string> onInvalid) {
			if (txDate > TimeProvider.Now())
				onInvalid ("Cannot execute transactions in the future!");
			else if (!force && (txDate.Year < TimeProvider.Now().Year || txDate.Month < TimeProvider.Now().Month))
				onInvalid ("Cannot execute transactions before current month. Use -force to override.");
			else
				onValid ();
		}

		public Balance Calculate_end_of_month_balance(DateTime date) {
			var monthlyBalances = Calculate_monthly_balances (this.transactions);

			var month = new DateTime (date.Year, date.Month, 1);
			return monthlyBalances.First (b => b.Month == month);
		}

		public IEnumerable<Balance> Calculate_monthly_balances(IEnumerable<Transaction> transactions) {
			// pro monat die tx summieren
			var monthlySums = transactions.Select(tx => new { Month = tx.TransactionDate.Normalize(), Amount = tx.Value})
				.GroupBy(tx => tx.Month)
				.Select(g => new {Month = g.Key, Sum = g.Sum(tx => tx.Amount)});										  
			// monatsendstände akkumulieren
			var balances = new List<Balance> ();
			foreach (var s in monthlySums) {
				var b = new Balance{Month = s.Month, Value = s.Sum};
				if (balances.Count > 0)
					b.Value += balances [balances.Count - 1].Value;
				balances.Add (b);
			}
			return balances;
		}
	}
}