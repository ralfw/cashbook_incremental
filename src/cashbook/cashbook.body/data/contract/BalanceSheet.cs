using System;
using eventstore.contract;
using eventstore.internals;
using System.Collections.Generic;

namespace cashbook.body.data.contract
{
	public class BalanceSheet {
		public DateTime Month;
		public IEnumerable<Item> Items;
	}	
}