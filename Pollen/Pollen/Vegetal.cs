using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollen
{
    /// <summary>
    /// Classe représentant un vegetal
    /// </summary>
    public class Vegetal
    {
        /// <summary>
        /// Nom du vegetal
        /// </summary>
        public string Nom;

        /// <summary>
        /// Couleur en fct du niveau de pollenisation
        /// </summary>
        public string Couleur;

        /// <summary>
        /// Niveau de pollenisation
        /// </summary>
        public int Niveau;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="couleur">Couleur</param>
        public Vegetal(string nom, string couleur)
        {
            Nom = nom;
            Couleur = couleur;
            switch (couleur.ToUpper())
            {
                default:
                case "FFFFFF":
                    Niveau = 0;
                    break;

                case "00FF00":
                    Niveau = 1;
                    break;

                case "00B050":
                    Niveau = 2;
                    break;

                case "FFFF00":
                    Niveau = 3;
                    break;

                case "F79646":
                    Niveau = 4;
                    break;

                // TODO CONFIRMER LA COULEUR
                case "FF0000":
                    Niveau = 5;
                    break;
            }
        }

        /// <summary>
        /// Nom, niveau, couleur
        /// </summary>
        /// <returns>Nom, niveau, couleur</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2} ", Nom, Niveau, Couleur);
        }
    }
}
