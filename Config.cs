using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWAPIBulletHoleVisualizer
{
    public class Config
    {
        public string SerializeToWebhook { get; set; } = string.Empty;
        public int ServerNum { get; set; } = -1;
        public bool ShowPort { get; set; } = true;
    }
}
