using UnityEngine;
using UnityEngine.UI; // 需要引用 UnityEngine.UI 才能存取 Image 元件

[RequireComponent(typeof(Image))] // 確保物件上必須有 Image 元件
public class AlphaHitFix : MonoBehaviour
{
    // 透過 Inspector 調整閾值，預設設為 0.1
    [Range(0.01f, 1.0f)] 
    [Tooltip("设置 Alpha 阈值。只有当图像的不透明度高于此值时，点按才有效。")]
    [SerializeField] private float hitThreshold = 0.1f;

    void Awake()
    {
        // 獲取物件上的 Image 元件
        Image imageComponent = GetComponent<Image>();

        if (imageComponent != null)
        {
            // [關鍵步驟]：強制設定 Alpha 點擊閾值
            imageComponent.alphaHitTestMinimumThreshold = hitThreshold;
            
            Debug.Log($"[AlphaHitFix] 已為 {gameObject.name} 設定 Alpha 點擊閾值為: {hitThreshold}");
        }
        else
        {
            Debug.LogError("AlphaHitFix 腳本需要附加在帶有 Image 元件的物件上。");
        }
    }
}