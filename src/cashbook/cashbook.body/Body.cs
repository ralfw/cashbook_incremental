using System;

namespace cashbook.body
{
	public class Body
	{
		public void Deposit(DateTime transactionDate, double amount, string description,
							Action<Balance> onSuccess, Action<string> onError
		) {
			Console.WriteLine ("body.deposit: {0}, {1}, {2}", transactionDate, amount, description);
			if (DateTime.Now.Second % 2 == 0)
				onSuccess (new Balance{ CuttoffDate = DateTime.Now, Amount = 10 * amount });
			else
				onError ("Cannot deposit money!");
		}
	}


	public class Balance {
		public DateTime CuttoffDate;
		public double Amount;
	}
}

