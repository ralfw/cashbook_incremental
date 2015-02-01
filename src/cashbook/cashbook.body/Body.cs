using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.body.data.contract;

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
					onSuccess(new Balance{CuttoffDate = transactionDate, Amount = amount * 10});
				},
				onError);
		}


		private void Validate_transaction_date(DateTime txDate, 
											   Action onValid, Action<string> onInvalid) {
			if (txDate > DateTime.Now)
				onInvalid ("Cannot execte transactions in the future!");
			else if (txDate.Year < DateTime.Now.Year || txDate.Month < DateTime.Now.Month)
				onInvalid ("Cannot execute transactions before current month. Use -force to override.");
			else
				onValid ();
		}
	}
}