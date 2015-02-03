using System;
using System.Linq;
using cashbook.body;
using CLAP;
using cashbook.contracts.data;
using cashbook.contracts;

namespace cashbook.console
{
	class Head {
		IBody body;

		public Head(IBody body) {
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
			Console.WriteLine (Format_BalanceSheetItem (sheetVM.Items.First (), true));
			sheetVM.Items.Skip (1)
						 .Take (sheetVM.Items.Length - 2)
						 .ToList ()
						 .ForEach (i => Console.WriteLine(Format_BalanceSheetItem(i, false)));
			Console.WriteLine (Format_BalanceSheetItem (sheetVM.Items.Last (), true));
		}

		static string Format_BalanceSheetItem(BalanceSheet.Item item, bool balanceOnly) {
			return string.Format ("{0:dd}.\t{1, -40}\t{2, 15}\t{3, 15:c}", 
									item.TransactionDate, 
									item.Description, 
									balanceOnly ? "" : item.Value.ToString("c"), 
									item.RunningTotalValue);
		}

			

		[Verb]
		void Withdraw (
			[Required, Aliases("d")] 								DateTime transactionDate, 
			[Required, Aliases("a")] 								decimal amount, 
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
			[Required, Aliases("a")] 					decimal amount, 
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
