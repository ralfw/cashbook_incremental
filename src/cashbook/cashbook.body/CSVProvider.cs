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

	public class CSVProvider {
		public void Export(IEnumerable<BalanceSheet.Item> items, string filepath) {
			using(var sw = new StreamWriter(filepath)) {
				foreach(var item in items)
					sw.WriteLine("{0:d};\"{1}\";{2:f}", item.TransactionDate, item.Description, item.Value);
			}
		}
	}
}