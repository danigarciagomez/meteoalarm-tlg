using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meteoalarm_tlg
{
    internal class Aviso
    {
        public string Identifier { get; set; }
        public string Sender { get; set; }
        public string Status { get; set; }
        public string MsgType { get; set; }
        public string Category { get; set; }
        public string Urgency { get; set; }
        public string Severity { get; set; }
        public string Certainty { get; set; }
        public string Effective { get; set; }
        public string Expires { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }
        public string AwarenessLevel { get; set; }
        public string AwarenessType { get; set; }
        public string AreaDescription { get; set; }
        public string EmmaId { get; set; }
    }
}
