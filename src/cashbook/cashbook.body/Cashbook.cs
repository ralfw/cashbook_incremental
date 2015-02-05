using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.contracts.data;
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


		public BalanceSheet this[DateTime month] { get {
				var nmonth = month.ToMonth ();
			    var txItems = Get_balance_sheet_items_for_month(nmonth);
			    var balances = Calculate_balances_at_start_and_end_of_month(nmonth);
			    var allItems = Assemble_balance_sheet_items_for_month(balances, txItems, nmonth);
			    return new BalanceSheet{ Month = nmonth, Items = allItems.ToArray() };
			}
		}

	
	    private BalanceSheet.Item[] Get_balance_sheet_items_for_month(DateTime nmonth)
	    {
	        return this.transactions
	                        .Where(tx => tx.TransactionDate.ToMonth() == nmonth)
	                        .Select(tx => new BalanceSheet.Item {
	                                TransactionDate = tx.TransactionDate,
	                                Description = tx.Description,
	                                Value = tx.Value
	                            })
	                        .OrderBy(i => i.TransactionDate)
	                        .ToArray();
	    }

	    private Tuple<Balance,Balance> Calculate_balances_at_start_and_end_of_month(DateTime nmonth)
	    {
            // calculate monthly balance at start of month
	        var monthlyBalances = Calculate_monthly_balances(this.transactions).ToArray();

	        var previousBalance = monthlyBalances.LastOrDefault(b => b.Month < nmonth) ??
	                              new Balance {Month = nmonth.AddMonths(-1), Value = 0};

	        // calculate monthly balance at end of month
	        var currentBalance = monthlyBalances.FirstOrDefault(b => b.Month == nmonth) ??
	                             new Balance {Month = nmonth, Value = previousBalance.Value};
	        return new Tuple<Balance, Balance>(previousBalance, currentBalance);
	    }

        private static IEnumerable<BalanceSheet.Item> Assemble_balance_sheet_items_for_month(Tuple<Balance, Balance> balances, BalanceSheet.Item[] txItems, DateTime nmonth)
        {
            // adjust running monthly value in items
            var runningValue = balances.Item1.Value;
            foreach (var item in txItems)
            {
                runningValue += item.Value;
                item.RunningTotalValue = runningValue;
            }

            // add items for balances
            var allItems = new[] {
	                new BalanceSheet.Item {
	                        TransactionDate = nmonth,
	                        Description = "Initial balance",
	                        Value = 0,
	                        RunningTotalValue = balances.Item1.Value}}
                    .Concat(txItems)
                    .Concat(new[] {
	                        new BalanceSheet.Item {
	                                TransactionDate = nmonth.AddMonths(1).AddDays(-1),
	                                Description = "Final balance",
	                                Value = 0,
	                                RunningTotalValue = balances.Item2.Value}});
            return allItems;
        }


		public BalanceSheet.Item[] Get_balance_sheet_items_in_month_range(DateTime[] months) {

			var balanceSheets = months.Select (m => this [m]);
			return balanceSheets.SelectMany (bs => bs.TransactionItems).ToArray();
		}


		public Balance Calculate_end_of_month_balance(DateTime date) {
			var monthlyBalances = Calculate_monthly_balances (this.transactions);

			var month = new DateTime (date.Year, date.Month, 1);
			return monthlyBalances.First (b => b.Month == month);
		}


		public IEnumerable<Balance> Calculate_monthly_balances(IEnumerable<Transaction> transactions) {
			// pro monat die tx summieren
			var monthlySums = transactions.Select(tx => new { Month = tx.TransactionDate.ToMonth(), Amount = tx.Value})
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