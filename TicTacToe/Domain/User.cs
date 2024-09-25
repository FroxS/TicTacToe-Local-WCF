using System;

namespace TicTacToe
{
    public class User
    {
        public User()
        {
            UserId = Guid.NewGuid().ToString().Split('-')[4];
        }

        public string UserId { get; set; }
        public string Name { get; set; }
        public char Char { get; set; }
        public DateTime TimeCreated { get; set; }
    }
}
