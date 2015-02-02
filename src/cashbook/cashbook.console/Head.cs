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
		void Sheet(
			[Aliases("m,d,date")] DateTime month
		) {
			var sheetVM = this.body.Load_monthly_balance_sheet (month);

			Console.WriteLine ("{0:MMMM yyyy}", sheetVM.Month);
			foreach(var item in sheetVM.Items) {
				Console.WriteLine ("{0:dd}.\t{1}\t{2:c}\t{3:c}", item.TransactionDate, item.Description, item.Value, item.RunningTotalValue);
			}
		}
			

		[Verb]
		void Withdraw (
			[Required, Aliases("d")] 								DateTime transactionDate, 
			[Required, Aliases("a")] 								double amount, 
			[Required, Aliases("desc"), DefaultValue("Deposit")] 	string description,
			[Aliases("f")] 											bool force) {
			body.Withdraw (transactionDate, amount, description, force,
				(Balance newBalance) =>
					Console.WriteLine("New balance at end of {0:MMM yyyy}: {1:C}", newBalance.Month, newBalance.Value),
				(string errormsg) => {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(errormsg);
				}
			);
		}


		[Verb]
		void Deposit (
			[Required, Aliases("d")] 					DateTime transactionDate, 
			[Required, Aliases("a")] 					double amount, 
			[Aliases("desc"), DefaultValue("Deposit")] 	string description,
			[Aliases("f")] 								bool force) {
			body.Deposit (transactionDate, amount, description, force,
				(Balance newBalance) =>
				Console.WriteLine("New balance at end of {0:MMM yyyy}: {1:C}", newBalance.Month, newBalance.Value),
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
