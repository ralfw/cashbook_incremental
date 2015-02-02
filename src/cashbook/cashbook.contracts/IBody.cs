using System;
using cashbook.contracts.data;

namespace cashbook.contracts
{
	public interface IBody
	{
		BalanceSheet Load_monthly_balance_sheet (DateTime month);


		void Deposit (DateTime transactionDate, double amount, string description, bool force,
		             Action<Balance> onSuccess, Action<string> onError);


		void Withdraw (DateTime transactionDate, double amount, string description, bool force,
		              Action<Balance> onSuccess, Action<string> onError);
	}
}

