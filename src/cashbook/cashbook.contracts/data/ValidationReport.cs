using System;
using System.Collections.Generic;
using System.Linq;

namespace cashbook.contracts.data
{
	public class ValidationReport {
		public ValidationResult DateValidatedForDeposit;
		public ValidationResult DateValidatedForWithdrawal;
		public ValidationResult DescriptionValidatedForWithdrawal;
		public ValidationResult AmountValidated;

		public ValidationResult OverallResult {
			get { 
				return ValidationResult.Combine (this.DateValidatedForDeposit, 
												 this.DateValidatedForWithdrawal, 
												 this.DescriptionValidatedForWithdrawal,
												 this.AmountValidated);
			}
		}

		public class ValidationResult {
			public bool IsValid = true;
			public string Explanation = "";

			public static ValidationResult Combine(params ValidationResult[] result) {
				return new ValidationResult { 
					IsValid = result.All(r => r.IsValid),
					Explanation = string.Join("\n", result.Select(r => r.Explanation))
				};
			}
		}
	}
}