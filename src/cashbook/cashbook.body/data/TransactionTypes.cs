using System;
using eventstore.contract;
using eventstore.internals;
using System.Collections.Generic;

namespace cashbook.body.data
{
	public enum TransactionTypes {
		Deposit,
		Withdrawal
	}
}