// 檔案：CharacterCardScript.cs (最終完整版，手動實現按鈕反饋)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// 實現 IPointerDownHandler 和 IPointerUpHandler 接口
public class CharacterCardScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI Component Bindings")]
    
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI rarityText;
    [SerializeField] private TextMeshProUGUI physATKText;
    [SerializeField] private TextMeshProUGUI magicATKText;
    [SerializeField] private Image characterIcon;
    
    private TrainingCrystal _crystalData;
    private Vector2 _pointerDownPosition; 
    private float _clickTolerance = 5f; 
    
    // 【按鈕反饋參數】
    private RectTransform _rectTransform;
    private Vector3 _originalScale = Vector3.one;
    private Vector3 _pressedScale = new Vector3(0.95f, 0.95f, 0.95f); // 按下時縮小 5%

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        if (_rectTransform != null)
        {
            _originalScale = _rectTransform.localScale;
        }
    }
    
    // ----------------------------------------------------------------------
    // IPointerDownHandler (按下：處理點擊起始，並提供按鈕反饋)
    // ----------------------------------------------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        // 記錄按下的起始位置
        _pointerDownPosition = eventData.position;
        
        // 【按鈕反饋】: 按下時縮小卡片
        SetScale(_pressedScale);
        Debug.Log($"【點擊偵測】: OnPointerDown 觸發。時間: {Time.frameCount}，執行縮放反饋。");
    }

    // ----------------------------------------------------------------------
    // IPointerUpHandler (抬起：判斷是否為點擊，並觸發邏輯，恢復反饋)
    // ----------------------------------------------------------------------
    public void OnPointerUp(PointerEventData eventData)
    {
        // 【按鈕反饋】: 恢復原始大小
        SetScale(_originalScale);

        // 判斷是否在容忍距離內 (即不是拖曳)
        if (Vector2.Distance(_pointerDownPosition, eventData.position) > _clickTolerance)
        {
            Debug.Log($"【點擊偵測】: OnPointerUp 視為拖曳，不開彈窗。");
            return;
        }
        
        // 點擊事件已確認
        Debug.LogWarning($"【點擊偵測】: OnPointerUp 確認為點擊。時間: {Time.frameCount}");

        if (_crystalData == null)
        {
            Debug.LogError("[CardScript ERROR]: Crystal 數據為空，無法傳遞！");
            return;
        }

        if (CharacterRosterUI.Instance != null)
        {
            CharacterRosterUI.Instance.ShowDetailPanel(_crystalData);
            Debug.Log($"[UI Flow]: OnPointerUp 成功呼叫 RosterUI。");
        }
        else
        {
            Debug.LogError("CharacterRosterUI 單例未找到！");
        }
    }

    /// <summary>
    /// 設置卡片顯示的內容
    /// </summary>
    public void Setup(CharacterCardConfig config, TrainingCrystal crystal)
    {
        if (characterIcon != null) 
            characterIcon.sprite = config.CardArtwork;
        
        // ... (其他 Text 設置)
        
        _crystalData = crystal; 
    }
    
    /// <summary>
    /// 【新增輔助函數】: 執行縮放變換，用於模擬按鈕反饋
    /// </summary>
    private void SetScale(Vector3 targetScale)
    {
        if (_rectTransform != null)
        {
            _rectTransform.localScale = targetScale;
        }
    }
}