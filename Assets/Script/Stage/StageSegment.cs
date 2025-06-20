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

        // 実際の回転処理
        transform.Rotate(0f, rotationSpeed * rotationInput * Time.deltaTime, 0f);
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
#if UNITY_EDITOR
        return 0f; // エディタではジャイロ無効
#else
        Quaternion att = Input.gyro.attitude;
        Quaternion corrected = new Quaternion(att.x, att.y, -att.z, -att.w);
        float roll = corrected.eulerAngles.z;
        if (roll > 180f) roll -= 360f;

        return Mathf.Clamp(roll / 30f, -1f, 1f);
#endif
    }

    /// <summary>
    /// 衝突判定
    /// </summary>
    public void StopStageMoving() {
        stopMove = true;
    }
}