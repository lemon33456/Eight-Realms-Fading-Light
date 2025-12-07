using UnityEngine;
using UnityEngine.UI; // 為了使用 Image 和 Button 類型
using TMPro; // 為了使用 TextMeshPro
using System; // 為了使用 Action<T> 事件

public class UI_TopLeft : MonoBehaviour
{
    // C# 事件：當頭像區域被點擊時，通知 LobbyManager
    public event Action OnPlayerInfoClicked;

    // === Inspector 連結 ===
    [SerializeField] private Image img_Avatar; // 玩家頭像圖片
    [SerializeField] private TextMeshProUGUI txt_PlayerName; // 玩家名稱
    [SerializeField] private TextMeshProUGUI txt_PlayerLevel; // 玩家等級
    
    // 新增：用於接收點擊的按鈕組件引用
    [SerializeField] private Button btn_PlayerInfo; 

    // 儲存當前數據
    private string currentPlayerName = "N/A";
    private int currentPlayerLevel = 0;

    void Awake()
    {
        // 註冊按鈕點擊事件
        if (btn_PlayerInfo)
        {
            // 當按鈕被點擊時，直接觸發 OnPlayerInfoClicked 事件
            btn_PlayerInfo.onClick.AddListener(() => OnPlayerInfoClicked?.Invoke());
        }
    }

    /// <summary>
    /// 公開方法：用於外部 (例如 LobbyManager) 調用來更新玩家資訊
    /// </summary>
    public void UpdatePlayerInfo(string playerName, int playerLevel, Sprite avatarSprite = null)
    {
        currentPlayerName = playerName;
        currentPlayerLevel = playerLevel;

        // 1. 更新名稱
        if (txt_PlayerName != null)
        {
            txt_PlayerName.text = currentPlayerName;
        }

        // 2. 更新等級
        if (txt_PlayerLevel != null)
        {
            txt_PlayerLevel.text = $"Lv.{currentPlayerLevel.ToString()}";
        }

        // 3. 更新頭像 (如果提供了新的 Sprite)
        if (img_Avatar != null && avatarSprite != null)
        {
            img_Avatar.sprite = avatarSprite;
        }
        
        Debug.Log($"玩家資訊更新: 名稱 {playerName}, 等級 {playerLevel}");
    }
}