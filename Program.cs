 namespace TSPGeneticAlgorithm
 {
    /// <summary>
    /// Główna klasa programu, rozwiązująca problem komiwojażera przy użyciu algorytmu genetycznego.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Główna metoda programu.
        /// </summary>
        /// <param name="args">Argumenty wiersza poleceń.</param>
        
        public static void Main(string[] args)
        {
            try
            {
                // Wczytanie danych z pliku
                var tspData = LoadTspData("bier127.tsp");

                // Rozwiązanie problemu komiwojażera algorytmem genetycznym
                var solution = SolveTsp(tspData, 10000);

                // Zapisanie rozwiązania do pliku
                SaveSolution(solution, "solution.txt");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Plik wejściowy nie został znaleziony.");
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"Niepoprawne dane wejściowe: {ex.Message}");
            }
            catch (Exception)
            {
                Console.WriteLine("Wystąpił nieoczekiwany błąd.");
            }
        }

        # region TspData-LoadTspData
        /// <summary>
        /// Wczytuje dane problemu komiwojażera z pliku.
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku z danymi.</param>
        /// <returns>Obiekt zawierający wczytane dane.</returns>
        private static TspData LoadTspData(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var dimensionLine = lines.FirstOrDefault(l => l.StartsWith("DIMENSION"));
            if (dimensionLine == null)
                throw new InvalidDataException("Brak linii DIMENSION w pliku wejściowym.");

            if (!int.TryParse(dimensionLine.Split(':')[1], out int dimension))
                throw new InvalidDataException("Niepoprawna wartość DIMENSION w pliku wejściowym.");

            var nodeCoordSectionIndex = Array.IndexOf(lines, "NODE_COORD_SECTION") + 1;
            if (nodeCoordSectionIndex == 0)
                throw new InvalidDataException("Brak sekcji NODE_COORD_SECTION w pliku wejściowym.");

            var cities = new List<City>();
            for (int i = nodeCoordSectionIndex; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    break;

                var parts = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                    throw new InvalidDataException($"Niepoprawny format współrzędnych miasta w linii {i + 1}.");

                if (!int.TryParse(parts[0], out int number) || !int.TryParse(parts[1], out int x) || !int.TryParse(parts[2], out int y))
                    throw new InvalidDataException($"Niepoprawne wartości współrzędnych miasta w linii {i + 1}.");

                cities.Add(new City(number, x, y));
            }

            return new TspData(dimension, cities);
        }
        #endregion


        #region TspSolution-SolveTsp

        /// <summary>
        /// Rozwiązuje problem komiwojażera przy użyciu algorytmu genetycznego.
        /// </summary>
        /// <param name="tspData">Obiekt zawierający dane problemu komiwojażera.</param>
        /// <param name="maxMilliseconds">Maksymalny czas wykonania w milisekundach.</param>
        /// <returns>Obiekt reprezentujący znalezione rozwiązanie.</returns>
        private static TspSolution SolveTsp(TspData tspData, int maxMilliseconds)
        {
            // Parametry algorytmu genetycznego
            int populationSize = 10000;
            int maxGenerations = 1000;
            int maxIterationsWithoutImprovement = 1000;
            double mutationRate = 0.3;
            double crossoverRate = 0.1;
            double eliteRate = 0.1; // Elitarna wartość procentowa

            // Inicjalizacja populacji
            var population = new List<TspSolution>();
            for (int i = 0; i < populationSize; i++)
                population.Add(TspSolution.CreateRandom(tspData));

            // Ustawienie stopera czasu
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            // Inicjalizacja zmiennych warunków stopu
            int generationWithoutImprovement = 0;
            int generation = 0;

            // Rozwiązanie problemu komiwojażera algorytmem genetycznym
            while (generation < maxGenerations && generationWithoutImprovement < maxIterationsWithoutImprovement && stopwatch.ElapsedMilliseconds < maxMilliseconds)
            {
                // Selekcja elitarna
                int eliteCount = (int)(populationSize * eliteRate);
                var elite = population.OrderBy(s => s.Cost).Take(eliteCount).ToList();

                // Selekcja
                var newPopulation = new List<TspSolution>();
                for (int i = 0; i < populationSize - eliteCount; i++)
                {
                    var tournament = new List<TspSolution>();
                    for (int j = 0; j < 5; j++) // losowo wybierz 5 osobników
                    {
                        tournament.Add(population[Random.Next(population.Count)]);
                    }
                    var winner = tournament.OrderBy(s => s.Cost).First(); // wybierz zwycięzcę
                    newPopulation.Add(winner); // dodaj zwycięzcę do nowej populacji
                }

                // Dodanie elitarnych osobników do nowej populacji
                newPopulation.AddRange(elite);

                population = newPopulation;

                // Krzyżowanie
                for (int i = 0; i < populationSize / 2; i += 2)
                {
                    if (Random.NextDouble() < crossoverRate)
                    {
                        var offspring1 = new TspSolution();
                        var offspring2 = new TspSolution();
                        Crossover(population[i], population[i + 1], out offspring1, out offspring2);
                        population.Add(offspring1);
                        population.Add(offspring2);
                    }
                    else
                    {
                        population.Add(population[i]);
                        population.Add(population[i + 1]);
                    }
                }

                // Mutacja
                for (int i = populationSize / 2; i < population.Count; i++)
                {
                    if (Random.NextDouble() < mutationRate)
                        Mutate(population[i]);
                }
                // Aktualizacja warunków stopu
                generation++;
                if (population[0].Cost < population[populationSize / 2].Cost)
                    generationWithoutImprovement = 0;
                else
                    generationWithoutImprovement++;

                // Wypisanie informacji o postępach algorytmu co 10 iteracji
                if (generation % 10 == 0)
                {
                    var bestSolution = population[0];
                    var timeElapsed = stopwatch.Elapsed;
                    Console.WriteLine($"Generation {generation}, Best solution: {bestSolution}, Elapsed time: {timeElapsed}");
                }
            }

            // Zakończenie stopera czasu i zwrócenie najlepszego znalezionego rozwiązania
            stopwatch.Stop();
            var totalTimeElapsed = stopwatch.Elapsed;
            
           return population[0];
        }
        #endregion


        #region  Crossover
        /// <summary>
        /// Wykonuje operację krzyżowania na dwóch rozwiązaniach problemu komiwojażera.
        /// </summary>
        /// <param name="parent1">Pierwsze rodzicowskie rozwiązanie.</param>
        /// <param name="parent2">Drugie rodzicowskie rozwiązanie.</param>
        /// <param name="offspring1">Pierwsze potomstwo.</param>
        /// <param name="offspring2">Drugie potomstwo.</param>
        private static void Crossover(TspSolution parent1, TspSolution parent2, out TspSolution offspring1, out TspSolution offspring2)
        {
            int[] order1 = new int[parent1.Order.Length];
            int[] order2 = new int[parent2.Order.Length];
            Array.Copy(parent1.Order, order1, order1.Length);
            Array.Copy(parent2.Order, order2, order2.Length);

            int a = Random.Next(order1.Length);
            int b = Random.Next(order1.Length);
            if (a > b)
                Swap(ref a, ref b);

            for (int i = a; i <= b; i++)
                Swap(ref order1[i], ref order2[Array.IndexOf(order2, order1[i])]);

            offspring1 = new TspSolution(order1);
            offspring2 = new TspSolution(order2);
        }
        #endregion

        #region Mutate
        /// <summary>
        /// Wykonuje operację mutacji na rozwiązaniu problemu komiwojażera.
        /// </summary>
        /// <param name="solution">Rozwiązanie poddane mutacji.</param>
        private static void Mutate(TspSolution solution)
        {
            int a = Random.Next(solution.Order.Length);
            int b;
            do
                b = Random.Next(solution.Order.Length);
            while (a == b);

            Swap(ref solution.Order[a], ref solution.Order[b]);
        }
        #endregion

        #region SaveSolution
        /// <summary>
        /// Zapisuje znalezione rozwiązanie problemu komiwojażera do pliku.
        /// </summary>
        /// <param name="solution">Znalezione rozwiązanie.</param>
        /// <param name="filePath">Ścieżka do pliku.</param>
        private static void SaveSolution(TspSolution solution, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Route length: {solution.Cost}");
                writer.WriteLine(string.Join(" ", solution.Order) + " " + solution.Cost);
            }
        }
        #endregion

        #region Random 
        /// <summary>
        /// Zamienia miejscami wartości dwóch  zmiennych. <summary>
        /// Zamienia miejscami wartości dwóch  zmiennych.
        /// </summary>

        private static Random Random { get; } = new Random();
        #endregion

        #region Swap
        /// <summary>
        /// Zamienia wartości dwóch zmiennych.
        /// </summary>
        /// <param name="a">Pierwsza zmienna.</param>
        /// <param name="b">Druga zmienna.</param>
        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }
        #endregion
    }
}
