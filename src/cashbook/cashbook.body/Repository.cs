using System;
using eventstore.contract;
using eventstore.internals;

namespace cashbook.body
{

	public class Repository {
		IEventStore es;

		public Repository(IEventStore es) {
			this.es = es;
		}

		public void Make_deposit(DateTime transactionDate, double amount, string description) {
			var e = new Event (transactionDate.ToContext(), 
							   "DepositMade", 
							   string.Format ("{0:s}\t{1}\t{2}", transactionDate, amount, description));
			this.es.Record (e);
		}
	}


	static class RepositoryExtensions {
		public static string ToContext(this DateTime date) {
			return string.Format ("{0:yyyyMM}", date);
		}
	}
}