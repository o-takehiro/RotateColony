using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CountdownManager : SystemObject {
    [SerializeField] private TMP_Text countdownText; // インスペクターで設定
    [SerializeField] private float displayTime = 1f; // 1 数字あたりの表示時間

    public override async UniTask Initialize() {
        // 初期化処理（特に無し）
        await UniTask.CompletedTask;
    }

    public async UniTask StartCountdown(int startNumber = 3) {
        await UniTask.Delay(1000);
        countdownText.gameObject.SetActive(true);

        for (int i = startNumber; i > 0; i--) {
            await ShowNumber(i.ToString(), displayTime);
        }

        await ShowNumber("START!", displayTime);

        countdownText.gameObject.SetActive(false);
    }

    // 数字やSTART!を表示してフェードイン・フェードアウト
    private async UniTask ShowNumber(string text, float duration) {
        countdownText.text = text;

        // フェードイン
        await FadeText(0f, 1f, duration * 0.5f);

        // フェードアウト
        await FadeText(1f, 0f, duration * 0.5f);
    }

    // アルファを変えてフェードする
    private async UniTask FadeText(float from, float to, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            countdownText.alpha = alpha;
            await UniTask.Yield();
        }
        countdownText.alpha = to;
    }
}