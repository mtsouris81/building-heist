using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    [Serializable]
    public class Player
    {
        public string Name { get; set; }
        public string AuthToken { get; set; }
        public string Id { get; set; }
        public string Username { get; set; }
        public string ImageUrl { get; set; }
        public int Floor { get; set; }
        public int? Room { get; set; }

        public Player Clone()
        {
            return new Player()
            {
                Name = this.Name,
                Id = this.Id,
                Username = this.Username,
                Floor = this.Floor,
                Room = this.Room,
                ImageUrl = this.ImageUrl,
                AuthToken = this.AuthToken
            };
        }

    }
}
