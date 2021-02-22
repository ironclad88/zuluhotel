using Server;
using Server.Network;
using static Server.Configurations.MessagingConfiguration;

namespace Scripts.Zulu.Utilities
{
    public static class Messages
    {
        public static void SendSuccessMessage(this Mobile mobile, string text) => 
            mobile.SendAsciiMessage(ZHConfig.Messaging.SuccessHue, text);
        
        public static void SendSuccessMessage(this Mobile mobile, int number, string args = "") =>
            mobile.SendLocalizedMessage(number, args, ZHConfig.Messaging.SuccessHue);
        
        
        
        
        public static void SendFailureMessage(this Mobile mobile, string text) => 
            mobile.SendAsciiMessage(ZHConfig.Messaging.FailureHue, text);
        
        public static void SendFailureMessage(this Mobile mobile, int number, string args = "") =>
            mobile.SendLocalizedMessage(number, args, ZHConfig.Messaging.FailureHue);
        
    }
}