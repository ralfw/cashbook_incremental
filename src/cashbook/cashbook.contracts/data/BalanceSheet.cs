using System;
using System.Linq;

namespace cashbook.contracts.data
{
	public class BalanceSheet {
		public DateTime Month;
		public Item[] Items;

		public Item[] TransactionItems {
			get { 
				return Items.Skip (1).Take (Items.Length - 2).ToArray ();
			}
		}

		public class Item {
			public DateTime TransactionDate;
			public string Description;
			public decimal Value;
			public decimal RunningTotalValue;
		}
	}	
}