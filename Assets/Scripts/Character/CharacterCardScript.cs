// CharacterCardScript.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果您使用 TextMeshPro 顯示文字

public class CharacterCardScript : MonoBehaviour
{
    [Header("UI Components")]
    // **【重要】** 請連結角色名稱的 Text 元件
    [SerializeField] private TextMeshProUGUI characterNameText; 
    // **【重要】** 請連結角色圖片的 Image 元件
    [SerializeField] private Image characterIcon;

    private int _characterID;

    /// <summary>
    /// 設置角色卡片的顯示內容。
    /// </summary>
    /// <param name="id">要顯示的角色唯一 ID。</param>
    public void Setup(int id)
    {
        _characterID = id;
        
        // **【未來待辦】** 這裡您需要從全局的資料表讀取 ID 對應的名稱、稀有度、頭像等資訊
        string name = $"角色 #{id}"; // 暫時使用 ID 作為名稱
        
        // 假設您有一個 Asset Bundle 或 Resources 系統來載入圖片
        // Sprite icon = Resources.Load<Sprite>($"Characters/Icon_{id}"); 

        if (characterNameText != null)
        {
            characterNameText.text = name;
        }
        
        // if (characterIcon != null && icon != null)
        // {
        //     characterIcon.sprite = icon;
        // }
    }
    
    // 您可以為卡片添加點擊事件，用於開啟角色詳細資訊介面
    public void OnCardClicked()
    {
        Debug.Log($"點擊了角色 ID: {_characterID}");
        // 這裡可以呼叫一個 Manager 來開啟角色詳細資訊彈窗
    }
}