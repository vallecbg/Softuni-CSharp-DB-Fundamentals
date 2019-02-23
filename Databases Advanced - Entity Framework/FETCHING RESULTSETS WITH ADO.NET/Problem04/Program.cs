using AdoNetExercise;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Problem04
{
    class Program
    {
        static void Main(string[] args)
        {
            string minionName = string.Empty;
            int minionAge = 0;
            string minionTown = string.Empty;
            string villainName = string.Empty;

            while (minionName.Equals(string.Empty))
            {
                Console.Write("Enter minion name: ");
                minionName = Console.ReadLine().Trim();
            }

            //The user inputs an age and if it is correct it is parsed if not he is asked again until a valid input is entered.
            Console.Write("Enter minion age: ");
            while (!int.TryParse(Console.ReadLine(), out minionAge) || minionAge < 1)
            {
                Console.WriteLine("Age must be a positive number greather than 0 !");
                Console.Write("Enter minion age: ");
            }

            while (minionTown.Equals(string.Empty))
            {
                Console.Write("Enter a town name: ");
                minionTown = Console.ReadLine().Trim();
            }

            while (villainName.Equals(string.Empty))
            {
                Console.Write("Enter a villain name: ");
                villainName = Console.ReadLine().Trim();
            }

            bool townExists = false;

            bool villainExists = false;

            SqlConnection connection = new SqlConnection(Configuration.ConnectionString);

            connection.Open();
            using (connection)
            {
                string townCheckQuery = $@"SELECT COUNT(*) FROM Towns WHERE Name = @MinionTown";

                SqlCommand cmdTown = new SqlCommand(townCheckQuery, connection);
                cmdTown.Parameters.Add(new SqlParameter("MinionTown", minionTown));

                if ((int)cmdTown.ExecuteScalar() > 0)
                {
                    townExists = true;
                }

                string villainCheckQuery = $@"SELECT COUNT(*) FROM Villains WHERE Name = @VillainName";

                SqlCommand cmdVillain = new SqlCommand(villainCheckQuery, connection);
                cmdVillain.Parameters.Add(new SqlParameter("VillainName", villainName));

                if ((int)cmdVillain.ExecuteScalar() > 0)
                {
                    villainExists = true;
                }

                //Its better if we also had the country but it is not included in this exercise. Here we add the town if it does not exist.
                if (!townExists)
                {
                    SqlCommand insertTown = new SqlCommand($@"INSERT Towns(Name)VALUES(@MinionTown)", connection);
                    insertTown.Parameters.Add(new SqlParameter("Name", minionTown));

                    insertTown.ExecuteNonQuery();
                    Console.WriteLine($"Town {minionTown} was added to the database.");
                }

                //Here we add the villain if he does not exist.
                if (!villainExists)
                {
                    SqlCommand insertVillain = new SqlCommand($@"INSERT Villains(Name, EvilnessFactor)VALUES(@VillainName, 'evil')", connection);
                    insertVillain.Parameters.Add(new SqlParameter("Name", villainName));

                    insertVillain.ExecuteNonQuery();
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                //We add the Minion
                SqlCommand insertMinion = new SqlCommand($@"
INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@villainId, @minionId)
INSERT INTO Villains (Name, EvilnessFactorId)  VALUES (@villainName, 4)
INSERT INTO Minions (Name, Age, TownId) VALUES (@nam, @age, @townId)", connection);
                insertMinion.Parameters.Add(new SqlParameter("villainId", 10));
                insertMinion.Parameters.Add(new SqlParameter("minionId", 10));
                insertMinion.Parameters.Add(new SqlParameter("villainName", villainName));
                insertMinion.Parameters.Add(new SqlParameter("nam", minionName));
                insertMinion.Parameters.Add(new SqlParameter("age", minionAge));
                insertMinion.Parameters.Add(new SqlParameter("townId", 1));

                insertMinion.ExecuteNonQuery();

                //We connect the villain with his minion.
                SqlCommand insertInMtMTable = new SqlCommand($@"INSERT MinionsVillains(MinionId, VillainId)
                SELECT TOP (1) m.Id, v.Id FROM Minions AS m, Villains AS v WHERE m.Name = @MinionName AND v.Name = @VillainName ORDER BY m.Id DESC, v.Id DESC", connection);
                insertInMtMTable.Parameters.Add(new SqlParameter("MinionName", minionName));
                insertInMtMTable.Parameters.Add(new SqlParameter("VillainName", villainName));

                insertInMtMTable.ExecuteNonQuery();

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }

        }
    }
}
