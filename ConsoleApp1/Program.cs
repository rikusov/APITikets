using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsMoney
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = new APITikets.APITiketsMoney();

            decimal b = a.GetTikets("KUF",new DateTime(2021, 4, 5), new DateTime(2021, 4, 16));

            Console.WriteLine(b + "");
            Console.Read();
            
        }
    }
}
