using System;
using eventstore.contract;
using eventstore.internals;

namespace cashbook.body.data.contract
{
	public class Balance {
		public DateTime CuttoffDate;
		public double Amount;
	}
}