#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using Unity.Notifications.Android;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    void Start() {
        if(!NotificationChannelExists("exit_channel")){
            CreateNotificationChannel();
        }
        ScheduleExitNotification();
    }
    void CreateNotificationChannel() {
        var channel = new AndroidNotificationChannel() {
            Id = "exit_channel",
            Name = "Exit Notifications",
            Importance = Importance.Default,
            Description = "Notifies when the app is closed",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }
    void ScheduleExitNotification() {
        // DateTime fireTime = DateTime.Now.AddSeconds(10);
        DateTime fireTime = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0);
        if(DateTime.Now > fireTime){
            fireTime = fireTime.AddDays(1);
        }

        var notification = new AndroidNotification {
            Title = "Come Back!",
            Text = "It's been a minute since you left. We miss you!",
            SmallIcon = "icon_small",
            FireTime = fireTime,
        };

        AndroidNotificationCenter.SendNotification(notification, "exit_channel");
    }
    public static bool NotificationChannelExists(string channelId)
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext"))
            using (AndroidJavaObject notificationManager = context.Call<AndroidJavaObject>("getSystemService", "notification"))
            {
                if (notificationManager == null)
                    return false;

                AndroidJavaObject channel = notificationManager.Call<AndroidJavaObject>("getNotificationChannel", channelId);
                return channel != null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error checking notification channel: " + e.Message);
            return false;
        }
    }
    

}

#endif