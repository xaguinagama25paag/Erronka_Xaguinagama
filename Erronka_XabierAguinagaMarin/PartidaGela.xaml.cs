using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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

namespace Erronka_XabierAguinagaMarin
{
    /// <summary>
    /// Interaction logic for PartidaGela.xaml
    /// </summary>
    public partial class PartidaGela : Window
    {
        readonly int idea;
        readonly StreamReader reader;
        readonly StreamWriter writer;
        readonly NetworkStream ns;
        readonly string rola;
        readonly List<string> jokalariLista;
        readonly List<string> a;
        readonly List<string> ratingLista;
        readonly List<bool> rated;
        readonly Thread entzuten;
        bool aldatzen = false;
        readonly string kodea;



        public PartidaGela(int id, StreamWriter writerra, StreamReader readerra, string rol, NetworkStream nsa)
        {
            InitializeComponent();
            idea = id;
            writer = writerra;
            reader = readerra;
            ns = nsa;
            rola = rol;
            a = ["a"];
            jokalariLista = [];

            if (rola == "Host")
            {
                writer.WriteLine("3");
                jokalariLista.Add(reader.ReadLine());
                kodea = reader.ReadLine();
                partidaCode.Text = "Partida kodigoa: " + kodea;
            }
            else
            {
                Hosteatu.Visibility = Visibility.Collapsed;
                writer.WriteLine("5");
                kodea = reader.ReadLine();
                partidaCode.Text = "Partida kodigoa: " + kodea;
                string mezuak = "e";
                while (mezuak != "fin")
                {
                    mezuak = reader.ReadLine();
                    if (mezuak != "fin")
                    {
                        jokalariLista.Add(mezuak);
                    }
                }

            }
            partaideLista.ItemsSource = a;
            partaideLista.ItemsSource = jokalariLista;
            entzuten = new Thread(delegate ()
            {
                Entzuten();
            });
            entzuten.Start();

        }
        public PartidaGela(int id, StreamWriter writerra, StreamReader readerra, string rol, NetworkStream nsa, string kodea, List<string> lista)
        {
            InitializeComponent();
            idea = id;
            writer = writerra;
            reader = readerra;
            ns = nsa;
            rola = rol;
            a = [];
            ratingLista = [];
            rated = [];
            partidaCode.Text = "Partida kodigoa: " + kodea;



            jokalariLista = lista;

            if (rola != "Host")
            {
                Hosteatu.Visibility = Visibility.Collapsed;
                string mezuak = "e";
            }
            entzuten = new Thread(delegate ()
            {
                Entzuten();
            });
            entzuten.Start();
            partaideLista.ItemsSource = a;
            partaideLista.ItemsSource = jokalariLista;
            puntuakBorde.Visibility = Visibility.Visible;
            foreach (var item in jokalariLista)
            {
                ratingLista.Add("☆☆☆☆☆");
                rated.Add(false);
                a.Add("a");
                /*★☆*/
            }
            partaidePuntuak.ItemsSource = a;
            partaidePuntuak.ItemsSource = ratingLista;

        }
        void Entzuten()
        {
            while (!aldatzen)
            {
                string linea;
                try
                {
                    linea = reader.ReadLine();
                    switch (linea)
                    {
                        case "1":
                            linea = reader.ReadLine().ToString();
                            jokalariLista.Add(linea);
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                partaideLista.ItemsSource = a;
                                partaideLista.ItemsSource = jokalariLista;
                            }));
                            break;
                        case "2":
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {

                                aldatzen = true;

                                var window = new Partidan(idea, writer, reader, rola, jokalariLista, ns, kodea);
                                this.Close();
                                window.ShowDialog();
                            }));
                            break;
                    }

                }
                catch
                {
                    if (aldatzen)
                    {
                        MessageBox.Show("Zerbitzariarekin konexioa moztu egin da");
                    }
                    break;

                }
            }
        }
        void Client_Closing(object sender, CancelEventArgs e)
        {
            if (!aldatzen)
            {
                ns.Close();
                reader.Close();
                writer.Close();
            }
        }

        private void Hosteatu_Click(object sender, RoutedEventArgs e)
        {
            writer.WriteLine("7");

        }

        private void PartaidePuntuak_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = partaidePuntuak.SelectedIndex;
            bool check = false;
            string u;
            if (i != -1)
            {
                if (!rated[i])
                {
                    u = Interaction.InputBox("Jarri " + jokalariLista[i] + "-ren kalifikazioa azkeneko partidan, 1-etik 5-era", "Kalifikazioa");
                    try
                    {
                        int o = int.Parse(u);
                        if (o <= 0 || o >= 6)
                        {
                            MessageBox.Show("zenbakia 1-etik 5-era izan behar da");
                        }
                        else
                        {
                            switch (o)
                            {

                                case 1:
                                    ratingLista[i] = "★☆☆☆☆";
                                    break;
                                case 2:
                                    ratingLista[i] = "★★☆☆☆";
                                    break;
                                case 3:
                                    ratingLista[i] = "★★★☆☆";
                                    break;
                                case 4:
                                    ratingLista[i] = "★★★★☆";
                                    break;
                                case 5:
                                    ratingLista[i] = "★★★★★";
                                    break;
                            }
                            check = true;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Jarri zenbaki bat 1-etik 5-era, mesedez");
                    }
                    if (check)
                    {
                        partaidePuntuak.ItemsSource = a;
                        partaidePuntuak.ItemsSource = ratingLista;
                        writer.WriteLine("9");
                        writer.WriteLine(u);
                        writer.WriteLine(i.ToString());
                        rated[i] = true;

                    }

                }
            }
        }
    }
}

