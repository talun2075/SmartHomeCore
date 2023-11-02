using System;

namespace SmartHome.Classes.Aurora.Events
{
    public class NotificationArgs : EventArgs
    {
        public Notification Notification { get; }

        public NotificationArgs(Notification notification)
        {
            Notification = notification;
        }
    }
}