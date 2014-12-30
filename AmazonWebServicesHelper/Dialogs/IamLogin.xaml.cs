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
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;
using AmazonWebServicesHelper.IAM;

namespace AmazonWebServicesHelper.Dialogs
{
    /// <summary>
    /// Interaction logic for IamLogin.xaml
    /// </summary>
    public partial class IamLogin : MahApps.Metro.Controls.MetroWindow
    {
        public IamLogin()
        {
            InitializeComponent();
        }

        public string HelperDirectory
        {
            get
            {
                return System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.awshelper";
            }
        }

        public IamCredential SelectedCredential { get; set; }
        public bool CredentialChosen { get; set; }

        public Dictionary<string, string> FileMap { get; set; }
        public List<IamCredential> Credentials { get; set; }
        public void LoadCredentialList()
        {
            Credentials = new List<IamCredential>();
            FileMap = new Dictionary<string, string>();

            if (!Directory.Exists(HelperDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(HelperDirectory);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden; 
            }

            foreach (string file in Directory.GetFiles(HelperDirectory))
            {
                try
                {
                    IamCredential cred = new IamCredential(file);
                    Credentials.Add(cred);
                    FileMap.Add(cred.AccessKey, file);
                }
                catch { }
            }

            this.Dispatcher.Invoke(() =>
                {
                    iamProfiles.ItemsSource = Credentials;
                    if (Credentials.Count > 0)
                    {
                        iamProfiles.SelectedIndex = 0;
                    }
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            (new System.Threading.Thread(() => LoadCredentialList())).Start();
            regionList.ItemsSource = Amazon.RegionEndpoint.EnumerableAllRegions;
            
        }

        private void customRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (customGrid == null || iamProfiles == null) return;
            if (profileRadio.IsChecked.Value)
            {
                customGrid.IsEnabled = false;
                iamProfiles.IsEnabled = true;
                addToProfileListBtn.IsEnabled = false;
                deleteProfileBtn.IsEnabled = true;
            }
            else
            {
                customGrid.IsEnabled = true;
                testPolicyBtn.IsEnabled = true;
                iamProfiles.IsEnabled = false;
                addToProfileListBtn.IsEnabled = true;
                deleteProfileBtn.IsEnabled = false;
            }
        }

        private void addToProfileListBtn_Click(object sender, RoutedEventArgs e)
        {
            if (accessKeyTxt.Text == null || accessKeyTxt.Text == "")
            {
                MessageBox.Show("You must enter an access key or select a valid profile to test the policy for.");
                return;
            }
            if (secretKeyTxt.Text == null || secretKeyTxt.Text == "")
            {
                MessageBox.Show("You must enter a secret key or select a valid profile to test the policy for.");
                return;
            }
            if (regionList.SelectedItem == null)
            {
                MessageBox.Show("You must select a region or select a valid profile to test the policy for.");
                return;
            }

            IamCredential cred = new IamCredential()
            {
                AccessKey = accessKeyTxt.Text,
                SecretKey = secretKeyTxt.Text,
                Region = (regionList.SelectedItem as Amazon.RegionEndpoint).SystemName,
            };
            try
            {
                string name = cred.Username;
                var filename = HelperDirectory + "\\" + sha256(name+DateTime.Now.ToLongDateString()+DateTime.Now.ToLongTimeString());
                while (File.Exists(filename))
                {
                    filename = filename + "0";
                }
                if (!cred.Save(filename))
                {
                    MessageBox.Show("The profile could not be saved. Please try again later.");
                }
                else
                {

                    FileMap.Add(cred.AccessKey, filename);
                    iamProfiles.ItemsSource = null;
                    Credentials.Add(cred);
                    iamProfiles.ItemsSource = Credentials;
                    accessKeyTxt.Text = "";
                    secretKeyTxt.Text = "";
                    regionList.SelectedIndex = -1;
                }
            }
            catch
            {
                MessageBox.Show("Invalid user. No user with that access key or secret was found.");
            }
        }

        private void testPolicyBtn_Click(object sender, RoutedEventArgs e)
        {
            PolicyExplorer policy = new PolicyExplorer();

            if (customRadio.IsChecked ?? false)
            {
                if (accessKeyTxt.Text == null || accessKeyTxt.Text == "")
                {
                    MessageBox.Show("You must enter an access key.");
                    return;
                }
                if (secretKeyTxt.Text == null || secretKeyTxt.Text == "")
                {
                    MessageBox.Show("You must enter a secret key.");
                    return;
                }
                if (regionList.SelectedItem == null)
                {
                    MessageBox.Show("You must select a region.");
                    return;
                }

                policy.Credential = new IamCredential()
                {
                    AccessKey = accessKeyTxt.Text,
                    SecretKey = secretKeyTxt.Text,
                    Region = (regionList.SelectedItem as Amazon.RegionEndpoint).SystemName
                };
            }
            else
            {
                try
                {
                    policy.Credential = iamProfiles.SelectedItem as IamCredential;
                }
                catch
                {
                    MessageBox.Show("You must select a valid profile.");
                    return;
                }
            }
            policy.ShowDialog();
        }

        private void iamProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (iamProfiles.SelectedItem == null)
            {
                deleteProfileBtn.IsEnabled = false;
                testPolicyBtn.IsEnabled = false;
            }
            else
            {
                deleteProfileBtn.IsEnabled = true;
            }
        }

        private string sha256(string password)
        {
            SHA256Managed crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            return
                Convert.ToBase64String(crypto)
                .Replace("<", "")
                .Replace(">", "")
                .Replace(":", "")
                .Replace("\"", "")
                .Replace("\\", "")
                .Replace("/", "")
                .Replace("|", "")
                .Replace("?", "")
                .Replace("*", "")
                .Replace("=", "")
                .Replace("!", "");
        }

        private void deleteProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (iamProfiles.SelectedItem as IamCredential != null)
            {
                IamCredential cred = iamProfiles.SelectedItem as IamCredential;
                if (FileMap.ContainsKey(cred.AccessKey))
                {
                    File.Delete(FileMap[cred.AccessKey]);
                    FileMap.Remove(cred.AccessKey);
                    Credentials.Remove(cred);
                    iamProfiles.ItemsSource = null;
                    iamProfiles.ItemsSource = Credentials;
                }
                else
                {
                    MessageBox.Show("The profile could not be found to delete. Try restarting the application.");
                    return;
                }
            }
        }

        private void beginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (profileRadio.IsChecked.Value)
            {
                if (iamProfiles.SelectedItem as IamCredential == null)
                {
                    MessageBox.Show("You must select a valid profile.");
                    return;
                }

                IamCredential cred = iamProfiles.SelectedItem as IamCredential;
                SelectedCredential = cred;
                CredentialChosen = true;
                this.Close();
            }
            else
            {
                if (accessKeyTxt.Text == null || accessKeyTxt.Text == "")
                {
                    MessageBox.Show("You must enter an access key or select a valid profile to test the policy for.");
                    return;
                }
                if (secretKeyTxt.Text == null || secretKeyTxt.Text == "")
                {
                    MessageBox.Show("You must enter a secret key or select a valid profile to test the policy for.");
                    return;
                }
                if (regionList.SelectedItem == null)
                {
                    MessageBox.Show("You must select a region or select a valid profile to test the policy for.");
                    return;
                }

                IamCredential cred = null;
                try
                {
                    cred = new IamCredential()
                    {
                        AccessKey = accessKeyTxt.Text,
                        SecretKey = secretKeyTxt.Text,
                        Region = (regionList.SelectedItem as Amazon.RegionEndpoint).SystemName,
                    };
                    string name = cred.Username;
                }
                catch
                {
                    MessageBox.Show("The Access Key or Secret entered did not Authenticate with AWS.");
                    return;
                }

                SelectedCredential = cred;
                CredentialChosen = true;
                this.Close();
            }
        }
    }
}
