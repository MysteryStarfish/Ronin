using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace MyTools {
    public static class Setup {
        
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders() {
            // 1. 先建立實體資料夾結構
            Folders.CreateDefault("_Project", 
                "Animation", 
                "Art", 
                "Materials", 
                "Prefabs", 
                "Scripts/Core",     // 核心系統
                "Scripts/Input",     // 核心系統
                "Scripts/UI",       // UI 系統
                "Scripts/Gameplay",  // 遊戲邏輯
                "Scripts/Test"     // 核心系統
            );

            // 2. 建立 asmdef 並設定依賴關係 (架構分層)
            
            // Core: 最底層，不依賴任何人
            Folders.CreateAssemblyDefinition("_Project/Scripts/Core", "MyGame.Core");

            // UI: 依賴 Core，且通常會依賴 Unity 的 TextMeshPro
            Folders.CreateAssemblyDefinition("_Project/Scripts/UI", "MyGame.UI", 
                "MyGame.Core", 
                "Unity.TextMeshPro" // 範例：加入外部套件依賴
            );
            // Input: 依賴 Core 和 UI
            Folders.CreateAssemblyDefinition("_Project/Scripts/Input", "MyGame.Gameplay", 
                "Unity.InputSystem"
            );
            Folders.CreateAssemblyDefinition("_Project/Scripts/Gameplay", "MyGame.Gameplay", 
                "MyGame.Core", 
                "MyGame.UI",
                "MyGame.Input"
            );
            Folders.CreateAssemblyDefinition("_Project/Scripts/Test", "MyGame.Gameplay", 
                "UnityEngine.TestRunner",
                "UnityEditor.TestRunner",
                "MyGame.Core", 
                "MyGame.UI", 
                "MyGame.Input", 
                "MyGame.Gameplay"
            );
            Folders.CreateDefault("_Project", "Scripts/Test/Core", "Scripts/Test/UI", "Scripts/Test/Input", "Scripts/Test/Gameplay", "Scripts/ScriptableObjects");

            // 3. 重新整理 AssetDatabase，讓 Unity 讀取新檔案
            AssetDatabase.Refresh();
        }

        // ... (其他的 MenuItems: ImportAssets, InstallPackages 保持不變，為節省篇幅省略) ...
        [MenuItem("Tools/Setup/Import My Favorite Assets")]
        public static void ImportMyFavoriteAssets()
        {
            Assets.ImportAsset(
                "DOTween HOTween v2.unitypackage", "Demigiant/ScriptingAnimation"
            );
        }

        [MenuItem("Tools/Setup/Install Netcode for GameObjects")]
        public static void InstallNetcodeForGameObjects()
        {
            Packages.InstallPackages(new[] { "com.unity.multiplayer.tools", "com.unity.netcode.gameobjects" });
        }

        [MenuItem("Tools/Setup/Install Unity AI Navigation")]
        public static void InstallUnityAINavigation()
        {
            Packages.InstallPackages(
                new[]
                {
                    "git+https://github.com/h8man/NavMeshPlus"
                });
        }

        [MenuItem("Tools/Setup/Install My Favorite Open Source")]
        public static void InstallOpenSource()
        {
            Packages.InstallPackages(
                new[]
                {
                    "git+https://github.com/KyleBanks/scene-ref-attribute", 
                    "git+https://github.com/starikcetin/Eflatun.SceneReference.git#3.1.1"
                });
        }


        // --- 工具類別 ---

        static class Folders {
            public static void CreateDefault(string root, params string[] folders) {
                var fullpath = Path.Combine(Application.dataPath, root);
                if (!Directory.Exists(fullpath)) Directory.CreateDirectory(fullpath);
                foreach (var folder in folders) CreateSubFolders(fullpath, folder);
            }
    
            private static void CreateSubFolders(string rootPath, string folderHierarchy) {
                var folders = folderHierarchy.Split('/');
                var currentPath = rootPath;
                foreach (var folder in folders) {
                    currentPath = Path.Combine(currentPath, folder);
                    if (!Directory.Exists(currentPath)) Directory.CreateDirectory(currentPath);
                }
            }

            /// <summary>
            /// 建立 .asmdef 檔案並設定依賴
            /// </summary>
            /// <param name="relativePath">相對於 Assets 的路徑 (例如 _Project/Scripts/Core)</param>
            /// <param name="assemblyName">Assembly 名稱 (例如 MyGame.Core)</param>
            /// <param name="references">依賴的其他 Assembly 名稱</param>
            public static void CreateAssemblyDefinition(string relativePath, string assemblyName, params string[] references) {
                string folderPath = Path.Combine(Application.dataPath, relativePath);
                string filePath = Path.Combine(folderPath, assemblyName + ".asmdef");

                // 防止重複建立覆蓋
                if (File.Exists(filePath)) return;

                // 處理引用字串：將陣列轉換為 JSON 格式 "Ref1", "Ref2"
                string referencesJson = "";
                if (references != null && references.Length > 0) {
                    // 使用 string.Join 串接，並加上引號
                    referencesJson = "\"" + string.Join("\", \"", references) + "\"";
                }

                // 組合 JSON 內容
                string jsonContent = $@"{{
    ""name"": ""{assemblyName}"",
    ""rootNamespace"": ""{assemblyName}"",
    ""references"": [ {referencesJson} ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";

                File.WriteAllText(filePath, jsonContent);
                Debug.Log($"Created Assembly Definition: {assemblyName} (Deps: {references.Length})");
            }
        }

        static class Packages {
            static AddRequest Request;
            static Queue<string> PackagesToInstall = new();

            public static void InstallPackages(string[] packages) {
                foreach (var package in packages) PackagesToInstall.Enqueue(package);
                if (PackagesToInstall.Count > 0) {
                    Request = Client.Add(PackagesToInstall.Dequeue());
                    EditorApplication.update += Progress;
                }
            }

            static async void Progress() {
                if (Request.IsCompleted) {
                    if (Request.Status == StatusCode.Success) Debug.Log("Installed: " + Request.Result.packageId);
                    else if (Request.Status >= StatusCode.Failure) Debug.Log(Request.Error.message);

                    EditorApplication.update -= Progress;
                    if (PackagesToInstall.Count > 0) {
                        await Task.Delay(1000);
                        Request = Client.Add(PackagesToInstall.Dequeue());
                        EditorApplication.update += Progress;
                    }
                }
            }
        }

        static class Assets {
            public static void ImportAsset(string asset, string subfolder, string rootFolder = "C:/Users/paulh/AppData/Roaming/Unity/Asset Store-5.x") {
                // 注意：請確保這裡的路徑在你的電腦上是正確的
                AssetDatabase.ImportPackage(Path.Combine(rootFolder, subfolder, asset), false);
            }
        }
    }
}