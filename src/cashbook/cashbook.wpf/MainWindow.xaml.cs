using System;
using System.Windows;
using System.Windows.Controls;
using cashbook.body;
using cashbook.body.data;
using eventstore;

namespace cashbook.wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TimeProvider.Now = () => DateTime.Now;

            var es = new FileEventStore("events");
            var repo = new Repository(es);
            var cashbookFactory = new Func<Transaction[], Cashbook>(transactions => new Cashbook(transactions));
            var csvProvider = new CSVProvider();
            var body = new Body(repo, cashbookFactory, csvProvider);

            var viewModel = new MainViewModel(body);
            this.DataContext = viewModel;
        }

        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox)
            {
                var tb = (TextBox) sender;
                    tb.SelectAll();
            }
        }
    }
}
