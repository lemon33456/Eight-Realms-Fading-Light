using UnityEngine;
using UnityEngine.UI;
using System;

// 定義左側功能按鈕的類型
public enum LeftFuncType { Camp, Mission, Claim }

public class UI_LeftFunctions : MonoBehaviour
{
    // C# 事件: 當按鈕被點擊時，觸發此事件
    public event Action<LeftFuncType> OnLeftFuncButtonClicked;

    // 連結到 Inspector 的 UI 按鈕
    [SerializeField] private Button btn_Camp;
    [SerializeField] private Button btn_Mission;
    [SerializeField] private Button btn_Claim;

    void Awake()
    {
        // 檢查並註冊三個按鈕的點擊事件
        if (btn_Camp)
            btn_Camp.onClick.AddListener(() => OnLeftFuncButtonClicked?.Invoke(LeftFuncType.Camp));
        
        if (btn_Mission)
            btn_Mission.onClick.AddListener(() => OnLeftFuncButtonClicked?.Invoke(LeftFuncType.Mission));

        if (btn_Claim)
            btn_Claim.onClick.AddListener(() => OnLeftFuncButtonClicked?.Invoke(LeftFuncType.Claim));
    }
}