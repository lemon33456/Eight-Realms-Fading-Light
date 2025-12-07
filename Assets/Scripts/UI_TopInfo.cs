using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 

// 讓腳本繼承 IPointerDownHandler 和 IPointerUpHandler 介面
public class UI_TopInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // === 資源 TMP 變數宣告區 ===
    [SerializeField] private TextMeshProUGUI txt_DiamondCount; 
    [SerializeField] private TextMeshProUGUI txt_GoldCount;
    [SerializeField] private TextMeshProUGUI txt_StaminaCount; // 僅顯示體力數量 X/Y
    
    // [新增] 體力恢復時間的文本組件
    [Tooltip("Panel_StaminaTimer 內的 TMP 文本組件")]
    [SerializeField] private TextMeshProUGUI txt_StaminaTimer; 

    // === 長按顯示資訊相關變數 ===
    [Header("長按設定")]
    [Tooltip("長按達到此秒數即顯示體力恢復資訊面板")]
    [SerializeField] private float holdDuration = 1.0f; 
    
    [Tooltip("包含恢復時間倒數、將在長按後顯示的資訊面板")]
    [SerializeField] private GameObject staminaTimerInfoPanel; 

    private bool isPointerDown = false;
    private float pointerDownTimer = 0f;
    
    // === 數據暫存 (原腳本內容) ===
    private int currentDiamond = 0;
    private int currentGold = 0;

    void Start()
    {
        // 確保剛開始資訊面板是隱藏的
        if (staminaTimerInfoPanel != null)
        {
            staminaTimerInfoPanel.SetActive(false);
        }
    }

    void Update()
    {
        // 如果滑鼠或手指正在按住
        if (isPointerDown)
        {
            pointerDownTimer += Time.deltaTime;

            // 判斷是否達到長按時間
            if (pointerDownTimer >= holdDuration)
            {
                // 達到長按條件，顯示資訊面板
                ShowStaminaInfo(true);
            }
        }
    }
    
    // === IPointerDownHandler 介面實作：按下事件 (開始計時) ===
    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        pointerDownTimer = 0f; // 重置計時器
    }
    
    // === IPointerUpHandler 介面實作：抬起事件 (隱藏面板) ===
    public void OnPointerUp(PointerEventData eventData)
    {
        // 不論是否達到長按時間，抬起時都隱藏面板
        ShowStaminaInfo(false);
        
        isPointerDown = false;
        pointerDownTimer = 0f; // 重置計時器
    }
    
    // === 資訊顯示控制函數 ===
    /// <summary>
    /// 顯示或隱藏體力恢復時間的面板
    /// </summary>
    private void ShowStaminaInfo(bool show)
    {
        if (staminaTimerInfoPanel != null)
        {
            staminaTimerInfoPanel.SetActive(show); 
        }
    }

    // === 公開方法：資源更新 (原腳本內容) ===
    
    /// <summary>
    /// 公開方法：用於外部調用來更新非體力資源顯示 (金幣/鑽石)
    /// </summary>
    public void UpdateResourceDisplay(int diamond, int gold, int stamina)
    {
        currentDiamond = diamond;
        currentGold = gold;
        
        if (txt_DiamondCount != null)
        {
            txt_DiamondCount.text = currentDiamond.ToString();
        }
        if (txt_GoldCount != null)
        {
            txt_GoldCount.text = currentGold.ToString();
        }
        
        // 這裡不再需要 stamina 參數，但保留方法簽名
        Debug.Log($"資源更新: 鑽石 {diamond}, 金幣 {gold}, 體力 {stamina}");
    }

    /// <summary>
    /// 公開方法：專門用於每秒更新體力數量和倒數計時器
    /// </summary>
    public void UpdateStaminaTimerDisplay(int currentStamina, int maxStamina, int timeRemainingSeconds)
    {
        if (txt_StaminaCount == null) return;
        
        // 1. 永遠更新體力數量 (txt_StaminaCount)
        txt_StaminaCount.text = $"{currentStamina}/{maxStamina}";

        // 2. 更新獨立的倒數計時器文本 (txt_StaminaTimer)
        if (txt_StaminaTimer != null)
        {
            if (currentStamina < maxStamina)
            {
                // 將秒數轉換為 M:SS 格式
                int minutes = timeRemainingSeconds / 60;
                int seconds = timeRemainingSeconds % 60;
                
                // 獨立的恢復時間格式，例如：(5:00 恢復)
                txt_StaminaTimer.text = $"({minutes:D1}:{seconds:D2} 恢復)"; 
            }
            else
            {
                // 體力已滿時，在倒數文本上顯示 MAX 訊息
                txt_StaminaTimer.text = "(MAX)";
            }
        }
    }
}