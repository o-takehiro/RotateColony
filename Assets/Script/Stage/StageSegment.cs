using UnityEngine;

public class StageSegment : MonoBehaviour {
    private bool shouldRotate = false;

    [SerializeField]
    private PlayerMove _playerMove = null;

    // ’Ê‰ß”»’èƒtƒ‰ƒOiStageManager‚ªg‚¤j
    public bool hasPassed = false;

    private Quaternion initialGyroAttitude;

    /// <summary>
    /// ‰¡Œü‚«•â³
    /// </summary>
    public void CalibrateGyro() {
        // ‰¡Œü‚«Šî€‚Ì•â³
        Quaternion referenceRotation = Quaternion.Euler(0, 0, 90);
        initialGyroAttitude = referenceRotation * Input.gyro.attitude;
    }

    void Start() {
        Input.gyro.enabled = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {
            _playerMove = playerObj.GetComponent<PlayerMove>();
        }

        // ‰¡Œü‚«‚Ì•â³‚ğŠ|‚¯‚é
        CalibrateGyro();
    }

    /// <summary>
    /// ‰ñ“]‹–‰Â‚ğØ‚è‘Ö‚¦
    /// </summary>
    public void EnableRotation(bool enable) {
        shouldRotate = enable;
    }

    private void Update() {
        if (!shouldRotate) return;

        float rotationSpeed = 60f;
        float rotationInput = GetPCInput() + GetGyroInput();
        float rota = rotationSpeed * rotationInput * Time.deltaTime;

        // ‰ñ“]‚ğs‚¤
        transform.Rotate(0f, rota, 0f);

        // Šp“x‚ğŒ©‚Ä‚¨‚­b
        float currentYAngle = transform.eulerAngles.y;
        if (currentYAngle > 180f) currentYAngle -= 360f; // -180`+180 ‚É³‹K‰»

        if (Mathf.Abs(currentYAngle) < 5f) {
            // Šp“x0‹‚É‹ß‚Ã‚¢‚½‚ç‰ñ“]‚ğ~‚ß‚é
            shouldRotate = false;
        }
    }

    /// <summary>
    /// PC–îˆóƒL[‘€ì
    /// </summary>
    private float GetPCInput() {
        if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
        if (Input.GetKey(KeyCode.RightArrow)) return 1f;
        return 0f;
    }

    /// <summary>
    /// ƒWƒƒƒCƒ“ü—Í
    /// </summary>
    private float GetGyroInput() {
        // ‰¡Œü‚«‚ÌŠî€
        Quaternion referenceRotation = Quaternion.Euler(0, 0, 90);
        Quaternion current = referenceRotation * Input.gyro.attitude;

        // ‰Šúp¨‚Æ‚Ì·
        Quaternion corrected = Quaternion.Inverse(initialGyroAttitude) * current;

        // ƒIƒCƒ‰[‚É•ÏŠ·
        Vector3 euler = corrected.eulerAngles;

        float roll = euler.z;
        if (roll > 180f) roll -= 360f;

        return Mathf.Clamp(roll / 20f, -1f, 1f);
    }
}