// 檔案：GameDataService.cs
using UnityEngine;
using System;
using System.IO; 
using System.Collections.Generic; 
using System.Linq; // 引入 Linq 命名空間，用於 ToDictionary

public static class GameDataService
{
    // ----------------------------------------------------------------------
    // I. 配置數據存儲 (Config Data Storage)
    // ----------------------------------------------------------------------
    
    // 用 Dictionary 存儲所有 CharacterCardConfig，以便通過 CardID 快速查找
    private static Dictionary<string, CharacterCardConfig> _cardConfigMap;
    
    // 用 Dictionary 存儲所有 CharacterEntityConfig，以便通過 EntityID 快速查找
    private static Dictionary<string, CharacterEntityConfig> _entityConfigMap;
    
    
    // ----------------------------------------------------------------------
    // II. 玩家數據存取與存檔 (Player Data Access & Saving)
    // ----------------------------------------------------------------------
    
    private static PlayerData _currentPlayerData;
    // 保持您的 File.IO 存檔路徑邏輯
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
    
    // **【刪除/修正】** 刪除舊的 GetOwnedCharacterIDs()，它不再適用於新的 string ID 結構。
    // 如果需要清單，應直接訪問 CurrentPlayerData.Crystals 或 CurrentPlayerData.OwnedCardIDs

    public static void LoadPlayerData(string userID)
    {
        // 確保靜態配置數據已載入，否則後續 PlayerData 初始化可能會失敗
        if (_cardConfigMap == null)
        {
            InitializeConfigs(); 
        }
        
        // ... (保持您的 File.IO 讀取邏輯)
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
        // ... (保持您的 File.IO 儲存邏輯)
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
        // ... (保持您的初始化邏輯，這裡會調用 PlayerData 的建構子)
        _currentPlayerData = new PlayerData(
            userID, 
            "新手冒險者", 
            500,    
            5,      
            PlayerData.MAX_STAMINA, 
            1       
        );
        SavePlayerData(); 
    }

    // ----------------------------------------------------------------------
    // III. 靜態配置加載 (Static Config Loading) 【新增】
    // ----------------------------------------------------------------------
    
    // 【重要】建議在 MasterManager 啟動時最優先調用此方法
    public static void InitializeConfigs()
    {
        if (_cardConfigMap != null) return; // 防止重複加載
        
        Debug.Log("GameDataService: Initializing Game Configs...");
        
        try
        {
            // 假設所有配置檔都在 Resources/GameConfigs/Characters/ 下
            string configPath = "GameConfigs/Characters";
            
            // 1. 加載所有的 CharacterCardConfig
            CharacterCardConfig[] cardConfigs = Resources.LoadAll<CharacterCardConfig>(configPath);
            _cardConfigMap = cardConfigs.ToDictionary(config => config.CardID, config => config);
            
            // 2. 加載所有的 CharacterEntityConfig
            CharacterEntityConfig[] entityConfigs = Resources.LoadAll<CharacterEntityConfig>(configPath);
            // 檢查 EntityID 是否為空或重複是良好的實踐
            _entityConfigMap = entityConfigs.Where(config => !string.IsNullOrEmpty(config.EntityID))
                                            .ToDictionary(config => config.EntityID, config => config);

            Debug.Log($"GameDataService: Loaded {_cardConfigMap.Count} Character Cards and {_entityConfigMap.Count} Character Entities.");
        }
        catch (Exception e)
        {
            Debug.LogError($"GameDataService: 配置數據加載失敗! 請檢查 Resources 資料夾結構和 ScriptableObject 設定。錯誤: {e.Message}");
        }
        
    }


    // ----------------------------------------------------------------------
    // IV. 配置查詢函數 (Config Query Functions) 【新增】
    // ----------------------------------------------------------------------

    // 通過 CardID 獲取 CharacterCardConfig (用於獲取立繪和基礎成長數據)
    public static CharacterCardConfig GetCardConfigByID(string cardID)
    {
        if (_cardConfigMap == null) InitializeConfigs();
        
        if (_cardConfigMap.TryGetValue(cardID, out CharacterCardConfig config))
        {
            return config;
        }
        
        Debug.LogWarning($"Character Card Config not found for ID: {cardID}");
        return null;
    }

    // 通過 EntityID 獲取 CharacterEntityConfig (用於獲取頭像和 Live2D 模型)
    public static CharacterEntityConfig GetEntityConfigByID(string entityID)
    {
        if (_entityConfigMap == null) InitializeConfigs();
        
        if (_entityConfigMap.TryGetValue(entityID, out CharacterEntityConfig config))
        {
            return config;
        }
        
        Debug.LogWarning($"Character Entity Config not found for ID: {entityID}");
        return null;
    }

    // ----------------------------------------------------------------------
    // V. 遊戲邏輯函數 (Stamina Recalculation)
    // ----------------------------------------------------------------------

    private static void RecalculateStamina()
    {
        // ... (保持您的體力計算邏輯)
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
            
            // 體力恢復時間戳修正，確保精確計算下一次恢復時間
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