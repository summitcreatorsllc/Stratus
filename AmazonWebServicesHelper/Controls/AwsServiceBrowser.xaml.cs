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

namespace AmazonWebServicesHelper.Controls
{
    /// <summary>
    /// Interaction logic for AwsServiceBrowser.xaml
    /// </summary>
    public partial class AwsServiceBrowser : UserControl
    {
        public AwsServiceBrowser()
        {
            InitializeComponent();


            List<Icons.ServiceItem> items = Icons.ServiceItem.GetAllServices();
            List<Icons.ServiceItem> secondHalf = new List<Icons.ServiceItem>(items.Skip(22).ToList());
            items.RemoveRange(22,items.Count-22);
            List<Icons.ServiceItem> firstHalf = new List<Icons.ServiceItem>(items);

            lsView1.ItemsSource = firstHalf;
            CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(lsView1.ItemsSource);
            PropertyGroupDescription groupDescription1 = new PropertyGroupDescription("Category");
            view1.GroupDescriptions.Add(groupDescription1);

            lsView2.ItemsSource = secondHalf;
            CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(lsView2.ItemsSource);
            PropertyGroupDescription groupDescription2 = new PropertyGroupDescription("Category");
            view2.GroupDescriptions.Add(groupDescription2);
        }
    }
}
