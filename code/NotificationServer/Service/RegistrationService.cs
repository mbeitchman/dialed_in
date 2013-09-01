// NotificationServer - RegistrationService.cs

// This file implements the WCF "service contract" (essentially, the interface) exposed by web service

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NotificationServer.Service
{
    public class RegistrationService : IRegistrationService
    {
        public static event EventHandler<SubscriptionEventArgs> Subscribed;

        private static List<Uri> subscribers = new List<Uri>();
        private static object obj = new object();               // Lock object

        #region IRegistrationService Members
        // Implement the "Register" operation contract - allow channel URIs to subscribe to the service
        public void Register(string uri)
        {
            Uri channelUri = new Uri(uri, UriKind.Absolute);
            Subscribe(channelUri);
        }

        // Implement the "Unregister" operation contract - allow channel URIs to unsubscribe to the service
        public void Unregister(string uri)
        {
            Uri channelUri = new Uri(uri, UriKind.Absolute);
            Unsubscribe(channelUri);
        }
        #endregion

        #region Subscription/unsubscribing logic
        private void Subscribe(Uri channelUri)
        {
            lock (obj)
            {
                if (!subscribers.Exists((u) => u == channelUri))
                {
                    subscribers.Add(channelUri);
                }
            }
            OnSubscribed(channelUri, true);
        }

        public static void Unsubscribe(Uri channelUri)
        {
            lock (obj)
            {
                subscribers.Remove(channelUri);
            }
            OnSubscribed(channelUri, false);
        }
        #endregion

        #region Helper public functionality
        // List of current subscribers is publicly accessible
        public static List<Uri> GetSubscribers()
        {
            return subscribers;
        }
        #endregion

        #region Helper private functionality
        private static void OnSubscribed(Uri channelUri, bool isActive)
        {
            EventHandler<SubscriptionEventArgs> handler = Subscribed;
            if (handler != null)
            {
                handler(null,
                  new SubscriptionEventArgs(channelUri, isActive));
            }
        }
        #endregion

        #region Internal SubscriptionEventArgs class definition
        public class SubscriptionEventArgs : EventArgs
        {
            public SubscriptionEventArgs(Uri channelUri, bool isActive)
            {
                this.ChannelUri = channelUri;
                this.IsActive = isActive;
            }

            public Uri ChannelUri { get; private set; }
            public bool IsActive { get; private set; }
        }
        #endregion
    }
}
