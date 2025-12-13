// 檔案：CharacterStatsCalculator.cs

using UnityEngine; // 【關鍵修正】引入 UnityEngine 命名空間

public static class CharacterStatsCalculator
{
    /// <summary>
    /// 根據角色等級和配置計算當前屬性值。
    /// </summary>
    /// <param name="baseStat">角色 Level 1 時的基礎數值 (例如 BaseAttackPhys)</param>
    /// <param name="growthRate">角色每級成長值 (例如 BaseATK_PhysGrowth)</param>
    /// <param name="level">角色的當前等級 (從 TrainingCrystal 獲取)</param>
    /// <returns>計算後的當前屬性值 (整數)</returns>
    public static int CalculateCurrentStat(int baseStat, float growthRate, int level)
    {
        // 確保等級至少為 1
        level = Mathf.Max(1, level); 
        
        // 成長計算: GrowthRate * (Level - 1)
        float statIncrease = growthRate * (level - 1);
        
        // 總屬性 = 基礎屬性 + 增長值
        // 使用 Mathf.RoundToInt 確保結果是整數
        int currentStat = baseStat + Mathf.RoundToInt(statIncrease);
        
        return currentStat;
    }
}