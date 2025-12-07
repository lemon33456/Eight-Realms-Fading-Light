using UnityEngine;
using UnityEngine.UI;
using System;

// 為了讓 UI_BottomNav 知道點擊的是哪個按鈕
public enum NavType { Role, Team, Home, Inventory, Recruit }

public class UI_BottomNav : MonoBehaviour
{
    // C# 事件: 當按鈕被點擊時，會觸發這個事件，並傳遞點擊的類型 NavType
    public event Action<NavType> OnNavButtonClicked;

    // 連結到 Inspector 的 UI 按鈕。 [SerializeField] 讓私有變數能在 Inspector 中顯示並拖曳賦值
    [SerializeField] private Button btn_Role;
    [SerializeField] private Button btn_Team;
    [SerializeField] private Button btn_Home;
    [SerializeField] private Button btn_Inventory;
    [SerializeField] private Button btn_Recruit;

    // Awake 在遊戲開始時運行一次，用於設置按鈕的監聽器
    void Awake()
    {
        // 檢查所有按鈕是否已在 Inspector 中連結
        if (btn_Role)
            // 當 btn_Role 被點擊時，觸發 OnNavButtonClicked 事件，並傳遞 NavType.Role
            btn_Role.onClick.AddListener(() => OnNavButtonClicked?.Invoke(NavType.Role));

        if (btn_Team)
            btn_Team.onClick.AddListener(() => OnNavButtonClicked?.Invoke(NavType.Team));
        
        if (btn_Home)
            btn_Home.onClick.AddListener(() => OnNavButtonClicked?.Invoke(NavType.Home));

        if (btn_Inventory)
            btn_Inventory.onClick.AddListener(() => OnNavButtonClicked?.Invoke(NavType.Inventory));

        if (btn_Recruit)
            btn_Recruit.onClick.AddListener(() => OnNavButtonClicked?.Invoke(NavType.Recruit));
    }
}