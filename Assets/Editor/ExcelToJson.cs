using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Data;
using Excel;
using LitJson;

public class DataPath
{
    public static readonly string excelPath = Application.streamingAssetsPath + "/剧情.xlsx";
}

public class ExcelToJson : EditorWindow
{
    [MenuItem("工具/Excel转Json")]
    static void ShowWindow()
    {
        ExcelToJson win = GetWindow<ExcelToJson>();
        win.titleContent = new GUIContent("Excle转Json");
        win.Show();
    }



    private string connect;
    private string filePath;
    Object excel;
    string FilePath
    {
        get
        {
            if (excel == null)
            {
                return "";
            }
            return AssetDatabase.GetAssetPath(excel);
        }
    }

    //string[] select = new string[] { "第一行", "第二行" };
    int startRow=1;

    private void OnGUI()
    {
        excel = EditorGUILayout.ObjectField("Excel文件", excel, typeof(Object), false, null);
        //startRow = EditorGUILayout.Popup("读取开始位置", startRow, select);

        if (GUILayout.Button("转换成Json"))
        {
            DataSet dataSet = ReadExcel(FilePath);
            List<Dictionary<string, string>> data = WriteToDict(dataSet.Tables[0]);
            WriteToJson(data);
        }
    }

    public void LoadExcel()
    {
        DataSet dataSet = ReadExcel(DataPath.excelPath);
        List<Dictionary<string, string>> data = WriteToDict(dataSet.Tables[0]);
        WriteToJson(data);

    }

    DataSet ReadExcel(string filePath)
    {
        FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(file);
        return reader.AsDataSet();
    }

    /// <summary>
    /// 将table表写成字典链表
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    List<Dictionary<string, string>> WriteToDict(DataTable table)
    {
        List<Dictionary<string, string>> allData = new List<Dictionary<string, string>>();
        string[] title = new string[table.Rows[startRow].ItemArray.Length];
        for (int i = 0; i < table.Rows[startRow].ItemArray.Length; i++)
        {
            title[i] = table.Rows[startRow][i].ToString();
        }
        for (int i = startRow + 1; i < table.Rows.Count; i++)
        {
            Dictionary<string, string> tempData = new Dictionary<string, string>();
            for (int j = 0; j < title.Length; j++)
            {
                tempData.Add(title[j], table.Rows[i][j].ToString());
            }
            allData.Add(tempData);
        }
        return allData;
    }

    /// <summary>
    /// 将字典链表写成Json
    /// </summary>
    /// <param name="dictList"></param>
    void WriteToJson(List<Dictionary<string, string>> dictList)
    {
        string newFile = "DialogJson.txt";
        //if (excel!=null)
        //{
        //    newFile = excel.name + ".txt";
        //}
        //string newPath = FilePath.Substring(0, FilePath.Length - excel.name.Length - Path.GetExtension(FilePath).Length) + newFile;
        string newPath = Application.dataPath+"/Resources/Story/" + newFile;
        StreamWriter sw = new StreamWriter(newPath);
        sw.Write("[");
        bool first = true;
        foreach (var dict in dictList)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sw.Write(",");
            }
            JsonData jsonData = new JsonData();
            foreach (var key in dict.Keys)
            {
                //    //判断能否转成int
                //    int re;
                //    bool res = int.TryParse(dict[key], out re);
                //    if (res)
                //    {
                //        jsonData[key.ToString()] = int.Parse(dict[key]);
                //    }
                //    else
                //    {
                jsonData[key.ToString()] = dict[key];
                //    }
            }
            string jsonStr = jsonData.ToJson();
            sw.Write(jsonStr);
        }
        sw.Write("]");
        sw.Close();
        Debug.Log("转换成功");
    }
}
