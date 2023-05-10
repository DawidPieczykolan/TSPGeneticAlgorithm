/// <summary>
/// Klasa reprezentująca miasto.
/// </summary>

namespace TSPGeneticAlgorithm
{
    // <summary>
    /// Inicjalizuje nową instancję klasy City.
    /// </summary>
    /// <param name="number">Numer miasta.</param>
    /// <param name="x">Współrzędna X miasta.</param>
    /// <param name="y">Współrzędna Y miasta.</param>
    public class City
    {
        public City(int number, int x, int y)
        {
            Number = number;
            X = x;
            Y = y;
        }

        #region Pobiera numer miasta.
        /// <summary>
        /// Pobiera numer miasta.
        /// </summary>

        public int Number { get; }
        #endregion

        #region Pobiera współrzędną X 
        /// <summary>
        /// Pobiera współrzędną X miasta.
        /// </summary>
        public int X { get; }
        #endregion

        #region Pobiera współrzędną Y
        /// <summary>
        /// Pobiera współrzędną Y miasta.
        /// </summary>
        public int Y { get; }
        #endregion
    }
}
