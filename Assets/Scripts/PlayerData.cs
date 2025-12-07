using System;
using UnityEngine; // 僅使用 Mathf

// [Serializable] 是未來用於存檔/讀檔的關鍵
[Serializable]
public class PlayerData 
{
    // === 靜態常數 (全體玩家共享的規則) ===
    public const int MAX_STAMINA = 150; 
    public const int STAMINA_RECOVERY_TIME_SEC = 300; // 300 秒 = 5 分鐘

    // === 帳號識別資訊 ===
    public string UserID = "";
    public string PlayerName = "New Player";
    public int PlayerLevel = 1;
    
    // === 資源與貨幣 ===
    public int Gold = 0;
    public int Diamond = 0;
    public int Stamina = 0;
    
    // === 體力恢復時間標記 (上次計算或更新體力的時間戳) ===
    public long LastStaminaUpdateTimeTicks = 0;

    // -------------------------------------------------------------------
    // 建構子 (Constructor)
    // -------------------------------------------------------------------
    
    public PlayerData(string id, string name, int gold, int diamond, int stamina, int level)
    {
        UserID = id;
        PlayerName = name;
        PlayerLevel = level;
        Gold = gold;
        Diamond = diamond;
        Stamina = Mathf.Min(stamina, MAX_STAMINA); // 確保初始體力不超過最大值
        
        LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
    }
    
    // -------------------------------------------------------------------
    // 方法：計算下一次恢復所需剩餘秒數
    // -------------------------------------------------------------------
    
    /// <summary>
    /// 計算距離下一次體力恢復 (1點) 還剩多少秒。
    /// </summary>
    public int GetTimeRemainingToRecover()
    {
        if (Stamina >= MAX_STAMINA)
        {
            return 0; // 體力已滿
        }

        // 1. 將上次更新時間戳 (Ticks) 轉換回 DateTime
        DateTime lastUpdate = new DateTime(LastStaminaUpdateTimeTicks);
        
        // 2. 計算下次恢復的目標時間
        DateTime nextRecoveryTime = lastUpdate.AddSeconds(STAMINA_RECOVERY_TIME_SEC);
        
        // 3. 計算目標時間與當前時間的差值
        TimeSpan timeRemaining = nextRecoveryTime - DateTime.Now;
        
        if (timeRemaining.TotalSeconds > 0)
        {
            // 返回剩餘秒數 (向上取整確保倒數計時不會閃爍)
            return (int)Math.Ceiling(timeRemaining.TotalSeconds);
        }
        
        return 0; // 時間已過，應立即恢復
    }
}