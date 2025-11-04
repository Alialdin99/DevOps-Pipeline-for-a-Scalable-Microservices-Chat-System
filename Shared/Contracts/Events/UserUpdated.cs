using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Events
{
    public class UserUpdated
    {
        public string UserId { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
