using UnityEngine;

public class StageSegment : MonoBehaviour {
    private bool shouldRotate = false;
    private bool stopMove = false;



    void Start() {
        Input.gyro.enabled = true;
    }

    public void EnableRotation(bool _enable) {
        shouldRotate = _enable;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update() {
        if (!shouldRotate) return;

        float rotationSpeed = 60f;
        float rotationInput = GetPCInput() + GetGyroInput();

        float rota = rotationSpeed * rotationInput * Time.deltaTime;

        // 実際の回転処理
        transform.Rotate(0f, -rota, 0f);
    }

    /// <summary>
    /// PCの左右キーによる回転入力
    /// </summary>
    private float GetPCInput() {
        if (Input.GetKey(KeyCode.LeftArrow)) return 1f;
        if (Input.GetKey(KeyCode.RightArrow)) return -1f;
        return 0f;
    }

    /// <summary>
    /// スマホの傾き（ジャイロ）による回転入力
    /// </summary>
    private float GetGyroInput() {
        // ジャイロセンサーの向きを取得
        Quaternion att = Input.gyro.attitude;
        // 持ち方に応じた方向のずれを補正
        Quaternion corrected = new Quaternion(att.x, att.y, -att.z, -att.w);
        // 横向きでの姿勢調整
        Vector3 euler = corrected.eulerAngles;
        // X軸を補正
        // 角度を-180 ~ 180に変換
        float roll = euler.z;
        if (roll > 180f) roll -= 360f;
        // スマホを20度傾けたら最大量になる。
        return Mathf.Clamp(roll / 20f, -1f, 1f);
    }

    /// <summary>
    /// 衝突判定
    /// </summary>
    public void StopStageMoving() {
        stopMove = true;
    }
}