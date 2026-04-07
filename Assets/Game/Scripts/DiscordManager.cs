using System;
using UnityEngine;

public class DiscordManager : MonoBehaviour
{
    private string name24;
    private string gde = "Меню";
    public long time;

#if !UNITY_ANDROID && !UNITY_IOS && DISCORD_SDK
    private Discord.Discord discord;
#endif

    private void Start()
    {
        name24 = login.username;
        time = DateTimeOffset.Now.ToUnixTimeSeconds();

#if !UNITY_ANDROID && !UNITY_IOS && DISCORD_SDK
        try
        {
            discord = new Discord.Discord(1215401955057213552, (ulong)Discord.CreateFlags.NoRequireDiscord);
            ChangeActivity();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Discord SDK init failed: " + ex.Message);
        }
#else
        Debug.Log("Discord Rich Presence disabled on this platform/build.");
#endif
    }

    private void OnDisable()
    {
#if !UNITY_ANDROID && !UNITY_IOS && DISCORD_SDK
        if (discord != null)
        {
            discord.Dispose();
            discord = null;
        }
#endif
    }

    public void ChangeActivity()
    {
#if !UNITY_ANDROID && !UNITY_IOS && DISCORD_SDK
        if (discord == null)
            return;

        string state = settingsController.nickname + " делает фичи яйца";
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = state,
            Details = gde,
            Assets =
            {
                LargeImage = "logo",
                LargeText = gde,
                SmallImage = "logo",
                SmallText = string.IsNullOrEmpty(name24) ? login.username : name24
            },
            Timestamps =
            {
                Start = time
            }
        };
        activityManager.UpdateActivity(activity, res => Debug.Log("ACTIVITY UPDATED!"));
#endif
    }

    public void buton(string newprichina)
    {
        gde = newprichina;
        ChangeActivity();
    }

    private void Update()
    {
#if !UNITY_ANDROID && !UNITY_IOS && DISCORD_SDK
        if (discord != null)
            discord.RunCallbacks();
#endif
    }
}
