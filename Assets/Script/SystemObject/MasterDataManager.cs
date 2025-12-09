using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MasterDataManager {
    // マスターデータのファイルパス
    private static readonly string _DATA_PATH = "MasterData/";
    // 読み込んだマスターデータ
    public static List<List<Entity_ScoreData.Param>> ScoreData = null;
    public static List<List<Entity_TimeScore.Param>> TimeScore = null;


    /// <summary>
    /// 全てのマスターデータを読み込む
    /// </summary>
    public static void LoadAllData() {
        ScoreData = Load<Entity_ScoreData, Entity_ScoreData.Sheet, Entity_ScoreData.Param>("ScoreData");
        TimeScore = Load<Entity_TimeScore, Entity_TimeScore.Sheet, Entity_TimeScore.Param>("TimeScore");
    }

    /// <summary>
    /// マスターデーターを読み込む
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="dataName"></param>
    /// <returns></returns>
    private static List<List<T3>> Load<T1, T2, T3>(string dataName) where T1 : ScriptableObject {
        // ファイルを読み込む
        T1 sourceData = Resources.Load<T1>(_DATA_PATH + dataName);
        // 名称指定でシートを取得
        FieldInfo sheetField = typeof(T1).GetField("sheets");
        List<T2> sheetListData = sheetField.GetValue(sourceData) as List<T2>;

        // 名称指定でフィールドを取得
        FieldInfo listField = typeof(T2).GetField("list");
        List<List<T3>> paramList = new List<List<T3>>();
        foreach (object elem in sheetListData) {
            List<T3> param = listField.GetValue(elem) as List<T3>;
            paramList.Add(param);
        }
        return paramList;
    }

}
