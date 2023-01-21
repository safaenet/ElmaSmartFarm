using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ElmaSmartFarm.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FarmViewer : UserControl, INotifyPropertyChanged
    {
        public FarmViewer()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public FarmModel Farm
        {
            get { return (FarmModel)GetValue(FarmProperty); }
            set { SetValue(FarmProperty, value); }
        }

        public static readonly DependencyProperty FarmProperty =
            DependencyProperty.Register(nameof(Farm), typeof(FarmModel), typeof(FarmViewer), new PropertyMetadata(null, (s, e) => { if (s is FarmViewer c) c.OnFarmChanged(); }));

        protected virtual void OnFarmChanged()
        {

        }
    }
}