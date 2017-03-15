﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLayer;
using System.Data.Linq;

namespace DataLayer
{
    public class UsersProvider
    {
        static UsersProvider()
        {
            Database.InitializeTable(typeof(UserEntity));
        }

        public static bool IsEmptyUserDatabase()
        {
            bool lExist = false;
            if (Database.OpenConnection())
            {
                DataContext db = new DataContext(Database.get_connectionString);
                Table<UserEntity> Users = db.GetTable<UserEntity>();

                lExist = (0 < Users.Count());

                Database.CloseConnection();
            }
            return !lExist;
        }

        public static bool IsValidUserID(string userID)
        {
            bool lExist = false;
            if (Database.OpenConnection())
            {
                DataContext db = new DataContext(Database.get_connectionString);
                Table<UserEntity> Users = db.GetTable<UserEntity>();
                var q = from u in Users
                        where u.Username == userID
                        select u;
                lExist = (0 < q.Count());

                Database.CloseConnection();
            }
            return lExist;
        }

        public static bool IsValidPassword(string userID,string pw)
        {
            bool lExist = false;
            if (Database.OpenConnection())
            {
                DataContext db = new DataContext(Database.get_connectionString);
                Table<UserEntity> Users = db.GetTable<UserEntity>();
                var q = from u in Users
                        where (u.Username == userID)
                        select u;
                foreach(var user in q)
                {
                    lExist = lExist || EncriptionProvider.Confirm(pw, user.Password, EncriptionProvider.Supported_HA.SHA256);
                }

                Database.CloseConnection();
            }
            return lExist;
        }

        public static bool NewUser(string userID,string password)
        {
            bool lSuccess = false;
            if (!IsValidUserID(userID))
                if (Database.OpenConnection())
                {
                    DataContext db = new DataContext(Database.get_connectionString);
                    Table<UserEntity> Users = db.GetTable<UserEntity>();
                    Users.InsertOnSubmit(
                        new UserEntity(
                            userID,
                            EncriptionProvider.ComputeHash(password, EncriptionProvider.Supported_HA.SHA256, null)));

                    try
                    {
                        db.SubmitChanges();
                        lSuccess = true;
                    }
                    catch
                    {

                    }
                    Database.CloseConnection();
                }
            return lSuccess;
        }

        public static bool DeleteUser(string userID)
        {
            bool lSuccess = false;
            bool lExist = false;
            if (Database.OpenConnection())
            {
                DataContext db = new DataContext(Database.get_connectionString);
                Table<UserEntity> Users = db.GetTable<UserEntity>();
                var q = from u in Users
                        where (u.Username == userID)
                        select u;
                foreach (var user in q)
                {
                    lExist = true;
                    Users.DeleteOnSubmit(user);
                }
                if (lExist)
                {
                    try
                    {
                        db.SubmitChanges();
                        lSuccess = true;
                    }
                    catch
                    {

                    }
                }
                Database.CloseConnection();
            }
            return lSuccess;
        }


        //Table<Customer> Customers = db.GetTable<Customer>();

        //Customers.InsertOnSubmit(new Customer("ID007", "London"));
        //// Submit the change to the database.
        //try
        //{
        //    db.SubmitChanges();
        //}
        //catch (Exception e)
        //{
        //    MessageBox.Show(e.ToString());
        //}


        //// Query for customers from London
        //var q =
        //   from c in Customers
        //   where c.City == "London"
        //   select c;
        //foreach (var cust in q)
        //    MessageBox.Show(String.Format("id = {0}, City = {1}", cust.CustomerID, cust.City));





    }
}