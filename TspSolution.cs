/// <summary>
/// Klasa reprezentująca rozwiązanie problemu komiwojażera.
/// </summary>


namespace TSPGeneticAlgorithm
{
    public class TspSolution
    {
        private static TspData _tspData;

        public TspSolution()
        {
            Order = new int[_tspData.Dimension];
            for (int i = 0; i < Order.Length; i++)
                Order[i] = i + 1;
        }


        #region TspSolution
        /// <summary>
        /// Inicjalizuje nową instancję klasy TspSolution.
        /// </summary>
        /// <param name="order">Kolejność odwiedzania miast.</param>
        public TspSolution(int[] order)
            : this()
        {
            Array.Copy(order, Order, order.Length);
            CalculateCost();
        }
        #endregion


        /// <summary>
        /// Pobiera lub ustawia kolejność odwiedzania miast.
        /// </summary>
        public int[] Order { get; }

        /// <summary>
        /// Pobiera koszt (długość trasy) rozwiązania.
        /// </summary>
        public double Cost { get; private set; }


        /// <summary>
        /// Tworzy losowe rozwiązanie problemu komiwojażera na podstawie danych.
        /// </summary>
        /// <param name="tspData">Dane problemu komiwojażera.</param>
        /// <returns>Losowe rozwiązanie problemu komiwojażera.</returns>
        public static TspSolution CreateRandom(TspData tspData)
        {
            _tspData = tspData;

            var solution = new TspSolution();
            for (int i = solution.Order.Length - 1; i >= 1; i--)
                Swap(ref solution.Order[i], ref solution.Order[Random.Next(i + 1)]);

            solution.CalculateCost();
            return solution;
        }

        private static void Swap(ref int v1, ref int v2)
        {
            int temp = v1;
            v1 = v2;
            v2 = temp;
        }
        /// <summary>
        /// Oblicza koszt (długość trasy) rozwiązania.
        /// </summary>
        private void CalculateCost()
        {
            Cost = _tspData.Distances[Order[Order.Length - 1], Order[0]];
            for (int i = 0; i < Order.Length - 1; i++)
                Cost += _tspData.Distances[Order[i], Order[i + 1]];
        }

        private static Random Random { get; } = new Random();
    }
}
