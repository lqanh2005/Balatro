#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class RecipeEditorWindow : OdinEditorWindow
{
    [MenuItem("Tools/Recipe Editor (Odin)")]
    private static void OpenWindow()
    {
        GetWindow<RecipeEditorWindow>().Show();
    }

    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    [PropertyOrder(-10)]
    public RecipeDatabaseSO recipeDatabase;

    [PropertyOrder(-9)]
    [EnumToggleButtons, OnValueChanged("FilterRecipes")]
    public EditorTab currentTab = EditorTab.PlayerProgress;

    public enum EditorTab
    {
        PlayerProgress,
        RecipeDefinitions,
        All
    }

    [TableList(ShowIndexLabels = true), ShowIf("ShowPlayerProgress")]
    public List<RecipeDisplayData> RecipesProgress = new();

    [TableList(ShowIndexLabels = true), ShowIf("ShowRecipeDefinitions")]
    public List<RecipeDefinitionData> RecipeDefinitions = new();

    [ReadOnly, ShowInInspector, LabelText("Save Status")]
    private string saveStatus = "";

    private Sequence statusSequence;

    protected override void OnEnable()
    {
        base.OnEnable();
        // Tìm RecipeDatabaseSO nếu chưa được gán
        if (recipeDatabase == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:RecipeDatabaseSO");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                recipeDatabase = AssetDatabase.LoadAssetAtPath<RecipeDatabaseSO>(path);
            }
        }
        LoadData();
    }

    private bool ShowPlayerProgress()
    {
        return currentTab == EditorTab.PlayerProgress || currentTab == EditorTab.All;
    }

    private bool ShowRecipeDefinitions()
    {
        return currentTab == EditorTab.RecipeDefinitions || currentTab == EditorTab.All;
    }

    private void FilterRecipes()
    {
        // Không cần làm gì đặc biệt, chỉ để refresh UI khi chuyển tab
    }

    [PropertyOrder(10)]
    [Button(ButtonSizes.Medium), GUIColor(0f, 0.8f, 1f)]
    private void LoadData()
    {
        // Tải dữ liệu tiến trình người chơi
        RecipesProgress.Clear();
        var all = RecipeManager.LoadAll();
        foreach (var pair in all)
        {
            RecipesProgress.Add(new RecipeDisplayData
            {
                recipe = pair.Key,
                data = new RecipeData(pair.Value.level, pair.Value.coin, pair.Value.multi)
            });
        }

        // Tải định nghĩa công thức từ database
        RecipeDefinitions.Clear();
        if (recipeDatabase != null)
        {
            foreach (var entry in recipeDatabase.defaultRecipes)
            {
                RecipeDefinitions.Add(new RecipeDefinitionData
                {
                    recipe = entry.recipe,
                    ingredients = entry.ingredients,
                    defaultCoin = entry.defaultCoin,
                    defaultMulti = entry.defaultMulti,
                    defaultLevel = entry.defaultLevel
                });
            }
        }
    }

    [PropertyOrder(11)]
    [Button(ButtonSizes.Medium), GUIColor(0.2f, 1f, 0.2f)]
    private void SaveAll()
    {
        // Lưu dữ liệu tiến trình người chơi
        Dictionary<Recipe, RecipeData> dict = new();
        foreach (var item in RecipesProgress)
        {
            dict[item.recipe] = item.data;
        }

        RecipeManager.SaveAll(dict);

        // Lưu định nghĩa công thức vào ScriptableObject
        if (recipeDatabase != null)
        {
            recipeDatabase.defaultRecipes.Clear();
            foreach (var item in RecipeDefinitions)
            {
                recipeDatabase.defaultRecipes.Add(new RecipeDefaultEntry
                {
                    recipe = item.recipe,
                    ingredients = item.ingredients,
                    defaultCoin = item.defaultCoin,
                    defaultMulti = item.defaultMulti,
                    defaultLevel = item.defaultLevel
                });
            }

            EditorUtility.SetDirty(recipeDatabase);
            AssetDatabase.SaveAssetIfDirty(recipeDatabase);
        }

        ShowSaveStatus("All recipes saved!", Color.green);
    }

    [PropertyOrder(12)]
    [Button(ButtonSizes.Medium), GUIColor(1f, 0.5f, 0f), ShowIf("@recipeDatabase != null")]
    private void SyncFromDatabaseToPlayerProgress()
    {
        if (recipeDatabase == null) return;

        var playerData = RecipeManager.LoadAll();
        bool hasChanges = false;

        // Cập nhật dữ liệu người chơi từ database mặc định
        foreach (var entry in recipeDatabase.defaultRecipes)
        {
            if (!playerData.ContainsKey(entry.recipe))
            {
                playerData[entry.recipe] = new RecipeData(entry.defaultLevel, entry.defaultCoin, entry.defaultMulti);
                hasChanges = true;
            }
        }

        if (hasChanges)
        {
            RecipeManager.SaveAll(playerData);
            LoadData(); // Tải lại dữ liệu
            ShowSaveStatus("Player data synced with database definitions!", new Color(1f, 0.5f, 0f));
        }
        else
        {
            ShowSaveStatus("No changes needed!", Color.blue);
        }
    }

    [PropertyOrder(13)]
    [Button(ButtonSizes.Medium), GUIColor(1f, 0.2f, 0.2f)]
    private void ResetPlayerProgress()
    {
        if (recipeDatabase != null)
        {
            if (EditorUtility.DisplayDialog("Reset Player Progress",
                "Are you sure you want to reset all player recipe progress to database defaults?",
                "Yes, Reset", "Cancel"))
            {
                RecipeManager.ResetAllRecipe(recipeDatabase);
                LoadData();
                ShowSaveStatus("Player progress reset to defaults!", new Color(1f, 0.2f, 0.2f));
            }
        }
        else
        {
            ShowSaveStatus("Recipe database not assigned!", Color.red);
        }
    }

    [PropertyOrder(14)]
    [Button(ButtonSizes.Medium), GUIColor(0.7f, 0f, 1f)]
    private void AddNewRecipe()
    {
        // Tạo một enum dialog để chọn Recipe
        var recipes = Enum.GetValues(typeof(Recipe))
            .Cast<Recipe>()
            .Where(r => !RecipeDefinitions.Any(def => def.recipe == r))
            .ToList();

        if (recipes.Count == 0)
        {
            ShowSaveStatus("All recipes are already defined!", Color.yellow);
            return;
        }

        var menu = new GenericMenu();
        foreach (var recipe in recipes)
        {
            menu.AddItem(new GUIContent(recipe.ToString()), false, () => {
                RecipeDefinitions.Add(new RecipeDefinitionData
                {
                    recipe = recipe,
                    ingredients = new List<IngredientType>(),
                    defaultCoin = 0,
                    defaultMulti = 1,
                    defaultLevel = 1
                });

                // Tự động lưu nếu database đã được gán
                if (recipeDatabase != null)
                {
                    recipeDatabase.defaultRecipes.Add(new RecipeDefaultEntry
                    {
                        recipe = recipe,
                        ingredients = new List<IngredientType>(),
                        defaultCoin = 0,
                        defaultMulti = 1,
                        defaultLevel = 1
                    });

                    EditorUtility.SetDirty(recipeDatabase);
                    AssetDatabase.SaveAssetIfDirty(recipeDatabase);
                }
            });
        }

        menu.ShowAsContext();
    }

    [Serializable]
    public class RecipeDisplayData
    {
        [HorizontalGroup("Row"), ReadOnly, LabelWidth(60)]
        public Recipe recipe;

        [HorizontalGroup("Row"), LabelWidth(50)]
        public RecipeData data;

        [HorizontalGroup("Row"), Button("Save", ButtonSizes.Small)]
        private void Save()
        {
            RecipeManager.SetRecipe(recipe, data);
            Debug.Log($"Saved progress for {recipe}");
        }
    }

    [Serializable]
    public class RecipeDefinitionData
    {
        [HorizontalGroup("Recipe"), ReadOnly, LabelWidth(60)]
        public Recipe recipe;

        [VerticalGroup("Details"), LabelText("Ingredients")]
        [ListDrawerSettings(DraggableItems = true, ShowItemCount = true)]
        public List<IngredientType> ingredients = new List<IngredientType>();

        [HorizontalGroup("Details/Stats"), LabelWidth(40)]
        public int defaultLevel = 1;

        [HorizontalGroup("Details/Stats"), LabelWidth(40)]
        public int defaultCoin = 0;

        [HorizontalGroup("Details/Stats"), LabelWidth(40)]
        public int defaultMulti = 1;

        [HorizontalGroup("Recipe"), Button("Save", ButtonSizes.Small)]
        private void Save()
        {
            var window = EditorWindow.GetWindow<RecipeEditorWindow>();
            if (window.recipeDatabase != null)
            {
                // Tìm và cập nhật entry trong database
                var existingEntry = window.recipeDatabase.defaultRecipes
                    .FirstOrDefault(e => e.recipe == recipe);

                if (existingEntry != null)
                {
                    existingEntry.ingredients = ingredients;
                    existingEntry.defaultCoin = defaultCoin;
                    existingEntry.defaultMulti = defaultMulti;
                    existingEntry.defaultLevel = defaultLevel;
                }
                else
                {
                    // Thêm mới nếu chưa có
                    window.recipeDatabase.defaultRecipes.Add(new RecipeDefaultEntry
                    {
                        recipe = recipe,
                        ingredients = ingredients,
                        defaultCoin = defaultCoin,
                        defaultMulti = defaultMulti,
                        defaultLevel = defaultLevel
                    });
                }

                EditorUtility.SetDirty(window.recipeDatabase);
                AssetDatabase.SaveAssetIfDirty(window.recipeDatabase);
                Debug.Log($"Saved definition for {recipe}");
            }
        }

        [HorizontalGroup("Recipe"), Button("X", ButtonSizes.Small), GUIColor(1f, 0.3f, 0.3f)]
        private void Delete()
        {
            if (EditorUtility.DisplayDialog("Delete Recipe Definition",
                $"Are you sure you want to delete the definition for {recipe}?",
                "Delete", "Cancel"))
            {
                var window = EditorWindow.GetWindow<RecipeEditorWindow>();

                // Xóa khỏi database
                if (window.recipeDatabase != null)
                {
                    window.recipeDatabase.defaultRecipes.RemoveAll(e => e.recipe == recipe);
                    EditorUtility.SetDirty(window.recipeDatabase);
                    AssetDatabase.SaveAssetIfDirty(window.recipeDatabase);
                }

                // Xóa khỏi danh sách hiển thị
                window.RecipeDefinitions.RemoveAll(d => d.recipe == recipe);
            }
        }
    }

    private void ShowSaveStatus(string message, Color color)
    {
        saveStatus = message;

        if (statusSequence != null && statusSequence.IsActive())
        {
            statusSequence.Kill();
        }

        statusSequence = DOTween.Sequence();
        statusSequence.AppendCallback(() =>
        {
            EditorGUIUtility.PingObject(this); // flash window
        });
        statusSequence.Append(DOVirtual.Color(Color.clear, color, 0.5f, c =>
        {
            GUI.contentColor = c;
        }));
        statusSequence.AppendInterval(2f);
        statusSequence.AppendCallback(() => saveStatus = "");
    }

    [PropertyOrder(99)]
    [Button(ButtonSizes.Medium), GUIColor(0.5f, 0.5f, 1f)]
    private void CreateNewRecipeDatabase()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create Recipe Database",
            "RecipeDatabase",
            "asset",
            "Choose a location to save the recipe database"
        );

        if (!string.IsNullOrEmpty(path))
        {
            var newDatabase = ScriptableObject.CreateInstance<RecipeDatabaseSO>();
            AssetDatabase.CreateAsset(newDatabase, path);
            AssetDatabase.SaveAssets();

            recipeDatabase = newDatabase;
            ShowSaveStatus("Created new Recipe Database!", new Color(0.5f, 0.5f, 1f));
        }
    }
    [PropertyOrder(15)]
    [Button(ButtonSizes.Medium), GUIColor(0.8f, 0.8f, 0.2f), ShowIf("@recipeDatabase != null")]
    private void UpdateFromIngredients()
    {
        if (recipeDatabase == null) return;

        // Hiển thị dialog để chọn cách cập nhật
        var option = EditorUtility.DisplayDialogComplex(
            "Update Recipes from Ingredients",
            "Do you want to update the recipe database by scanning the game for all possible ingredient combinations?",
            "Scan & Update", // option 0
            "Cancel", // option 1
            "Simulation Only" // option 2
        );

        if (option == 1) // Cancel
            return;

        bool simulationOnly = (option == 2);

        // Giả lập tìm tất cả các tổ hợp nguyên liệu có thể
        // Đây chỉ là ví dụ, bạn cần thay thế bằng logic thực tế
        var allIngredients = Enum.GetValues(typeof(IngredientType)).Cast<IngredientType>().ToList();
        var newRecipesFound = 0;

        // Ví dụ cho tổ hợp 3 nguyên liệu
        for (int i = 0; i < allIngredients.Count; i++)
        {
            for (int j = i; j < allIngredients.Count; j++)
            {
                for (int k = j; k < allIngredients.Count; k++)
                {
                    var ingredients = new List<IngredientType> { allIngredients[i], allIngredients[j], allIngredients[k] };

                    // Kiểm tra xem tổ hợp này đã có trong database chưa
                    bool exists = false;
                    foreach (var entry in recipeDatabase.defaultRecipes)
                    {
                        if (IngredientsMatch(entry.ingredients, ingredients))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        // Tìm một Recipe chưa được sử dụng
                        var unusedRecipe = Enum.GetValues(typeof(Recipe))
                            .Cast<Recipe>()
                            .FirstOrDefault(r => !recipeDatabase.defaultRecipes.Any(entry => entry.recipe == r));

                        if (unusedRecipe == default(Recipe))
                        {
                            ShowSaveStatus("No unused Recipe enum values left!", Color.red);
                            return;
                        }

                        // Thêm công thức mới
                        if (!simulationOnly)
                        {
                            recipeDatabase.defaultRecipes.Add(new RecipeDefaultEntry
                            {
                                recipe = unusedRecipe,
                                ingredients = ingredients,
                                defaultCoin = 10, // Giá trị mặc định
                                defaultMulti = 1,
                                defaultLevel = 1
                            });
                        }

                        newRecipesFound++;
                    }
                }
            }
        }

        if (!simulationOnly && newRecipesFound > 0)
        {
            EditorUtility.SetDirty(recipeDatabase);
            AssetDatabase.SaveAssetIfDirty(recipeDatabase);
            LoadData(); // Tải lại dữ liệu
        }

        ShowSaveStatus($"Found {newRecipesFound} new potential recipes!" +
                       (simulationOnly ? " (Simulation only)" : " (Database updated)"), Color.yellow);
    }

    private bool IngredientsMatch(List<IngredientType> list1, List<IngredientType> list2)
    {
        if (list1.Count != list2.Count)
            return false;

        var copy1 = new List<IngredientType>(list1);

        foreach (var item in list2)
        {
            if (!copy1.Remove(item))
                return false;
        }

        return copy1.Count == 0;
    }
    [PropertyOrder(16)]
    [Button(ButtonSizes.Medium), GUIColor(1f, 0.7f, 0.2f), ShowIf("@recipeDatabase != null")]
    private void CheckRecipeDefinitionConflicts()
    {
        if (recipeDatabase == null) return;

        List<string> conflicts = new List<string>();

        // Kiểm tra xung đột nguyên liệu (2 công thức sử dụng cùng nguyên liệu)
        for (int i = 0; i < recipeDatabase.defaultRecipes.Count; i++)
        {
            var recipe1 = recipeDatabase.defaultRecipes[i];
            for (int j = i + 1; j < recipeDatabase.defaultRecipes.Count; j++)
            {
                var recipe2 = recipeDatabase.defaultRecipes[j];

                if (IngredientsMatch(recipe1.ingredients, recipe2.ingredients))
                {
                    conflicts.Add($"Recipe conflict: {recipe1.recipe} and {recipe2.recipe} use identical ingredients");
                }
            }
        }

        // Kiểm tra công thức trùng nhau
        var recipesByEnum = new HashSet<Recipe>();
        foreach (var entry in recipeDatabase.defaultRecipes)
        {
            if (!recipesByEnum.Add(entry.recipe))
            {
                conflicts.Add($"Duplicate recipe enum: {entry.recipe} appears multiple times");
            }
        }

        // Hiển thị kết quả
        if (conflicts.Count == 0)
        {
            ShowSaveStatus("No conflicts found!", Color.green);
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Found {conflicts.Count} conflicts:");
            foreach (var conflict in conflicts)
            {
                sb.AppendLine($"• {conflict}");
            }

            EditorGUIUtility.systemCopyBuffer = sb.ToString();
            EditorUtility.DisplayDialog("Recipe Conflicts",
                sb.ToString() + "\n\n(This list has been copied to clipboard)", "OK");
        }
    }
    [PropertyOrder(97)]
    [Button(ButtonSizes.Medium), GUIColor(0.3f, 0.8f, 0.8f), ShowIf("@recipeDatabase != null")]
    private void ExportToCsv()
    {
        if (recipeDatabase == null) return;

        string path = EditorUtility.SaveFilePanel(
            "Export Recipes to CSV",
            "",
            "recipes.csv",
            "csv");

        if (string.IsNullOrEmpty(path))
            return;

        try
        {
            using (var writer = new System.IO.StreamWriter(path))
            {
                // Viết tiêu đề
                writer.WriteLine("Recipe,Ingredients,DefaultCoin,DefaultMulti,DefaultLevel");

                // Viết dữ liệu
                foreach (var entry in recipeDatabase.defaultRecipes)
                {
                    string ingredients = string.Join(";", entry.ingredients);
                    writer.WriteLine($"{entry.recipe},{ingredients},{entry.defaultCoin},{entry.defaultMulti},{entry.defaultLevel}");
                }
            }

            ShowSaveStatus("CSV export successful!", Color.green);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error exporting to CSV: {ex.Message}");
            ShowSaveStatus("CSV export failed!", Color.red);
        }
    }

    [PropertyOrder(98)]
    [Button(ButtonSizes.Medium), GUIColor(0.3f, 0.8f, 0.8f), ShowIf("@recipeDatabase != null")]
    private void ImportFromCsv()
    {
        if (recipeDatabase == null) return;

        string path = EditorUtility.OpenFilePanel(
            "Import Recipes from CSV",
            "",
            "csv");

        if (string.IsNullOrEmpty(path))
            return;

        try
        {
            var lines = System.IO.File.ReadAllLines(path);
            if (lines.Length <= 1) // Chỉ có tiêu đề
            {
                ShowSaveStatus("CSV file contains no data!", Color.yellow);
                return;
            }

            var newEntries = new List<RecipeDefaultEntry>();
            for (int i = 1; i < lines.Length; i++) // Bỏ qua dòng tiêu đề
            {
                var parts = lines[i].Split(',');
                if (parts.Length < 5) continue;

                if (Enum.TryParse(parts[0], out Recipe recipe))
                {
                    var entry = new RecipeDefaultEntry();
                    entry.recipe = recipe;

                    // Xử lý nguyên liệu
                    var ingredientStrings = parts[1].Split(';');
                    foreach (var ingStr in ingredientStrings)
                    {
                        if (Enum.TryParse(ingStr, out IngredientType ingredient))
                        {
                            entry.ingredients.Add(ingredient);
                        }
                    }

                    // Xử lý các thông số
                    if (int.TryParse(parts[2], out int coin))
                    {
                        entry.defaultCoin = coin;
                    }

                    if (int.TryParse(parts[3], out int multi))
                    {
                        entry.defaultMulti = multi;
                    }

                    if (int.TryParse(parts[4], out int level))
                    {
                        entry.defaultLevel = level;
                    }

                    newEntries.Add(entry);
                }
            }

            if (EditorUtility.DisplayDialog("Import CSV",
                $"Found {newEntries.Count} recipes in CSV file. Do you want to replace the existing definitions?",
                "Replace All", "Cancel"))
            {
                recipeDatabase.defaultRecipes = newEntries;
                EditorUtility.SetDirty(recipeDatabase);
                AssetDatabase.SaveAssetIfDirty(recipeDatabase);
                LoadData(); // Tải lại dữ liệu
                ShowSaveStatus($"Imported {newEntries.Count} recipes successfully!", Color.green);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error importing from CSV: {ex.Message}");
            ShowSaveStatus("CSV import failed!", Color.red);
        }
    }
}
#endif
