using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erronka_XabierAguinagaMarin
{
    public class Erabiltzaile
    {
        private int id;

        private string izena;

        public Erabiltzaile(int id, string izena)
        {
            this.id = id;
            this.izena = izena;
        }
        public int getId()
        {
            return id;
        }
        public void setIzena(string izena)
        {
            this.izena = izena;
        }
        public string getIzena()
        {
            return izena;
        }
    }
}
