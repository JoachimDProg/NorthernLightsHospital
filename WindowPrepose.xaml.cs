using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
    /// Interaction logic for WindowPrepose.xaml
    /// </summary>
    public partial class WindowPrepose : Window
    {
        // objet BDD
        NorthernLightsHospitalEntities database;

        // variables patient
        string nom = "";
        string prenom = "";
        DateTime dateNaissance;
        string adresse = "";
        string ville = "";
        string province = "";
        string codePostal = "";
        string numeroTelephone = "";
        string assurance = "";

        // choix pour les comboBox patient
        string[] provinces = { "Quebec", "Ontario", "Alberta", "Manitoba" };
        string[] assurances = { "Carte Soleil", "Sunlife" };

        // variables admission
        string patient = "";
        string medecin = "";
        DateTime dateAdmission;
        bool chirurgie = false;
        DateTime dateChirurgie;
        string departement = "";
        string typeLit = "";
        bool television = false;
        bool telephone = false;
        string litDispo = "";
        static double prixJournalier = 0;
        static double prixLit = 0;

        // variables choix lits disponibles
        static Departement CBdepartement;
        static TypeLit CBtypeLit;
        static string deptID;
        static string typeLitID;

        public WindowPrepose()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // assignation de la bdd
            database = new NorthernLightsHospitalEntities();

            // initialisation des comboBox patient
            cb_province.DataContext = provinces;
            cb_assurance.DataContext = assurances;

            // initialisation des comboBox admission
            cb_patient.DataContext = database.Patients.ToList();
            cb_medecin.DataContext = database.Medecins.ToList();
            cb_departement.DataContext = database.Departements.ToList();
            cb_typeLit.DataContext = database.TypeLits.ToList();

            // initialisation des index des comboBox departement et typeLit, appelle aussi le selectionChanged de chacun
            cb_departement.SelectedIndex = 0;
            cb_typeLit.SelectedIndex = 2;

            // initialisation du textBox prix journalier avec formatage
            txt_prix.Text = prixJournalier.ToString("C");

            // query qui verifie le nombre de lits disponibles total
            var nbLitsDispoQuery = (from nbLits in database.Lits
                                    where nbLits.occupe == false
                                    select nbLits).Count();

            // nbLitsDispoQuery = 0;        // pour test aucun lit disponible
            if (nbLitsDispoQuery <= 0)
            {
                MessageBox.Show("Aucun lit Disponible, veuillez sortir des patient", "Aucun lit", MessageBoxButton.OK, MessageBoxImage.Warning);
                WindowMedecin windowMedecin = new WindowMedecin();
                this.Close();
                windowMedecin.Show();
            }
        }

        private void btn_enregistrerPatient_Click(object sender, RoutedEventArgs e)
        {
            // remplissage des variables temporaires avec les informations du patient
            nom = txt_nom.Text;
            prenom = txt_prenom.Text;
            // si aucune date de naissance est choisie, la valeur d'aujourd'hui sera choisie
            if (date_naissance.SelectedDate == null)
                dateNaissance = DateTime.Today;
            else
                dateNaissance = date_naissance.SelectedDate.Value;

            adresse = txt_adresse.Text;
            ville = txt_ville.Text;
            province = cb_province.Text;
            codePostal = txt_codePostal.Text;
            numeroTelephone = txt_telephone.Text;

            if (cb_assurance.Text == "Carte Soleil")
                assurance = "assPublique";
            else if (cb_assurance.Text == "Sunlife")
                assurance = "assPrivee";

            // variables pour tests de champs vides
            Object[] testPatient = { nom, prenom, dateNaissance, adresse, ville, province, codePostal, numeroTelephone, assurance };
            String[] nomTestPatient = { "Nom", "Prenom", "Naissance", "Adresse", "Ville", "Province", "Code Postal", "Telephone", "Assurance" };

            // si tout les champs sont remplis, creation d'une entree dans la bdd
            if (ChampRemplis(testPatient, nomTestPatient))
            {
                Patient patient = new Patient();
                patient.NSS = nom.Substring(0, 3) + prenom.Substring(0, 3);
                patient.nom = nom;
                patient.prenom = prenom;
                patient.dateNaissance = dateNaissance;
                patient.adresse = adresse;
                patient.ville = ville;
                patient.province = province;
                patient.codePostal = codePostal;
                patient.telephone = numeroTelephone;
                patient.idAssurance = assurance;

                database.Patients.Add(patient);

                try
                {
                    database.SaveChanges();
                    MessageBox.Show("Patient ajouté avec succès", "Ajout Patient", MessageBoxButton.OK, MessageBoxImage.Information);

                    // remise des champs patient par defaut
                    txt_nom.Clear();
                    txt_prenom.Clear();
                    txt_adresse.Clear();
                    txt_ville.Clear();
                    cb_province.SelectedIndex = 0;
                    txt_codePostal.Clear();
                    txt_telephone.Clear();
                    cb_assurance.SelectedIndex = 0;

                    // refresh du comboBox patient dans la section admission pour afficher le nouveau patient
                    cb_patient.DataContext = database.Patients.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btn_enregistrerAdmission_Click(object sender, RoutedEventArgs e)
        {
            // remplissage des variables temporaires avec les informations de l'admission
            patient = cb_patient.Text;
            medecin = cb_medecin.Text;

            // si aucune date d'admission est choisie, la valeur d'aujourd'hui sera choisie
            if (!date_admission.SelectedDate.HasValue)
                dateAdmission = DateTime.Today;
            else
                dateAdmission = date_admission.SelectedDate.Value;
            chirurgie = check_chirurgie.IsChecked.Value;

            // si le checkBox chirurgie est coche, il faut choisir une date sinon la valeur minimale sera choisie
            if (check_chirurgie.IsChecked.Value == true && date_chirurgie.SelectedDate.HasValue)
                dateChirurgie = date_chirurgie.SelectedDate.Value;
            else if (check_chirurgie.IsChecked.Value == true && !date_chirurgie.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez entrer une date de chirurgie", "Date Chirurgie", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
                dateChirurgie = DateTime.MinValue;

            departement = cb_departement.Text;
            typeLit = cb_typeLit.Text;
            television = check_television.IsChecked.Value;
            telephone = check_telephone.IsChecked.Value;

            if (lb_litsDispo.SelectedItem == null)
                litDispo = "";
            else
                litDispo = lb_litsDispo.SelectedItem.ToString();


            // variables pour tests de champs vides
            Object[] testAdmission = { patient, medecin, dateAdmission, chirurgie, dateChirurgie, departement, typeLit, litDispo };
            String[] nomTestAdmission = { "Patient", "Médecin", "Date Admission", "Chirurgie", "Date Chirurgie", "Département", "Type Lit", "Lits Disponibles" };

            // si tout les champs sont remplis, creation d'une entree dans la bdd
            if (ChampRemplis(testAdmission, nomTestAdmission))
            {
                Patient patient = cb_patient.SelectedItem as Patient;
                Medecin medecin = cb_medecin.SelectedItem as Medecin;
                Lit lit = lb_litsDispo.SelectedItem as Lit;

                Admission admission = new Admission();
                admission.idAdmission = patient.NSS + dateAdmission.Date.ToString("d");
                admission.chirurgie = chirurgie;
                admission.dateAdmission = dateAdmission;
                admission.dateChirurgie = dateChirurgie;
                admission.dateConge = DateTime.MinValue;
                admission.televiseur = television;
                admission.telephone = telephone;
                admission.NSS = patient.NSS;
                admission.numeroLit = lit.numeroLit;
                admission.idMedecin = medecin.idMedecin;
                lit.occupe = true;

                database.Admissions.Add(admission);

                try
                {
                    database.SaveChanges();
                    MessageBox.Show("Admission ajouté avec succès", "Nouvel Admission", MessageBoxButton.OK, MessageBoxImage.Information);

                    // remise des champs admission par defaut
                    cb_patient.SelectedIndex = -1;
                    cb_medecin.SelectedIndex = -1;
                    check_chirurgie.IsChecked = false;
                    cb_departement.SelectedIndex = 0;
                    cb_typeLit.SelectedIndex = 2;
                    check_television.IsChecked = false;
                    check_telephone.IsChecked = false;
                    lb_litsDispo.DataContext = ListBoxLitsUpdate(deptID, typeLitID);
                    txt_prix.Text = String.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void check_chirurgie_Checked(object sender, RoutedEventArgs e)
        {
            // permet de mettre la date de chirurgie s'il y a lieu
            date_chirurgie.IsEnabled = true;
            // les patients qui subiront une chirurgie sont automatiquement assigne une chambre du departement de chirurgie
            cb_departement.SelectedIndex = 1;
        }

        private void check_chirurgie_Unchecked(object sender, RoutedEventArgs e)
        {
            // empeche de mettre une date s'il n'y a pas de chirurgie
            date_chirurgie.IsEnabled = false;
        }

        private void cb_departement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // retourne l'index a -1 pour empecher une erreur
            lb_litsDispo.SelectedIndex = -1;

            // update du departement choisi
            CBdepartement = cb_departement.SelectedItem as Departement;
            deptID = CBdepartement.idDepartement;

            // update du listBox litsDispo data Context avec un query
            lb_litsDispo.DataContext = ListBoxLitsUpdate(deptID, typeLitID);

            // enleve le prix du lit qui a ete applique dans le listBoxSelectionChanged et remet le prix du lit a 0
            prixJournalier -= prixLit;
            txt_prix.Text = prixJournalier.ToString("C");
            prixLit = 0;
        }

        private void cb_typeLit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // retourne l'index a -1 pour empecher une erreur
            lb_litsDispo.SelectedIndex = -1;

            // update du type de lit choisi
            CBtypeLit = cb_typeLit.SelectedItem as TypeLit;
            typeLitID = CBtypeLit.idType;

            // update du listBox litsDispo data Context avec un query
            lb_litsDispo.DataContext = ListBoxLitsUpdate(deptID, typeLitID);

            // enleve le prix du lit qui a ete applique dans le listBoxSelectionChanged et remet le prix du lit a 0
            prixJournalier -= prixLit;
            txt_prix.Text = prixJournalier.ToString("C");
            prixLit = 0;
        }

        private void cb_patient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // si l'index est -1, retourne
            if (cb_patient.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                // les patients qui qui ont moins de 16 ans et qui ne sont pas admis en chirurgie
                // sont automatiquement assigne une chambre du departement de pediatrie
                Patient patient = cb_patient.SelectedItem as Patient;
                DateTime maintenant = DateTime.Today;
                DateTime anneeNaissance = patient.dateNaissance.Value;
                var age = maintenant.Year - anneeNaissance.Year;
                if (age <= 16 && !check_chirurgie.IsChecked.Value)
                {
                    cb_departement.SelectedIndex = 2;
                }
            }
        }

        private void check_television_Checked(object sender, RoutedEventArgs e)
        {
            prixJournalier += 42.5;
            txt_prix.Text = prixJournalier.ToString("C");
        }

        private void check_television_Unchecked(object sender, RoutedEventArgs e)
        {
            prixJournalier -= 42.5;
            txt_prix.Text = prixJournalier.ToString("C");
        }

        private void check_telephone_Checked(object sender, RoutedEventArgs e)
        {
            prixJournalier += 7.5;
            txt_prix.Text = prixJournalier.ToString("C");
        }

        private void check_telephone_Unchecked(object sender, RoutedEventArgs e)
        {
            prixJournalier -= 7.5;
            txt_prix.Text = prixJournalier.ToString("C");
        }

        private void lb_litsDispo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // si aucun lit n'est choisis
            if (lb_litsDispo.SelectedIndex == -1 || cb_patient.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                // met le patient et le lit choisis dans les objets
                Patient patient = cb_patient.SelectedItem as Patient;
                Lit litChoisi = lb_litsDispo.SelectedItem as Lit;

                // assurance du patient et requetes pour voir combien de lits de chaque categorie sont disponibles
                string assurancePatient = patient.idAssurance;
                var nbLitsStandardDispo = (from nbLits in database.Lits
                                           where nbLits.occupe == false && nbLits.idType == "standard"
                                           select nbLits).Count();
                var nbLitsSemiPriveDispo = (from nbLits in database.Lits
                                            where nbLits.occupe == false && nbLits.idType == "semi-privee"
                                            select nbLits).Count();

                // si le patient n'a pas d'assurance privee et que des lits standards sont disponibles
                if (assurancePatient == "assPublique" && nbLitsStandardDispo > 0 && litChoisi.idType == "semi-privee")
                {
                    prixLit = 267;
                    prixJournalier += prixLit;
                    txt_prix.Text = prixJournalier.ToString("C");
                }
                // si le patient n'a pas d'assurance privee et que des lits semi-privee sont disponibles
                else if (assurancePatient == "assPublique" && nbLitsSemiPriveDispo > 0 && litChoisi.idType == "privee")
                {
                    prixLit = 571;
                    prixJournalier += prixLit;
                    txt_prix.Text = prixJournalier.ToString("C");
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

        // verification de chacun des champs et affichage d'un message d'erreur s'il est vide
        private static bool ChampRemplis(object[] test, string[] nomTest)
        {
            bool testOK = false;

            for (int i = 0; i < test.Length; i++)
            {
                string testVide = test[i].ToString();

                if (String.IsNullOrEmpty(testVide))
                {
                    MessageBox.Show("Veuillez remplir le champ " + nomTest[i], "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
                // si le dernier champ n'est pas vide, le test est OK
                else if (i == test.Length - 1)
                    testOK = true;
            }
            return testOK;
        }

        // retourne la liste des lits disponibles avec le departement et le type de lit choisis
        private List<Lit> ListBoxLitsUpdate(string departement, string type)
        {
            var litdispoQuery = (from lit in database.Lits
                                 where (lit.idDepartement == departement
                                 && lit.idType == type && lit.occupe == false)
                                 select lit).ToList();

            return litdispoQuery;
        }
    }
}
