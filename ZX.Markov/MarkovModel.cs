using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZX.Markov
{
    public enum weather
    {
        Sunny, Cloudy, Rainy
    }
    public enum plantstate : int
    {
        Dry = 0, Dryish, Damp, Soggy
    }
    public class MarkovModel
    {

        /// <summary>
        /// 隐藏状态转换矩阵
        /// </summary>
        public double[,] p_state_tran = new double[,]
        {
            {0.500,0.375,0.125},
            {0.250,0.125,0.625},
            {0.250,0.375,0.375}
        };
        /// <summary>
        /// 隐藏状态和显示状态混淆矩阵
        /// </summary>
        public double[,] p_hidden_observe = new double[,] 
        { 
            {0.60,0.20,0.15,0.05}, 
            {0.25,0.25,0.25,0.25}, 
            {0.05,0.10,0.35,0.50},  
        };
        /// <summary>
        /// 观察到的状态
        /// </summary>
        public int[] state_sequence = new int[] { (int)plantstate.Dry, (int)plantstate.Damp, (int)plantstate.Soggy };
        public double[] pi = new double[] { 0.63, 0.17, 0.20 };
        public double Forward()
        {
            int stateCount = p_state_tran.GetLength(0);
            int T = state_sequence.Length;
            double[,] stateTimeProp = new double[stateCount, state_sequence.Length];
            for (int state = 0; state < stateCount; state++)
            {
                //第一天的状态转换
                //
                stateTimeProp[state, 0] = p_hidden_observe[state, state_sequence[0]] * pi[state];
            }
            for (int day = 1; day < T; day++)
            {
                for (int j = 0; j < stateCount; j++)
                {
                    double sum = 0;
                    for (int i = 0; i < stateCount; i++)
                    {
                        sum += stateTimeProp[i, day - 1] * p_state_tran[i, j];
                    }
                    stateTimeProp[j, day] = sum * p_hidden_observe[j, state_sequence[day]];
                    Console.WriteLine("day {0} state {1} p {2} ", day, j, stateTimeProp[j, day]);
                }
            }
            double p = 0;
            for (int i = 0; i < stateCount; i++)
            {
                p += stateTimeProp[i, T - 1];
            }
            return p;
        }


        public int[] Viterbi()
        {
            p_state_tran = new double[,] { 
                {0.333,0.333,0.333},
                {0.333,0.333,0.333},
                {0.333,0.333,0.333}
            };
            p_hidden_observe = new double[,] { 
                {0.5,0.5},
　　　　        {0.75,0.25},
　　　　        {0.25,0.75}
            };
            pi = new double[] { 0.333, 0.333, 0.333 };
            state_sequence = new int[] { 0, 0, 0, 0, 1, 0, 1, 1, 1, 1 };
            int stateCount = p_state_tran.GetLength(0);
            int T = state_sequence.Length;
            double[,] delta = new double[stateCount, T];
            int[] path = new int[T];
            int[,] psi = new int[stateCount, T];
            for (int state = 0; state < stateCount; state++)
            {
                //第一天的状态转换
                delta[state, 0] = p_hidden_observe[state, state_sequence[0]] * pi[state];
                psi[state, 0] = 0;
            }
            for (int day = 1; day < T; day++)
            {
                for (int j = 0; j < stateCount; j++)
                {
                    double max_p = delta[0, day - 1] * p_state_tran[0, j];
                    int max_link = 0;
                    for (int i = 1; i < stateCount; i++)
                    {
                        double v = delta[i, day - 1] * p_state_tran[i, j];
                        if (v > max_p)
                        {
                            max_p = v;
                            max_link = i;
                        }
                    }
                    delta[j, day] = max_p * p_hidden_observe[j, state_sequence[day]];
                    psi[j, day] = max_link;
                    Console.WriteLine("max p {0} max link {1}", max_p, (weather)max_link);
                }
            }
            path[T - 1] = 1;
            double p = 0;
            for (int i = 0; i < stateCount; i++)
            {
                if (delta[i, T - 1] > p)
                {
                    p = delta[i, T - 1];
                    path[T - 1] = i;
                }
            }
            for (int i = T - 2; i >= 0; i--)
            {
                path[i] = psi[path[i + 1], i + 1];
            }
            return path;
        }

        double[,] Alpha_t_s = new double[0, 0];
        double[,] aij = new double[0, 0];
        double[,] bjk = new double[0, 0];
        int[] k = new int[0];
        int N = 10;
        int T = 10;

        public void ForwardBack()
        {
        }
        public double bete_ti(int t, int T, int i)
        {
            if (t == T)
                return 1;
            return sigma_i(N, j => { return aij[i, j] * bjk[j, t + 1] * bete_ti(t + 1, T, j); });
        }
        public double gamma_ti(int t, int i)
        {
            return (alpha_ti(t, i) * bete_ti(t, T, i)) / sigma_i(N, k => { return alpha_ti(t, k) * bete_ti(t, T, k); });
        }
        public double epsilon_tij(int t, int i, int j)
        {
            Func<int, int, int, double> func = (tt, ii, jj) =>
            {
                return alpha_ti(tt, ii) * aij[ii, jj] * bjk[jj, k[tt + 1]] * bete_ti(tt + 1, T, jj);
            };
            return func(t, i, j) / sigma_ij(N, N, (ii, jj) => { return func(t, ii, jj); });
        }
        public double alpha_ti(int t, int j)
        {
            if (t == 0)
                return pi[j] * bjk[j, k[0]];
            else
                return sigma_i(N, i => { return alpha_ti(t - 1, i); }) * bjk[j, k[t - 1]]; ;
        }
        public double sigma_i(int i, Func<int, double> terms)
        {
            return 0;
        }
        public double sigma_ij(int i, int j, Func<int, int, double> terms)
        {
            return 0;
        }
        public double sigma_ijk(int i, int j, int k, Func<int, int, int, double> terms)
        {
            return 0;
        }
    }
}
