using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace cashbook.wpf
{
    public class SetFocusAfterClickBehavior : Behavior<ButtonBase>
    {
        private ButtonBase _button;

        protected override void OnAttached()
        {
            _button = AssociatedObject;

            _button.Click += AssociatedButtonClick;
        }

        protected override void OnDetaching()
        {
            _button.Click -= AssociatedButtonClick;
        }

        void AssociatedButtonClick(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(FocusElement);

            if (FocusElement is DatePicker)
            {
                // HACK: get the date-picker to focus the textbox-part by sending a Key.Up
                var eventArgs = new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    Keyboard.PrimaryDevice.ActiveSource,
                    0,
                    Key.Up) 
                    {RoutedEvent = UIElement.KeyDownEvent};

                FocusElement.RaiseEvent(eventArgs);                
            }
        }

        public Control FocusElement
        {
            get { return (Control)GetValue(FocusElementProperty); }
            set { SetValue(FocusElementProperty, value); }
        }

        public static readonly DependencyProperty FocusElementProperty =
            DependencyProperty.Register("FocusElement", typeof(Control), typeof(SetFocusAfterClickBehavior), new UIPropertyMetadata());
    }
}
