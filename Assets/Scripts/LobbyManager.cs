using UnityEngine;
using System; // 雖然此腳本內未直接使用，但習慣上會保留
using System.IO; // 雖然此腳本內未直接使用，但習慣上會保留

// 注意：請移除程式碼開頭所有多餘的 using static 指令，因為你已經使用 [SerializeField] 引用腳本，不應該使用 static 方式。
// using static UI_BottomNav; 
// 等等... 這些都應該移除

public class LobbyManager : MonoBehaviour
{
    // === 變數宣告區：UI 腳本引用 ===
    [Header("UI References")]
    [SerializeField] private UI_BottomNav bottomNav; 
    [SerializeField] private UI_RightFunctions rightFunctions; 
    [SerializeField] private UI_LeftFunctions leftFunctions;
    [SerializeField] private UI_TopRight topRightFunctions; 
    [SerializeField] private UI_TopInfo topInfo; 
    [SerializeField] private UI_TopLeft topLeftInfo; 

    // === 拍照模式/UI 隱藏控制 ===
    [Header("Photo Mode")]
    [SerializeField] private GameObject uiElementsToHide; 
    [SerializeField] private GameObject btnExitPhotoMode; 
    private bool isUIHidden = false;
    
    // === 體力計時器變數 ===
    [Header("Stamina Timer")]
    private float timer = 0f; 
    private const float UPDATE_INTERVAL = 1f; // 1 秒更新一次

    // -------------------------------------------------------------
    // Unity 生命週期
    // -------------------------------------------------------------

    void Awake() 
    {
        Debug.Log("Lobby Manager: 初始化中...");
        
        // 【優化點】：確保在所有 Start 函數運行前，數據服務已被載入
        // 由於 GameDataService 是靜態類別，這個呼叫將強制它在第一次被存取前初始化。
        // 但我們將載入邏輯放在 Start() 以確保 UI 組件已準備好。
    }

    void Start()
    {
        // 1. 【關鍵】：模擬登入成功，從 GameDataService 加載數據
        // 使用 DEFAULT_USER_ID 載入，讓 GameDataService 決定是讀檔還是創建新檔。
        GameDataService.LoadPlayerData(GameDataService.DEFAULT_USER_ID); 
        
        // 獲取當前玩家的數據實例 (數據已被 RecalculateStamina 修正)
        PlayerData currentData = GameDataService.CurrentPlayerData;

        // === 2. 訂閱所有 UI 區塊的事件 (維持不變) ===
        if (bottomNav != null)
            bottomNav.OnNavButtonClicked += HandleNavClick;
        if (rightFunctions != null)
            rightFunctions.OnRightFuncButtonClicked += HandleRightFuncClick;
        if (leftFunctions != null)
            leftFunctions.OnLeftFuncButtonClicked += HandleLeftFuncClick;
        if (topRightFunctions != null)
            topRightFunctions.OnTopRightButtonClicked += HandleTopRightClick;
        if (topLeftInfo != null)
        {
            topLeftInfo.OnPlayerInfoClicked += HandlePlayerInfoClick;
        }
        
        // === 3. UI 狀態及數據初始化 ===
        
        if (btnExitPhotoMode != null)
        {
            btnExitPhotoMode.SetActive(false);
        }
        
        // 初始化頂部非體力資源顯示
        if (topInfo != null)
        {
            // 注意：這裡只更新 Gold 和 Diamond，Stamina 會在 UpdateStaminaPerSecond 中更新
            topInfo.UpdateResourceDisplay(
                currentData.Diamond, 
                currentData.Gold, 
                currentData.Stamina
            ); 
        }
        
        // 初始化頂部左側玩家資訊
        if (topLeftInfo != null)
        {
            topLeftInfo.UpdatePlayerInfo(currentData.PlayerName, currentData.PlayerLevel); 
        }
        
        // [關鍵]：在 Start() 時就初始化體力計時器顯示
        UpdateStaminaPerSecond(); 

        Debug.Log("Lobby Manager: 遊戲大廳已啟動！");
    }

    void Update()
    {
        // 體力計時器邏輯：每秒執行一次
        timer += Time.deltaTime;
        if (timer >= UPDATE_INTERVAL)
        {
            timer = 0f;
            UpdateStaminaPerSecond(); 
        }
    }
    
    /// <summary>
    /// 【關鍵】確保在應用程式關閉前，數據被儲存。
    /// </summary>
    private void OnApplicationQuit()
    {
        // 遊戲退出時，強制進行一次體力恢復計算（確保時間戳最新）並存檔
        if (GameDataService.CurrentPlayerData != null)
        {
            // 存檔會觸發最新的數據序列化
            GameDataService.SavePlayerData(); 
        }
        
        // 移除所有事件訂閱，防止內存洩露
        UnsubscribeEvents();
    }
    
    // -------------------------------------------------------------
    // 數據與 UI 更新函數
    // -------------------------------------------------------------

    /// <summary>
    /// 專門處理體力顯示的每秒更新
    /// </summary>
    private void UpdateStaminaPerSecond()
    {
        if (topInfo == null) return;
        
        // 1. 再次從服務層讀取數據 (讀取時會觸發離線體力恢復 RecalculateStamina)
        // 透過這個呼叫，我們自動確保了體力數據是最新的。
        PlayerData data = GameDataService.CurrentPlayerData;
        
        // 2. 計算當前距離下次恢復還剩多少秒
        int timeRemaining = data.GetTimeRemainingToRecover();
        
        // 3. 更新 UI
        topInfo.UpdateStaminaTimerDisplay(
            data.Stamina, 
            PlayerData.MAX_STAMINA, // 從 PlayerData 讀取 Max 值
            timeRemaining
        );
        
        // 額外更新貨幣顯示 (僅當它們有變動時才需要，但這裡為保險起見可以再次呼叫)
        // 更好的做法是：在所有會影響 Gold/Diamond 的操作後，才呼叫 UI 更新。
        topInfo.UpdateResourceDisplay(data.Diamond, data.Gold, data.Stamina);
    }
    
    // -------------------------------------------------------------
    // 事件處理函數與輔助
    // -------------------------------------------------------------

    // (事件處理函數 HandleNavClick, HandleRightFuncClick, 等等... 保持不變)
    private void HandleNavClick(NavType type) 
    {
        Debug.Log($"導航到 [{type}] 畫面");
    }

    private void HandleRightFuncClick(RightFuncType type) 
    {
        Debug.Log($"導航到 [{type}] 功能");
    }
    
    private void HandleLeftFuncClick(LeftFuncType type) 
    {
        Debug.Log($"導航到 [{type}] 功能");
    }

    private void HandleTopRightClick(TopRightType type) 
    {
        if (type == TopRightType.HideUI)
        {
            ToggleUI();
        } else {
            Debug.Log($"開啟 [{type}] 介面");
        }
    }
    
    private void HandlePlayerInfoClick() 
    {
        Debug.Log("玩家頭像區域被點擊：導航到 [個人資料] 或 [設置介面]");
    }
    
    public void ToggleUI() 
    {
        isUIHidden = !isUIHidden;

        if (uiElementsToHide != null)
        {
            uiElementsToHide.SetActive(!isUIHidden);
            
            if (btnExitPhotoMode != null)
            {
                btnExitPhotoMode.SetActive(isUIHidden); 
            }

            Debug.Log("UI 狀態切換: " + (isUIHidden ? "已隱藏 (拍照模式)" : "已顯示"));
        }
        else
        {
            Debug.LogError("UI Elements To Hide 容器未連結，無法執行隱藏操作！");
        }
    }
    
    /// <summary>
    /// 統一解除事件訂閱，防止內存洩露
    /// </summary>
    private void UnsubscribeEvents()
    {
        if (bottomNav != null)
            bottomNav.OnNavButtonClicked -= HandleNavClick;
        if (rightFunctions != null)
            rightFunctions.OnRightFuncButtonClicked -= HandleRightFuncClick;
        if (leftFunctions != null)
            leftFunctions.OnLeftFuncButtonClicked -= HandleLeftFuncClick;
        if (topRightFunctions != null)
            topRightFunctions.OnTopRightButtonClicked -= HandleTopRightClick;
        if (topLeftInfo != null)
        {
            topLeftInfo.OnPlayerInfoClicked -= HandlePlayerInfoClick;
        }
    }
    
    // 當腳本被銷毀時，解除事件訂閱 (多一層保障，但在 OnApplicationQuit 已執行)
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}