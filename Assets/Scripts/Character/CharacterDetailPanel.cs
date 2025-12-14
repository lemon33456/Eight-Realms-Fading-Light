using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CharacterDetailPanel : MonoBehaviour
{
    // --- UI 綁定 ---
    [Header("UI Bindings")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Stats Text")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI physATKText;
    [SerializeField] private TextMeshProUGUI magicATKText;

    [SerializeField] private Button closeButton;

    // CanvasGroup
    private CanvasGroup _canvasGroup;

    // --- Data ---
    private TrainingCrystal _currentCrystal;
    private CharacterCardConfig _currentConfig;

    // ----------------------------------------------------
    // Unity Lifecycle
    // ----------------------------------------------------
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            Debug.LogError("[CharacterDetailPanel] 缺少 CanvasGroup 元件！");
            return;
        }

        // 綁定關閉按鈕
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }

        // 根物件預設為 Inactive，所以 Awake 時不用再隱藏 CanvasGroup
    }

    // ----------------------------------------------------
    // Public API
    // ----------------------------------------------------
    public void SetupAndShow(TrainingCrystal crystal)
    {
        if (crystal == null)
        {
            Debug.LogError("[Detail Panel]: 傳入的 TrainingCrystal 為空！");
            return;
        }

        _currentCrystal = crystal;
        _currentConfig = GameDataService.GetCardConfigByID(crystal.SourceCardID);

        if (_currentConfig == null)
        {
            Debug.LogError($"[Detail Panel]: 找不到 Card ID: {crystal.SourceCardID} 的配置！");
            return;
        }

        DisplayBasicInfo();
        DisplayCalculatedStats();

        // 啟用 GameObject 並開啟 CanvasGroup 互動
        gameObject.SetActive(true);
        SetVisibleState();

        // 清除 UI 選取，避免 Input System 卡焦點
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("[Detail Panel]: SetupAndShow 成功顯示角色資訊。");
    }

    public void HidePanel()
    {
        // 隱藏 GameObject
        SetHiddenState();
        gameObject.SetActive(false);

        _currentCrystal = null;
        _currentConfig = null;

        Debug.Log("[Detail Panel]: HidePanel，已隱藏。");
    }

    // ----------------------------------------------------
    // UI State Control
    // ----------------------------------------------------
    private void SetVisibleState()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private void SetHiddenState()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    // ----------------------------------------------------
    // UI Rendering
    // ----------------------------------------------------
    private void DisplayBasicInfo()
    {
        if (nameText != null)
            nameText.text = "Character Name Placeholder";

        if (rarityText != null)
            rarityText.text = "Rarity Placeholder";

        if (levelText != null)
            levelText.text = $"等級: Lv.{_currentCrystal.Level}";

        if (artworkImage != null)
        {
            Color c = artworkImage.color;
            c.a = 1f;
            artworkImage.color = c;

            artworkImage.sprite = _currentConfig.CardArtwork;

            if (!artworkImage.gameObject.activeInHierarchy)
                artworkImage.gameObject.SetActive(true);
        }
    }

    private void DisplayCalculatedStats()
    {
        if (hpText != null) hpText.text = "HP: 1000";
        if (physATKText != null) physATKText.text = "物攻: 100";
        if (magicATKText != null) magicATKText.text = "魔攻: 50";
    }
}
