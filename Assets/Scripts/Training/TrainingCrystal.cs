// 檔案：TrainingCrystal.cs
using System;
using UnityEngine;
using System.Collections.Generic; // 確保有 System.Collections.Generic

// 這是玩家在戰鬥中實際使用的單位，儲存了培育後的結果和裝備狀態。
[System.Serializable]
public class TrainingCrystal 
{
    // 唯一的實例 ID (使用 GUID 確保其在全球範圍內獨一無二，用於編隊和裝備追蹤)
    public string CrystalInstanceID; 
    
    // 連結到是用哪一張角色卡培育出來的 (例如 CC_ERFL_001_A)
    public string SourceCardID;      
    
    // 【新增核心屬性】等級和經驗值
    public int Level = 1; 
    public long Experience = 0; // 當前經驗值 (用於計算升級)
    
    // 培育後的最終數值
    public float FinalHP = 1000f; 
    public float FinalPhysicalAttack = 100f;
    public float FinalMagicAttack = 50f;
    public float FinalPhysicalDefense = 30f;
    public float FinalMagicDefense = 30f;
    public float FinalCriticalChance = 0.05f; // 爆擊率 (例如 5%)
    public float FinalCriticalDamage = 1.5f;  // 爆傷倍率 (例如 150%)
    public float FinalSpeed = 100f;

    // 裝備狀態
    public string EquippedWeaponID = "NONE";
    
    // 初始化一個新的結晶
    public TrainingCrystal(string sourceCardId)
    {
        CrystalInstanceID = Guid.NewGuid().ToString();
        SourceCardID = sourceCardId;
        
        // 確保新創建的結晶等級為 1
        Level = 1; 
        Experience = 0;

        // 【TODO】在正式系統中，這裡應該調用一個屬性計算服務，
        // 根據 CardConfig 和 Level=1 來設定初始的 Final 數值。
        
        // 暫時保持預設的 Final 數值。
    }
}