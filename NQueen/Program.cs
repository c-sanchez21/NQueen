using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NQueen
{
    class Program
    {
        public struct Solution
        {
            public Node Answer;
            public int SearchCosts;
            public TimeSpan RunTime;
        }

        static void Main(string[] args)
        {
            const int N = 14;
            const int MAX_TEST = 100;

            Solution s;
            long totalRunTime = 0;
            long totalSearchCost = 0;
            List<Solution> solvedSolutions = new List<Solution>();
            for (int i = 0; i < MAX_TEST; i++)
            {
                s = SteepestAscentHillClimbing(N);
                totalRunTime += (long)s.RunTime.TotalMilliseconds;
                totalSearchCost += s.SearchCosts;
                if (s.Answer.Value == 0)
                    solvedSolutions.Add(s);
            }

            Console.WriteLine("Steepest Ascent Hill Climbing Results: ");
            Console.Write("Avg Search Cost : ");
            Console.WriteLine((totalSearchCost / MAX_TEST).ToString());
            Console.Write("Avg Run Time : ");
            Console.WriteLine((totalRunTime/MAX_TEST).ToString()+"ms");
            Console.WriteLine("Solved Cases : " + solvedSolutions.Count.ToString() + " out of " + MAX_TEST);
            Console.Write("Percentage Solved : ");
            double percentageSolved = solvedSolutions.Count;
            percentageSolved /= MAX_TEST;            
            Console.WriteLine(percentageSolved.ToString());
            Console.Write("Push any key to continue to next search...");
            Console.ReadKey();
            Console.WriteLine();
            Console.WriteLine();
            
            solvedSolutions.Clear();
            TimeSpan runTime = new TimeSpan();
            long totalPop = 0;
            Console.WriteLine("Performing Genetic Search...");
            while(solvedSolutions.Count < 3)
            {
                s = GeneticAlgorithm(N, 500, 10000);
                runTime = runTime.Add(s.RunTime);
                totalPop =+ s.SearchCosts;
                if (s.Answer.Value == 0)
                {
                    Console.WriteLine("Genetic Algorithm Result :");
                    Console.WriteLine("Total Population Generated : "+totalPop.ToString());
                    Console.WriteLine("Run Time :" + runTime.TotalMilliseconds.ToString() + "ms");
                    Console.WriteLine(s.Answer);
                    Console.WriteLine();
                    solvedSolutions.Add(s);
                    runTime = new TimeSpan();
                    totalPop = 0;
                }                
            }
            Console.Write("Push any key to exit...");
            Console.ReadKey();
        }

        private static Solution SteepestAscentHillClimbing(int N)
        {
            DateTime start = DateTime.Now;
            Node n = GetRandomNode(N);            
            Solution s = new Solution();
            s.SearchCosts = 1;
            PriorityQueue<int, Node> Q;
            while (true)
            {
                List<Node> neighbors = GetNeighbors(n);
                Q = new PriorityQueue<int, Node>();
                foreach (Node neighbor in neighbors)
                {
                    Q.Enequeue(neighbor.Value, neighbor);                    
                    //s.SearchCosts++ //Increase Search Cost for each node generated.
                }
                Node bestValue = Q.Dequeue();
                if (bestValue.Value >= n.Value)
                    break;
                n = bestValue; //Make Move and Increase search cost (per Prof. Instruction)
                s.SearchCosts++;
            }

            s.RunTime = DateTime.Now - start;
            s.Answer = n;
            return s;
        }
        
        private static Random random = new Random(DateTime.Now.Millisecond);
        private static Node GetRandomNode(int N)
        {
            Node n = new Node(N);                        
            int j = 0;
            for (int i = 0; i < N; i++)
            {
                j = random.Next(N);
                n.Positions[i] = j;                
            }
            return n;
        }

        private class NodeSorter : IComparer<Node>
        {
            #region IComparer<Node> Members
            public int Compare(Node x, Node y)
            {
                return x.Value.CompareTo(y.Value);
            }
            #endregion
        }

        private static int max_fitness = 0;      
        private static Solution GeneticAlgorithm(int N, int maxPop, int maxGenerations)
        {
            DateTime start = DateTime.Now;
            Solution s = new Solution();
            max_fitness = 0;
            int i;
            for (i = N - 1; i > 0; i--)
                max_fitness += i;

            List<Node> pop = new List<Node>(maxPop + 1);
            List<Node> newPop = new List<Node>(maxPop + 1);
            NodeSorter sorter = new NodeSorter();

            Node n, parent1, parent2;
            Node[] children;
            Random rand = new Random();
            int totalFitness, r;
            for (i = 0; i < maxPop; i++)
                pop.Add(GetRandomNode(N));

            pop.Sort(sorter);
            n = pop[0];
            s.SearchCosts = pop.Count;
            int generation = 0;
            while (n.Value != 0 && generation < maxGenerations)
            {
                generation++;
                totalFitness = 0;
                for (i = 0; i < pop.Count; i++)
                    totalFitness += max_fitness - n.Value;

                newPop = new List<Node>(maxPop + 1);
                newPop.Add(n);

                while (newPop.Count < maxPop)
                {
                    //Selection 
                    parent1 = Selection(pop, rand.Next(0, totalFitness + 1));
                    parent2 = Selection(pop, rand.Next(0, totalFitness + 1));
                    while (parent1.Equals(parent2))
                        parent2 = Selection(pop, rand.Next(0, totalFitness + 1));

                    //Crossover
                    children = CrossOver(parent1, parent2, N);
                    foreach (Node child in children)
                    {
                        n = child;
                        r = rand.Next(100);
                        if (r > 80)
                        {
                            //if (r > 90)
                                //n = GetRandomNode(N); //Complete Mutation
                            //else n = Mutation(child);  //Partial Mutation                        
                            n = Mutation(child); //Partial Mutation
                        }
                        newPop.Add(n);                       
                    }
                }
                pop = newPop;
                pop.Sort(sorter);
                s.SearchCosts += pop.Count;
                n = pop[0];
            }

            s.RunTime = DateTime.Now - start;
            s.Answer = n;
            return s;
        }                  

        private static Node Selection(List<Node> population, int r)
        {
            int sum = 0;
            int idx;
            for (idx = population.Count - 1; idx > 0 && sum < r; idx--)
                sum += max_fitness - population[idx].Value;
            return population[idx];    
        }

        private static Node Mutation(Node n)
        {
            List<Node> neighbors = GetNeighbors(n);                        
            return neighbors[random.Next(neighbors.Count)];
        }

        private static Node[] CrossOver(Node parent1, Node parent2, int N)
        {                        
            int crossoverPoint = random.Next(1, N - 1);
            Node child1 = parent1.Copy();
            Node child2 = parent2.Copy();                        
            int i = crossoverPoint;
            for (; i < N; i++)
            {
                child1.Positions[i] = parent2.Positions[i];
                child2.Positions[i] = parent1.Positions[i];
            }
            return new Node[2] { child1, child2 };
        }

        /// <summary>
        /// Gets the neihboring nodes of the parameter node. Only 1 queen per column.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static List<Node> GetNeighbors(Node n)
        {
            int N = n.Positions.Length;
            Node neighbor;
            List<Node> list = new List<Node>();
            for (int i = 0; i < N; i++)
            {
                int j = n.Positions[i];
                for (int y = j - 1; y > 0; y--)
                {
                    neighbor = n.Copy();                    
                    neighbor.Positions[i] = y;                                                           
                    list.Add(neighbor);
                }
                for (int y = j + 1; y < N; y++)
                {
                    neighbor = n.Copy();                    
                    neighbor.Positions[i] = y;
                    list.Add(neighbor);
                }
            }
            return list;
        }
    }
}
