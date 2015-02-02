using System;
using eventstore.contract;
using eventstore.internals;
using System.Collections.Generic;

namespace cashbook.body.data.contract
{
	public class BalanceSheet {
		public DateTime Month;
		public Item[] Items;

		public class Item {
			public DateTime TransactionDate;
			public string Description;
			public double Value;
			public double RunningTotalValue;
		}
	}	
}