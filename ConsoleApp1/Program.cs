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
            var a = new APITiketsMoney.APITiketsMoney();

            decimal b = a.GetTikets("MOW",new DateTime(2021, 4, 15), new DateTime(2021, 4, 21));

            Console.WriteLine(b + "");
            Console.Read();
            
        }
    }
}
