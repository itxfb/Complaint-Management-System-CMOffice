﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.SimpleDB.Model;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace PITB.CMS_Common.Models.Custom
{

    public class Pair<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }



        public Pair(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
    }

    public class Pair<T1, T2, T3>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public T3 Item3 { get; set; }

        public Pair(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
    }


    public class Pair<T1, T2, T3, T4> 
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set;  }

        public T3 Item3 { get; set;  }
        public T4 Item4 { get; set; }


        public Pair(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }
    }

    public class Pair<T1, T2, T3, T4, T5>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }

        public T5 Item5 { get; set; }

        public Pair(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
        }
    }
}