using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Destructible : MonoBehaviour {
    private int durability = 400;

    // 被ダメ
    public void TakeDamage(int amount) {
        durability -= amount;
        if (durability <= 0) {
            // エフェクト再生
            Vector3 efPos = transform.position;
            efPos.y += transform.position.y * -4f;
            EffectManager.Instance.Play("Nuke",efPos);
            // SE再生
            DestroySE();

            // 耐久値がなくなり次第削除
            Destroy(gameObject);
        }
    }

    private async void DestroySE() {
        await SoundManager.instance.PlaySE(8);
    }


}
