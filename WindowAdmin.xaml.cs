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
    /// Interaction logic for WindowAdmin.xaml
    /// </summary>
    public partial class WindowAdmin : Window
    {
        // objet BDD
        NorthernLightsHospitalEntities database;

        // variables medecin
        string nom = "";
        string prenom = "";

        public WindowAdmin()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // assignation de la bdd
            database = new NorthernLightsHospitalEntities();

            // initialisation du listView medecin avec les medecins existants
            listView_medecin.DataContext = database.Medecins.ToList();
        }

        private void btn_ajouter_Click(object sender, RoutedEventArgs e)
        {
            // remplissage des variables temporaires avec les informations du medecin
            nom = txt_nom.Text;
            prenom = txt_prenom.Text;

            // si tout les champs sont remplis, creation d'une entree dans la bdd
            if (String.IsNullOrEmpty(nom) || String.IsNullOrEmpty(prenom))
            {
                MessageBox.Show("Veuillez remplir tout les champs", "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Medecin medecin = new Medecin();
                medecin.idMedecin = nom.Substring(0, 3) + prenom.Substring(0, 3);
                medecin.nom = nom;
                medecin.prenom = prenom;

                database.Medecins.Add(medecin);

                try
                {
                    database.SaveChanges();
                    MessageBox.Show("Médecin ajouté avec succès", "Ajout Médecin", MessageBoxButton.OK, MessageBoxImage.Information);
                    txt_nom.Clear();
                    txt_prenom.Clear();
                    listView_medecin.DataContext = database.Medecins.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_effacer_Click(object sender, RoutedEventArgs e)
        {
            // permet d'effacer les champs
            txt_nom.Clear();
            txt_prenom.Clear();
            listView_medecin.SelectedIndex = -1;    // remet le selected item a -1 pour deselection
        }

        private void btn_modifier_Click(object sender, RoutedEventArgs e)
        {
            // remplissage des variables temporaires avec les informations du medecin
            nom = txt_nom.Text;
            prenom = txt_prenom.Text;

            // si tout les champs sont remplis, modification de l'entree dans la bdd
            if (String.IsNullOrEmpty(nom) || String.IsNullOrEmpty(prenom))
            {
                MessageBox.Show("Veuillez remplir tout les champs", "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Medecin temp = listView_medecin.SelectedItem as Medecin;
                temp.nom = nom;
                temp.prenom = prenom;

                try
                {
                    database.SaveChanges();
                    MessageBox.Show("Modification(s) effectué avec succès!", "Modification Médecin", MessageBoxButton.OK, MessageBoxImage.Information);
                    txt_nom.Clear();
                    txt_prenom.Clear();
                    listView_medecin.SelectedIndex = -1;
                    listView_medecin.DataContext = database.Medecins.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_supprimer_Click(object sender, RoutedEventArgs e)
        {
            Medecin temp = listView_medecin.SelectedItem as Medecin;

            database.Medecins.Remove(temp);

            try
            {
                database.SaveChanges();
                MessageBox.Show("Suppression effectué avec succès!", "Suppression Médecin", MessageBoxButton.OK, MessageBoxImage.Information);
                txt_nom.Clear();
                txt_prenom.Clear();
                listView_medecin.SelectedIndex = -1;                        // remet le selected item a -1 pour empecher une erreur et deselection
                listView_medecin.DataContext = database.Medecins.ToList();  // refresh du listView
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                MessageBox.Show("Vous ne pouvez pas supprimer ce médecin car il est assigné a un/des patients", "Suppression Impossible", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void listView_medecin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // verifie si un item existant est selectionne et remplis les champs pour modification
            if (listView_medecin.SelectedIndex >= 0)
            {
                // cast du selected item en une variable Medecin pour pouvoir extraire ses proprietes
                Medecin temp = listView_medecin.SelectedItem as Medecin;

                txt_nom.Text = temp.nom;
                txt_prenom.Text = temp.prenom;
            }
        }

        private void btn_retour_Click(object sender, RoutedEventArgs e)
        {
            // retour a la page login
            WindowLogin windowLogin = new WindowLogin();
            this.Close();
            windowLogin.Show();
        }
    }
}
