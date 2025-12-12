// MasterManager.cs (最終修正 - 補齊 ToggleUI 邏輯)
using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine.InputSystem; 
using static UnityEngine.SceneManagement.SceneManager; 

// 假設 TopRightType, NavType, UI_BottomNav, UI_TopRight 等類型已在別處定義

public class MasterManager : MonoBehaviour
{
    // === 變數宣告區：UI 腳本引用 ===
    [Header("UI References")]
    [SerializeField] private UI_BottomNav bottomNav;
    [SerializeField] private UI_TopRight topRightFunctions;
    // 【關鍵】: 必須確保此 List 包含所有要隱藏的固定 UI 根物件 (例如: BottomNav Canvas, TopRight Canvas等)
    [SerializeField] private List<GameObject> persistentUIRootsToHide; 

    // === 場景管理 ===
    [Header("Scene Management")]
    [SerializeField]
    [Tooltip("角色庫內容場景的名稱")]
    private string RosterSceneName = "CharacterRosterScene"; 
    
    [SerializeField]
    [Tooltip("預設的主頁面內容場景名稱")]
    private string HomeSceneName = "HomeScene"; 
    
    private string _currentContentScene = "";

    // === 拍照模式/UI 隱藏控制 ===
    [Header("Photo Mode")]
    [SerializeField] private GameObject btnExitPhotoMode; 
    [SerializeField] private GraphicRaycaster exitButtonRaycaster; 
    private bool isUIHidden = false;
    [SerializeField] private float autoHideDelay = 3.0f; 
    private float inputDetectedTimer = 0f; 

    // 新版輸入系統的核心引用
    [Header("New Input System")]
    [SerializeField] 
    [Tooltip("將 GameControls.inputactions 檔案拖曳到此處")]
    private InputActionAsset controlsAsset;
    
    private InputActionMap uiActionMap; 
    private InputAction clickAction;
    
    // 體力計時器變數 (略)
    private float timer = 0f; 
    private const float UPDATE_INTERVAL = 1f; 


    // -------------------------------------------------------------
    // Unity 生命週期
    // -------------------------------------------------------------
    
    void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        
        // 取得 UI Action Map 和 Click Action 的實際引用
        if (controlsAsset != null)
        {
            uiActionMap = controlsAsset.FindActionMap("UI"); 
            if (uiActionMap != null) clickAction = uiActionMap.FindAction("Click");
            
            if (clickAction != null)
            {
                clickAction.performed += HandleClickDetected;
                Debug.Log("【輸入DEBUG】: Click Action 訂閱成功。");
            } else { Debug.LogError("【輸入錯誤】: 找不到名為 'Click' 的 Action！"); }
        } else {
            Debug.LogError("【輸入致命錯誤】: controlsAsset 欄位為空 (null)！");
        }
    }
    
    void Start()
    {
        // 2. 訂閱事件 
        if (bottomNav != null)
        {
             bottomNav.OnNavButtonClicked += HandleNavClick;
             Debug.Log("【NavDEBUG】: UI_BottomNav 事件訂閱成功。");
        } else { Debug.LogError("【Nav致命錯誤】: bottomNav 變數為 null！"); }
       
        if (topRightFunctions != null)
        {
             topRightFunctions.OnTopRightButtonClicked += HandleTopRightClick;
             Debug.Log("【NavDEBUG】: UI_TopRight 事件訂閱成功。");
        } else { Debug.LogError("【Nav致命錯誤】: topRightFunctions 變數為 null！"); }
        
        // 啟動時，載入預設的 Home 內容場景
        if (string.IsNullOrEmpty(_currentContentScene))
        {
             if (!string.IsNullOrEmpty(HomeSceneName))
             {
                 SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Additive);
                 _currentContentScene = HomeSceneName;
                 Debug.Log($"【場景DEBUG】: 初始場景 {HomeSceneName} 嘗試加載。");
             }
        }
        
        // 4. 初始化拍照模式按鈕狀態 (預設隱藏)
        if (btnExitPhotoMode != null)
        {
            btnExitPhotoMode.SetActive(false);
        }
        
        // 5. 確保 Raycaster 初始為禁用
        if (exitButtonRaycaster != null)
        {
            exitButtonRaycaster.enabled = false;
        }

        Debug.Log("Master Manager: 持久化 UI 框架已啟動！");
    }

    void Update()
    {
        // 體力計時器邏輯 (略)
        timer += Time.deltaTime;
        if (timer >= UPDATE_INTERVAL)
        {
            timer = 0f;
            // UpdateStaminaPerSecond(); 
        }
        
        // 拍照模式計時邏輯
        if (isUIHidden)
        {
            if (inputDetectedTimer > 0)
            {
                inputDetectedTimer -= Time.deltaTime;
                
                if (inputDetectedTimer <= 0)
                {
                    if (btnExitPhotoMode != null)
                    {
                        btnExitPhotoMode.SetActive(false);
                    }
                    inputDetectedTimer = 0f;
                }
            }
        }
    }
    
    // -------------------------------------------------------------
    // 核心：新版輸入事件處理函數
    // -------------------------------------------------------------
    
    private void HandleClickDetected(InputAction.CallbackContext context)
    {
        if (isUIHidden)
        {
            if (btnExitPhotoMode != null)
            {
                btnExitPhotoMode.SetActive(true);
            }
            inputDetectedTimer = autoHideDelay;
            Debug.Log("【新版偵測成功】: Click Action Performed. 退出按鈕已啟用。");
        }
    }

    // -------------------------------------------------------------
    // 核心：場景內容切換邏輯 (HandleNavClick & SwitchContentScene)
    // -------------------------------------------------------------
    
    private void HandleNavClick(NavType type) 
    {
        Debug.Log($"MasterManager 收到事件，類型為 [{type}]");
        
        switch (type)
        {
            case NavType.Home: 
                SwitchContentScene(HomeSceneName);
                break;
                
            case NavType.Role: 
                SwitchContentScene(RosterSceneName);
                break;
        }
    }

    public void SwitchContentScene(string targetSceneName)
    {
        // ... (場景加載邏輯保持不變) ...
        if (targetSceneName == _currentContentScene)
        {
            Debug.Log($"內容場景已經是 {targetSceneName}，無需切換。");
            return;
        }
        
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("ERROR: 目標場景名稱為空，無法切換！");
            return;
        }

        // 1. 卸載舊的內容場景
        if (!string.IsNullOrEmpty(_currentContentScene))
        {
            Scene sceneToUnload = SceneManager.GetSceneByName(_currentContentScene);
            if (sceneToUnload.IsValid() && sceneToUnload.isLoaded)
            {
                SceneManager.UnloadSceneAsync(_currentContentScene); 
                Debug.Log($"【場景DEBUG】: 卸載舊場景 {_currentContentScene} 成功。");
            }
        }
        
        // 檢查目標場景是否已在 Build Settings 中
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(targetSceneName);
        if (buildIndex == -1)
        {
            Debug.LogError($"【場景致命錯誤】: 場景 '{targetSceneName}' 未在 Build Settings 中註冊！加載失敗！");
            return; 
        } else {
            Debug.Log($"【場景DEBUG】: 場景 '{targetSceneName}' 在 Build Index {buildIndex}，開始加載...");
        }
        
        // 2. 異步加載新的內容場景
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Additive); 
        
        // 3. 更新追蹤變數
        _currentContentScene = targetSceneName;
    }

    // -------------------------------------------------------------
    // 核心：拍照模式/UI 隱藏邏輯 【重要修正】: 補齊 UI 隱藏邏輯
    // -------------------------------------------------------------

    public void ToggleUI() 
    {
        isUIHidden = !isUIHidden;
        
        // 1. 隱藏/顯示 MasterScene 上的所有持久化 UI 根物件
        if (persistentUIRootsToHide != null)
        {
            foreach (GameObject uiRoot in persistentUIRootsToHide)
            {
                if (uiRoot != null)
                {
                    uiRoot.SetActive(!isUIHidden);
                    Debug.Log($"【UI DEBUG】: 持久化 UI 根物件 {uiRoot.name} SetActive(!isUIHidden = {!isUIHidden})");
                }
            }
        } 
        else
        {
            Debug.LogWarning("【UI DEBUG】: persistentUIRootsToHide 列表為空！沒有 UI 根物件被控制。");
        }

        // 2. 隱藏/顯示當前載入的內容場景 UI (中間內容)
        if (!string.IsNullOrEmpty(_currentContentScene))
        {
            Scene contentScene = SceneManager.GetSceneByName(_currentContentScene);
            
            if (contentScene.isLoaded)
            {
                GameObject[] rootObjects = contentScene.GetRootGameObjects();
                
                foreach (GameObject root in rootObjects)
                {
                    // 尋找內容場景 Canvas 的根物件並控制它的 SetActive 狀態
                    Canvas contentCanvas = root.GetComponentInChildren<Canvas>(true);
                    
                    if (contentCanvas != null)
                    {
                        contentCanvas.gameObject.SetActive(!isUIHidden);
                        Debug.Log($"【UI DEBUG】: 內容場景 UI Canvas SetActive(!isUIHidden = {!isUIHidden})");
                        break; 
                    }
                }
            }
        }

        // 3. 控制 Raycaster
        if (exitButtonRaycaster != null)
        {
            exitButtonRaycaster.enabled = isUIHidden;
        }

        // 4. 處理退出拍照模式按鈕 
        if (btnExitPhotoMode != null)
        {
            if (isUIHidden)
            {
                btnExitPhotoMode.SetActive(false); 
                inputDetectedTimer = 0f; 
                uiActionMap?.Enable(); // 啟用偵測
            }
            else
            {
                btnExitPhotoMode.SetActive(false);
                inputDetectedTimer = 0f;
                uiActionMap?.Disable(); // 禁用偵測
            }
        }
        
        Debug.Log($"拍照模式切換到：{(isUIHidden ? "UI 隱藏" : "UI 顯示")}");
    }
    
    // -------------------------------------------------------------
    // 清理與禁用
    // -------------------------------------------------------------

    private void OnDestroy()
    {
        if (clickAction != null)
        {
            clickAction.performed -= HandleClickDetected;
        }
        uiActionMap?.Disable();
        
        // 取消訂閱 Nav 事件 (避免物件銷毀時事件仍連結著)
        if (bottomNav != null)
            bottomNav.OnNavButtonClicked -= HandleNavClick;
        if (topRightFunctions != null)
            topRightFunctions.OnTopRightButtonClicked -= HandleTopRightClick; 
    }
    
    private void OnEnable()
    {
        if(isUIHidden)
        {
            uiActionMap?.Enable();
        }
    }

    private void OnDisable()
    {
        uiActionMap?.Disable();
    }
    
    // -------------------------------------------------------------
    // 輔助函數 (保持不變)
    // -------------------------------------------------------------
    private void HandleTopRightClick(TopRightType type) 
    {
        switch (type)
        {
            case TopRightType.HideUI:
                ToggleUI();
                break;
        }
    }
    
    // 假設有這個函數
    private void UpdateStaminaPerSecond() { } 
    private void HandleRightFuncType(RightFuncType type) { }
    private void HandleLeftFuncType(LeftFuncType type) { }
}