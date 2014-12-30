using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AmazonWebServicesHelper.IAM;
using AmazonWebServicesHelper.Dialogs;

namespace Stratus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IamCredential Credential { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IamLogin login = new IamLogin();
            login.Closing += login_Closed;
            login.Show();
            
        }

        void login_Closed(object sender, EventArgs e)
        {
            var login = sender as IamLogin;
            if (!login.CredentialChosen)
            {
                return;
            }

            Credential = login.SelectedCredential;
            Stratus.MainWindow window = new MainWindow();
            window.Show();
        }
    }
}
