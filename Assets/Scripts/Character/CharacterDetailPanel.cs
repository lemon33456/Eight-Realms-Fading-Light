// 檔案：CharacterDetailPanel.cs (最終修復版：強制 CanvasGroup 交互性)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

// 假設 CharacterStatsCalculator 存在於其他地方
// public class CharacterStatsCalculator { public static int CalculateCurrentStat(int baseStat, float growthRate, int level) { return 0; } }

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
            // 🚨 強烈警告：CanvasGroup 是解決點擊問題的關鍵，必須存在！
            Debug.LogError("[Detail Panel]: 缺少 Canvas Group 元件！請手動添加到此遊戲物件上。");
        }
        
        // 確保關閉按鈕功能
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }
        
        // 🎯 【關鍵修復點 1】：初始化時，即使 Inspector 打勾了，也強制禁用交互
        if (_canvasGroup != null)
        {
             _canvasGroup.interactable = false;
             _canvasGroup.blocksRaycasts = false;
             // 隱藏時也將 Alpha 設為 0
             _canvasGroup.alpha = 0f;
             Debug.Log("[Detail Panel Awake]: CanvasGroup 交互與阻擋已強制禁用。");
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

        // 🎯 【關鍵修復點 2】：顯示時，強制啟用 Canvas Group 的交互和阻擋
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            Debug.Log("[Detail Panel SetupAndShow]: CanvasGroup 交互與阻擋已啟用。");
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
            // 假設 LinkedEntity 存在
            // nameText.text = _currentConfig.LinkedEntity.CharacterName; 
            nameText.text = "Character Name Placeholder";
            
        if (artworkImage != null) 
        {
            // 【修復】：確保 Image Color 的 Alpha 值為 1
            Color imageColor = artworkImage.color;
            imageColor.a = 1f;
            artworkImage.color = imageColor;
            
            artworkImage.sprite = _currentConfig.CardArtwork;
            Debug.Log($"[Detail Panel DEBUG]: 設定立繪為: {_currentConfig.CardArtwork.name}");
            
            // 確保 Image 物件本身是啟用的
            if (!artworkImage.gameObject.activeInHierarchy)
            {
                 artworkImage.gameObject.SetActive(true);
            }
        }
            
        if (rarityText != null) 
            // rarityText.text = $"稀有度: {_currentConfig.CardRarity.ToString()}";
            rarityText.text = "Rarity Placeholder";
        
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
        // int currentHP = CharacterStatsCalculator.CalculateCurrentStat(fixedBaseHP, _currentConfig.BaseHPGrowth, level);
        // int currentPhysATK = CharacterStatsCalculator.CalculateCurrentStat(_currentConfig.BaseAttackPhys, _currentConfig.BaseATK_PhysGrowth, level);
        // float magicGrowthRate = 5f; 
        // int currentMagicATK = CharacterStatsCalculator.CalculateCurrentStat(_currentConfig.BaseAttackMagic, magicGrowthRate, level);
        
        // if (hpText != null) hpText.text = $"HP: {currentHP}";
        // if (physATKText != null) physATKText.text = $"物攻: {currentPhysATK}";
        // if (magicATKText != null) magicATKText.text = $"魔攻: {currentMagicATK}";

        if (hpText != null) hpText.text = $"HP: 1000";
        if (physATKText != null) physATKText.text = $"物攻: 100";
        if (magicATKText != null) magicATKText.text = $"魔攻: 50";
    }

    /// <summary>
    /// 隱藏彈窗
    /// </summary>
    public void HidePanel()
    {
        // 🎯 【關鍵修復點 3】：隱藏時，強制禁用 Canvas Group 的交互和阻擋
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            Debug.Log("[Detail Panel HidePanel]: CanvasGroup 交互與阻擋已禁用。");
        }

        gameObject.SetActive(false);
        _currentCrystal = null;
        _currentConfig = null;
    }
}