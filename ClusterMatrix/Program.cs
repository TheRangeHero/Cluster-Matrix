using System;
using System.Collections.Generic;

namespace ClusterMatrix
{
    /*Develop a program that generates a square matrix populated with random numbers below 13. 
     * The program should then identify clusters comprising more than five adjacent elements with the same value.
     * Please ensure that adjacent refers to elements that are positioned next to each other in either a row or a column, and diagonal elements should not be included in this definition. 
     * The matrix size should be provided as a command-line argument. 
     * Print statistics for the average cluster size and the average number of clusters based on 5513 matrices. 
     * Also, determine the most frequently occurring digit or digits.*/

    internal class Program
    {
        private const int MATRICESCOUNT = 5513;
        private const int MAXEXCLUSIVE = 13;


        static int Main(string[] args)
        {
            int matrixCount;
            int mSize;

            matrixCount = MATRICESCOUNT;

            if (args.Length < 1 || !int.TryParse(args[0], out mSize) || mSize <= 0)
            {
                string got = args.Length < 1 ? "(nothing)" : $" \"{args[0]}\" ";
                Console.WriteLine($"Invalid matrix size. I expected a positive whole number, but got {got}.");
                Console.WriteLine("Example: dotnet run -- <positive number>");
                return 1;
            }

            Random randomNumber = new();

            long totalAcceptedClusters = 0;
            long totalAcceptedClusterCells = 0;

            long[] digitOccurringFreq = new long[MAXEXCLUSIVE];

            int[,] grid = new int[mSize, mSize];
            bool[,] visited = new bool[mSize, mSize];

            int[] rowDirections = new[] { -1, 1, 0, 0 };
            int[] columnDirections = new[] { 0, 0, -1, 1 };

            int estimatedQueueCapacity = Math.Max(16, ((mSize * mSize) / 4));
            Queue<(int r, int c)> queue = new Queue<(int r, int c)>(estimatedQueueCapacity);

            for (int i = 0; i < matrixCount; i++)
            {

                //Matrix creation and population
                for (int r = 0; r < mSize; r++)
                {
                    for (int c = 0; c < mSize; c++)
                    {

                        int num = randomNumber.Next(MAXEXCLUSIVE);
                        grid[r, c] = num;
                        digitOccurringFreq[num]++;
                        visited[r, c] = false;
                    }
                }

                //BFS logic start point
                for (int r = 0; r < mSize; r++)
                {
                    for (int c = 0; c < mSize; c++)
                    {
                        if (visited[r, c])
                            continue;

                        int value = grid[r, c];
                        int size = 0;

                        queue.Clear();
                        visited[r, c] = true;
                        queue.Enqueue((r, c));

                        //Cluster checking logic start point
                        while (queue.Count > 0)
                        {
                            var (curRow, curCol) = queue.Dequeue();
                            size++;


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
            for (int i = 0; i < MAXEXCLUSIVE; i++)
            {
                if (digitOccurringFreq[i] > maxFreq)
                    maxFreq = digitOccurringFreq[i];
            }

            List<int> mostFrequentDigits = new List<int>();
            for (int i = 0; i < MAXEXCLUSIVE; i++)
            {
                if (digitOccurringFreq[i] == maxFreq)
                    mostFrequentDigits.Add(i);
            }



            Console.WriteLine($"\n ===Results===");
            Console.WriteLine($"N = {mSize}");
            Console.WriteLine($"Matrices = {matrixCount}");
            Console.WriteLine($"");

            Console.WriteLine($"Average number of qualifying clusters per matrix: {avgClustersPerMatrix:F6}");
            Console.WriteLine($"Average qualifying cluster size: {avgClusterSize:F6}");
            Console.WriteLine($"Total qualifying clusters found: {totalAcceptedClusters}");
            Console.WriteLine($"");

            Console.WriteLine($"Most frequent digit frequency: {maxFreq}");
            Console.WriteLine($"Most frequent digit(s): {string.Join(", ", mostFrequentDigits)}");
            Console.WriteLine($"");

            return 0;
        }
    }
}
