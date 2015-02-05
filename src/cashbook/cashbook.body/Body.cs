using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.contracts.data;
using System.Collections.Generic;
using cashbook.body.data;
using System.Linq;
using System.IO;
using cashbook.contracts;

namespace cashbook.body
{
	public class Body : IBody
	{
	    readonly Repository repo;
	    readonly Func<Transaction[], Cashbook> cashbookFactory;

		public Body(Repository repo, Func<Transaction[],Cashbook> cashbookFactory) {
			this.cashbookFactory = cashbookFactory;
			this.repo = repo;
		}


		public ValidationReport Validate_candidate_transaction (DateTime transactionDate, string description, decimal amount, bool force) {
			return new ValidationReport{ 
				DateValidatedForDeposit = CashbookValidation.Validate_transaction_date (TransactionTypes.Deposit, transactionDate, force),
				DateValidatedForWithdrawal =  CashbookValidation.Validate_transaction_date (TransactionTypes.Withdrawal, transactionDate, force),
				DescriptionValidatedForWithdrawal = CashbookValidation.Validate_transaction_description(TransactionTypes.Withdrawal, description),
				AmountValidated = CashbookValidation.Validate_transaction_amount(amount),
			};
		}
			

		public BalanceSheet Load_monthly_balance_sheet(DateTime month) {
			var allTx = this.repo.Load_all_transactions ();
			var cb = this.cashbookFactory (allTx.ToArray());
			return cb[month];
		}


		public void Deposit(DateTime transactionDate, decimal amount, string description, bool force,
							Action<Balance> onSuccess, Action<string> onError
		) {
			CashbookValidation.Validate_transaction_data (TransactionTypes.Deposit,  transactionDate, description, amount, force,
				() => {
                    this.repo.Make_deposit(transactionDate, Math.Abs(amount), description); 
                    
                    var transactions = this.repo.Load_all_transactions().ToArray();
				    var cb = this.cashbookFactory(transactions);

					var newBalance = cb.Calculate_end_of_month_balance(transactionDate);
					onSuccess(newBalance);
				},
				onError);
		}


		public void Withdraw(DateTime transactionDate, decimal amount, string description, bool force,
							 Action<Balance> onSuccess, Action<string> onError
		) {
			CashbookValidation.Validate_transaction_data(TransactionTypes.Withdrawal, transactionDate, description, amount, force,
				() => {
					this.repo.Make_withdrawal(transactionDate, Math.Abs(amount), description); 

					var transactions = this.repo.Load_all_transactions().ToArray();
					var cb = this.cashbookFactory(transactions);

					var newBalance = cb.Calculate_end_of_month_balance(transactionDate);
					onSuccess(newBalance);
				},
				onError);
		}


		public ExportReport Export(DateTime fromMonth, DateTime toMonth) {
			// create month range
			if (toMonth == DateTime.MinValue) toMonth = fromMonth;
			fromMonth = fromMonth.ToMonth ();
			toMonth = toMonth.ToMonth ();

			if (toMonth < fromMonth) {
				var t = toMonth;
				toMonth = fromMonth;
				fromMonth = t;
			}

			var months = new List<DateTime> ();
			var currMonth = fromMonth;
			while (currMonth <= toMonth) {
				months.Add (currMonth);
				currMonth = currMonth.AddMonths (1);
			}
				
			// get balance sheets for month range
			var allTx = this.repo.Load_all_transactions ();
			var cb = this.cashbookFactory (allTx.ToArray());
			var balanceSheets = months.Select (m => cb [m]);

			// extract transaction items
			var txItems = balanceSheets.SelectMany (bs => bs.TransactionItems).ToArray();

			// create filename
			var filename = string.Format ("cashbook-{0:yyyyMM}-{1:yyyyMM}.csv", fromMonth, toMonth);

			// write items to file
			using(var sw = new StreamWriter(filename)) {
				foreach(var txi in txItems)
					sw.WriteLine("{0:d};\"{1}\";{2:f}", txi.TransactionDate, txi.Description, txi.Value);
			}
				
			return new ExportReport{ Filename = filename, NumberOfTransactions =  txItems.Length };
		}
	}
}