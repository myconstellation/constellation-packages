namespace Pollen
{
    public struct Risque
    {
        public int id;
        public string couleur;
    }

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
        /// <param name="niveau">Niveau</param>
        public Vegetal(string nom, string couleur, int niveau)
        {
            Nom = nom;
            Couleur = couleur;
            Niveau = niveau;
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
