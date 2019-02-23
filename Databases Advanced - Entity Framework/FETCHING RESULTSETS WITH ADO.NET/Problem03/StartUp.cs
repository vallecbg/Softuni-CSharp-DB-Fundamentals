using AdoNetExercise;
using System;
using System.Data.SqlClient;

namespace Problem03
{
    public class StartUp
    {
        public static void Main()
        {
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var villainId = int.Parse(Console.ReadLine());
                var villainCmd = @"SELECT Name FROM Villains WHERE Id = @Id";
                var minionsCmd = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @Id
                                ORDER BY m.Name";

                PrintVillainName(connection, villainId, villainCmd);
                PrintMinionsServingToVillain(connection, villainId, minionsCmd);
            }
        }

        public static void PrintVillainName(SqlConnection connection, int villainId, string cmdText)
        {
            using (var command = new SqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@Id", villainId);
                var villainName = command.ExecuteScalar();

                if (villainName == null)
                {
                    Console.WriteLine("No villain with ID 10 exists in the database.");
                    Environment.Exit(Environment.ExitCode);
                }
                else
                {
                    Console.WriteLine($"Villain: {villainName}");
                }
            }
        }


        public static void PrintMinionsServingToVillain(SqlConnection connection, int villainId, string cmdText)
        {
            using (var command = new SqlCommand(cmdText, connection))
            {
                command.Parameters.AddWithValue("@Id", villainId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        var minionNumber = 1;

                        while (reader.Read())
                        {
                            Console.WriteLine($"{minionNumber++}. {reader["Name"]} {reader["Age"]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("(no minions)");
                    }
                }
            }
        }
    }
}
