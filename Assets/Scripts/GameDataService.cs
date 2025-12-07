using UnityEngine;
using System;
using System.IO; // 新增：用於檔案讀寫

public static class GameDataService
{
    private static PlayerData _currentPlayerData;
    
    // [關鍵] 定義玩家數據的存檔路徑（使用 Unity 的持久化資料路徑，確保安全存儲）
    private static readonly string SAVE_FILE_PATH = Path.Combine(Application.persistentDataPath, "player_data.json");
    
    // 定義一個模擬的初始用戶ID
    public const string DEFAULT_USER_ID = "PLAYER_001";

    public static PlayerData CurrentPlayerData
    {
        get
        {
            if (_currentPlayerData == null)
            {
                // [關鍵修正]: 如果為 null，強制嘗試載入預設數據，而不是僅返回錯誤數據
                LoadPlayerData(DEFAULT_USER_ID); 
            }
            
            RecalculateStamina();
            return _currentPlayerData;
        }
    }

    /// <summary>
    /// [未來後端整合點]：模擬登入成功後，根據 UserID 從本機或伺服器加載玩家數據。
    /// </summary>
    /// <param name="userID">玩家的唯一ID。</param>
    public static void LoadPlayerData(string userID)
    {
        Debug.Log($"GameDataService: 嘗試加載 UserID [{userID}] 的玩家數據...");

        // -------------------------------------------------------------
        // [核心載入邏輯]：模擬從數據庫讀取 (目前為本地 JSON 檔案)
        // -------------------------------------------------------------
        
        if (File.Exists(SAVE_FILE_PATH))
        {
            try
            {
                string json = File.ReadAllText(SAVE_FILE_PATH);
                // 使用 JsonUtility 反序列化 (從 JSON 轉回 PlayerData 物件)
                _currentPlayerData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log($"GameDataService: 數據從存檔加載成功！玩家: {_currentPlayerData.PlayerName}, 等級: {_currentPlayerData.PlayerLevel}");
            }
            catch (Exception e)
            {
                Debug.LogError($"GameDataService: 讀檔失敗，創建新數據。錯誤: {e.Message}");
                // 讀檔失敗，創建新帳號
                InitializeNewPlayerData(userID);
            }
        }
        else 
        {
            Debug.LogWarning("GameDataService: 找不到本地存檔，創建新帳號數據。");
            // 找不到存檔，創建新帳號
            InitializeNewPlayerData(userID);
        }

        // 確保在載入成功後，即使數據未變更，也進行一次體力恢復計算
        RecalculateStamina();
        // [新增] 在載入後主動呼叫 UI 更新 (如果你的 UI 系統依賴事件，則發射事件)
        // 這裡你需要確保你的 UI 腳本能夠訂閱或接收到更新通知。
    }

    /// <summary>
    /// [核心存檔邏輯]：將當前玩家數據儲存到本機檔案 (模擬儲存到伺服器)。
    /// </summary>
    public static void SavePlayerData()
    {
        if (_currentPlayerData == null)
        {
            Debug.LogError("無法儲存：CurrentPlayerData 為空。");
            return;
        }
        
        // 1. 確保體力時間標記更新到當前時間 (重要！防止體力恢復跳躍)
        // 只有在數據變更時才需要手動調整，但為保險起見，可以在這裡更新 LastStaminaUpdateTimeTicks
        // 我們依賴 RecalculateStamina 裡的邏輯來處理時間戳。

        // 2. 將 PlayerData 物件序列化為 JSON 字串
        string json = JsonUtility.ToJson(_currentPlayerData);

        // 3. 寫入檔案
        try
        {
            File.WriteAllText(SAVE_FILE_PATH, json);
            // [未來] 這裡將替換為：向伺服器發送 POST 請求，傳遞 JSON 數據。
            Debug.Log($"GameDataService: 數據成功儲存到 {SAVE_FILE_PATH}");
        }
        catch (Exception e)
        {
            Debug.LogError($"GameDataService: 存檔失敗。錯誤: {e.Message}");
        }
    }
    
    // ==============================================================
    // 輔助函數
    // ==============================================================
    
    private static void InitializeNewPlayerData(string userID)
    {
        // [關鍵] 這是新帳號的起始數據
        _currentPlayerData = new PlayerData(
            userID, 
            "新手冒險者", 
            500,    // Gold 起始值
            5,      // Diamond 起始值
            PlayerData.MAX_STAMINA, // 滿體力
            1       // 等級 1
        );
        // 新帳號建立後，立即存檔
        SavePlayerData(); 
    }

    // [體力恢復函數保持不變，但請注意它會修改 _currentPlayerData 的值]
    private static void RecalculateStamina()
    {
        // ... (保持原有的 RecalculateStamina 邏輯) ...
        // ... 體力恢復邏輯 ...
        
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