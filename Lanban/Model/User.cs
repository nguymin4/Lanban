using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lanban.Model
{
    public class UserModel
    {
        public int User_ID { get; set; }
        public string Username { get; set; }
        public string Name{ get; set; }
        public int Role { get; set; }
        public string Avatar { get; set; }
    }

    public class CommentDisplay
    {
        public int Comment_ID { get; set; }
        public int User_ID { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
    }

    public class CommentModel
    {
        public int Task_ID { get; set; }
        public int Project_ID { get; set; }
        public int User_ID { get; set; }
        public string Content { get; set; }
    }
}