using Microsoft.Azure.ActiveDirectory.GraphClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AADGraphAPI.Models
{
    public class GroupsMembersViewModel
    {
        public List<Group> Groups { get; set; }
        public string SelectedGroupName { get; set; }
        public List<User> GroupMembers { get; set; }

    }
}