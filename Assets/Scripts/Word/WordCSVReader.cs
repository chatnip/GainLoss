using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WordCSVReader
{
    public static Word[] Parse(string _CSVFileName)
    {
        List<Word> testWordList = new();
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length - 1;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Word wordData = new();

            wordData.wordName = row[1];
            //wordData.wordPlace = row[2];
            //wordData.wordActionEvent = row[3];

            List<WordActionData> wordActionList = new();

            do
            {
                WordActionData action = new()
                {
                    wordActionName = row[4],
                    actionSentence = row[5],
                    wordActionBool = bool.Parse(row[6]),
                    stressGage = int.Parse(row[7]),
                    angerGage = int.Parse(row[8]),
                    riskGage = int.Parse(row[9]),
                    streamEventID = int.Parse(row[10])
                };

                wordActionList.Add(action);

                if (++i < data.Length - 1)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }

            }
            while (row[0].ToString() == "");

            //wordData.wordActionDatas = wordActionList;
            testWordList.Add(wordData);
        }
        return testWordList.ToArray();
    }
}