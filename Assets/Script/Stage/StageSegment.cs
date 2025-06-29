using UnityEngine;

public class StageSegment : MonoBehaviour {
    private bool shouldRotate = false;

    [SerializeField]
    private PlayerMove _playerMove = null;

    // 通過判定フラグ（StageManagerが使う）
    public bool hasPassed = false;

    void Start() {
        Input.gyro.enabled = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            _playerMove = playerObj.GetComponent<PlayerMove>();
        }
    }

    /// <summary>
    /// 回転許可を切り替え
    /// </summary>
    public void EnableRotation(bool enable) {
        shouldRotate = enable;
    }

    private void Update() {
        if (!shouldRotate) return;

        float rotationSpeed = 60f;
        float rotationInput = GetPCInput() + GetGyroInput();
        float rota = rotationSpeed * rotationInput * Time.deltaTime;

        transform.Rotate(0f, -rota, 0f);
    }

    /// <summary>
    /// PCの矢印キーによる回転入力
    /// </summary>
    private float GetPCInput() {
        if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
        if (Input.GetKey(KeyCode.RightArrow)) return 1f;
        return 0f;
    }

    /// <summary>
    /// スマホの傾き（ジャイロ）による回転入力
    /// </summary>
    private float GetGyroInput() {
        Quaternion att = Input.gyro.attitude;
        Quaternion corrected = new Quaternion(att.x, att.y, -att.z, -att.w);
        Vector3 euler = corrected.eulerAngles;

        float roll = euler.z;
        if (roll > 180f) roll -= 360f;

        return Mathf.Clamp(roll / 20f, -1f, 1f);
    }
}