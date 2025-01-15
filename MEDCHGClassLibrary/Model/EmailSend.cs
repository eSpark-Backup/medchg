using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Models
{
    public class EmailSend
    {
        public string SenderAddress { get; init; }

        public string SenderPassword { get; init; }

        public string ReceiverAddress { get; init; }

        public string SenderName { get; init; }

        public string ReceiverName { get; init; }

        public string Subject { get; init; }

        public string Body { get; init; }

        public DateTime Created { get; init; }
    }
}
