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
using System.Windows.Shapes;
using AmazonWebServicesHelper.IAM;

namespace AmazonWebServicesHelper.Dialogs
{
    /// <summary>
    /// Interaction logic for PolicyExplorer.xaml
    /// </summary>
    public partial class PolicyExplorer : MahApps.Metro.Controls.MetroWindow
    {
        public PolicyExplorer()
        {
            InitializeComponent();
        }

        private IamCredential cred;
        public IamCredential Credential
        {
            get
            {
                return cred;
            }
            set
            {
                cred = value;
                Title = "Policy Explorer - " + cred.Username;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (new System.Threading.Thread(() => LoadPolicies())).Start();
        }

        private void LoadPolicies()
        {
            List<IamPolicy> Policies = Credential.Policies;

            this.Dispatcher.Invoke(() => LoadTree(Policies));
        }
        private void LoadTree(List<IamPolicy> policies)
        {
            TreeViewItem userItem = new TreeViewItem();
            userItem.Header = "User-Inherited Policies";

            TreeViewItem groupItem = new TreeViewItem();
            groupItem.Header = "Group-Inherited Policies";

            foreach (IamPolicy policy in policies)
            {
                TreeViewItem pitem = new TreeViewItem();
                pitem.Header = "Policy From " + policy.OriginName + " [" + policy.PolicyVersion.ToShortDateString() + "]";

                foreach (IamPolicyStatement statement in policy.Statements)
                {
                    TreeViewItem sitem = new TreeViewItem();
                    sitem.Header = statement.Rule.ToString() + " ON " + statement.Resource.ToString();

                    TreeViewItem resourceitem = new TreeViewItem();
                    resourceitem.Header = "Resource";

                    resourceitem.Items.Add(new TreeViewItem() { Header = "Partition: " + statement.Resource.Partition });
                    resourceitem.Items.Add(new TreeViewItem() { Header = "Service: " +  statement.Resource.ServiceNamespace });
                    resourceitem.Items.Add(new TreeViewItem() { Header = "Region: " + statement.Resource.Region });
                    resourceitem.Items.Add(new TreeViewItem() { Header = "Account: " + statement.Resource.Account });
                    if (statement.Resource.ResourceType != null) resourceitem.Items.Add(new TreeViewItem() { Header = "Resource Type: " + statement.Resource.ResourceType });
                    resourceitem.Items.Add(new TreeViewItem() { Header = "Resource: " + statement.Resource.Resource });

                    TreeViewItem actionitem = new TreeViewItem();
                    actionitem.Header = "Actions";

                    foreach (IamPolicyAction action in statement.Actions)
                    {
                        TreeViewItem aservitem = new TreeViewItem();
                        aservitem.Header = "Service: " + action.ServiceName;

                        TreeViewItem aaitem = new TreeViewItem();
                        aaitem.Header = "Action: " + action.ActionName;


                        actionitem.Items.Add(aservitem);
                        actionitem.Items.Add(aaitem);
                    }

                    sitem.Items.Add(resourceitem);
                    sitem.Items.Add(actionitem);
                    pitem.Items.Add(sitem);
                }



                if (policy.Origin == IamPolicyOrigin.Group) groupItem.Items.Add(pitem);
                else userItem.Items.Add(pitem);
            }

            policyTree.Items.Clear();
            policyTree.Items.Add(userItem);
            policyTree.Items.Add(groupItem);
            userItem.ExpandSubtree();
            groupItem.ExpandSubtree();

            policyTree.Visibility = System.Windows.Visibility.Visible;
            loadingRing.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
