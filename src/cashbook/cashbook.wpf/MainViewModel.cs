using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using cashbook.contracts.data;
using cashbook.wpf.Annotations;

namespace cashbook.wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private DateTime _selectedMonth;
        private readonly DelegateCommand _selectMonth;

        private BalanceSheet _shownBalanceSheet;

        private DateTime _editDate;
        private string _editDescription;
        private double _editAmount;
        private readonly DelegateCommand _withdraw;
        private readonly DelegateCommand _deposit;


        public MainViewModel()
        {
            _selectedMonth = FirstOfMonth(DateTime.Today);
            _selectMonth = new DelegateCommand(ShowSelectedMonth);

            _shownBalanceSheet = new BalanceSheet
            {
                Month = _selectedMonth,
                Items = new[]
                {
                    new BalanceSheet.Item()
                    {
                        Description = "description",
                        RunningTotalValue = 10.0,
                        TransactionDate = DateTime.Now,
                        Value = 5.0
                    }
                }
            };

            _editAmount = 0.0;
            _editDate = DateTime.Today;
            _editDescription = "";
            _withdraw = new DelegateCommand(Withdraw, ValidateEdit);
            _deposit = new DelegateCommand(Deposit, ValidateEdit);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                if (_selectedMonth != value)
                {
                    _selectedMonth = value;
                    OnPropertyChanged("SelectedMonth");
                }
            }
        }

        public ICommand SelectMonth
        {
            get { return _selectMonth; }
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
                    _editDate = value;
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
            throw new NotImplementedException();
        }

        private void Deposit()
        {
            throw new NotImplementedException();
        }

        private void Withdraw()
        {
            throw new NotImplementedException();
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
