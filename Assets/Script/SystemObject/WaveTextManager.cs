/*
 *  @file   WaveTextManager
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// タイトル画面の動くテキスト
/// </summary>
public class WaveTextManager : SystemObject {
    public TMP_Text tmpText;                // TextMeshProUGUI
    private const float WAVA_SPEED = 2f;    // 波の速さ
    private const float WAVA_HEIGHT = 5f;   // 波の高さ

    private TMP_TextInfo textInfo;          // テキストの文字列保持構造体
    private Vector3[] vertices;             // 各文字の頂点データ

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        if (tmpText != null) {
            tmpText.ForceMeshUpdate();
            // テキスト情報を取得して保持
            textInfo = tmpText.textInfo;
        }
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Awake() {
        if (tmpText != null) {
            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    void Update() {
        if (tmpText == null || textInfo == null) return;
        // ウェーブアニメーションを行う
        AnimateWave();
    }

    /// <summary>
    /// ウェーブアニメ－ション
    /// </summary>
    private void AnimateWave() {
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;
        // 各文字事の処理
        for (int i = 0; i < textInfo.characterCount; i++) {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;                // 現在の文字の頂点インデックス
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;   // 使用しているマテリアルのインデックス

            vertices = textInfo.meshInfo[materialIndex].vertices;   // 頂点配列を取得

            // 波を上下に動かすOffset
            Vector3 offset = new Vector3(0, Mathf.Sin(Time.time * WAVA_SPEED + i) * WAVA_HEIGHT, 0);

            // 現在の文字にオフセットを適用
            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;

            // 頂点データを更新して画面に表示
            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }
    }

}
