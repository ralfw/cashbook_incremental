using System;
using System.Linq;
using eventstore.contract;
using eventstore.internals;
using System.Collections.Generic;
using cashbook.body.data;

namespace cashbook.body
{

	public class Repository {
		enum Eventnames {
			DepositMade,
			WithdrawalMade
		}

		IEventStore es;

		public Repository(IEventStore es) {
			this.es = es;
		}


		public void Make_deposit(DateTime transactionDate, decimal amount, string description) {
			var e = new Event (transactionDate.ToContext(), 
							   Eventnames.DepositMade.ToString(), 
							   string.Format ("{0:s}\t{1}\t{2}", transactionDate, amount, description));
			this.es.Record (e);
		}


		public void Make_withdrawal(DateTime transactionDate, decimal amount, string description) {
			var e = new Event (transactionDate.ToContext(), 
							   Eventnames.WithdrawalMade.ToString(), 
							   string.Format ("{0:s}\t{1}\t{2}", transactionDate, amount, description));
			this.es.Record (e);
		}


		public IEnumerable<Transaction> Load_all_transactions() {
			var allEvents = this.es.Replay ();

			var txs = new List<Transaction> ();
			foreach (var e in allEvents) {
				var fields = e.Payload.Split ('\t');
				txs.Add(new Transaction {
					Type = (Eventnames)Enum.Parse (typeof(Eventnames), e.Name) == Eventnames.DepositMade
						   ? TransactionTypes.Deposit : TransactionTypes.Withdrawal,
					TransactionDate = DateTime.Parse (fields [0]),
					Amount = decimal.Parse (fields [1]),
					Description = fields [2]});
			}
			return txs.OrderBy (tx => tx.TransactionDate);
		}
	}


	static class RepositoryExtensions {
		public static string ToContext(this DateTime date) {
			return string.Format ("{0:yyyyMM}", date);
		}
	}
}