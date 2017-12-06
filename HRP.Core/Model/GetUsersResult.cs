using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRP.Core.Model
{
    public class GetUsersResult
    {
        public IEnumerable<UserModel> Users { get; set; }

        public int Total { get; set; }
    }
}
