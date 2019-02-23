using AdoNetExercise;
using System;
using System.Data.SqlClient;

namespace Problem09
{
    class Program
    {
        static void Main(string[] args)
        {
            int minionId = -1;

            Console.Write("Enter minion id: ");
            while (!int.TryParse(Console.ReadLine(), out minionId) || minionId < 1)
            {
                Console.WriteLine("Id must be a positive number greather than 0 !");
                Console.Write("Enter minion Id: ");
            }


            SqlConnection connection = new SqlConnection(Configuration.ConnectionString);


            connection.Open();


            using (connection)
            {
                string query = @"EXEC usp_GetOlder @MinionId ";

                SqlCommand cmd = new SqlCommand(query, connection);

                cmd.Parameters.Add(new SqlParameter("MinionId", minionId));


                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine(" Minion name | Age ");
                    Console.WriteLine("-------------|-----");
                    Console.WriteLine($"{reader[0],12} |{reader[1],3}");
                }
                else
                {
                    Console.WriteLine($"No minion with id: {minionId} found.");
                }
            }
        }
    }
}
