// 檔案：CharacterDetailPanel.cs (兼容回溯版 - 移除 MasterManager 依賴)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class CharacterDetailPanel : MonoBehaviour
{
    // --- UI 綁定 ---
    [Header("UI Bindings")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI levelText;
    
    // 屬性文本
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI physATKText;
    [SerializeField] private TextMeshProUGUI magicATKText;
    
    [SerializeField] private Button closeButton; // 關閉按鈕
    
    // Canvas Group 引用
    private CanvasGroup _canvasGroup; 
    
    // --- 數據儲存 ---
    private TrainingCrystal _currentCrystal;
    private CharacterCardConfig _currentConfig;
    
    void Awake()
    {
        // 獲取 Canvas Group 元件
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            Debug.LogWarning("[Detail Panel]: 缺少 Canvas Group 元件，如果 UI 無法顯示，請手動添加。");
        }
        
        // 確保關閉按鈕功能
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }
        
        // 彈窗初始化時先隱藏
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 設置並顯示詳細資訊彈窗。
    /// </summary>
    public void SetupAndShow(TrainingCrystal crystal)
    {
        _currentCrystal = crystal;
        
        _currentConfig = GameDataService.GetCardConfigByID(crystal.SourceCardID);

        if (_currentConfig == null)
        {
            Debug.LogError($"[Detail Panel Error]: 找不到 Card ID: {crystal.SourceCardID} 的配置檔！");
            return;
        }
        
        DisplayBasicInfo();
        DisplayCalculatedStats(); 
        
        gameObject.SetActive(true);

        // 【核心修復】：強制 Canvas Group Alpha 為 1，確保可見
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
        }
        
        Canvas.ForceUpdateCanvases(); 
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    
    /// <summary>
    /// 顯示名稱、立繪、等級和稀有度等基礎資訊。
    /// </summary>
    private void DisplayBasicInfo()
    {
        if (nameText != null) 
            nameText.text = _currentConfig.LinkedEntity.CharacterName;
            
        if (artworkImage != null) 
        {
            // 【修復】：確保 Image Color 的 Alpha 值為 1
            Color imageColor = artworkImage.color;
            imageColor.a = 1f;
            artworkImage.color = imageColor;
            
            artworkImage.sprite = _currentConfig.CardArtwork;
        }
            
        if (rarityText != null) 
            rarityText.text = $"稀有度: {_currentConfig.CardRarity.ToString()}";
        
        if (levelText != null) 
            levelText.text = $"等級: Lv.{_currentCrystal.Level}"; 
    }
    
    /// <summary>
    /// 呼叫計算器服務，計算並顯示當前屬性。(假設 CharacterStatsCalculator 存在)
    /// </summary>
    private void DisplayCalculatedStats()
    {
        int level = _currentCrystal.Level;
        // 假設 CharacterStatsCalculator.CalculateCurrentStat 存在
        int fixedBaseHP = 1000; 
        int currentHP = CharacterStatsCalculator.CalculateCurrentStat(fixedBaseHP, _currentConfig.BaseHPGrowth, level);
        int currentPhysATK = CharacterStatsCalculator.CalculateCurrentStat(_currentConfig.BaseAttackPhys, _currentConfig.BaseATK_PhysGrowth, level);
        float magicGrowthRate = 5f; 
        int currentMagicATK = CharacterStatsCalculator.CalculateCurrentStat(_currentConfig.BaseAttackMagic, magicGrowthRate, level);
        
        if (hpText != null) hpText.text = $"HP: {currentHP}";
        if (physATKText != null) physATKText.text = $"物攻: {currentPhysATK}";
        if (magicATKText != null) physATKText.text = $"魔攻: {currentMagicATK}";
    }

    /// <summary>
    /// 隱藏彈窗
    /// </summary>
    public void HidePanel()
    {
        // 隱藏時可以選擇將 Alpha 設為 0
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
        }

        // 🚨 移除了 MasterManager 輸入解鎖邏輯

        gameObject.SetActive(false);
        _currentCrystal = null;
        _currentConfig = null;
    }
}