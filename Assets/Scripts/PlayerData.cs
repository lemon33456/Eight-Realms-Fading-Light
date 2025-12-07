// PlayerData.cs 保持不變
using System;
using UnityEngine;

// [關鍵] 必須保留，才能使用 JsonUtility 進行存檔/讀檔
[Serializable]
public class PlayerData 
{
    // ... 保持所有欄位和方法不變 ...
    public const int MAX_STAMINA = 150; 
    public const int STAMINA_RECOVERY_TIME_SEC = 300; // 5 分鐘

    public string UserID = "";
    public string PlayerName = "New Player";
    public int PlayerLevel = 1;
    
    public int Gold = 0;
    public int Diamond = 0;
    public int Stamina = 0;
    
    public long LastStaminaUpdateTimeTicks = 0;

    public PlayerData(string id, string name, int gold, int diamond, int stamina, int level)
    {
        // ... (保持建構子邏輯) ...
        UserID = id;
        PlayerName = name;
        PlayerLevel = level;
        Gold = gold;
        Diamond = diamond;
        Stamina = Mathf.Min(stamina, MAX_STAMINA);
        
        LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
    }
    
    public int GetTimeRemainingToRecover()
    {
        // ... (保持 GetTimeRemainingToRecover 邏輯) ...
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