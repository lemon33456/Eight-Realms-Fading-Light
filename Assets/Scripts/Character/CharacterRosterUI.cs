// 檔案：CharacterRosterUI.cs (最終功能兼容版 - 可顯示角色卡並修復單擊)

using UnityEngine;
using System.Collections; // 協程需要這個
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

// 假設 TrainingCrystal, CharacterCardConfig, GameDataService 存在於其他地方
// 為了編譯通過，如果它們未被識別，您可能需要確保它們在正確的命名空間中或是在同一層級可見。

public class CharacterRosterUI : MonoBehaviour 
{
    // 【單例】
    public static CharacterRosterUI Instance { get; private set; }

    [Header("Roster Settings")]
    [SerializeField] private GameObject CharacterCardPrefab; 
    [SerializeField] private Transform ContentParent; 
    
    [Header("Detail Panel")]
    [SerializeField] private CharacterDetailPanel detailPanel; // 連結 Detail Panel 實例

    private TrainingCrystal _pendingCrystal;
    
    void Awake()
    {
        // 實現單例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // 確保配置數據已載入 (如果需要)
        // GameDataService.InitializeConfigs(); 
        DisplayOwnedCharacters(); 
    }

    /// <summary>
    /// 從 GameDataService 讀取數據並在 UI 上顯示角色。
    /// </summary>
    private void DisplayOwnedCharacters() 
    {
        // 🚨 關鍵恢復：嘗試從您的數據服務中獲取實際的角色列表
        
        // 假設 GameDataService.CurrentPlayerData 包含 Crystals
        List<TrainingCrystal> playerCrystals = GameDataService.CurrentPlayerData.Crystals;

        if (playerCrystals == null || playerCrystals.Count == 0)
        {
            Debug.LogWarning("【數據錯誤】: 玩家的 Crystals 清單是空的，或 GameDataService.CurrentPlayerData 為空！無法載入卡片。");
            return;
        }

        ClearRoster();
        
        Debug.Log($"【RosterUI DEBUG】: 成功檢測到 {playerCrystals.Count} 個角色結晶。開始載入流程...");

        foreach (TrainingCrystal crystal in playerCrystals) 
        {
            string cardIdToLookup = crystal.SourceCardID;
            
            // 嘗試獲取角色卡配置
            CharacterCardConfig cardConfig = GameDataService.GetCardConfigByID(cardIdToLookup);

            if (cardConfig == null) 
            {
                Debug.LogError($"【配置錯誤】: 找不到 CardID: {cardIdToLookup} 的配置檔！跳過此角色。");
                continue;
            }
            if (cardConfig.CardArtwork == null) 
            {
                Debug.LogError($"【立繪錯誤】: CardID: {cardIdToLookup} 的 CardArtwork 欄位為空！");
                // 仍繼續載入，但立繪會是空的
            }

            // 實例化卡片
            GameObject card = Instantiate(CharacterCardPrefab, ContentParent);
            
            CharacterCardScript cardScript = card.GetComponent<CharacterCardScript>();
            if (cardScript != null)
            {
                // 設置卡片數據
                cardScript.Setup(cardConfig, crystal); 
            }
            else
            {
                Debug.LogError($"【腳本錯誤】: CharacterCardPrefab 上找不到 CharacterCardScript 元件！");
            }
        }
    }
    
    private void ClearRoster()
    {
        if (ContentParent == null) 
        {
            Debug.LogError("ContentParent 未綁定，無法清除舊卡片！");
            return;
        }
        foreach (Transform child in ContentParent)
        {
            Destroy(child.gameObject);
        }
    }
    
    /// <summary>
    /// 供 CharacterCardScript 調用，顯示詳細資訊彈窗。
    /// </summary>
    public void ShowDetailPanel(TrainingCrystal crystal)
    {
        if (detailPanel == null)
        {
            Debug.LogError("Detail Panel 未連結！請檢查 Inspector 中的 RosterUI 綁定。");
            return;
        }
        
        _pendingCrystal = crystal; 

        // 【單擊修復】: 啟動協程，將激活邏輯推遲到下一幀的渲染階段之後。
        StartCoroutine(DelayedSetupAndShowCoroutine());
        
        Debug.Log($"[UI Flow]: 成功傳遞數據，並啟動 Coroutine 延遲激活。");
    }
    
    // 實際執行 Detail Panel 設置與顯示的延遲協程
    private IEnumerator DelayedSetupAndShowCoroutine()
    {
        // 等待當前幀的末尾。確保所有輸入事件和 Update() 循環都已完成。
        yield return new WaitForEndOfFrame();
        
        if (_pendingCrystal != null && detailPanel != null)
        {
            detailPanel.SetupAndShow(_pendingCrystal);
            Debug.Log($"[UI Flow]: Coroutine：成功在下一幀激活 Detail Panel。");
            _pendingCrystal = null; // 清除數據
        }
    }

    void OnDestroy()
    {
        // 清理單例引用
        if (Instance == this)
        {
            Instance = null;
        }
    }
}