using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PreliminarySurveySO_Extract))]
public class CustomButtonInspector : Editor
{
    private Color[] buttonColors = { Color.white, Color.yellow, new Color(1f, 0.5f, 0.5f), Color.blue, Color.black };
    private string[] buttonTexts = { "", "1", "2", "3", "B" };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PreliminarySurveySO_Extract PSSO_extra = (PreliminarySurveySO_Extract)target;

        GUILayout.Space(10);
        GUILayout.Label("Custom Buttons", EditorStyles.boldLabel);

        // '배열 초기화' 버튼 그리기
        if (GUILayout.Button("Reset Array"))
        {
            Undo.RecordObject(PSSO_extra, "Reset Array");
            ResetArray(PSSO_extra);
            EditorUtility.SetDirty(PSSO_extra);
        }

        // 커스텀 버튼 그리기
        for (int y = 0; y < 8; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < 12; x++)
            {
                Color currentColor = buttonColors[PSSO_extra.tempArray[y, x]];
                GUI.backgroundColor = currentColor;

                if (GUILayout.Button(buttonTexts[PSSO_extra.tempArray[y, x]], GUILayout.Width(20), GUILayout.Height(20)))
                {
                    Undo.RecordObject(PSSO_extra, "Change Button Value");
                    PSSO_extra.tempArray[y, x] = (PSSO_extra.tempArray[y, x] + 1) % 5;
                    EditorUtility.SetDirty(PSSO_extra);
                    SceneView.RepaintAll();
                }
            }
            GUILayout.EndHorizontal();
        }

        // 불러오기
        if (GUILayout.Button("Load"))
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    PSSO_extra.tempArray[y, x] = PSSO_extra.tileArray[y].LineIndex[x];
                    SceneView.RepaintAll();
                }
            }
        }

        // 저장
        if (GUILayout.Button("Save"))
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 12; x++)
                {
                    PSSO_extra.tileArray[y].LineIndex[x] = PSSO_extra.tempArray[y, x];
                }
            }
        }
        

        // 버튼 아래에 배열 값 그리기는 불필요함(버튼을 통해 직접 보고 수정 가능하기 때문)
    }

    private void ResetArray(PreliminarySurveySO_Extract PSSO_extra)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 12; x++)
            {
                PSSO_extra.tempArray[y, x] = 0;
            }
        }
    }
}