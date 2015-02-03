using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using cashbook.contracts;
using cashbook.contracts.data;
using cashbook.wpf.Annotations;

namespace cashbook.wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IBody _body;

        private DateTime _selectedMonth;

        private BalanceSheet _shownBalanceSheet;

        private bool _editForce;
        private DateTime _editDate;
        private string _editDescription;
        private decimal _editAmount;
        private readonly DelegateCommand _withdraw;
        private readonly DelegateCommand _deposit;


        public MainViewModel()
        {
            _withdraw = new DelegateCommand(Withdraw);
            _deposit = new DelegateCommand(Deposit);

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
            if (_body == null)
                return;

            _body.Deposit(EditDate, EditAmount, EditDescription, EditForce,
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
            if (_body == null)
                return;

            _body.Withdraw(EditDate, EditAmount, EditDescription, EditForce,
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
            EditAmount = 0.0m;
            EditForce = false;
            EditDate = DateTime.Today;
            EditDescription = "";
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
