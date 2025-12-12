// UI_BottomNav.cs
using UnityEngine;
using UnityEngine.UI;
using System;

// 為了讓 UI_BottomNav 知道點擊的是哪個按鈕
public enum NavType { Role, Team, Home, Inventory, Recruit }

public class UI_BottomNav : MonoBehaviour
{
    // C# 事件: 當按鈕被點擊時，會觸發這個事件，並傳遞點擊的類型 NavType
    public event Action<NavType> OnNavButtonClicked;

    [SerializeField] private Button btn_Role;
    [SerializeField] private Button btn_Team;
    [SerializeField] private Button btn_Home;
    [SerializeField] private Button btn_Inventory;
    [SerializeField] private Button btn_Recruit;

    // Awake 在遊戲開始時運行一次，用於設置按鈕的監聽器
    void Awake()
    {
        // ------------------------------------------------------------------
        // 【診斷 A 檢查點】確認按鈕元件是否連結並觸發事件
        // ------------------------------------------------------------------

        if (btn_Role)
            btn_Role.onClick.AddListener(() =>
            {
                Debug.Log("DEBUG A: Role 按鈕被點擊，正在發射 NavType.Role 事件！");
                OnNavButtonClicked?.Invoke(NavType.Role);
            });

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