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
    /// Interaction logic for WindowMedecin.xaml
    /// </summary>
    public partial class WindowMedecin : Window
    {
        // objet BDD
        NorthernLightsHospitalEntities database;

        public WindowMedecin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // assignation de la bdd
            database = new NorthernLightsHospitalEntities();

            // query pour afficher les patients qui sont encore a l'hopital
            var admissionsValides = (from admissions in database.Admissions
                                     where admissions.dateConge == DateTime.MinValue
                                     select admissions);
            list_admissions.DataContext = admissionsValides.ToList();
        }

        private void btn_conge_Click(object sender, RoutedEventArgs e)
        {
            // le patient choisis aura son conge et son lit sera disponible de nouveau
            if(list_admissions.SelectedItems.Count == 0)
            {
                MessageBox.Show("Veuillez choisir une admission", "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Admission admission = list_admissions.SelectedItem as Admission;
                Lit lit = (from l in database.Lits
                           where l.numeroLit == admission.numeroLit
                           select l).FirstOrDefault() as Lit;

                admission.dateConge = DateTime.Today;
                lit.occupe = false;

                try
                {
                    database.SaveChanges();
                    MessageBox.Show("Congé donné avec succès", "Congé", MessageBoxButton.OK, MessageBoxImage.Information);

                    list_admissions.SelectedIndex = -1;

                    list_admissions.DataContext = (from admissions in database.Admissions
                                                   where admissions.dateConge == DateTime.MinValue
                                                   select admissions).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_retour_Click(object sender, RoutedEventArgs e)
        {
            // retour a la page login
            WindowLogin windowLogin = new WindowLogin();
            this.Close();
            windowLogin.Show();
        }

        private void list_admissions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // pour effacer le champ lorsque l'index retourne a -1 apres un conge
            if(list_admissions.SelectedIndex == -1)
            {
                txt_nomPatient.DataContext = null;
            }
            // refresh du listBox s'il reste d'autres patients
            else
            {
                Admission admission = list_admissions.SelectedItem as Admission;
                txt_nomPatient.DataContext = (from patient in database.Patients
                                              where patient.NSS == admission.NSS
                                              select patient).FirstOrDefault();
            }
        }
    }
}
