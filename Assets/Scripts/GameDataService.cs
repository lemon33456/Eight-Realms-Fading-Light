// GameDataService.cs
using UnityEngine;
using System;
using System.IO; 
using System.Collections.Generic; // 引入 List

public static class GameDataService
{
    private static PlayerData _currentPlayerData;
    private static readonly string SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "player_data.json");
    public const string DEFAULT_USER_ID = "PLAYER_001";

    public static PlayerData CurrentPlayerData
    {
        get
        {
            if (_currentPlayerData == null)
            {
                LoadPlayerData(DEFAULT_USER_ID); 
            }
            RecalculateStamina();
            return _currentPlayerData;
        }
    }

    // **【新增】** 獲取玩家持有的角色 ID 清單
    public static List<int> GetOwnedCharacterIDs()
    {
        // 確保數據已載入
        if (_currentPlayerData == null)
        {
            LoadPlayerData(DEFAULT_USER_ID); 
        }
        // 回傳清單的複製，避免外部直接修改數據，更安全
        return new List<int>(_currentPlayerData.OwnedCharacterIDs); 
    }

    public static void LoadPlayerData(string userID)
    {
        Debug.Log($"GameDataService: 嘗試加載 UserID [{userID}] 的玩家數據...");

        if (File.Exists(SAVE_FILE_PATH))
        {
            try
            {
                string json = File.ReadAllText(SAVE_FILE_PATH);
                _currentPlayerData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log($"GameDataService: 數據從存檔加載成功！玩家: {_currentPlayerData.PlayerName}, 等級: {_currentPlayerData.PlayerLevel}");
            }
            catch (Exception e)
            {
                Debug.LogError($"GameDataService: 讀檔失敗，創建新數據。錯誤: {e.Message}");
                InitializeNewPlayerData(userID);
            }
        }
        else 
        {
            Debug.LogWarning("GameDataService: 找不到本地存檔，創建新帳號數據。");
            InitializeNewPlayerData(userID);
        }
        RecalculateStamina();
    }

    public static void SavePlayerData()
    {
        if (_currentPlayerData == null)
        {
            Debug.LogError("無法儲存：CurrentPlayerData 為空。");
            return;
        }
        
        string json = JsonUtility.ToJson(_currentPlayerData);

        try
        {
            File.WriteAllText(SAVE_FILE_PATH, json);
            Debug.Log($"GameDataService: 數據成功儲存到 {SAVE_FILE_PATH}");
        }
        catch (Exception e)
        {
            Debug.LogError($"GameDataService: 存檔失敗。錯誤: {e.Message}");
        }
    }
    
    private static void InitializeNewPlayerData(string userID)
    {
        _currentPlayerData = new PlayerData(
            userID, 
            "新手冒險者", 
            500,    // Gold 起始值
            5,      // Diamond 起始值
            PlayerData.MAX_STAMINA, // 滿體力
            1       // 等級 1
        );
        SavePlayerData(); 
    }

    private static void RecalculateStamina()
    {
        if (_currentPlayerData == null) return;
        
        if (_currentPlayerData.Stamina >= PlayerData.MAX_STAMINA)
        {
            _currentPlayerData.LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
            return;
        }

        DateTime lastUpdate = new DateTime(_currentPlayerData.LastStaminaUpdateTimeTicks);
        TimeSpan timeElapsed = DateTime.Now - lastUpdate;
        
        int recoverIntervalSeconds = PlayerData.STAMINA_RECOVERY_TIME_SEC;
        int recoveredPoints = (int)(timeElapsed.TotalSeconds / recoverIntervalSeconds);
        
        if (recoveredPoints > 0)
        {
            _currentPlayerData.Stamina = Mathf.Min(
                _currentPlayerData.Stamina + recoveredPoints, 
                PlayerData.MAX_STAMINA
            );
            
            long recoveryTimeOffset = (long)recoveredPoints * recoverIntervalSeconds * TimeSpan.TicksPerSecond;
            _currentPlayerData.LastStaminaUpdateTimeTicks += recoveryTimeOffset;
            
            Debug.Log($"GameDataService: 離線恢復 {recoveredPoints} 點體力。當前體力: {_currentPlayerData.Stamina}");
            
            if (_currentPlayerData.Stamina >= PlayerData.MAX_STAMINA)
            {
               _currentPlayerData.LastStaminaUpdateTimeTicks = DateTime.Now.Ticks;
            }
        }
    }
}