using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCSVReader : MonoBehaviour
{
    public TestWord[] Parse(string _CSVFileName)
    {
        List<TestWord> testWordList = new List<TestWord>();
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        string[] data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length - 1;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            TestWord testWord = new TestWord();

            testWord.wordName = row[1];

            List<TestWordAction> wordActionList = new List<TestWordAction>();

            do
            {
                TestWordAction action = new TestWordAction()
                {
                    wordActionName = row[2],
                    wordActionBool = bool.Parse(row[3]),
                    stressGage = int.Parse(row[4]),
                    angerGage = int.Parse(row[5]),
                    riskGage = int.Parse(row[6]),
                    streamEventID = int.Parse(row[7])
                };           

                wordActionList.Add(action);

                if(++i < data.Length - 1)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
                
            }
            while (row[0].ToString() == "");

            testWord.testWordActions = wordActionList;
            testWordList.Add(testWord);
        }
        return testWordList.ToArray();
    }

    private void Start()
    {
        TestWord[] test = Parse("TestCVSFile");
    }
}


public class TestWord
{
    public string wordName;
    public List<TestWordAction> testWordActions = new();
}

public class TestWordAction
{
    public string wordActionName;
    public bool wordActionBool;
    public int stressGage;
    public int angerGage;
    public int riskGage;
    public int? streamEventID;
}
