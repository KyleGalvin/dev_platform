using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager.Sdk.Models
{
    public class RealmRequest
    {
        public bool enabled { get; set; } = true;
        public string realm { get; set; }
    }
}
