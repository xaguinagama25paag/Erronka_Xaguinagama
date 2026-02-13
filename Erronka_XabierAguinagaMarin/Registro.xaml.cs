using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Erronka_XabierAguinagaMarin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Registro : Window
    {
        readonly int port = 13000;
        IPAddress ipa;
        TcpClient client;
        NetworkStream ns;
        StreamReader reader;
        StreamWriter writer;
        readonly string linea = "";
        readonly Thread entzuten;
        public Registro()
        {
            InitializeComponent();
            Konektatu();
        }

        private void Konektatu()
        {
            new Thread(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    status.Text = "Estatus: Konektatzen";
                    status.Foreground = Brushes.Black;
                }));
                ipa = IPAddress.Parse("127.0.0.1");
                client = new TcpClient();
                try
                {
                    client.Connect(ipa, port);
                    ns = client.GetStream();
                    reader = new StreamReader(ns, Encoding.UTF8);
                    writer = new StreamWriter(ns, Encoding.UTF8)
                    {
                        AutoFlush = true
                    };
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        status.Text = "Estatus: Konektatuta";
                        status.Foreground = Brushes.LightGreen;
                        if (berriro.Visibility == Visibility.Visible)
                        {
                            berriro.Visibility = Visibility.Collapsed;
                        }
                    }));
                    //  entzuten = new Thread(delegate ()
                    //  {
                    //     Entzuten();
                    //  });
                    //  entzuten.Start();

                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        status.Text = "Estatus: Deskonektatuta";
                        status.Foreground = Brushes.Red;
                        berriro.Visibility = Visibility.Visible;
                    }));

                }

            }).Start();
        }

        /* void Entzuten()
         {
             while (true)
             {
                 try
                 {
                     linea = reader.ReadLine();

                 }
                 catch
                 {
                     MessageBox.Show("Zerbitzariarekin konexioa moztu egin da");
                     break;

                 }
             }
         }*/


        private void Erregistratu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                writer.WriteLine("4");
                writer.WriteLine(izena.Text);
                writer.WriteLine(pasahitza.Password.ToString());
                string a = reader.ReadLine();
                if (a == "true")
                {
                    MessageBox.Show("Kontua sortuta");
                    var window = new MainWindow();
                    this.Close();
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Erabiltzailea existitzen da, saiatu beste izen batekin");
                }
            }
            catch {
                MessageBox.Show("Ez dago zerbitzariarekin konekxioa");
            }
        }
        private void SaiatuBerriro(object sender, RoutedEventArgs e)
        {
            Konektatu();
        }

        private void Loginera_Click(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            this.Close();
            window.ShowDialog();
        }
        void Client_Closing(object sender, CancelEventArgs e)
        {

        }
        }
}