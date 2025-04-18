using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

public class LevelData : MonoBehaviour
{ 
    public LevelConfig levelConfig;
    public BlockBase blockBase;
    public List<BlockBase> listBlock = new List<BlockBase>();
    
    public void Init()
    {
        if(listBlock != null && listBlock.Count > 0)
        {
            foreach(var block in listBlock)
            {
                if(block != null)
                    Destroy(block.gameObject);
            }
            listBlock.Clear();
        }

        var pathLevel = $"Levels/Level_{UseProfile.CurrentLevel}";
        TextAsset lvJson = Resources.Load<TextAsset>(pathLevel);
        if(lvJson != null)
        {
            levelConfig = JsonConvert.DeserializeObject<LevelConfig>(lvJson.text);

            // Tính toán offset để căn giữa
            float offsetX = -(levelConfig.dataBlock.GetLength(0) - 1) * 0.5f;
            float offsetY = -(levelConfig.dataBlock.GetLength(1) - 1) * 0.5f;

            for(int i = 0; i < levelConfig.dataBlock.GetLength(0); i++)
            {
                for(int j = 0; j < levelConfig.dataBlock.GetLength(1); j++)
                {
                    var temp = Instantiate(blockBase, transform);
                    // Áp dụng offset vào vị trí
                    temp.transform.localPosition = new Vector3(
                        i + offsetX, 
                        j + offsetY,
                        0
                    );
                    listBlock.Add(temp);
                }
            }
        }
        else
        {
            Debug.LogError($"Không tìm thấy file level: {pathLevel}");
        }
    }
}
