using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using cashbook.body;
using cashbook.contracts.data;
using cashbook.wpf.Annotations;

namespace cashbook.wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Body _body;


        private DateTime _selectedMonth;

        private BalanceSheet _shownBalanceSheet;

        private DateTime _editDate;
        private string _editDescription;
        private double _editAmount;
        private readonly DelegateCommand _withdraw;
        private readonly DelegateCommand _deposit;


        public MainViewModel()
        {
            _selectedMonth = FirstOfMonth(DateTime.Today);

            _shownBalanceSheet = new BalanceSheet
            {
                Month = _selectedMonth,
                Items = new BalanceSheet.Item[]{}
            };

            ClearEdit();

            _withdraw = new DelegateCommand(Withdraw, ValidateEdit);
            _deposit = new DelegateCommand(Deposit, ValidateEdit);
        }

        public MainViewModel(Body body)
            : this()
        {
            this._body = body;
            ShowSelectedMonth();
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            get { return ShownBalanceSheet.Items.Select(item => new BalanceSheetItemViewModel(item)); }
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
                    _withdraw.CheckPossibleCanExecuteChange();
                    _deposit.CheckPossibleCanExecuteChange();
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
                    _withdraw.CheckPossibleCanExecuteChange();
                    _deposit.CheckPossibleCanExecuteChange();
                }
            }
        }

        public double EditAmount
        {
            get { return _editAmount; }
            set
            {
                if (Math.Abs(_editAmount - value) >= 0.01)
                {
                    _editAmount = value;
                    OnPropertyChanged("EditAmount");
                    _withdraw.CheckPossibleCanExecuteChange();
                    _deposit.CheckPossibleCanExecuteChange();
                }
            }
        }

        public ICommand WithdrawCommand
        {
            get { return _withdraw; }
        }

        public ICommand DepositCommand
        {
            get { return _deposit; }
        }

        private void ShowSelectedMonth()
        {
            if (_body == null)
                return;

            ShownBalanceSheet = _body.Load_monthly_balance_sheet(SelectedMonth);
        }

        private void Deposit()
        {
            if (!ValidateEdit() || _body == null)
                return;

            _body.Deposit(EditDate, EditAmount, EditDescription, 
                true,
                _ =>
                {
                    ClearEdit();
                    ShowSelectedMonth();
                },
                errormsg =>
                {
                    MessageBox.Show(errormsg, "could not withdraw", MessageBoxButton.OK, MessageBoxImage.Error);
                });
        }

        private void Withdraw()
        {
            if (!ValidateEdit() || _body == null)
                return;

            _body.Withdraw(EditDate, EditAmount, EditDescription, 
                true,
                _ =>
                {
                    ClearEdit();
                    ShowSelectedMonth();
                },
                errormsg =>
                {
                    MessageBox.Show(errormsg, "could not withdraw", MessageBoxButton.OK, MessageBoxImage.Error);
                });
        }

        private void ClearEdit()
        {
            _editAmount = 0.0;
            _editDate = DateTime.Today;
            _editDescription = "";
        }


        private bool ValidateEdit()
        {
            return
                EditDate.Date <= DateTime.Today
                && !string.IsNullOrEmpty(EditDescription)
                && EditAmount > 0;

        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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

        public BalanceSheetItemViewModel(BalanceSheet.Item item)
        {
            _item = item;
        }

        public DateTime TransactionDate
        {
            get { return _item.TransactionDate; }
        }

        public string Description
        {
            get { return _item.Description; }
        }

        public double Value
        {
            get { return _item.Value; }
        }

        public double RunningTotalValue
        {
            get { return _item.RunningTotalValue; }
        }
    }
}
