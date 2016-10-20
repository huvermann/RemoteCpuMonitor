using RemoteCpuMonitor.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RemoteCpuMonitor.Views
{
    /// <summary>
    /// Interaction logic for MonitorListItemView
    /// </summary>
    public partial class MonitorListItemView : UserControl
    {

        //public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(MonitorListItemView), new UIPropertyMetadata(MyPropertyChangedHandler));
        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(MonitorListItemView), new PropertyMetadata(""));

        private static void MyPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine(e);
        }

        public string HeaderText
        {
            get {
                return (string)GetValue(HeaderTextProperty);
            }
            set {
                var oldValue = (string)GetValue(HeaderTextProperty);
                if (oldValue != value) SetValue(HeaderTextProperty, value);
            }
        }


        public MonitorListItemView()
        {
            InitializeComponent();
            var binding = new Binding("HeaderText") { Mode = BindingMode.TwoWay };
            this.SetBinding(HeaderTextProperty, binding);
        }

        
    }
}
