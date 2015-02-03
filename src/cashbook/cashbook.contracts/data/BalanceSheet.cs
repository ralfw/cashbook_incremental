using System;

namespace cashbook.contracts.data
{
	public class BalanceSheet {
		public DateTime Month;
		public Item[] Items;

		public class Item {
			public DateTime TransactionDate;
			public string Description;
			public decimal Value;
			public decimal RunningTotalValue;
		}
	}	
}