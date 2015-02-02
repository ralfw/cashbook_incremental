using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.body.data.contract;
using System.Collections.Generic;
using cashbook.body.data;
using System.Linq;

namespace cashbook.body
{

	public static class TimeProvider {
		public static Func<DateTime> Now { get; set; }
	}
}