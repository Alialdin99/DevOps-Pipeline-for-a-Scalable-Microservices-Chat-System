using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Events
{
    public class UserDeleted
    {
        public string UserId { get; set; } = default!;
        public DateTime DeletedAt { get; set; }
    }
}
