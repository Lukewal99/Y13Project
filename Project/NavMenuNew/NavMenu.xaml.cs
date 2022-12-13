using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NavMenuNew
{
    /// <summary>
    /// Interaction logic for NavMenu.xaml
    /// </summary>
    public partial class NavMenu : UserControl
    {
        UserControl Model;
        public NavMenu()
        {
            UserControl Model = new UserControl();
            InitializeComponent();
        }


        private void ModelOne_Click(object sender, RoutedEventArgs e)
        {
            Model = new Model1();
            Canvas.SetLeft(Model, 0);
            Canvas.SetTop(Model, 0);
            
            Canvas MainCanvas = (Canvas)this.Parent;
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(Model);
        }

        private void ModelTwo_Click(object sender, RoutedEventArgs e)
        {
            Model = new Model2();
            Canvas.SetLeft(Model, 0);
            Canvas.SetTop(Model, 0);

            Canvas MainCanvas = (Canvas)this.Parent;
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(Model);
        }

        private void ModelThree_Click(object sender, RoutedEventArgs e)
        {
            Model = new Model3();
            Canvas.SetLeft(Model, 0);
            Canvas.SetTop(Model, 0);
            Canvas MainCanvas = (Canvas)this.Parent;
            MainCanvas.Children.Clear();
            MainCanvas.Children.Add(Model);

        }
    }
}
