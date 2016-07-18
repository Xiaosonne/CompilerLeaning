using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZX.Markov
{
    class Program
    {
        static void Main(string[] args)
        {
            MarkovModel m = new MarkovModel();

            Console.WriteLine(m.Forward());
            m.Viterbi();
            Console.Read();
        }
    }
}
