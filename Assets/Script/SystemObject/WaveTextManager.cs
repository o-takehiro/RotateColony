using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveTextManager : SystemObject {
    public TMP_Text tmpText;        // TextMeshProUGUIÇInspectorÇ≈ê›íË
    public float waveSpeed = 2f;    // îgÇÃë¨Ç≥
    public float waveHeight = 5f;   // îgÇÃçÇÇ≥

    private TMP_TextInfo textInfo;
    private Vector3[] vertices;

    /// <summary>
    /// èâä˙âª
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        if (tmpText != null) {
            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;
        }
        await UniTask.CompletedTask;
    }

    void Awake() {
        if (tmpText != null) {
            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;
        }
    }
    // çXêV
    void Update() {
        if (tmpText == null || textInfo == null) return;
        AnimateWave();
    }

    // WaveText
    private void AnimateWave() {
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++) {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

            vertices = textInfo.meshInfo[materialIndex].vertices;

            Vector3 offset = new Vector3(0, Mathf.Sin(Time.time * waveSpeed + i) * waveHeight, 0);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;

            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }

}
