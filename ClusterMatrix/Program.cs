/*Develop a program that generates a square matrix populated with random numbers below 13. 
 * The program should then identify clusters comprising more than five adjacent elements with the same value.
 * Please ensure that adjacent refers to elements that are positioned next to each other in either a row or a column, and diagonal elements should not be included in this definition. 
 * The matrix size should be provided as a command-line argument. 
 * Print statistics for the average cluster size and the average number of clusters based on 5513 matrices. 
 * Also, determine the most frequently occurring digit or digits.*/

namespace ClusterMatrix
{
    internal class Program
    {
        private const int MatricesCount = 5513;
        private const int MaxExclusive = 13;

        /*---------------Debut Mode--------------*/
        private static readonly bool  IsTest = false;
        //Test params
        private const int MatricesTestCount = 3;
        private const int MatricesSize = 80;
        private const bool PrintTestMatrices = true;

        private const bool EnableChecks = true;

        /*---------------Debug Mode--------------*/
        static int Main(string[] args)
        {
            int matrixCount;
            int mSize;

            if (IsTest)
            {
                matrixCount = MatricesTestCount;
                mSize = MatricesSize;

                if (PrintTestMatrices)
                    Console.WriteLine($"===  Test mode active MCount={matrixCount} and MSize={mSize}");

            }
            else
            {
                matrixCount = MatricesCount;

                if (args.Length < 1 || !int.TryParse(args[0], out mSize) || mSize <= 0)
                {
                    string got = args.Length < 1 ? "(nothing)" : $" \"{args[0]}\" ";
                    Console.WriteLine($"Invalid matrix size. I expected a positive whole number, but got {got}.");
                    Console.WriteLine("Example: ClusterMatrix 20");
                    return 1;
                }

            }

            Random randomNumber = new();

            long totalAcceptedClusters = 0;
            long totalAcceptedClusterCells = 0;

            long[] digitOccurringFreq = new long[MaxExclusive];

            int[,] grid = new int[mSize, mSize];
            bool[,] visited = new bool[mSize, mSize];

            int[] rowDirections = [ -1, 1, 0, 0 ];
            int[] columnDirections = [ 0, 0, -1, 1 ];

            int estimatedQueueCapacity = Math.Max(16, ((mSize * mSize) / 4));
            Queue<(int r, int c)> queue = new Queue<(int r, int c)>(estimatedQueueCapacity);

            for (int i = 0; i < matrixCount; i++)
            {

                //Matrix creation step
                for (int r = 0; r < mSize; r++)
                {
                    for (int c = 0; c < mSize; c++)
                    {

                        int num = randomNumber.Next(MaxExclusive);
                        grid[r, c] = num;
                        digitOccurringFreq[num]++;
                        visited[r, c] = false;
                    }
                }

                if (IsTest && PrintTestMatrices)
                {
                    Console.WriteLine($"\n===== Matrix {i + 1}/{matrixCount} (N={mSize}) =====\n");
                    //PrintMatrix(grid, mSize);
                }

                for (int r = 0; r < mSize; r++)
                {
                    for (int c = 0; c < mSize; c++)
                    {
                        if (visited[r, c])
                            continue;

                        int value = grid[r, c];
                        int size = 0;


                        /*=== Added for testing ===*/
                        int startRow = r;
                        int startCol = c;

                        int minRow = r;
                        int maxRow = r;
                        int minCol = c;
                        int maxCol = c;
                        /*=== Added for testing ===*/


                        queue.Clear();
                        visited[r, c] = true;
                        queue.Enqueue((r, c));

                        while (queue.Count > 0)
                        {
                            var (curRow, curCol) = queue.Dequeue();
                            size++;

                            /*=== Added for testing ===*/
                            if (curRow < minRow) minRow = curRow;
                            if (curRow > maxRow) maxRow = curRow;
                            if (curCol < minCol) minCol = curCol;
                            if (curCol > maxCol) maxCol = curCol;
                            /*=== Added for testing ===*/


                            for (int k = 0; k < 4; k++)
                            {
                                int newRow = curRow + rowDirections[k];
                                int newCol = curCol + columnDirections[k];

                                if (newRow < 0 || newRow >= mSize || newCol < 0 || newCol >= mSize)
                                    continue;

                                if (visited[newRow, newCol])
                                    continue;

                                if (grid[newRow, newCol] != value)
                                    continue;

                                visited[newRow, newCol] = true;
                                queue.Enqueue((newRow, newCol));
                            }
                        }

                        if (size > 5)
                        {
                            totalAcceptedClusters++;
                            totalAcceptedClusterCells += size;

                            if (IsTest && PrintTestMatrices)
                            {
                                Console.WriteLine(
                                                   $"Qualifying cluster -> Value={value}, Size={size}, " +
                                                   $"Start=({startRow},{startCol}), " +
                                                   $"Bounds=[({minRow},{minCol}) -> ({maxRow},{maxCol})]");

                                PrintMatrix(grid, mSize,
                                            startRow, startCol,
                                            minRow, minCol,
                                            maxRow, maxCol);
                            }
                        }
                    }
                }
            }

            double avgClustersPerMatrix = totalAcceptedClusters / (double)matrixCount;
            double avgClusterSize;
            if (totalAcceptedClusters == 0)
            {
                avgClusterSize = 0.0;
            }
            else
            {
                avgClusterSize = totalAcceptedClusterCells / (double)totalAcceptedClusters;
            }

            long maxFreq = 0;
            for (int i = 0; i < MaxExclusive; i++)
            {
                if (digitOccurringFreq[i] > maxFreq)
                    maxFreq = digitOccurringFreq[i];
            }

            List<int> mostFrequentDigits = [];
            for (int i = 0; i < MaxExclusive; i++)
            {
                if (digitOccurringFreq[i] == maxFreq)
                    mostFrequentDigits.Add(i);
            }

            /*====Added for testing statistics====*/
            long expectedTotalCells = matrixCount * (long)mSize * mSize;
            if (EnableChecks)
            {
                long sumFreq = 0;
                for (int d = 0; d < MaxExclusive; d++)
                    sumFreq += digitOccurringFreq[d];

                if (sumFreq != expectedTotalCells)
                {
                    Console.WriteLine(" CHECK FAILED!");
                    Console.WriteLine($"Sum(digitOccurringFreq) = {sumFreq} but expected {expectedTotalCells}");
                    return 2;
                }
            }

            Console.WriteLine($"\n ===Results===");
            Console.WriteLine($"N = {mSize}");
            Console.WriteLine($"Matrices = {matrixCount}");
            Console.WriteLine($"Total cells generated = {expectedTotalCells}");
            Console.WriteLine($"");


            Console.WriteLine($"Average number of qualifying clusters per matrix: {avgClustersPerMatrix:F6}");
            Console.WriteLine($"Average qualifying cluster size: {avgClusterSize:F6}");
            Console.WriteLine($"Total qualifying clusters found: {totalAcceptedClusters}");
            Console.WriteLine($"");



            Console.WriteLine($"Most frequent digit frequency: {maxFreq}");
            Console.WriteLine($"Most frequent digit(s): {string.Join(", ", mostFrequentDigits)}");
            Console.WriteLine($"");




            /*===Added for testing statistics===*/
            Console.WriteLine("Digit frequencies (0..12):");
            for (int d = 0; d < MaxExclusive; d++)
                Console.WriteLine($"{d,2}: {digitOccurringFreq[d]}");
            return 0;
        }

        private static void PrintMatrix(int[,] grid, int n,
                                        int startRow = -1, int startCol = -1,
                                        int minRow = -1, int minCol = -1,
                                        int maxRow = -1, int maxCol = -1)
        {
            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    bool isStart = (r == startRow && c == startCol);
                    bool insideBounds =
                        r >= minRow && r <= maxRow &&
                        c >= minCol && c <= maxCol;

                    // Save current color
                    var oldColor = Console.ForegroundColor;

                    if (isStart)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (insideBounds)
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.Write($"{grid[r, c],2} ");

                    // Restore color
                    Console.ForegroundColor = oldColor;
                }

                Console.WriteLine();
            }
        }
    }
}
