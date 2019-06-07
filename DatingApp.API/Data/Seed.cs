using System.Collections.Generic;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _db;
        public Seed(DataContext db)
        {
            _db = db;
        }

        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach (var user in users)
            {
                // we created all user seed data with a password of 'password'
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash("password", out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();

                _db.Users.Add(user);
            }

            _db.SaveChanges();
        }

        // the only reason we are duplicating this code from AuthRepo is because these seed users are purely for development purposes only
        // we could make the method public and static from our AuthRepo, but we don't want to disturb production level code for development purposes only
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //anything within the using method will be disposed of after it is used. 
            //it boils down to try{stuff in user method}, finally{obj.Dispose()}
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }           
        }
    }
}