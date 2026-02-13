using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Erronka_XabierAguinagaMarin;

namespace Erronka_XabierAguinagaMarin_Zerbitzari
{

    public partial class Datubasea
    {
        static SQLiteConnection dbConnection;
        public Datubasea()
        {

           dbConnection = new SQLiteConnection(@"Data Source=datubasea.s3db");
        }

        public static void Konektatu()
        {
            dbConnection.Open();
        }
        public static void Itzali() {
            dbConnection.Close();
        }

        public Erabiltzaile lortuErabiltzailea(string izena, string pasahitza)
        {
            SQLiteCommand command = new SQLiteCommand("Select * from Erabiltzaileak where izena='"+izena+"' and pasahitza='"+pasahitza+"';", dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Erabiltzaile((int)(long)reader["id"], (string)reader["izena"]);
            }
            else
            {
                return new Erabiltzaile(-1,"Ezer");
            }
                
        }
        public string erabiltzaileExistitzen(string izena)
        {
            SQLiteCommand command = new SQLiteCommand("Select * from Erabiltzaileak where izena='" + izena + "';", dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return "false";
            }
            else
            {
                return "true";
            }

        }
        public string lortuErabiltzaile(int id)
        {
            SQLiteCommand command = new SQLiteCommand("Select * from Erabiltzaileak where id='" + id + "';", dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (string)reader["izena"];
            }
            else
            {
                return "true";
            }
        }

                public List<int> LortuPuntuazioak(int id)
        {
            SQLiteCommand command = new SQLiteCommand("Select * from Puntuazioa where idErabiltzaile='" + id + "';", dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<int> lista = new List<int>();
            if (reader.Read())
            {
                lista.Add((int)(long)reader["puntuazio"]);
            }
            else
            {
                lista.Add(0);
                return lista;
            }
                while (reader.Read())
                {
                    lista.Add((int)(long)reader["puntuazio"]);
                }
            return lista;

        }

        public List<int> LortuEmandakoPuntuazioak(int id)
        {
            SQLiteCommand command = new SQLiteCommand("Select * from Puntuazioa where idEbaluatzaile='" + id + "';", dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            List<int> lista = new List<int>();
            if (reader.Read())
            {
                lista.Add((int)(long)reader["puntuazio"]);
                while (reader.Read())
                {
                    lista.Add((int)(long)reader["puntuazio"]);
                }
            }
            else
            {
                lista.Add(0);
                return lista;
            }

            return lista;

        }

        public void sortuKontua(string izena, string pasahitza)
        {
            SQLiteCommand command = new SQLiteCommand("insert into Erabiltzaileak(izena, pasahitza) values ('"+izena+"','"+pasahitza+"')", dbConnection);
            command.ExecuteReader();

        }
        public void sortuRating(int rating, int idKalifikatzaile, int idKalifikatuta)
        {
            SQLiteCommand command = new SQLiteCommand("insert into Puntuazioa(idErabiltzaile, puntuazio, idEbaluatzaile) values ('" + idKalifikatuta + "','" + rating +"','" + idKalifikatzaile + "')", dbConnection);
            command.ExecuteReader();

        }
    }
}
