using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : SystemObject {

    public static MenuManager instance { get; private set; } = null;

    private List<MenuBase> _menuList = null;

    public override async UniTask Initialize() {
        instance = this;
        _menuList = new List<MenuBase>(256);
        await UniTask.CompletedTask;
    }


    /// <summary>
    /// メニューの取得
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T Get<T>(string name = null) where T : MenuBase {
        for (int i = 0, max = _menuList.Count; i < max; i++) {
            T menu = _menuList[i] as T;
            if (menu == null) continue;
            return menu;
        }
        return Load<T>(name);
    }

    /// <summary>
    /// メニューの読み込み
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    private T Load<T>(string name) where T : MenuBase {
        // 読み込み
        T menu = Resources.Load<T>(name);
        if (menu == null) return null;

        T createMenu = Instantiate(menu, transform);
        if (createMenu == null) return null;

        // タイトルメニュー以外のメニュー
        if (!(createMenu is MenuTitle)) {
            // ロード時には非表示にしておく
            createMenu.gameObject.SetActive(false);
        }

        _menuList.Add(createMenu);
        return createMenu;

    }


}
