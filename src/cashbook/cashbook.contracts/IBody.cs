using System;
using cashbook.contracts.data;

namespace cashbook.contracts
{
	public interface IBody
	{
		ValidationReport Validate_candidate_transaction (DateTime transactionDate, string description, decimal amount, bool force);

		BalanceSheet Load_monthly_balance_sheet (DateTime month);

		void Deposit (DateTime transactionDate, decimal amount, string description, bool force,
		             Action<Balance> onSuccess, Action<string> onError);
			
		void Withdraw (DateTime transactionDate, decimal amount, string description, bool force,
		              Action<Balance> onSuccess, Action<string> onError);

		ExportReport Export (DateTime fromMonth, DateTime toMonth);
	}
}

