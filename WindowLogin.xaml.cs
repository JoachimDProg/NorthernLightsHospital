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

namespace NorthernLightsHospital
{
    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin : Window
    {
        // username et password hardcode pour acces aux fenetres specifiques
        const string userMedecin = "medecin";
        const string passMedecin = "medecin";
        const string userAdmin = "admin";
        const string passAdmin = "admin";
        const string userPrepose = "prepose";
        const string passPrepose = "prepose";

        public WindowLogin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txt_username.Focus();
        }


        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            // remplissage des variables temporaires avec les informations de login
            string user = txt_username.Text;
            string pass = txt_password.Password;

            // si tout les champs sont remplis, redirection vers la fenetre 
            if (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Veuillez remplir tout les champs", "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (user.Equals(userMedecin) && pass.Equals(passMedecin))
                {
                    WindowMedecin windowMedecin = new WindowMedecin();
                    this.Close();
                    windowMedecin.Show();
                }
                else if (user.Equals(userAdmin) && pass.Equals(passAdmin))
                {
                    WindowAdmin windowAdmin = new WindowAdmin();
                    this.Close();
                    windowAdmin.Show();
                }
                else if (user.Equals(userPrepose) && pass.Equals(passPrepose))
                {
                    WindowPrepose windowPrepose = new WindowPrepose();
                    this.Close();
                    windowPrepose.Show();
                }
                else
                {
                    MessageBox.Show("Les informations entrées sont invalides!", "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
                    txt_username.Clear();
                    txt_password.Clear();
                    txt_username.Focus();
                }
            }
        }
    }
}
