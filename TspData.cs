/// <summary>
/// Klasa reprezentująca dane problemu komiwojażera.
/// </summary>

namespace TSPGeneticAlgorithm
{
    public class TspData
    {
        /// <summary>
        /// Inicjalizuje nową instancję klasy TspData.
        /// </summary>
        /// <param name="dimension">Wymiar problemu (liczba miast).</param>
        /// <param name="cities">Lista miast.</param>
         
        public TspData(int dimension, List<City> cities)
        {
            Dimension = dimension;
            Cities = cities;
            Distances = new double[Dimension + 1, Dimension + 1];
            for (int i = 1; i <= Dimension; i++)
                for (int j = i + 1; j <= Dimension; j++)
                    Distances[i, j] = Distances[j, i] = Distance(Cities[i - 1], Cities[j - 1]);
        }
        #region Dimension
        /// <summary>
        /// Pobiera wymiar problemu (liczbę miast).
        /// </summary>
        public int Dimension { get; }
        #endregion

        #region List
        /// <summary>
        /// Pobiera listę miast.
        /// </summary>
        public List<City> Cities { get; }
        #endregion

        #region Distances macież
        /// <summary>
        /// Pobiera macierz odległości między miastami.
        /// </summary>
        public double[,] Distances { get; }
        #endregion

        #region Distance 
        /// <summary>
        /// Oblicza odległość między dwoma miastami.
        /// </summary>
        /// <param name="city1">Pierwsze miasto.</param>
        /// <param name="city2">Drugie miasto.</param>
        /// <returns>Odległość między miastami.</returns>
        private double Distance(City city1, City city2)
        {
            return Math.Abs(city1.X - city2.X) + Math.Abs(city1.Y - city2.Y);
        }
        #endregion
    }
}
