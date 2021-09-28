using System;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace SteamGameNotes
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }
    }
}
