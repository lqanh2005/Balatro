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
                data = new RecipeData(pair.Value.coin, pair.Value.multi, pair.Value.level)
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

}
#endif
