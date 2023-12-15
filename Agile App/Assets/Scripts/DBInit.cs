using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class DBInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //creates a table
        string dbName = Path.Combine(Application.persistentDataPath, "settings.db");
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DROP TABLE IF EXISTS test; CREATE TABLE IF NOT EXISTS test(key VARCHAR(20), value INT);";
                command.ExecuteNonQuery();
            }
            connection.Close();
            Debug.Log("Database Created");

        }

        //add a dummy value
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO test(key,value) VALUES ('" + "Colourblind" + "', '" + 0 + "');INSERT INTO test(key,value) VALUES ('" + "Bigger Text" + "', '" + 1 + "');";
                command.ExecuteNonQuery();
            }

            connection.Close();
            Debug.Log("Dummy values Created");
        }
    }


}
