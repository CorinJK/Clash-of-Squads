﻿using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace DevelopersHub.RealtimeNetworking.Server
{
    class Database
    {
        #region MySQL
        
        private static MySqlConnection _mysqlConnection;
        private const string _mysqlServer = "127.0.0.1";
        private const string _mysqlUsername = "root";
        private const string _mysqlPassword = "";
        private const string _mysqlDatabase = "clash_of_squads";

        public static MySqlConnection mysqlConnection
        {
            get
            {
                if (_mysqlConnection == null || _mysqlConnection.State == ConnectionState.Closed)
                {
                    try
                    {
                        _mysqlConnection = new MySqlConnection("SERVER=" + _mysqlServer + "; DATABASE=" + _mysqlDatabase + "; UID=" + _mysqlUsername + "; PASSWORD=" + _mysqlPassword + ";");
                        _mysqlConnection.Open();
                        Console.WriteLine("Connection established with MySQL database.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to connect the MySQL database.");
                    }
                }
                else if (_mysqlConnection.State == ConnectionState.Broken)
                {
                    try
                    {
                        _mysqlConnection.Close();
                        _mysqlConnection = new MySqlConnection("SERVER=" + _mysqlServer + "; DATABASE=" + _mysqlDatabase + "; UID=" + _mysqlUsername + "; PASSWORD=" + _mysqlPassword + ";");
                        _mysqlConnection.Open();
                        Console.WriteLine("Connection re-established with MySQL database.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to connect the MySQL database.");
                    }
                }
                return _mysqlConnection;
            }
        }

        public static void Demo_MySQL_1()
        {
            string query = String.Format("UPDATE table SET int_column = {0}, string_column = '{1}', datetime_column = NOW();", 123, "Hello World");
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public static void Demo_MySQL_2()
        {
            string query = String.Format("SELECT column1, column2 FROM table WHERE column3 = {0} ORDER BY column1 DESC;", 123);
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int column1 = int.Parse(reader["column1"].ToString());
                            string column2 = reader["column2"].ToString();
                        }
                    }
                }
            }
        }
        public static void AuthenticatePlayer(int id, string device)
        {
            string query = String.Format("SELECT id FROM accounts WHERE device_id = '{0}';", device);
            bool found = false;
            using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            long account_id = long.Parse(reader["id"].ToString());
                            AuthenticationResponse(id, account_id);
                            found = true;
                        }
                    }
                }
            }

            if (found == false)
            {
                query = String.Format("INSERT INTO accounts (device_id) VALUES('{0}');", device);
                using (MySqlCommand command = new MySqlCommand(query, mysqlConnection))
                {
                    command.ExecuteNonQuery();
                    long account_id = command.LastInsertedId;
                    AuthenticationResponse(id, account_id);
                }
            }
        }

        public static void AuthenticationResponse(int clientID, long accountID)
        {
            Sender.TCP_Send(clientID, 1, accountID);
        }
        #endregion
    }
}