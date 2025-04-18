using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

public class EditLevel : SerializedMonoBehaviour
{
    public int id;
    
    [SerializeField, OnValueChanged("OnSizeChanged")]
    private int row = 5; // Mặc định 5 hàng
    
    [SerializeField, OnValueChanged("OnSizeChanged")] 
    private int col = 5; // Mặc định 5 cột
    
    [TableMatrix(HorizontalTitle = "Data Block", SquareCells = true, DrawElementMethod = "DrawDataElement")]
    public Data[,] dataBlock;

    private void OnEnable()
    {
        // Khởi tạo mảng với giá trị mặc định
        if(dataBlock == null)
        {
            dataBlock = new Data[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    dataBlock[i, j] = new Data { valueNumber = 0, isActive = false };
                }
            }
        }
    }

    // Phương thức vẽ cho mỗi ô trong TableMatrix
    private static Data DrawDataElement(Rect rect, Data value)
    {
        if (value == null)
            value = new Data { valueNumber = 0, isActive = false };

        // Vẽ giá trị và checkbox
        value.valueNumber = EditorGUI.IntField(
            new Rect(rect.x, rect.y, rect.width, rect.height * 0.7f), 
            value.valueNumber
        );
        
        value.isActive = EditorGUI.Toggle(
            new Rect(rect.x, rect.y + rect.height * 0.7f, rect.width, rect.height * 0.3f),
            value.isActive
        );

        return value;
    }

    [Button("Random Array")]
    private void RandomArray()
    {
        bool hasEmptyCell;
        do
        {
            // Reset mảng về 0
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    dataBlock[i,j] = new Data { valueNumber = 0, isActive = false };
                }

            // Chọn vị trí bắt đầu ngẫu nhiên cho số 1
            int currentX = Random.Range(0, row);
            int currentY = Random.Range(0, col);
            dataBlock[currentX, currentY] = new Data { valueNumber = 1, isActive = false };

            int totalElements = row * col;
            int currentNumber = 2;

            while (currentNumber <= totalElements)
            {
                List<Vector2Int> possibleMoves = new List<Vector2Int>();
                CheckPossibleMove(currentX + 1, currentY, possibleMoves);
                CheckPossibleMove(currentX - 1, currentY, possibleMoves);
                CheckPossibleMove(currentX, currentY + 1, possibleMoves);
                CheckPossibleMove(currentX, currentY - 1, possibleMoves);

                if (possibleMoves.Count > 0)
                {
                    int randomIndex = Random.Range(0, possibleMoves.Count);
                    Vector2Int nextMove = possibleMoves[randomIndex];
                    dataBlock[nextMove.x, nextMove.y] = new Data 
                    { 
                        valueNumber = currentNumber,
                        isActive = false 
                    };
                    currentX = nextMove.x;
                    currentY = nextMove.y;
                    currentNumber++;
                }
                else
                {
                    bool found = false;
                    for (int i = 0; i < row && !found; i++)
                    {
                        for (int j = 0; j < col && !found; j++)
                        {
                            if (dataBlock[i,j].valueNumber == 0)
                            {
                                if (HasAdjacentNumber(i, j, currentNumber - 1))
                                {
                                    dataBlock[i,j] = new Data 
                                    { 
                                        valueNumber = currentNumber,
                                        isActive = false 
                                    };
                                    currentX = i;
                                    currentY = j;
                                    currentNumber++;
                                    found = true;
                                }
                            }
                        }
                    }
                    if (!found) break;
                }
            }

            // Kiểm tra ô trống
            hasEmptyCell = false;
            for (int i = 0; i < row && !hasEmptyCell; i++)
            {
                for (int j = 0; j < col && !hasEmptyCell; j++)
                {
                    if (dataBlock[i,j].valueNumber == 0)
                    {
                        hasEmptyCell = true;
                    }
                }
            }
        } while (hasEmptyCell);
    }

    private void CheckPossibleMove(int x, int y, List<Vector2Int> moves)
    {
        if (IsValidPosition(x, y) && dataBlock[x,y].valueNumber == 0)
        {
            moves.Add(new Vector2Int(x, y));
        }
    }

    private bool HasAdjacentNumber(int x, int y, int number)
    {
        if (IsValidPosition(x + 1, y) && dataBlock[x + 1, y].valueNumber == number) return true;
        if (IsValidPosition(x - 1, y) && dataBlock[x - 1, y].valueNumber == number) return true;
        if (IsValidPosition(x, y + 1) && dataBlock[x, y + 1].valueNumber == number) return true;
        if (IsValidPosition(x, y - 1) && dataBlock[x, y - 1].valueNumber == number) return true;
        return false;
    }

    private bool IsValidPosition(int r, int c)
    {
        return r >= 0 && r < row && c >= 0 && c < col;
    }

    private void OnSizeChanged()
    {
        Data[,] newArray = new Data[row, col];
        if (dataBlock != null)
        {
            int minRow = Mathf.Min(row, dataBlock.GetLength(0));
            int minCol = Mathf.Min(col, dataBlock.GetLength(1));
            
            for (int i = 0; i < minRow; i++)
            {
                for (int j = 0; j < minCol; j++)
                {
                    newArray[i,j] = dataBlock[i,j];
                }
            }
        }
        dataBlock = newArray;
    }

    // Properties để truy cập từ code
    public int Row
    {
        get { return row; }
        set 
        { 
            row = value;
            OnSizeChanged();
        }
    }

    public int Col
    {
        get { return col; }
        set
        {
            col = value;
            OnSizeChanged();
        }
    }

    [Button("Save Level")]
    private void SaveLevel()
    {
        // Tạo đối tượng LevelConfig để serialize
        LevelConfig levelConfig = new LevelConfig
        {
            id = this.id,
            dataBlock = dataBlock // Không cần copy mảng vì Newtonsoft.Json có thể serialize mảng 2 chiều
        };

        // Convert sang JSON sử dụng Newtonsoft.Json
        string jsonData = JsonConvert.SerializeObject(levelConfig, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented, // Format JSON đẹp
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });

        // Tạo đường dẫn tới thư mục Resources/Levels
        string directoryPath = Path.Combine(Application.dataPath, "Resources/Levels");
        
        // Tạo thư mục nếu chưa tồn tại
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Tạo tên file
        string fileName = $"Level_{id}.txt";
        string fullPath = Path.Combine(directoryPath, fileName);

        // Lưu file
        File.WriteAllText(fullPath, jsonData);
        
        Debug.Log($"Đã lưu level tại: {fullPath}");

#if UNITY_EDITOR
        // Refresh để Unity nhận biết file mới trong Editor
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    [Button("Spam Level")]
    private void SpamLevel()
    {
        for (int levelId = 1; levelId <= 100; levelId++)
        {
            // Random kích thước mảng (tối đa 6x6)
            row = Random.Range(3, 7); // từ 3 đến 6
            col = Random.Range(3, 7);
            OnSizeChanged();

            // Tạo mảng số ngẫu nhiên
            RandomArray();

            // Random 1-2 ô có isActive = true
            int activeCount = Random.Range(1, 3); // 1 hoặc 2
            List<Vector2Int> availablePositions = new List<Vector2Int>();

            // Tạo danh sách tất cả vị trí
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    availablePositions.Add(new Vector2Int(i, j));
                }
            }

            // Random vị trí để set isActive = true
            for (int i = 0; i < activeCount; i++)
            {
                if (availablePositions.Count > 0)
                {
                    int randomIndex = Random.Range(0, availablePositions.Count);
                    Vector2Int pos = availablePositions[randomIndex];
                    dataBlock[pos.x, pos.y].isActive = true;
                    availablePositions.RemoveAt(randomIndex);
                }
            }

            // Lưu level
            id = levelId;
            SaveLevel();
        }

        Debug.Log("Đã tạo xong 100 level!");
    }
}       

[System.Serializable]
public class Data
{
    public int valueNumber;
    public bool isActive;

    public override string ToString()
    {
        return valueNumber.ToString();
    }
}

[System.Serializable]
public class LevelConfig
{
    public int id;
    public Data[,] dataBlock;
}