using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using VoidDaysServerLibrary;

namespace VoidDaysHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string binDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine(binDir);
            Environment.SetEnvironmentVariable("PATH", path + ";" + binDir);
            // Step 1 Create a URI to serve as the base address.  
            Uri baseAddress = new Uri("http://3.16.24.128:8733/Design_Time_Addresses/VoidDaysServerLibrary/");

            // Step 2 Create a ServiceHost instance  
            ServiceHost selfHost = new ServiceHost(typeof(VoidDaysLoginService), baseAddress);

            try
            {
                // Step 3 Add a service endpoint.  
                selfHost.AddServiceEndpoint(typeof(IVoidDaysLoginService), new BasicHttpBinding(), "VoidDaysLoginService");

                // Step 4 Enable metadata exchange.  
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

                // Step 5 Start the service.  
                selfHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                // Close the ServiceHostBase to shutdown the service.  
                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}
