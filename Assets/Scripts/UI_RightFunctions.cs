using UnityEngine;
using UnityEngine.UI;
using System;

// 定義右側功能按鈕的類型
public enum RightFuncType { Training, Story, Shop, Achievement }

public class UI_RightFunctions : MonoBehaviour
{
    // C# 事件: 當按鈕被點擊時，觸發此事件，並傳遞點擊的類型 RightFuncType
    public event Action<RightFuncType> OnRightFuncButtonClicked;

    // 連結到 Inspector 的 UI 按鈕。您需要在 Unity 中拖曳連結它們
    [SerializeField] private Button btn_Training;
    [SerializeField] private Button btn_Story;
    [SerializeField] private Button btn_Shop;
    [SerializeField] private Button btn_Achievement;

    void Awake()
    {
        // 檢查並註冊四個按鈕的點擊事件
        if (btn_Training)
            btn_Training.onClick.AddListener(() => OnRightFuncButtonClicked?.Invoke(RightFuncType.Training));
        
        if (btn_Story)
            btn_Story.onClick.AddListener(() => OnRightFuncButtonClicked?.Invoke(RightFuncType.Story));

        if (btn_Shop)
            btn_Shop.onClick.AddListener(() => OnRightFuncButtonClicked?.Invoke(RightFuncType.Shop));

        if (btn_Achievement)
            btn_Achievement.onClick.AddListener(() => OnRightFuncButtonClicked?.Invoke(RightFuncType.Achievement));
    }
}