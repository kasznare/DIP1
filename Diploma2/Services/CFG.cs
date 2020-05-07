using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diploma2.Utilities;

namespace Diploma2.Services
{
    // C# program to print all // combination of size r // in an array of size n 
    public class CFG
    {
        /* arr[] ---> Input Array 
        data[] ---> Temporary array to store current combination 
        start & end ---> Staring and Ending indexes in arr[] 
        index ---> Current index in data[] 
        r ---> Size of a combination to be printed */
        public static List<int[]> indices = new List<int[]>();
        static void combinationUtil(int[] arr, int[] data,
            int start, int end, int index, int r)
        {
            // Current combination is ready to be printed, print it 
            if (index == r)
            {
                int[] localdata = new int[r];
                for (int j = 0; j < r; j++)
                {
                    localdata[j] = data[j];
                    Logger.WriteLog(data[j] + " ");
                }
                indices.Add(localdata);
                Logger.WriteLog("");
                return;
            }

            // replace index with all possible elements. The condition "end-i+1 >= 
            // r-index" makes sure that including one element at index will make a 
            // combination with remaining elements at remaining positions 
            for (int i = start; i <= end && end - i + 1 >= r - index; i++)
            {
                data[index] = arr[i];
                combinationUtil(arr, data, i + 1, end, index + 1, r);
            }
        }

        // The main function that prints all combinations of size r
        // in arr[] of size n. This function mainly uses combinationUtil() 
        public static List<int[]> printCombination(int[] arr, int n, int r)
        {
            indices.Clear();
            // A temporary array to store all combination one by one 
            int[] data = new int[r];

            // Print all combination using temprary array 'data[]' 
            combinationUtil(arr, data, 0, n - 1, 0, r);
            return indices;
        }
        // Driver Code 
        //static public void Main()
        //{
        //    int[] arr = { 1, 2, 3, 4, 5 };
        //    int r = 3;
        //    int n = arr.Length;
        //    printCombination(arr, n, r);
        //}





















        // A utility function to find the 
        // vertex with minimum distance 
        // value, from the set of vertices 
        // not yet included in shortest 
        // path tree 
        static int V = 9;
        int minDistance(int[] dist,
                        bool[] sptSet)
        {
            // Initialize min value 
            int min = int.MaxValue, min_index = -1;

            for (int v = 0; v < V; v++)
                if (sptSet[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    min_index = v;
                }

            return min_index;
        }

        // A utility function to print 
        // the constructed distance array 
        private void printSolution(int[] dist, int n)
        {
            Logger.WriteLog("Vertex Distance from Source\n");
            for (int i = 0; i < V; i++)
            {
                Logger.WriteLog(i + " \t\t " + dist[i] + "\n");

            }
        }

        // Function that implements Dijkstra's single source shortest path algorithm 
        // for a graph represented using adjacency matrix representation 
        public int[] dijkstra(int[,] graph, int src, int length)
        {
            V = length;
            int[] dist = new int[V]; // The output array. dist[i] will hold the shortest 
                                     // distance from src to i 

            // sptSet[i] will true if vertex i is included in shortest path 
            // tree or shortest distance from src to i is finalized 
            bool[] sptSet = new bool[V];

            // Initialize all distances as INFINITE and stpSet[] as false 
            for (int i = 0; i < V; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
            }

            // Distance of source vertex from itself is always 0 
            dist[src] = 0;

            // Find shortest path for all vertices 
            for (int count = 0; count < V - 1; count++)
            {
                // Pick the minimum distance vertex from the set of vertices not yet 
                // processed. u is always equal to src in first iteration. 
                int u = minDistance(dist, sptSet);

                // Mark the picked vertex as processed 
                sptSet[u] = true;

                // Update dist value of the adjacent vertices of the picked vertex. 
                for (int v = 0; v < V; v++)

                    // Update dist[v] only if is not in sptSet, there is an edge from u 
                    // to v, and total weight of path from src to v through u is smaller 
                    // than current value of dist[v] 
                    try
                    {
                        if (!sptSet[v] && graph[u, v] != 0 &&
                             dist[u] != int.MaxValue && dist[u] + graph[u, v] < dist[v])
                        {
                            dist[v] = dist[u] + graph[u, v];

                        }
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
            }

            // print the constructed distance array 
            printSolution(dist, V);
            return dist;
        }

        // Driver Code 
        //public static void Main()
        //{
        //    /* Let us create the example graph discussed above */
        //    int[,] graph = new int[,] { { 0, 4, 0, 0, 0, 0, 0, 8, 0 },
        //                              { 4, 0, 8, 0, 0, 0, 0, 11, 0 },
        //                              { 0, 8, 0, 7, 0, 4, 0, 0, 2 },
        //                              { 0, 0, 7, 0, 9, 14, 0, 0, 0 },
        //                              { 0, 0, 0, 9, 0, 10, 0, 0, 0 },
        //                              { 0, 0, 4, 14, 10, 0, 2, 0, 0 },
        //                              { 0, 0, 0, 0, 0, 2, 0, 1, 6 },
        //                              { 8, 11, 0, 0, 0, 0, 1, 0, 7 },
        //                              { 0, 0, 2, 0, 0, 0, 6, 7, 0 } };
        //    GFG t = new GFG();
        //    t.dijkstra(graph, 0);
        //}


    }


}
