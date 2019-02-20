using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using VoidDaysServerLibrary.Services;

namespace VoidDaysServerLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class VoidDaysLoginService : IVoidDaysLoginService
    {
        string _rootDb = "voiddaysroot";
        string _connectionString = "Server=localhost; Port=3306; Uid=crate;Database=voiddaysroot; Pwd=Sprint100;convert zero datetime=True;max pool size=200;sslmode=none;";
        UserService _userService;
        MySqlConnection _dbConn;
        public VoidDaysLoginService()
        {
            _userService = new UserService();
        }
        public void CreateUser(string username, string password)
        {
            try
            {
                Console.WriteLine("create user " + username + " " + password);
                _dbConn = new MySql.Data.MySqlClient.MySqlConnection(_connectionString);
                Console.WriteLine("connection " + _connectionString);
                MySqlTransaction transaction;

                MySqlCommand newUserCommand = _dbConn.CreateCommand();
                newUserCommand.CommandText = "create user @user identified by @password;";
                newUserCommand.Parameters.AddWithValue("@user", username);
                newUserCommand.Parameters.AddWithValue("@password", password);

                string passwordHash = _userService.EncryptPassword(password);

                MySqlCommand getSuffixCommand = _dbConn.CreateCommand();
                getSuffixCommand.CommandText = "select MAX(tablesuffix) FROM  " + _rootDb + ".users";


                try
                {
                    _dbConn.Open();
                }
                catch (Exception erro)
                {
                    Console.WriteLine("Could not connect");
                }

                string schemaPrefix = "voiddays_";
                int suffix = 0;

                newUserCommand.ExecuteNonQuery();
                var reader = getSuffixCommand.ExecuteReader();
                Console.WriteLine("before read ");
                if (reader.Read())
                {
                    try
                    {
                        suffix = reader.GetInt32(0);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                reader.Close();
                ++suffix;
                Console.WriteLine("suffix " + suffix);
                var schemaName = schemaPrefix + suffix;
                MySqlCommand createSchemaCommand = _dbConn.CreateCommand();
                createSchemaCommand.CommandText = "create schema " + schemaName;

                MySqlCommand dropSchemaCommand = _dbConn.CreateCommand();
                dropSchemaCommand.CommandText = "drop schema " + schemaName;

                MySqlCommand grantCommand = _dbConn.CreateCommand();
                grantCommand.CommandText = "GRANT ALL ON " + schemaName + ".* To @username";
                grantCommand.Parameters.AddWithValue("@username", username);

                MySqlCommand flushCommand = _dbConn.CreateCommand();
                flushCommand.CommandText = "Flush privileges";

                MySqlCommand insertUserTable = _dbConn.CreateCommand();
                insertUserTable.CommandText = "INSERT INTO `" + _rootDb + "`.`users` (`username`, `password`, `schemaname`, `tablesuffix`) VALUES (@username, @passwordHash, @schemaName, @suffix);";
                insertUserTable.Parameters.AddWithValue("@username", username);
                insertUserTable.Parameters.AddWithValue("@passwordHash", passwordHash);
                insertUserTable.Parameters.AddWithValue("@schemaName", schemaName);
                insertUserTable.Parameters.AddWithValue("@suffix", suffix);



                transaction = _dbConn.BeginTransaction();

                createSchemaCommand.Connection = _dbConn;
                createSchemaCommand.Transaction = transaction;

                insertUserTable.Connection = _dbConn;
                insertUserTable.Transaction = transaction;

                grantCommand.Connection = _dbConn;
                grantCommand.Transaction = transaction;

                flushCommand.Connection = _dbConn;
                flushCommand.Transaction = transaction;

                dropSchemaCommand.Connection = _dbConn;
                dropSchemaCommand.Transaction = transaction;
                try
                {

                    createSchemaCommand.ExecuteNonQuery();
                    Console.WriteLine("schema ");
                    grantCommand.ExecuteNonQuery();
                    Console.WriteLine("grant ");
                    insertUserTable.ExecuteNonQuery();
                    Console.WriteLine("insert user ");
                    flushCommand.ExecuteNonQuery();
                    Console.WriteLine("flush ");
                    dropSchemaCommand.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine("commit ");
                }
                catch (Exception e)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (MySqlException ex)
                    {
                        if (transaction.Connection != null)
                        {
                            Console.WriteLine("An exception of type " + ex.GetType() +
                            " was encountered while attempting to roll back the transaction.");
                        }
                    }
                    Console.WriteLine("An exception of type " + e.GetType() + " was encountered while inserting the data.");
                    Console.WriteLine("Neither record was written to database.");
                }
                finally
                {
                    _dbConn.Close();
                }
                Console.WriteLine("created user!");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return;
        }

        public string LoginUser(string username, string password)
        {
            _dbConn = new MySqlConnection(_connectionString);
            _dbConn.Open();

            MySqlCommand getUser = _dbConn.CreateCommand();
            getUser.CommandText = "select password,schemaname FROM  " + _rootDb + ".users where username = @username";
            getUser.Parameters.AddWithValue("@username", username);

            var reader = getUser.ExecuteReader();
            string passwordHash = "";
            string schemaname = "";
            if (reader.Read())
            {
                passwordHash = reader.GetString(0);
                schemaname = reader.GetString(1);
            }

            if (_userService.VerifyPasswordHash(password, passwordHash))
                return schemaname;

            else return null;

        }
    }
}
