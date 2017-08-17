using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Options;
using BeltExam.Models;
using Microsoft.AspNetCore.Identity;
 
namespace BeltExam.Factory
{
    public class UserFactory : IFactory<User>
    {
        private readonly IOptions<MySqlOptions> MySqlConfig;
        public UserFactory(IOptions<MySqlOptions> config)
        {
            MySqlConfig = config;

        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(MySqlConfig.Value.ConnectionString);
            }
        }
        public void AddUser(User user){
            using (IDbConnection dbConnection = Connection){
                string query = "INSERT INTO users (FirstName, LastName, Username, Email, Password)" +
                                @"VALUES (@FirstName, @LastName, @Username, @Email, @Password)";

                dbConnection.Open();
                dbConnection.Execute(query, user);
            }
        }
        public bool ValidateUser(string username, string password){
            using(IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM users WHERE (username = '{username}')";
                dbConnection.Open();
                User user = dbConnection.Query<User>(query).SingleOrDefault();
                if(user != null && password != null){
                    var Hasher = new PasswordHasher<User>();
                    if(Hasher.VerifyHashedPassword(user, user.Password, password) != 0){
                        return true;
                    }
                }
                return false;
            }
        }
        public User GetUserByUsername(string username){
            using(IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM users WHERE (Username = '{username}')";
                dbConnection.Open();
                User user = dbConnection.Query<User>(query).SingleOrDefault();
                return user;
            }
        }
        public User GetUserById(int id){
            using(IDbConnection dbConnection = Connection){
                string query = $"SELECT * FROM users WHERE (Id = {id})";
                dbConnection.Open();
                User user = dbConnection.Query<User>(query).SingleOrDefault();
                return user;
            }
        }
        public User GetUserForHtml(int id){
            using (IDbConnection dbConnection = Connection){
                string query = $"SELECT Id, FirstName, LastName, Wallet FROM users WHERE (Id = {id})";
                dbConnection.Open();
                return dbConnection.Query<User>(query).SingleOrDefault();
            }
        }
    }
}