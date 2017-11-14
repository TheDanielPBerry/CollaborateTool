using CollaborateDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Collaborate.Models
{
    public class TeamBuildViewModel
    {
        
        public List<User> Users { get; set; }
        public List<bool> Team { get; set; }
        public Board Board { get; set; }

        public int UserId { get; set; }
        
    }
}