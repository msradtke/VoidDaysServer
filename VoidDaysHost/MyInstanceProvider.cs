using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using VoidDaysServerLibrary;

namespace VoidDaysHost
{
    class MyInstanceProvider : IEndpointBehavior, IInstanceProvider
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.InstanceProvider = this;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            //IVoidDaysLoginService service = new VoidDaysLoginService();
            //service.ope += new EventHandler<EventArgs>(service_MyEvent);
            //return service;
            throw new NotImplementedException();
        }

        void service_MyEvent(object sender, EventArgs e)
        {
            Console.WriteLine("In service_MyEvent");
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }

    }
}
