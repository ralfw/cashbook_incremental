using System;
using cashbook.body;
using CLAP;
using cashbook.body.data.contract;

namespace cashbook.console
{
	class Head {
		Body body;

		public Head(Body body) {
			this.body = body;
		}

		public void Run(string[] args) {
			Parser.Run (args, this);
		}

		[Verb(IsDefault=true)]
		void Sheet() {
			Console.WriteLine ("sheet...");
		}

		[Verb]
		void Withdraw (){
			Console.WriteLine ("withdraw...");
		}

		[Verb]
		void Deposit (
			[Required, Aliases("d")] 					DateTime transactionDate, 
			[Required, Aliases("a")] 					double amount, 
			[Aliases("desc"), DefaultValue("Deposit")] 	string description) {
			body.Deposit (transactionDate, amount, description,
				(Balance newBalance) =>
					Console.WriteLine("New balance as of {0:d}: {1:C}", newBalance.CuttoffDate, newBalance.Amount),
				(string errormsg) => {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(errormsg);
				}
			);
		}

		[Verb]
		void Export() {
			Console.WriteLine ("export...");
		}
	}
}
