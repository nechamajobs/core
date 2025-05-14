﻿using OurApi.Models;
using System.Collections.Generic;
using System.Linq;
using OurApi.Models;
using OurApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace OurApi.Services
{
    public class userService : Iuser
    {
        List<User> users { get; }
        int nextId = 3;
        private string fileName;

        public userService()
        {
            fileName = Path.Combine("data", "user.json");
            using (var jsonFile = File.OpenText(fileName))
            {
                users = JsonSerializer.Deserialize<List<User>>(jsonFile.ReadToEnd(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

        }
        private void saveToFile()
        {

            File.WriteAllText(fileName, JsonSerializer.Serialize(users));
        }
        public List<User> GetAll() => users;
        public User Get(int id) => users.FirstOrDefault(u => u.id == id);
        public int Add(User u)
        {
            u.id =users.Max(p => p.id) + 1;
;
            // u.Password = u.Password;
            users.Add(u);
         saveToFile();

            return u.id;
        }
       
        public int ExistUser(string name, string password)
        {
            User existUser = users.FirstOrDefault(u => u.Username.Equals(name) && u.Password.Equals(password));
            if (existUser != null)
                return existUser.id;
            return -1;
        }

         public void Delete(int id) 
        {
            var user = Get(id);
            if (user is null)
                return;

            users.Remove(user);
            saveToFile();
        }

        public void Update(User u)
        {
            var index = users.FindIndex(p => p.Password == u.Password);
            if (index == -1)
                return;

            users[index] = u;
        }

        public int Count { get => users.Count(); }
    }
}

