using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.contracts.data;
using System.Collections.Generic;
using cashbook.body.data;
using System.Linq;

namespace cashbook.body
{

	public static class CashbookExtensions {
		public static DateTime Normalize(this DateTime date) {
			return new DateTime (date.Year, date.Month, 1);
		}
	}
}