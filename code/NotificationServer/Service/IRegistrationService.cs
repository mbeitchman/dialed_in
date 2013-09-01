// NotificationServer - IRegistrationService.cs

// This file defines the WCF "service contract" (essentially, the interface) exposed by web service

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace NotificationServer.Service
{
    [ServiceContract]
    public interface IRegistrationService
    {
        [OperationContract, WebGet]
        void Register(string uri);

        [OperationContract, WebGet]
        void Unregister(string uri);
    }
}