// UI_TopRight.cs 腳本
using UnityEngine;
using UnityEngine.UI;
using System;

// 定義頂部右側按鈕的類型 (新增 MainMenu)
public enum TopRightType { Announcement, Mailbox, HideUI, MainMenu }

public class UI_TopRight : MonoBehaviour
{
    public event Action<TopRightType> OnTopRightButtonClicked;

    // 新增：主選單按鈕的引用
    [SerializeField] private Button btn_Announcement;
    [SerializeField] private Button btn_Mailbox;
    [SerializeField] private Button btn_HideUI;
    [SerializeField] private Button btn_MainMenu; // <--- 新增此行

    void Awake()
    {
        // 檢查並註冊按鈕的點擊事件
        if (btn_Announcement)
            btn_Announcement.onClick.AddListener(() => OnTopRightButtonClicked?.Invoke(TopRightType.Announcement));
        
        if (btn_Mailbox)
            btn_Mailbox.onClick.AddListener(() => OnTopRightButtonClicked?.Invoke(TopRightType.Mailbox));

        if (btn_HideUI)
            btn_HideUI.onClick.AddListener(() => OnTopRightButtonClicked?.Invoke(TopRightType.HideUI));
            
        // 新增：主選單按鈕的事件註冊
        if (btn_MainMenu) 
            btn_MainMenu.onClick.AddListener(() => OnTopRightButtonClicked?.Invoke(TopRightType.MainMenu)); // <--- 新增此行
    }
}