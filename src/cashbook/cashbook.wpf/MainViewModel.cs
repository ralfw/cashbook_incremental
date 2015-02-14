using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using cashbook.body;
using cashbook.contracts;
using cashbook.contracts.data;
using cashbook.wpf.Annotations;

namespace cashbook.wpf
{
    public enum TransactionType
    {
        Withdraw,
        Deposit
    }

    public class MainViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private readonly IBody _body;

        private DateTime _selectedMonth;

        private BalanceSheet _shownBalanceSheet;

        private bool _editForce;
        private DateTime _editDate;
        private string _editDescription;
        private decimal _editAmount;
        private TransactionType _editTransactionType;
        private readonly DelegateCommand _transactionCommand;
        private readonly DelegateCommand _exportCommand;

        private readonly Dictionary<string, ICollection<string>>
            _validationErrors = new Dictionary<string, ICollection<string>>();

        public MainViewModel()
        {
            _transactionCommand = new DelegateCommand(ProcessTransaction, () => !HasErrors);
            _exportCommand = new DelegateCommand(Export, () => true);

            _selectedMonth = FirstOfMonth(DateTime.Today);

            _shownBalanceSheet = new BalanceSheet
            {
                Month = _selectedMonth,
                Items = new BalanceSheet.Item[]{}
            };

            ClearEdit();
       }

        public MainViewModel(IBody body)
            : this()
        {
            this._body = body;
            ShowSelectedMonth();
        }

        public DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = FirstOfMonth(value);
                    OnPropertyChanged("SelectedMonth");
                    ShowSelectedMonth();
                }
            }
        }

        public BalanceSheet ShownBalanceSheet
        {
            get
            {
                return _shownBalanceSheet;
            }
            protected set
            {
                _shownBalanceSheet = value;
                OnPropertyChanged("ShownBalanceSheet");
                OnPropertyChanged("ShownMonth");
                OnPropertyChanged("ShownBalanceSheetItems");
            }
        }

        public string ShownMonth
        {
            get { return ShownBalanceSheet.Month.ToString("MMMM yyyy"); }
        }

        public IEnumerable<BalanceSheetItemViewModel> ShownBalanceSheetItems
        {
            get
            {
                var lastIndex = ShownBalanceSheet.Items.Length - 1;
                return
                    ShownBalanceSheet.Items
                        .Select((item, index) => 
                            new BalanceSheetItemViewModel(item, index == 0 || index == lastIndex));
            }
        }

        public DateTime EditDate
        {
            get { return _editDate; }
            set
            {
                if (_editDate != value)
                {
                    _editDate = value.Date;
                    OnPropertyChanged("EditDate");
                }
            }
        }

        public string EditDescription
        {
            get { return _editDescription; }
            set
            {
                if (_editDescription != value)
                {
                    _editDescription = value;
                    OnPropertyChanged("EditDescription");
                }
            }
        }

        public decimal EditAmount
        {
            get { return _editAmount; }
            set
            {
                if (Math.Abs(_editAmount - value) >= 0.01m)
                {
                    _editAmount = value;
                    OnPropertyChanged("EditAmount");
                }
            }
        }

        public bool EditForce
        {
            get { return _editForce; }
            set
            {
                if (_editForce != value)
                {
                    _editForce = value;
                    OnPropertyChanged("EditForce");
                }
            }
        }

        public TransactionType EditTransactionType
        {
            get { return _editTransactionType; }
            set
            {
                if (_editTransactionType != value)
                {
                    _editTransactionType = value;
                    OnPropertyChanged("EditTransactionType");
                }
            }
        }

        public ICommand TransactionCommand
        {
            get { return _transactionCommand; }
        }

        public ICommand ExportCommand
        {
            get { return _exportCommand; }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors
        {
            get { return _validationErrors.Count > 0; }
        }

        private void ShowSelectedMonth()
        {
            if (_body == null)
                return;

            ShownBalanceSheet = _body.Load_monthly_balance_sheet(SelectedMonth);
        }

        private void Export()
        {
            var fromMonth = SelectedMonth;
            var toMonth = SelectedMonth;

            var exportReport = _body.Export(fromMonth, toMonth);

            MessageBox.Show(String.Format("exported to: {0}", exportReport.Filename));
        }


        private void ProcessTransaction()
        {
            if (_body == null)
                return;

            switch (EditTransactionType)
            {
                case TransactionType.Withdraw:
                    _body.Withdraw(EditDate, EditAmount, EditDescription, EditForce,
                        UpdateUI, ShowErrorMessage("could not withdraw"));
                    break;
                case TransactionType.Deposit:
                    _body.Deposit(EditDate, EditAmount, EditDescription, EditForce,
                        UpdateUI, ShowErrorMessage("could not deposit"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateUI()
        {
            ClearEdit();
            ShowSelectedMonth();
        }

        private void UpdateUI(Balance ignore)
        {
            UpdateUI();
        }

        private Action<string> ShowErrorMessage(string caption)
        {
            return errormsg => MessageBox.Show(errormsg, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ClearEdit()
        {
            EditAmount = 0.0m;
            EditForce = false;
            EditDate = DateTime.Today;
            EditDescription = "";
            EditTransactionType = TransactionType.Withdraw;
        }

        private void ValidateInput()
        {
            if (_body == null)
                return;

            ClearAllErrors();
            var validationReport = _body.Validate_candidate_transaction(EditDate, EditDescription, EditAmount, EditForce);

            ValidateEditField(validationReport.AmountValidated, "EditAmount");

            switch (EditTransactionType)
            {
                case TransactionType.Withdraw:
                    ValidateEditField(validationReport.DescriptionValidatedForWithdrawal, "EditDescription");
                    ValidateEditField(validationReport.DateValidatedForWithdrawal, "EditDate");
                    break;
                case TransactionType.Deposit:
                    ValidateEditField(validationReport.DateValidatedForDeposit, "EditDate");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void ValidateEditField(ValidationReport.ValidationResult validation, string propertyName)
        {
            if (!validation.IsValid)
                SetErrors(propertyName, validation.Explanation);
        }

        private void ClearAllErrors()
        {
            _validationErrors.Clear();
            OnErrorsChanged("EditAmount");
            OnErrorsChanged("EditDescription");
            OnErrorsChanged("EditDate");
        }

        private void SetErrors(string propertyName, params string[] messages)
        {
            if (messages.Length == 0)
                _validationErrors.Remove(propertyName);
            else
                _validationErrors[propertyName] = messages;

            OnErrorsChanged(propertyName);
        }

        private void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));

            _transactionCommand.CheckPossibleCanExecuteChange();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            ValidateInput();
        }

        private static DateTime FirstOfMonth(DateTime time)
        {
            var day = time.Date;
            return day.AddDays(-day.Day + 1);
        }

    }

    public class BalanceSheetItemViewModel
    {
        private readonly BalanceSheet.Item _item;
        private readonly bool _suppressAmount;

        public BalanceSheetItemViewModel(BalanceSheet.Item item, bool suppressAmount)
        {
            _item = item;
            _suppressAmount = suppressAmount;
        }

        public DateTime TransactionDate
        {
            get { return _item.TransactionDate; }
        }

        public string Description
        {
            get { return _item.Description; }
        }

        public string Value
        {
            get
            {
                if (_suppressAmount)
                    return "";

                return _item.Value.ToString("0.00€");
            }
        }

        public decimal RunningTotalValue
        {
            get { return _item.RunningTotalValue; }
        }

    }
}
