using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        // by adding these 2 fields EF will understand that when a user is deleted, all photos that belong to that user should also be deleted
        // EF will create the foreign keys in the database for us based on these
        public User User { get; set; }
        public int UserId { get; set; }
    }
}