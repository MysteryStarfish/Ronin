using UnityEditor;
using System;

namespace Ronin.Core
{
    public static class MonoScriptExtensions
    {
        /// <summary>
        /// 檢查這個腳本內的類別，是否繼承或實作了 T
        /// </summary>
        public static bool IsSubclassOf<T>(this MonoScript script)
        {
            if (script == null) return false;

            // 1. 把 MonoScript 轉成真正的 C# Type
            Type scriptType = script.GetClass();

            if (scriptType == null)
            {
                // 如果腳本編譯錯誤，或是檔名跟類別名不符，這裡會是 null
                return false; 
            }

            // 2. 檢查相容性 (IsAssignableFrom 是反射界的瑞士刀)
            // 它的意思是：變數 T t = new scriptType(); 合不合法？
            return typeof(T).IsAssignableFrom(scriptType);
        }
    }
}