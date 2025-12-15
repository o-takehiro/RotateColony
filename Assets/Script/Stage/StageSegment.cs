/*
 *  @file    StageSegment.cs
 *  @author  oorui
 */

using UnityEngine;

public class StageSegment : MonoBehaviour {
    private bool shouldRotate = false;

    [SerializeField]
    private PlayerMove _playerMove = null;

    // 通過判定フラグ
    public bool hasPassed = false;

    private Quaternion initialGyroAttitude;

    /// <summary>
    /// 横向き補正
    /// </summary>
    public void CalibrateGyro() {
        // 横向き基準の補正
        Quaternion referenceRotation = Quaternion.Euler(0, 0, 90);
        // ジャイロ操作
        initialGyroAttitude = referenceRotation * Input.gyro.attitude;
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start() {
        Input.gyro.enabled = true;
        // プレイヤーを取得
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            _playerMove = playerObj.GetComponent<PlayerMove>();
        }

        // スマホの向きの補正を掛ける
        CalibrateGyro();
    }

    /// <summary>
    /// 回転許可を切り替え
    /// </summary>
    public void EnableRotation(bool enable) {
        shouldRotate = enable;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update() {
        if (!shouldRotate) return;

        float rotationSpeed = 60f;
        float rotationInput = GetPCInput() + GetGyroInput();
        float rota = rotationSpeed * rotationInput * Time.deltaTime;

        // 回転を行う
        transform.Rotate(0f, rota, 0f);

        // 角度
        float currentYAngle = transform.eulerAngles.y;
        if (currentYAngle > 180f) currentYAngle -= 360f;

        if (Mathf.Abs(currentYAngle) < 5f) {
            // 角度0°に近づいたら回転を止める
            shouldRotate = false;
        }
    }

    /// <summary>
    /// PC矢印キー操作
    /// </summary>
    private float GetPCInput() {
        if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
        if (Input.GetKey(KeyCode.RightArrow)) return 1f;
        return 0f;
    }

    /// <summary>
    /// ジャイロ入力
    /// </summary>
    private float GetGyroInput() {
        // 横向きの基準
        Quaternion referenceRotation = Quaternion.Euler(0, 0, 90);
        Quaternion current = referenceRotation * Input.gyro.attitude;

        // 初期姿勢との差
        Quaternion corrected = Quaternion.Inverse(initialGyroAttitude) * current;

        // オイラーに変換
        Vector3 euler = corrected.eulerAngles;

        float roll = euler.z;
        if (roll > 180f) roll -= 360f;

        return Mathf.Clamp(roll / 20f, -1f, 1f);
    }
}