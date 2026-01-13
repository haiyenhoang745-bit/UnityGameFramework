using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TextureFormatChecker : Editor
{
    // ================= 配置区域 =================
    
    // 1. 设置搜索路径 (必须以 "Assets" 开头)
    //private static string searchPath = "Assets/Textures"; 
    private static string searchPath = "Assets/GameMain/UI/UISprites/Common"; 
    
    // 2. 设置是否开启筛选模式
    // true: 仅输出符合 targetFormatName 的纹理
    // false: 输出文件夹下所有纹理，不进行筛选
    private static bool filterByFormat = true; 

    // 3. 定义目标格式字符串 (例如 "ASTC_6x6", "DXT5", "RGBA32" 等)
    // 这里使用的是 TextureFormat 枚举的字符串名称
    private static string targetFormatName = "DXT1";

    // ===========================================

    [MenuItem("Tools/Check Texture Formats")]
    public static void CheckTextures()
    {
        // 确保路径有效
        if (!AssetDatabase.IsValidFolder(searchPath))
        {
            Debug.LogError($"[TextureChecker] 路径无效: {searchPath}，请检查代码中的 searchPath 变量。");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { searchPath });
        
        Debug.Log($"--- 开始检查 --- 路径: {searchPath} | 模式: {(filterByFormat ? "筛选: " + targetFormatName : "输出所有")}");

        int matchCount = 0;
        int totalCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (texture == null) continue;

            string currentFormat = texture.format.ToString();
            totalCount++;

            // 核心逻辑判断
            if (filterByFormat)
            {
                // 模式 True: 只要符合指定的格式才输出
                // 使用 Contains 稍微灵活一点，或者使用 == 严格匹配
                if (currentFormat.Equals(targetFormatName, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"<color=green>[符合]</color> {texture.name} | 格式: {currentFormat}", texture);
                    matchCount++;
                }
            }
            else
            {
                // 模式 False: 全部输出
                // 为了区分，我把符合的高亮绿色，不符合的显示白色
                if (currentFormat.Equals(targetFormatName, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"<color=green>[Target]</color> {texture.name} | 格式: {currentFormat}", texture);
                }
                else
                {
                    Debug.Log($"[其它] {texture.name} | 格式: {currentFormat}", texture);
                }
            }
        }

        // 总结输出
        if (filterByFormat)
        {
            Debug.Log($"--- 检查结束 --- 共扫描 {totalCount} 张，符合条件 {matchCount} 张。");
        }
        else
        {
            Debug.Log($"--- 检查结束 --- 共列出 {totalCount} 张纹理。");
        }
    }
}