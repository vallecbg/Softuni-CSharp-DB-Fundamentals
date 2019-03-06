using System;
using P03_SalesDatabase.Data;
using Microsoft.EntityFrameworkCore.Design;

namespace P03_SalesDatabase
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            using (SalesContext context = new SalesContext())
            {
            }
        }
    }
}
