using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApp12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string connectionStr = "datasource=127.0.0.1;" +
            "port=3306;" +
            "username=root;" +
            "password=;" +
            "database=hardver;";
        List<Termek> termekek = new List<Termek>();
        MySqlConnection SQLkapcsolat;


        public MainWindow()
        {
            InitializeComponent();

            AdatbazisMegnyitas();
            GyartokBetoltese();
            KategoriakBetoltese();

            TermekekBetolteseListaba();

            //Loaded += MainWindow_Loaded;
        }

        /*private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }*/

        private void btnSzukit_Click(object sender, RoutedEventArgs e)
        {
            termekek.Clear();
            string SQLSzukitettLista = String.Empty;//SzukitoLekerdezesEloallitasa()
;

            MySqlCommand SQLparancs = new MySqlCommand(SQLSzukitettLista, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(eredmenyOlvaso.GetString("Kategória"),                       eredmenyOlvaso.GetString("Gyártó"),
                                   eredmenyOlvaso.GetString("Név"),
                                   eredmenyOlvaso.GetInt32("Ár"),
                                   eredmenyOlvaso.GetInt32("Garidő"));

                termekek.Add(uj);
            }
            eredmenyOlvaso.Close();
            dgTermekek.Items.Refresh();
        }

        private void btnMentes_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = new StreamWriter("valami.csv");

            foreach (var item in termekek)
            {
                sw.WriteLine(item.ToCSVString());
            }
            sw.Close();
            MessageBox.Show("Sikeres mentés!");
        }

        private void KategoriakBetoltese()
        {
            string SQLKategoriakRendezve = "SELECT DISTINCT kategória FROM termékek ORDER BY kategória;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLKategoriakRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            cbKategoria.Items.Add(" - Nincs megadva - ");
            while (eredmenyOlvaso.Read())
            {
                cbKategoria.Items.Add(eredmenyOlvaso.GetString(0));
            }
            eredmenyOlvaso.Close();
            cbKategoria.SelectedIndex = 0;
        }

        private void GyartokBetoltese()
        {
            string SQLGyartokRendezve = "SELECT DISTINCT Gyártó FROM termékek ORDER BY Gyártó;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLGyartokRendezve, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            cbGyarto.Items.Add(" - Nincs megadva - ");
            while (eredmenyOlvaso.Read())
            {
                cbGyarto.Items.Add(eredmenyOlvaso.GetString(0));
            }
            eredmenyOlvaso.Close();
            cbGyarto.SelectedIndex = 0;
        }

        private void TermekekBetolteseListaba()
        {
            string SQLOsszesTermek = "SELECT * FROM termékek;";
            MySqlCommand SQLparancs = new MySqlCommand(SQLOsszesTermek, SQLkapcsolat);
            MySqlDataReader eredmenyOlvaso = SQLparancs.ExecuteReader();

            while (eredmenyOlvaso.Read())
            {
                Termek uj = new Termek(eredmenyOlvaso.GetString(1),                       
                                    eredmenyOlvaso.GetString(2), 
                                   eredmenyOlvaso.GetString(3), 
                                   eredmenyOlvaso.GetInt32(4), 
                                   eredmenyOlvaso.GetInt32(5));

                termekek.Add(uj);
            }
            eredmenyOlvaso.Close();
            dgTermekek.Items.Refresh();
        }

        private string SzukitoLekerdezesEloallitasa()
        {
            bool vanMarFeltetel = false;
            string SQLSzukitettLista = "SELECT * FROM termékek ";

            if (cbGyarto.SelectedIndex > 0 || cbKategoria.SelectedIndex > 0 || txtTermek.Text != "")
            {
                SQLSzukitettLista += "WHERE ";
            }
            if (cbGyarto.SelectedIndex > 0)
            {
                SQLSzukitettLista += $"gyártó='{cbGyarto.SelectedItem}'";
                vanMarFeltetel = true;
            }
            if (cbKategoria.SelectedIndex > 0)
            {
                if (vanMarFeltetel)
                {
                    SQLSzukitettLista += "AND ";
                }
                SQLSzukitettLista += $"kategória='{cbKategoria.SelectedItem}'";
                vanMarFeltetel = true;
            }
            if (txtTermek.Text != "")
            {
                if (vanMarFeltetel)
                {
                    SQLSzukitettLista += "AND ";
                }
                SQLSzukitettLista += $"név LIKE '%{txtTermek.Text}'";
            }
            return SQLSzukitettLista;
        }

        private void AdatbazisMegnyitas()
        {
            try
            {
                SQLkapcsolat = new MySqlConnection(connectionStr);
                SQLkapcsolat.Open();
            }
            catch (MySqlException hiba)
            {
                MessageBox.Show(hiba.Message);
                this.Close();
            }
        }

        private void AdatbazisLezarasa()
        {
            SQLkapcsolat.Close();
            SQLkapcsolat.Dispose();
        }
    }
}
