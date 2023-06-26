using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using GameAnalyticsSDK;
using GameAnalyticsSDK.Events;
using UnityEngine;
using Zenject;

public class AnalyticsController : IInitializable
{
    private const string CLICK_UNLOCK_ITEM = "click_unlock_item";
    private const string ITEM_UNLOCK = "item_unlock";
    private const string CLICK_LEVELS = "click_levels";
    private const string CLICK_COINS = "click_coins";
    private const string CLICK_LEVEL_PROGRESS = "click_level_progress";
    private const string CLICK_LEVEL_START = "click_level_start";
    private const string LEVEL_START = "level_start";
    private const string LEVEL_COMPLETE = "level_complete";
    private const string CLICK_CLAIM = "click_claim";
    private const string CLICK_LEVELS_CHOOSE_SETTING = "click_levels_choose_setting";
    private const string CLICK_LEVEL_START_LOCKED = "click_level_start_locked";
    private const string CLICK_UNLOCK_ITEM_LOCKED = "click_unlock_item_locked";
    private const string SETTING_ID = "setting_id";
    
    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool firebaseInitialized = false;
    
    public void Initialize()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) 
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                firebaseInitialized = true;
            } 
            else 
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        
        GameAnalytics.Initialize();
    }
    
    public void ClickLevelsChooseSetting(int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_LEVELS_CHOOSE_SETTING, SETTING_ID, currentSettingsId);
        
        GameAnalytics.NewDesignEvent(CLICK_LEVELS_CHOOSE_SETTING, currentSettingsId);
    }
    
    public void ClickUnlockItems(int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_UNLOCK_ITEM, SETTING_ID, currentSettingsId);
        
        GameAnalytics.NewDesignEvent(CLICK_UNLOCK_ITEM, currentSettingsId);
    }
    
    public void ItemUnlock(string item_id, int item_cost, int balance, int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(ITEM_UNLOCK, new []
            {
                new Parameter("item_id", item_id),
                new Parameter("item_cost", item_cost),
                new Parameter("balance", balance),
                new Parameter(SETTING_ID, currentSettingsId)
            });
        
        var eventData = new Dictionary<string, object>()
        {
            {"item_id", item_id},
            {"item_cost", item_cost},
            {"balance", balance},
            {SETTING_ID, currentSettingsId}
        };
        
        GA_Design.NewEvent(ITEM_UNLOCK, eventData);
    }

    public void ClickLevels()
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_LEVELS);
        
        GameAnalytics.NewDesignEvent(CLICK_LEVELS);
    }
    
    public void ClickCoins(int coins_amount)
    {
        if(!firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_COINS, "coins_amount", coins_amount);
        
        var eventData = new Dictionary<string, object>(){{"coins_amount", coins_amount}};
        GA_Design.NewEvent(CLICK_COINS, eventData);
    }
    
    public void ClickLevelProgress(int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_LEVEL_PROGRESS,SETTING_ID, currentSettingsId );
        
        var eventData = new Dictionary<string, object>(){{SETTING_ID, currentSettingsId}};
        GA_Design.NewEvent(CLICK_LEVEL_PROGRESS, eventData);
    }
    
    public void ClickLevelStart(int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_LEVEL_START, SETTING_ID, currentSettingsId );
        
        var eventData = new Dictionary<string, object>(){{SETTING_ID, currentSettingsId}};
        GA_Design.NewEvent(CLICK_LEVEL_START, eventData);
    }
    
    public void ClickLevelStartLocked(int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_LEVEL_START_LOCKED, SETTING_ID, currentSettingsId );
        
        var eventData = new Dictionary<string, object>(){{SETTING_ID, currentSettingsId}};
        GA_Design.NewEvent(CLICK_LEVEL_START_LOCKED, eventData);
    }
    
    public void ClickUnlockItemLocked(int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_UNLOCK_ITEM_LOCKED, SETTING_ID, currentSettingsId );
        
        var eventData = new Dictionary<string, object>(){{SETTING_ID, currentSettingsId}};
        GA_Design.NewEvent(CLICK_UNLOCK_ITEM_LOCKED, eventData);
    }
    
    public void LevelStart(int currentSettingsId)
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(LEVEL_START, SETTING_ID, currentSettingsId );
        
        var eventData = new Dictionary<string, object>(){{SETTING_ID, currentSettingsId}};
        
        GA_Design.NewEvent(LEVEL_START, eventData);
    }

    //positive_spawned, positive_collected_1, positive_collected_2, positive_collected_3, negative_spawned, negative_collected_1, negative_collected_2, negative_collected_3, coins_spawned, coins_collected_1, coins_collected_2, coins_collected_3, duration  
    public void LevelComplete(Dictionary<string, int> param)
    {
        foreach (var p in param)
        {
            Debug.Log($"name: {p.Key}, value: {p.Value}");
        }

        if (firebaseInitialized)
        {
            var sParameters = param.Select(k => new Parameter(k.Key, k.Value)).ToArray();
            FirebaseAnalytics.LogEvent(LEVEL_COMPLETE, sParameters);
        }

        var dic = param.ToDictionary<KeyValuePair<string, int>, string, object>(value => value.Key, value => value.Value);

        GA_Design.NewEvent(LEVEL_COMPLETE, dic);
    }
    
    public void ClickClaim()
    {
        if(firebaseInitialized)
            FirebaseAnalytics.LogEvent(CLICK_CLAIM);
        
        GameAnalytics.NewDesignEvent(CLICK_CLAIM);
    }
}