using UnityEngine;
// 確保所有 using static 都已包含
using static UI_BottomNav;
using static UI_RightFunctions;
using static UI_LeftFunctions;
using static UI_TopRight;
using static UI_TopInfo; 
using static UI_TopLeft; 

public class LobbyManager : MonoBehaviour
{
    // === 變數宣告區：UI 腳本引用 ===
    [SerializeField] private UI_BottomNav bottomNav; 
    [SerializeField] private UI_RightFunctions rightFunctions; 
    [SerializeField] private UI_LeftFunctions leftFunctions;
    [SerializeField] private UI_TopRight topRightFunctions; 
    [SerializeField] private UI_TopInfo topInfo; 
    [SerializeField] private UI_TopLeft topLeftInfo; 

    // === 拍照模式/UI 隱藏控制 ===
    [SerializeField] private GameObject uiElementsToHide; 
    [SerializeField] private GameObject btnExitPhotoMode; 
    private bool isUIHidden = false;
    
    // === 體力計時器變數 ===
    private float timer = 0f; 
    private const float UPDATE_INTERVAL = 1f; // 1 秒更新一次


    void Awake() 
    {
        Debug.Log("Lobby Manager: 初始化中...");
    }

    void Start()
    {
        // 模擬登入成功，從 GameDataService 加載數據
        GameDataService.LoadPlayerData("GUEST_001"); 
        
        // 獲取當前玩家的數據實例
        PlayerData currentData = GameDataService.CurrentPlayerData;

        // === 1. 訂閱所有 UI 區塊的事件 (省略細節，假設已連線) ===
        if (bottomNav != null)
            bottomNav.OnNavButtonClicked += HandleNavClick;
        // ... (其他事件訂閱) ...
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
        
        // === 2. UI 狀態及數據初始化 ===
        
        if (btnExitPhotoMode != null)
        {
            btnExitPhotoMode.SetActive(false);
        }
        
        // 初始化頂部非體力資源顯示
        if (topInfo != null)
        {
            topInfo.UpdateResourceDisplay(currentData.Diamond, currentData.Gold, currentData.Stamina); 
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
    /// 專門處理體力顯示的每秒更新
    /// </summary>
    private void UpdateStaminaPerSecond()
    {
        if (topInfo == null) return;
        
        // 1. 再次從服務層讀取數據 (讀取時會觸發離線體力恢復 RecalculateStamina)
        PlayerData data = GameDataService.CurrentPlayerData;
        
        // 2. 計算當前距離下次恢復還剩多少秒
        int timeRemaining = data.GetTimeRemainingToRecover();
        
        // 3. 更新 UI
        topInfo.UpdateStaminaTimerDisplay(
            data.Stamina, 
            PlayerData.MAX_STAMINA, // 從 PlayerData 讀取 Max 值
            timeRemaining
        );
    }
    
    // --- 事件處理函數 (省略細節，已在先前步驟完成) ---

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

    // 當腳本被銷毀時，解除事件訂閱
    private void OnDestroy()
    {
        // ... (確保所有事件都已解除訂閱) ...
    }
}