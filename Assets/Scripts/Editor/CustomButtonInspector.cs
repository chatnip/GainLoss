using UnityEngine;
using UnityEditor;

// ���� typeof(�����) ���� ����� �κ��� ����Ǵ� ��ũ��Ʈ �̸��� ���ƾ� �ؿ�
// Scripts ���� �ȿ� Editor ��� ������ �ϳ� ���� �� ��ũ��Ʈ�� �־��ּ���

[CustomEditor(typeof(PreliminarySurveySO_Extract))]
public class CustomButtonInspector : Editor
{
    private int[,] buttonValues;
    private Color[] buttonColors = { Color.white, Color.yellow, new Color(1f, 0.5f, 0.5f), Color.blue, Color.black };
    private string[] buttonTexts = { "", "1", "2", "3", "B" };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PreliminarySurveySO_Extract PSSO_extra = (PreliminarySurveySO_Extract)target;
        buttonValues = PSSO_extra.array;

        GUILayout.Space(10);
        GUILayout.Label("Custom Buttons", EditorStyles.boldLabel);

        // Draw Reset Array button
        if (GUILayout.Button("Reset Array"))
        {
            ResetArray();
        }

        // Draw custom buttons
        for (int y = 0; y < 8; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < 12; x++)
            {
                Color currentColor = buttonColors[buttonValues[y, x]];
                GUI.backgroundColor = currentColor;

                if (GUILayout.Button(buttonTexts[buttonValues[y, x]], GUILayout.Width(20), GUILayout.Height(20)))
                {
                    // Change button value and color
                    buttonValues[y, x] = (buttonValues[y, x] + 1) % 5;
                    SceneView.RepaintAll();
                }
            }
            GUILayout.EndHorizontal();
        }

        // Draw array values below the buttons
        GUILayout.Space(10);
        GUILayout.Label("Button Values", EditorStyles.boldLabel);
        for (int y = 0; y < 8; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < 12; x++)
            {
                GUILayout.Label(buttonValues[y, x].ToString(), GUILayout.Width(20), GUILayout.Height(20));
            }
            GUILayout.EndHorizontal();
        }
    }

    private void ResetArray()
    {
        PreliminarySurveySO_Extract PSSO_extra = (PreliminarySurveySO_Extract)target;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 12; x++)
            {
                PSSO_extra.array[y, x] = 0;
            }
        }
    }
}