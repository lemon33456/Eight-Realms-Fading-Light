// 檔案：PlayerData.cs
using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class PlayerData 
{
    public const int MAX_STAMINA = 150; 
    public const int STAMINA_RECOVERY_TIME_SEC = 300; // 5 分鐘

    public string UserID = "";
    public string PlayerName = "New Player";
    public int PlayerLevel = 1;
    
    public int Gold = 0;
    public int Diamond = 0;
    public int Stamina = 0;
    
    // 角色卡 ID 清單
    public List<string> OwnedCardIDs = new List<string>(); 
    // 訓練結晶清單 (實戰單位)
    public List<TrainingCrystal> Crystals = new List<TrainingCrystal>(); 
    
    public long LastStaminaUpdateTimeTicks = 0;

    // =======================================================
    // 【修正 1】 新增無參數建構子
    // 這確保 JSON 讀取失敗時，能正確初始化新玩家數據
    // =======================================================
    public PlayerData() : this(
        // 傳遞預設值給參數建構子
        GameDataService.DEFAULT_USER_ID, 
        "新米訓練師", 
        100, // 初始金幣
        50,  // 初始鑽石
        MAX_STAMINA, // 初始體力滿格
        1
    )
    {
        // 此處不需要額外邏輯，因為它調用了下方的參數建構子
    }

    // =======================================================
    // 【修正 2】 參數建構子 (保持您的邏輯，但被無參數建構子調用)
    // =======================================================
    public PlayerData(string id, string name, int gold, int diamond, int stamina, int level)
    {
        UserID = id;
        PlayerName = name;
        PlayerLevel = level;
        Gold = gold;
        Diamond = diamond;
        Stamina = Mathf.Min(stamina, MAX_STAMINA);
        
        LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
        
        // 【賦予起始角色的核心邏輯】
        // 由於這個構造函數會被無參數構造函數調用，我們在這裡確保初始化只發生一次
        if (Crystals.Count == 0) // 僅在列表為空時進行初始化
        {
            string startCardID = "CC_ERFL_001_A"; 
            
            // 1. 玩家擁有這張卡片
            OwnedCardIDs.Add(startCardID); 

            // 2. 創建一個從這張卡片培育出來的起始結晶
            TrainingCrystal initialCrystal = new TrainingCrystal(startCardID);
            Crystals.Add(initialCrystal);
            
            Debug.Log($"【PlayerData DEBUG】: 成功賦予起始角色結晶 ID: {startCardID}");
        }
    }
    
    // 體力恢復計算函數 (保持不變)
    public int GetTimeRemainingToRecover()
    {
        if (Stamina >= MAX_STAMINA) return 0;

        DateTime lastUpdate = new DateTime(LastStaminaUpdateTimeTicks);
        DateTime nextRecoveryTime = lastUpdate.AddSeconds(STAMINA_RECOVERY_TIME_SEC);
        TimeSpan timeRemaining = nextRecoveryTime - DateTime.Now;
        
        if (timeRemaining.TotalSeconds > 0)
        {
            return (int)Math.Ceiling(timeRemaining.TotalSeconds);
        }
        
        return 0;
    }
}