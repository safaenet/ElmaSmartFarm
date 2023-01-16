using ElmaSmartFarm.SharedLibrary.Models;
using ElmaSmartFarm.SharedLibrary.Models.Sensors;
using System.Windows;
using System.Windows.Controls;

namespace ElmaSmartFarm.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FarmViewer : UserControl
    {
        public FarmViewer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public FarmModel Farm
        {
            get { return (FarmModel)GetValue(FarmProperty); }
            set { SetValue(FarmProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Farm.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FarmProperty =
            DependencyProperty.Register("Farm", typeof(FarmModel), typeof(FarmViewer), new PropertyMetadata(null));

        public ScalarSensorModel? Scalar => Farm?.Scalars?.Sensors[0];
    }
}