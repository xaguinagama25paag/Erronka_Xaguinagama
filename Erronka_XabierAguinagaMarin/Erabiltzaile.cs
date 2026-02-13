using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erronka_XabierAguinagaMarin
{
   public class Erabiltzaile(int id, string izena)
    {
        private readonly int id = id;

        private string izena = izena;

        public int GetId()
        {
            return id;
        }
        public void SetIzena(string izena) {
            this.izena = izena;
        }
        public string GetIzena() { 
        return izena;
        }
    }
}
