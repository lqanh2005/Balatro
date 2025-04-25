#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using DG.Tweening;
using System;

public class RecipeEditorWindow : OdinEditorWindow
{
    [MenuItem("Tools/Recipe Editor (Odin)")]
    private static void OpenWindow()
    {
        GetWindow<RecipeEditorWindow>().Show();
    }

    [TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
    public List<RecipeDisplayData> Recipes = new();

    [ReadOnly, ShowInInspector, LabelText("Save Status")]
    private string saveStatus = "";

    private Sequence statusSequence;

    protected override void OnEnable()
    {
        base.OnEnable();
        LoadData();
    }

    [Button(ButtonSizes.Medium), GUIColor(0f, 0.8f, 1f)]
    private void LoadData()
    {
        Recipes.Clear();
        var all = RecipeManager.LoadAll();
        foreach (var pair in all)
        {
            Recipes.Add(new RecipeDisplayData
            {
                recipe = pair.Key,
                data = new RecipeData(pair.Value.coin, pair.Value.multi)
            });
        }
    }

    [Button(ButtonSizes.Medium), GUIColor(0.2f, 1f, 0.2f)]
    private void SaveAll()
    {
        Dictionary<Recipe, RecipeData> dict = new();
        foreach (var item in Recipes)
        {
            dict[item.recipe] = item.data;
        }

        RecipeManager.SaveAll(dict);
        ShowSaveStatus("All recipes saved!", Color.green);
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
            Debug.Log($"Saved {recipe}");
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
