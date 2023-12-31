using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CSVWriter : MonoBehaviour
{
    public static List<Dictionary<string, object>> SaveCSV_StreamEventDatas(string id, string filePath, string fileName)
    {
        

        //CSV데이터 가져와 string값으로 저장
        string path = filePath + fileName;
        string s_sentenceSheetTemp = File.ReadAllText(path);
        string[] s_sentenceSheetCell = s_sentenceSheetTemp.Split(',', '\n');

        //각각의 줄을 저장
        List<string> s_id = new List<string>();
        List<string> s_isCreated = new List<string>();
        List<string> s_value = new List<string>();

        for (int i = 0; i < (s_sentenceSheetCell.Length - 1) / 3; i++)
        {
            s_id.Add(s_sentenceSheetCell[i]);
            s_isCreated.Add(s_sentenceSheetCell[i + (s_sentenceSheetCell.Length - 1) / 3]);
            s_value.Add(s_sentenceSheetCell[i + (s_sentenceSheetCell.Length - 1) * 2 / 3]);
        }
        int changeValueNum = 0;
        for (int i = 1; i < (s_sentenceSheetCell.Length - 1) / 3; i++)
        {
            if (s_id[i] == id) { changeValueNum = i; }
        }
        s_isCreated[changeValueNum] = "TRUE";
        int temp = Convert.ToInt32(s_value[changeValueNum]) + 1;
        s_value[changeValueNum] = temp.ToString();
        s_sentenceSheetTemp = "";

        InputCSV(s_sentenceSheetCell, s_id);
        InputCSV(s_sentenceSheetCell, s_isCreated);
        InputCSV(s_sentenceSheetCell, s_value);

        void InputCSV(string[] cell, List<string> value)
        {
            for (int i = 0; i < (cell.Length - 1) / 3; i++)
            {
                s_sentenceSheetTemp += value[i];
                if(i == cell.Length - 2)
                { break; }
                else if (i == ((s_sentenceSheetCell.Length - 1) / 3) - 1)
                { s_sentenceSheetTemp += "\n"; }
                else
                { s_sentenceSheetTemp += ","; }
            }
        }
        List<Dictionary<string, object>> datas = CSVReader.Read(s_sentenceSheetTemp);

        #region

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(s_sentenceSheetTemp);
        StreamWriter outStream = System.IO.File.CreateText(filePath + fileName);
        outStream.Write(stringBuilder.ToString());
        outStream.Close();

        #endregion

        return datas;
    }
    public static void SaveCSV(string OriginalFilePath, string OriginalFileName, string MoveFilePath, string MoveFileName)
    {
        if (!File.Exists(MoveFilePath + MoveFileName))
        {
            FileStream fs = new FileStream(MoveFilePath + MoveFileName, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);
            sw.Close();

            string s_sentenceSheetTemp = File.ReadAllText(OriginalFilePath + OriginalFileName);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(s_sentenceSheetTemp);
            StreamWriter outStream = File.CreateText(MoveFilePath + MoveFileName);
            outStream.Write(stringBuilder.ToString());
            outStream.Close();
        }
    }
}