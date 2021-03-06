﻿using System;

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


        public int[] Viterbi(double[,] p_state_tran, double[,] p_hidden_observe, double[] pi, int[] state_sequence)
        {
            //p_state_tran = new double[,] { 
            //    {0.333,0.333,0.333},
            //    {0.333,0.333,0.333},
            //    {0.333,0.333,0.333}
            //};
            //p_hidden_observe = new double[,] { 
            //    {0.5,0.5},
            //{0.75,0.25},
            //{0.25,0.75}
            //};
            //pi = new double[] { 0.333, 0.333, 0.333 };
            //state_sequence = new int[] { 0, 0, 0, 0, 1, 0, 1, 1, 1, 1 };
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
                    var day1 = day;
                    var j1 = j;
                    var arr = Enumerable.Range(1, stateCount - 1).Select(State => new
                    {
                        State = State,
                        probably = delta[State, day1 - 1] * p_state_tran[State, j1]
                    });
                    var max = arr.AsParallel().OrderByDescending(ip => ip.probably).First();
                    delta[j, day] = max.probably * p_hidden_observe[j, state_sequence[day]];
                    psi[j, day] = max.State;
                    Console.WriteLine("max p {0} max link {1}", max.probably, max.State);
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
    }
}
