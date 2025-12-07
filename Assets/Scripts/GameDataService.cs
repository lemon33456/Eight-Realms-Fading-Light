using UnityEngine;
using System;

// 靜態類別，用於全局存取玩家數據，不依賴於任何 GameObject。
public static class GameDataService
{
    private static PlayerData _currentPlayerData;

    /// <summary>
    /// 公開屬性：提供當前玩家數據的介面，並在讀取前執行體力恢復計算。
    /// </summary>
    public static PlayerData CurrentPlayerData
    {
        get
        {
            if (_currentPlayerData == null)
            {
                Debug.LogError("Player Data not loaded! Returning default placeholder data.");
                _currentPlayerData = new PlayerData("ERROR_000", "ERROR", 0, 0, 0, 1);
            }
            
            // [關鍵]：在返回數據之前，先執行體力恢復計算
            RecalculateStamina();

            return _currentPlayerData;
        }
    }

    /// <summary>
    /// [未來後端整合點]：模擬登入成功後，根據 UserID 從服務器加載玩家數據。
    /// </summary>
    public static void LoadPlayerData(string userID)
    {
        Debug.Log($"GameDataService: 嘗試根據 UserID [{userID}] 模擬加載玩家數據...");
        
        // --- 模擬不同帳號的載入數據 ---
        
        if (userID == "GUEST_001")
        {
            _currentPlayerData = new PlayerData(
                "GUEST_001", 
                "訪客", 
                5000,   // Gold
                100,    // Diamond
                100,    // Stamina (未滿，方便測試恢復)
                50      // Level
            );
        }
        else 
        {
            _currentPlayerData = new PlayerData(
                userID, 
                "冒險者小明", 
                100,    // Gold
                5,      // Diamond
                20,     // Stamina
                10      // Level
            );
        }
        
        Debug.Log($"GameDataService: 數據加載成功！玩家: {CurrentPlayerData.PlayerName}, 等級: {CurrentPlayerData.PlayerLevel}");
    }
    
    // ==============================================================
    // 核心體力恢復計算函數 (處理離線恢復)
    // ==============================================================
    private static void RecalculateStamina()
    {
        if (_currentPlayerData.Stamina >= PlayerData.MAX_STAMINA)
        {
            _currentPlayerData.LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
            return;
        }

        // 1. 計算自上次更新以來經過的時間
        DateTime lastUpdate = new DateTime(_currentPlayerData.LastStaminaUpdateTimeTicks);
        TimeSpan timeElapsed = DateTime.Now - lastUpdate;
        
        // 2. 計算已經可以恢復的體力點數
        int recoverIntervalSeconds = PlayerData.STAMINA_RECOVERY_TIME_SEC;
        int recoveredPoints = (int)(timeElapsed.TotalSeconds / recoverIntervalSeconds);
        
        // 3. 執行恢復操作
        if (recoveredPoints > 0)
        {
            // 計算新的體力，並確保不超過最大值
            _currentPlayerData.Stamina = Mathf.Min(
                _currentPlayerData.Stamina + recoveredPoints, 
                PlayerData.MAX_STAMINA
            );
            
            // 4. 更新下次恢復的基準時間
            long recoveryTimeOffset = (long)recoveredPoints * recoverIntervalSeconds * TimeSpan.TicksPerSecond;
            _currentPlayerData.LastStaminaUpdateTimeTicks += recoveryTimeOffset;
            
            Debug.Log($"GameDataService: 離線恢復 {recoveredPoints} 點體力。當前體力: {_currentPlayerData.Stamina}");
            
            // 如果恢復後剛好達到最大值，確保時間標記是當前時間
            if (_currentPlayerData.Stamina >= PlayerData.MAX_STAMINA)
            {
                 _currentPlayerData.LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
            }
        }
    }
}