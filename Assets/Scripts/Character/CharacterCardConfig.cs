// 檔案：CharacterCardConfig.cs

using UnityEngine;

// 假設 Rarity enum 已經定義在某個地方
public enum Rarity { N, R, SR, SSR, UR } 

[CreateAssetMenu(fileName = "CC_NewCard", menuName = "GameData/Character Card")]
public class CharacterCardConfig : ScriptableObject
{
    [Header("Identity")]
    public string CardID;
    
    // 連結到角色的基礎身分和外觀資源 (例如 Live2D 模型)
    public CharacterEntityConfig LinkedEntity;
    
    // 用於角色庫列表或詳細資訊的主立繪
    public Sprite CardArtwork;

    // --- 新增屬性 ---
    [Header("Rarity & Base Attributes")]
    public Rarity CardRarity = Rarity.R; 
    
    // Level 1 時的基礎能力值
    [Tooltip("角色 Level 1 時的基礎物理攻擊力")]
    public int BaseAttackPhys; 
    
    [Tooltip("角色 Level 1 時的基礎魔法攻擊力")]
    public int BaseAttackMagic; 
    
    // --- 成長率 ---
    [Header("Growth Rates")]
    [Tooltip("角色每升一級 HP 增加的數值")]
    public float BaseHPGrowth; 
    
    [Tooltip("角色每升一級物理攻擊力增加的數值")]
    public float BaseATK_PhysGrowth; 
    
    // 您可以在此處繼續添加其他成長屬性，例如防禦力、暴擊率等。
}