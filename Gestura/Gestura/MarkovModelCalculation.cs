#region Library Files
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Gestura
{
    class MarkovModelCalculation
    {
        #region Variables and Initializations
        private double[,] A = new double[8, 8];
        private double[,] B = new double[8, 8];
        private double[,] PI = new double[1, 8];
        private DataProcess dataProcess = new DataProcess();
        #endregion

        #region Constructor
        /*public MarkovModelCalculation()
        {
            dataProcess.readHMMData(ref A, "A");
            dataProcess.readHMMData(ref B, "B");
            dataProcess.readHMMData(ref PI, "PI");
        }*/
        #endregion

        #region HMM Data Functions
        //initialising the arrays for HMM data for processing
        public void readHMMData(int num)
        {
            String name1 = "A" ;
            String name2 = "B" ;
            dataProcess.readHMMData(ref A, name1, num);
            dataProcess.readHMMData(ref B, name2, num);
            dataProcess.readHMMData(ref PI, "PI",-1);
        }

        private void deleteHMMData(int num)
        {
            String name1 = "A" + num;
            String name2 = "B" + num;
            dataProcess.deleteHMMData(name1);
            dataProcess.deleteHMMData(name2);
        }

        //updating values of the A, B, PI into the database
        private void updateHMMData(int num)
        {
            String name1 = "A";
            String name2 = "B";
            dataProcess.updateHMMData(A, name1, num);
            dataProcess.updateHMMData(B, name2, num);
        }
        #endregion

        #region Data Functions

        /*public void HMMAlgorithms()
        {
            int[] obs = { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2 };
            // Training Set
            int[][] obs_learn = new int[][]
            {       //left
                new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2 },
                new int[] { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 2 },
                new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 7, 7, 2 },
                new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 2 },
                    //right
                new int[] { 2, 2, 5, 5, 4, 4, 4, 4, 5, 5, 4, 4, 4 },
                new int[] { 6, 6, 4, 4, 4, 4, 4, 4, 4, 4, 2, 2, 6 },
                new int[] { 2, 2, 2, 2, 4, 4, 4, 4, 4, 4, 4, 4, 4 },
                new int[] { 2, 2, 6, 6, 4, 4, 4, 4, 4, 4, 4, 4, 2 },
                    //up
                new int[] { 2, 2, 5, 5, 6, 6, 6, 6, 7, 7, 6, 6, 7 },
                new int[] { 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 2 },
                new int[] { 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 0, 0, 0 },
                new int[] { 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 2 },
                    //down
                new int[] { 4, 4, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 },
                new int[] { 4, 4, 2, 2, 2, 2, 2, 2, 1, 1, 2, 2, 2 },
                new int[] { 4, 4, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 2 },
                new int[] { 4, 4, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 4 },
                    //semi-clock
                new int[] { 6, 6, 5, 5, 4, 4, 4, 4, 3, 3, 3, 3, 2 },
                new int[] { 6, 6, 5, 5, 4, 4, 4, 4, 3, 3, 3, 3, 3 },
                new int[] { 6, 6, 5, 5, 4, 4, 3, 3, 3, 3, 2, 2, 2 },
                new int[] { 6, 6, 6, 6, 5, 5, 4, 4, 3, 3, 3, 3, 2 },
                    //semi-anti
                new int[] { 7, 7, 7, 7, 7, 7, 1, 1, 0, 0, 1, 1, 1 },
                new int[] { 7, 7, 7, 7, 0, 0, 0, 0, 0, 0, 1, 1, 2 },
                new int[] { 6, 6, 7, 7, 0, 0, 0, 0, 0, 0, 1, 1, 2 },
                new int[] { 7, 7, 7, 7, 0, 0, 7, 7, 1, 1, 1, 1, 2 },
                    //circular-clock
                new int[] { 6, 6, 6, 6, 4, 4, 4, 4, 2, 2, 1, 1, 0 },
                new int[] { 6, 6, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1, 0 },
                new int[] { 6, 6, 5, 5, 4, 4, 2, 2, 1, 1, 0, 0, 6 },
                new int[] { 6, 6, 5, 5, 3, 3, 2, 2, 0, 0, 0, 0, 7 },
                    //circular-anti
                new int[] { 7, 7, 7, 7, 0, 0, 0, 0, 2, 2, 4, 4, 4 },
                new int[] { 7, 7, 0, 0, 1, 1, 3, 3, 3, 3, 4, 4, 6 },
                new int[] { 6, 6, 0, 0, 0, 0, 1, 1, 2, 2, 4, 4, 5 },
                new int[] { 6, 6, 7, 7, 0, 0, 0, 0, 2, 2, 4, 4, 4 },
            };

            int[][] obs_seq = new int[4][]; // sub-set

            for (int i = 0; i < 8; i++) 
            {
                for (int j = 0; j < 4; j++)
                    obs_seq[j] = obs_learn[i * 4 + j];

                //reading A, B, PI for processing
                readHMMData(i + 1);

                //double prob;

                for (int k = 0; k < 10; k++)
                    //Learn(obs_seq, 0, 0.001,i+1);

                //delete the previous database entries of A, B
                deleteHMMData(i + 1);

                //update the new dvalues of A, B to database
                updateHMMData(i + 1);

                int rows, colsA,colsB;
                if (i == 0)
                    rows = 2;
                else
                    rows = i + 1;

                colsA = rows;
                colsB = 8;

                Console.WriteLine("For i: " + i);
                Console.WriteLine("Value of A: ");
                for (int k = 0; k < rows; k++)
                {
                    for (int l = 0; l < colsA; l++)
                        Console.Write(A[k,l] + " ");
                    Console.WriteLine("");
                }
                Console.WriteLine("Value of B: ");
                for (int k = 0; k < rows; k++)
                {
                    for (int l = 0; l < colsB; l++)
                        Console.Write(B[k, l] + " ");
                    Console.WriteLine("");
                }
            }
            
        }*/

        /// <summary>
        ///  //Viterbi And Baum-Welch algo
        /// </summary>
        /*public double Evaluate(int[] obs)
        {
            bool logarithm = false;

            if (obs.Length == 0)
                return 0.0;

            // Forward algorithm
            double likelihood = 0;
            double[] coefficients;

            // Compute forward probabilities
            forward(obs, out coefficients);

            for (int i = 0; i < coefficients.Length; i++)
                likelihood += Math.Log(coefficients[i]);

            // Return the sequence probability
            return logarithm ? likelihood : Math.Exp(likelihood);
        }*/ //old
        public double Evaluate(int[] observations,int label)
        {
            return Evaluate(observations, label,false);
        }

        public double Evaluate(int[] observations, int label, bool logarithm)
        {
            /*if (observations == null)
                throw new ArgumentNullException("observations");

            if (observations.Length == 0)
                return 0.0;*/
            
            // Forward algorithm
            double likelihood = 0.0;
            double[] coefficients;

            // Compute forward probabilities
            forward(observations, out coefficients, label);

            for (int i = 0; i < coefficients.Length; i++)
                likelihood += Math.Log(coefficients[i]);

            // Return the sequence probability
            return logarithm ? likelihood : Math.Exp(likelihood);
        }

        private double[,] forward(int[] observations, out double[] c,int label)
        {
            int T = observations.Length;
            //double[] pi = Probabilities;
            //double[,] A = Transitions;
            readHMMData(label);

            int rows, colsA,colsB;
                if (label == 1)
                    rows = 2;
                else
                    rows = label;

                colsA = rows;
                colsB = 8;

            Console.WriteLine("\n\nFor label: " + label);
            Console.WriteLine("Value of A: ");
            for (int k = 0; k < rows; k++)
            {
                for (int l = 0; l < colsA; l++)
                    Console.Write(A[k, l] + " ");
                Console.WriteLine("");
            }
            Console.WriteLine("Value of B: ");
            for (int k = 0; k < rows; k++)
            {
                for (int l = 0; l < colsB; l++)
                    Console.Write(B[k, l] + " ");
                Console.WriteLine("");
            }

            Console.WriteLine("Value of PI: ");
            for (int k = 0; k < 8; k++)
                Console.Write(PI[0, k] + " ");

            int States = label;
            if (label == 1)
                States = 2;
            

            double[,] fwd = new double[T, States];
            c = new double[T];

            // 1. Initialization
            for (int i = 0; i < States; i++)
                c[0] += fwd[0, i] = PI[0,i] * B[i, observations[0]-1];

            if (c[0] != 0) // Scaling
            {
                for (int i = 0; i < States; i++)
                    fwd[0, i] = fwd[0, i] / c[0];
            }

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                for (int i = 0; i < States; i++)
                {
                    double p = B[i, observations[t]-1];

                    double sum = 0.0;
                    for (int j = 0; j < States; j++)
                        sum += fwd[t - 1, j] * A[j, i];
                    fwd[t, i] = sum * p;

                    c[t] += fwd[t, i]; // scaling coefficient
                }

                if (c[t] != 0) // Scaling
                {
                    for (int i = 0; i < States; i++)
                        fwd[t, i] = fwd[t, i] / c[t];
                }
            }
            return fwd;
        }

        /*public int[] Decode(int[] observations, out double probability)
        {
            // Viterbi algorithm.

            int T = observations.Length;
            int states = 8;
            int minState;
            double minWeight;
            double weight;

            int[,] s = new int[states, T];
            double[,] a = new double[states, T];


            // Base
            for (int i = 0; i < states; i++)
            {
                a[i, 0] = (-1.0 * System.Math.Log(PI[0, i])) - System.Math.Log(B[i, observations[0] - 1]);
            }

            // Induction
            for (int t = 1; t < T; t++)
            {
                for (int j = 0; j < states; j++)
                {
                    minState = 0;
                    minWeight = a[0, t - 1] - System.Math.Log(A[0, j]);

                    for (int i = 1; i < states; i++)
                    {
                        weight = a[i, t - 1] - System.Math.Log(A[i, j]);

                        if (weight < minWeight)
                        {
                            minState = i;
                            minWeight = weight;
                        }
                    }

                    a[j, t] = minWeight - System.Math.Log(B[j, observations[t] - 1]);
                    s[j, t] = minState;
                }
            }


            // Find minimum value for time T-1
            minState = 0;
            minWeight = a[0, T - 1];

            for (int i = 1; i < states; i++)
            {
                if (a[i, T - 1] < minWeight)
                {
                    minState = i;
                    minWeight = a[i, T - 1];
                }
            }

            // Trackback
            int[] path = new int[T];
            path[T - 1] = minState;

            for (int t = T - 2; t >= 0; t--)
                path[t] = s[path[t + 1], t + 1];


            probability = System.Math.Exp(-minWeight);
            return path;
        }*/ // old decode

        public int[] Decode(int[] observations, out double probability, int StateNo, int label)
        {
            // Viterbi algorithm.
            readHMMData(label + 1);
            int T = observations.Length;
            int states = label + 1;
            if (states == 1)
                states = 2;
            int minState;
            double minWeight;
            double weight;

            int[,] s = new int[states, T];
            double[,] a = new double[states, T];

            if (label + 1 == 1)
                label = 2;
            Console.WriteLine("Label: " + (label + 1) + " A: ");
            for (int i = 0; i < label + 1; i++)
            {
                for (int j = 0; j < label + 1; j++)
                    Console.Write(A[i, j] + " ");
                Console.WriteLine("");
            }
            Console.WriteLine("Label: " + (label + 1) + " B: ");
            for (int i = 0; i < label + 1; i++)
            {
                for (int j = 0; j < 8; j++)
                    Console.Write(B[i, j] + " ");
                Console.WriteLine("");
            }

            // Base
            for (int i = 0; i < states; i++)
            {
                a[i, 0] = (-1.0 * System.Math.Log(PI[0, i])) - System.Math.Log(B[i, observations[0] - StateNo]);
            }

            // Induction
            for (int t = 1; t < T; t++)
            {
                for (int j = 0; j < states; j++)
                {
                    minState = 0;
                    minWeight = a[0, t - 1] - System.Math.Log(A[0, j]);
                    for (int i = 1; i < states; i++)
                    {
                        weight = a[i, t - 1] - System.Math.Log(A[i, j]);
                        if (weight < minWeight)
                        {
                            minState = i;
                            minWeight = weight;
                        }
                    }
                    a[j, t] = minWeight - System.Math.Log(B[j, observations[t] - StateNo]);
                    s[j, t] = minState;
                }

            }

            // Find minimum value for time T-1
            minState = 0;
            minWeight = a[0, T - 1];

            for (int i = 1; i < states; i++)
            {
                if (a[i, T - 1] < minWeight)
                {
                    minState = i;
                    minWeight = a[i, T - 1];
                }
            }

            // Trackback
            int[] path = new int[T];
            path[T - 1] = minState;

            for (int t = T - 2; t >= 0; t--)
                path[t] = s[path[t + 1], t + 1];

            probability = System.Math.Exp(-minWeight);

            return path;
        }

        /*public double Learn(int[][] observations, int iterations, double tolerance,int label)
        {
            int N = observations.Length, States = label, Symbols = 8;
            int currentIteration = 1;
            bool stop = false;

            if (States == 1)
                States = 2;

            // Initialization
            double[][, ,] epsilon = new double[N][, ,]; // also referred as ksi or psi
            double[][,] gamma = new double[N][,];

            for (int i = 0; i < N; i++)
            {
                int T = observations[i].Length;
                epsilon[i] = new double[T, States, States];
                gamma[i] = new double[T, States];
            }

            // Calculate initial model log-likelihood
            double oldLikelihood = Double.MinValue;
            double newLikelihood = 0;

            do // Until convergence or max iterations is reached
            {
                // For each sequence in the observations input
                for (int i = 0; i < N; i++)
                {
                    var sequence = observations[i];
                    int T = sequence.Length;
                    double[] scaling;

                    // 1st step - Calculating the forward probability and the
                    //            backward probability for each HMM state.
                    double[,] fwd = forward(observations[i], out scaling, 0,States);
                    double[,] bwd = backward(observations[i], scaling,States);

                    // 2nd step - Determining the frequency of the transition-emission pair values
                    //            and dividing it by the probability of the entire string.

                    // Calculate gamma values for next computations
                    for (int t = 0; t < T; t++)
                    {
                        double s = 0;

                        for (int k = 0; k < States; k++)
                            s += gamma[i][t, k] = fwd[t, k] * bwd[t, k];

                        if (s != 0) // Scaling
                        {
                            for (int k = 0; k < States; k++)
                                gamma[i][t, k] /= s;
                        }
                    }

                    // Calculate epsilon values for next computations
                    for (int t = 0; t < T - 1; t++)
                    {
                        double s = 0;
                        for (int k = 0; k < States; k++)
                            for (int l = 0; l < States; l++)
                                s += epsilon[i][t, k, l] = fwd[t, k] * A[k, l] * bwd[t + 1, l] * B[l, sequence[t + 1]];

                        if (s != 0) // Scaling
                        {
                            for (int k = 0; k < States; k++)
                                for (int l = 0; l < States; l++)
                                    epsilon[i][t, k, l] /= s;
                        }
                    }

                    // Compute log-likelihood for the given sequence
                    for (int t = 0; t < scaling.Length; t++)
                        newLikelihood += Math.Log(scaling[t]);

                }

                // Average the likelihood for all sequences
                newLikelihood /= observations.Length;

                // Check if the model has converged or we should stop
                if (checkConvergence(oldLikelihood, newLikelihood,
                    currentIteration, iterations, tolerance))
                {
                    stop = true;
                }

                else
                {
                    // 3. Continue with parameter re-estimation
                    currentIteration++;
                    oldLikelihood = newLikelihood;
                    newLikelihood = 0.0;

                    // 3.1 Re-estimation of initial state probabilities 
                    for (int k = 0; k < States; k++)
                    {
                        double sum = 0;
                        for (int i = 0; i < N; i++)
                            sum += gamma[i][0, k];
                        PI[0, k] = sum / N;
                    }

                    // 3.2 Re-estimation of transition probabilities 
                    for (int i = 0; i < States; i++)
                    {
                        for (int j = 0; j < States; j++)
                        {
                            double den = 0, num = 0;
                            for (int k = 0; k < N; k++)
                            {
                                int T = observations[k].Length;
                                for (int l = 0; l < T - 1; l++)
                                    num += epsilon[k][l, i, j];

                                for (int l = 0; l < T - 1; l++)
                                    den += gamma[k][l, i];
                            }
                            A[i, j] = (den != 0) ? num / den : 0.0;
                        }
                    }

                    // 3.3 Re-estimation of emission probabilities
                    for (int i = 0; i < States; i++)
                    {
                        for (int j = 0; j < Symbols; j++)
                        {
                            double den = 0, num = 0;
                            for (int k = 0; k < N; k++)
                            {
                                int T = observations[k].Length;
                                for (int l = 0; l < T; l++)
                                {
                                    if (observations[k][l] == j)
                                        num += gamma[k][l, i];
                                }
                                for (int l = 0; l < T; l++)
                                    den += gamma[k][l, i];
                            }

                            // avoid locking a parameter in zero.
                            B[i, j] = (num == 0) ? 1e-10 : num / den;
                        }

                    }

                }

            } while (!stop);

            // Returns the model average log-likelihood
            return newLikelihood;
        }*/

        

        /*public int Compute(int[] sequence, out double likelihood)
        {
            int label = 0;
            double p=0.0;
            likelihood = 0.0;

            // For every model in the set,
            for (int i = 0; i < 8; i++)
            {
                // Evaluate the probability for the given sequence
                //double p = Evaluate(sequence);


                // And select the one which produces the highest probability
                if (p > likelihood)
                {
                    label = i;
                    likelihood = p;
                }
            }

            // Returns the index of the most likely model.
            return label;
        }*/

        /*private double[,] forward(int[] observations, out double[] c)
        {
            int T = observations.Length, States = label;
            double[,] fwd = new double[T, States];
            c = new double[T];

            // 1. Initialization
            for (int i = 0; i < States; i++)
                c[0] += fwd[0, i] = PI[0, i] * B[i, observations[0] - StateNo];

            if (c[0] != 0) // Scaling
            {
                for (int i = 0; i < States; i++)
                    fwd[0, i] = fwd[0, i] / c[0];

            }

            // 2. Induction
            for (int t = 1; t < T; t++)
            {
                for (int i = 0; i < States; i++)
                {
                    double p = B[i, observations[t] - StateNo];
                    double sum = 0.0;
                    for (int j = 0; j < States; j++)
                        sum += fwd[t - 1, j] * A[j, i];
                    fwd[t, i] = sum * p;
                    c[t] += fwd[t, i]; // scaling coefficient
                }

                if (c[t] != 0) // Scaling
                {
                    for (int i = 0; i < States; i++)
                        fwd[t, i] = fwd[t, i] / c[t];

                }
            }

            return fwd;
        }*/ //old

        private double[,] backward(int[] observations, double[] c,int label)
        {
            int T = observations.Length, States = label;

            //readHMMData();

            double[,] bwd = new double[T, States];

            // For backward variables, we use the same scale factors
            //   for each time t as were used for forward variables.

            // 1. Initialization
            for (int i = 0; i < States; i++)
                bwd[T - 1, i] = 1.0 / c[T - 1];

            // 2. Induction
            for (int t = T - 2; t >= 0; t--)
            {
                for (int i = 0; i < States; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < States; j++)
                        sum += A[i, j] * B[j, observations[t + 1]] * bwd[t + 1, j];
                    bwd[t, i] += sum / c[t];
                }

            }

            return bwd;
        }

        private bool checkConvergence(double oldLikelihood, double newLikelihood, int currentIteration, int maxIterations, double tolerance)
        {
            // Update and verify stop criteria
            if (tolerance > 0)
            {
                // Stopping criteria is likelihood convergence
                if (Math.Abs(oldLikelihood - newLikelihood) <= tolerance)
                    return true;

                if (maxIterations > 0)
                {
                    // Maximum iterations should also be respected
                    if (currentIteration >= maxIterations)
                        return true;
                }
            }
            else
            {
                // Stopping criteria is number of iterations
                if (currentIteration == maxIterations)
                    return true;
            }

            // Check if we have reached an invalid state
            if (Double.IsNaN(newLikelihood) || Double.IsInfinity(newLikelihood))
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
