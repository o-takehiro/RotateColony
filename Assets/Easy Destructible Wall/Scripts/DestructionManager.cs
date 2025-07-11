using UnityEngine;

namespace EasyDestuctibleWall {
    /// <summary>
    /// 破壊壁
    /// 使用アセット
    /// Easy Destructible Wall
    /// </summary>
    public class DestructionManager : MonoBehaviour {

        private float _health = 50f;
        private float _impactMultiplier = 10f;
        private float _twistMultiplier = 0.1f;

        // Rigidbodyのキャッシュ
        private Rigidbody cachedRigidbody;
        // プレイヤーのクラス
        [SerializeField]
        private PlayerMove _playerMove = null;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake() {
            cachedRigidbody = GetComponent<Rigidbody>();
            // プレイヤー散策
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) {
                _playerMove = playerObj.GetComponent<PlayerMove>();
            }
        }

        /// <summary>
        /// 毎フレーム更新
        /// </summary>
        private void FixedUpdate() {
            ApplyTwistDamage();
            CheckHealth();
        }

        /// <summary>
        /// オブジェクトの回転速度からダメージを算出し、ヘルスを減少させる
        /// </summary>
        private void ApplyTwistDamage() {
            // プレイヤーが加速中のみ回転ダメージを適用
            if (_playerMove == null || !_playerMove.boostFlag) return;

            float angularDamage = Mathf.Round(cachedRigidbody.angularVelocity.sqrMagnitude * _twistMultiplier);
            _health -= angularDamage;
        }

        /// <summary>
        /// ヘルスが0以下なら、子オブジェクトを分離し、自身を破壊する
        /// </summary>
        private void CheckHealth() {
            if (_health > 0f) return;

            foreach (Transform child in transform) {
                // 子オブジェクトにRigidbodyを追加して独立させる
                Rigidbody childRigidbody = child.gameObject.AddComponent<Rigidbody>();

                // 親子関係を解除
                child.parent = null;

                // 元の速度と回転を引き継ぐ
                childRigidbody.velocity = cachedRigidbody.GetPointVelocity(child.position);
                childRigidbody.AddTorque(cachedRigidbody.angularVelocity, ForceMode.VelocityChange);
            }

            // このオブジェクトを削除
            Destroy(gameObject);
        }

        /// <summary>
        /// 他オブジェクトと衝突した際の処理。衝突強度に応じてダメージを計算
        /// </summary>
        /// <param name="collision">衝突情報</param>
        private void OnCollisionEnter(Collision collision) {
            // プレイヤーが加速状態でないときは以下の処理を実行しない

            float relativeVelocity = collision.relativeVelocity.sqrMagnitude;
            float mass = collision.rigidbody ? collision.rigidbody.mass : 1f;
            float damage = relativeVelocity * _impactMultiplier * mass;

            _health -= damage;
        }
    }
}