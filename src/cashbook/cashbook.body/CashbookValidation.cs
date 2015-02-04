using System;
using eventstore.contract;
using eventstore.internals;
using cashbook.contracts.data;
using System.Collections.Generic;
using cashbook.body.data;
using System.Linq;

namespace cashbook.body
{
	class CashbookValidation {
		public static void Validate_transaction_data(TransactionTypes type, DateTime txDate, string description, decimal amount, bool force, 
			Action onValid, Action<string> onInvalid) 
		{
			var vr = Validate_transaction_date (type, txDate, force);
			if (!vr.IsValid) { onInvalid (vr.Explanation); return; }

			vr = Validate_transaction_description(type, description);
			if (!vr.IsValid) { onInvalid (vr.Explanation); return; }

			vr = Validate_transaction_amount (amount);
			if (!vr.IsValid) { onInvalid (vr.Explanation); return; }

			onValid ();
		}


		public static ValidationReport.ValidationResult Validate_transaction_date(TransactionTypes type, DateTime txDate, bool force) {
			if (txDate > TimeProvider.Now ())
				return new ValidationReport.ValidationResult {
					IsValid = false,
					Explanation = "Cannot execute transactions in the future!"
				};

			if (!force && (txDate.Year < TimeProvider.Now().Year || txDate.Month < TimeProvider.Now().Month))
				return new ValidationReport.ValidationResult {
					IsValid = false,
					Explanation = "Cannot execute transactions before current month! Use -force to override."
				};

			return new ValidationReport.ValidationResult ();
		}

		public static ValidationReport.ValidationResult Validate_transaction_description(TransactionTypes type, string description) {
			if (type == TransactionTypes.Withdrawal && string.IsNullOrEmpty ((description)))
				return new ValidationReport.ValidationResult{ 
					IsValid = false,
					Explanation = "Cannot execute withdrawal without a description!"
				};
			return new ValidationReport.ValidationResult ();
		}

		public static ValidationReport.ValidationResult Validate_transaction_amount(decimal amount) {
			if (amount == 0.0m)
				return new ValidationReport.ValidationResult{ 
					IsValid = false,
					Explanation = "Cannot execute transaction with 0.0 amount!"
				};
			return new ValidationReport.ValidationResult ();
		}
	}
}