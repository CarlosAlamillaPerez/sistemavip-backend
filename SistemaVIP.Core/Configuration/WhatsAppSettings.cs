using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVIP.Core.Configuration
{
    public class WhatsAppSettings
    {
        public string ApiKey { get; set; }
        public string[] AdminPhoneNumbers { get; set; }
        public int MaxRetries { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 1000;
        public int DailyMessageLimit { get; set; } = 600;  // Límite de CallMeBot
        public bool EnableNotifications { get; set; } = true;
    }
}