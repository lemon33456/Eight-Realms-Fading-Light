// PlayerData.cs
using System;
using UnityEngine;
using System.Collections.Generic; // 引入 List 所需的命名空間

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
    
    // **【新增】** 玩家擁有的角色 ID 清單 (這是核心數據)
    public List<int> OwnedCharacterIDs = new List<int>(); 
    
    public long LastStaminaUpdateTimeTicks = 0;

    public PlayerData(string id, string name, int gold, int diamond, int stamina, int level)
    {
        UserID = id;
        PlayerName = name;
        PlayerLevel = level;
        Gold = gold;
        Diamond = diamond;
        Stamina = Mathf.Min(stamina, MAX_STAMINA);
        
        LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
        
        // **【新增】** 賦予新玩家一些起始角色 (用於測試)
        OwnedCharacterIDs.Add(1001); // 假設起始角色 A
        OwnedCharacterIDs.Add(1002); // 假設起始角色 B
        OwnedCharacterIDs.Add(1003); 
        OwnedCharacterIDs.Add(2001); // 假設稀有角色
    }
    
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