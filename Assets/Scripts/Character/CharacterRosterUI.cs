// CharacterRosterUI.cs
using UnityEngine;
using System.Collections.Generic;

public class CharacterRosterUI : MonoBehaviour 
{
    [Header("Roster Settings")]
    // **【重要】** 在 Inspector 中將角色卡片的預製件連結到此處
    [SerializeField] private GameObject CharacterCardPrefab; 
    
    // **【重要】** 角色卡片列表的父物件 (Scroll View 的 Content)
    [SerializeField] private Transform ContentParent; 

    void Start() 
    {
        // 無需再處理返回按鈕的監聽器，因為返回由持久化 UI 處理。
        
        DisplayOwnedCharacters();
    }

    /// <summary>
    /// 從 GameDataService 讀取數據並在 UI 上顯示角色。
    /// </summary>
    private void DisplayOwnedCharacters() 
    {
        List<int> ownedIDs = GameDataService.GetOwnedCharacterIDs();

        if (ownedIDs.Count == 0)
        {
            Debug.LogWarning("玩家沒有任何角色。");
            return;
        }

        if (CharacterCardPrefab == null || ContentParent == null)
        {
            Debug.LogError("角色卡片預製件或 ContentParent 未連結！無法顯示角色庫。");
            return;
        }
        
        Debug.Log($"開始載入 {ownedIDs.Count} 個角色...");
        
        // 在載入新卡片前，先清除 ContentParent 下的所有舊物件（如果切換時未完全卸載場景，雖然 Additive 模式下通常會）
        // foreach (Transform child in ContentParent) { Destroy(child.gameObject); } 

        foreach (int charID in ownedIDs) 
        {
            // 實例化角色卡片
            GameObject card = Instantiate(CharacterCardPrefab, ContentParent);
            
            // 設置角色卡片數據 (需要 CharacterCardScript.cs)
            CharacterCardScript cardScript = card.GetComponent<CharacterCardScript>();
            if (cardScript != null)
            {
                cardScript.Setup(charID);
            }
        }
    }
}