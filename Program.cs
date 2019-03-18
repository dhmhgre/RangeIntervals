﻿/*
 GNU AFFERO GENERAL PUBLIC LICENSE
     Version 3, 19 November 2007
Copyright(c) [2017] [Grigoris Dimitroulakos] 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace RangeIntervals {


    public class MyRange : Range<Int32> {

        /// <summary>
        /// MANDATORY Parameterless Constructor
        /// Initializes a new instance of the <see cref="MyRange"/> class.
        /// </summary>
        public MyRange() {
        }

        public MyRange(Int32 x, Int32 y) : base(x, y) { }

        /// <summary>
        /// MANDATORY OVERRIDE
        /// Returns the next element of x
        /// </summary>
        /// <param name="x">An element in the T space</param>
        /// <returns></returns>
        public override int Next(int x) {
            return x + 1;
        }

        /// <summary>
        /// MANDATORY OVERRIDE
        /// Returns the previous element of x
        /// </summary>
        /// <param name="x">An element in the T space</param>
        /// <returns></returns>
        public override int Prev(int x) {
            return x - 1;
        }
    }

    class Program {
        static void Main(string[] args) {
            RangeSetO<MyRange, int> set = new RangeSetO<MyRange, int>();
            MyRange r2 = new MyRange(5, 6);
            MyRange r1 = new MyRange(6, 7);
            MyRange r3 = new MyRange(50, 60);
            r3.IsInRange(55);
            MyRange r4 = new MyRange(22, 28);
            MyRange r5 = new MyRange(1, 26);
            set.AddRange(r2);
            Console.WriteLine(set.ToString());
            set.AddRange(r1);
            Console.WriteLine(set.ToString());
            set.IsInSet(9);
            /*set.AddRange(r1);
            Console.WriteLine(set.ToString());
            set.AddRange(r2);
            Console.WriteLine(set.ToString());
            set.AddRange(r3);
            Console.WriteLine(set.ToString());
            set.AddRange(r4);
            Console.WriteLine(set.ToString());
            set.AddRange(r5);
            Console.WriteLine(set.ToString());*/

        }
    }
}
