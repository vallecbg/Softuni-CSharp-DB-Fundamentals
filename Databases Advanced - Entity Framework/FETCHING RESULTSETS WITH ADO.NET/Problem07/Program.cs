using AdoNetExercise;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Problem07
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(Configuration.ConnectionString);


            connection.Open();

            using (connection)
            {

                SqlCommand getNames = new SqlCommand(@"SELECT Name FROM Minions", connection);

                SqlDataReader reader = getNames.ExecuteReader();

                List<string> minionNames = new List<string>();

                while (reader.Read())
                {
                    minionNames.Add(reader[0].ToString());
                }

                reader.Close();

                if (minionNames.Count > 0)
                {
                    int limit = (int)(minionNames.Count / 2.00);

                    int last = minionNames.Count - 1;



                    for (int i = 0; i < limit; i++)
                    {
                        Console.WriteLine(minionNames[i]);
                        Console.WriteLine(minionNames[last - i]);
                    }
                    if (minionNames.Count % 2 != 0)
                    {
                        Console.WriteLine(minionNames[limit]);
                    }
                }
                else
                {
                    Console.WriteLine("You have no minions :(");
                }

            }
        }
    }
}
