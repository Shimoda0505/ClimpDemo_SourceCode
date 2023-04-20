using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//移動先の探索がうまくいかなかったため没


/// <summary>
/// プレイヤーの手IK
/// </summary>
public class PlayerHandIK : MonoBehaviour
{

    [Header("RayCast関連")]
    //Rayの長さ
    private static float _rayRange = 1.0f;

    //Rayを飛ばす位置の調整値
    private static float _rayPositionOffset = 1.5f;
    private static float _sRayPositionOffset = 3.3f;


    [Header("SphereCast関連")]
    //Rayの長さ
    private float _sRayRange = 2;

    //格納するtag名
    private string _rayCatchTag = "WallPoint";

    //移動先を格納するList
    private List<GameObject> _rayHits = new List<GameObject>();
   


    //メソッド部--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /****[前方に壁があるか]****/
    /// <summary>
    /// 前方に壁があるかのRayヒット処理
    /// </summary>
    /// <returns>前方に壁があるかどうか</returns>
    public bool HandRay()
    {

        //Rayを飛ばす処理
        Ray ray = new Ray(transform.position + transform.up * _rayPositionOffset, transform.forward);

        //Rayの視覚化
        Debug.DrawRay(transform.position + transform.up * _rayPositionOffset, transform.forward * _rayRange, Color.blue);

        //Rayの判定オブジェクト
        RaycastHit hit;

        //Rayの判定を返却
        return Physics.Raycast(ray, out hit, _rayRange);

    }


    /****[範囲内に移動先があるかを探索して移動]****/
    /// <summary>
    /// 移動先のポイントを探索
    /// </summary>
    public (GameObject, bool) SearchPoint(GameObject lastObj,Vector2 input,Transform headTr)
    {

        /****[ローカル変数の初期化]****/
        //移動先オブジェクトの初期化
        GameObject target = null;

        //移動先があるかどうか
        bool isMove = false;


        /****[SphereCastを使用した範囲探索]****/
        //SphereCastで範囲を探索して格納(原点,半径,向き)
        RaycastHit[] clampHits = Physics.SphereCastAll(transform.position + transform.up * _sRayPositionOffset, _sRayRange, transform.forward);

        //Ray配列の要素数が0ならnullを返却
        if (clampHits.Length < 0) { print("Ray配列の要素数が0"); return (target, isMove);  }

        //clampHitsに格納されているオブジェクトからWallPointタグのオブジェクトを再格納
        for (int i = 0; i <= clampHits.Length - 1; i++)
        {

            //clampHitsの各オブジェクトを設定
            GameObject newObj = null;
            newObj = clampHits[i].collider.gameObject;

            //tag名が一致 && 前回と違うオブジェクトならListに格納
            if (newObj.tag == _rayCatchTag && newObj != lastObj) { _rayHits.Add(newObj); }
        }

        //配列の要素数が0ならnullを返却
        if (_rayHits.Count < 0) { print("配列の要素数が0"); return (target, isMove); }


        /*****[入力方向 && 配列から最も近いオブジェクトを探索]****/
        //対象オブジェクトがプレイヤーと比較して上下左右どちらにあるか
        isMove = SearchPos(input.y, headTr, "上下");
        isMove = SearchPos(input.x, headTr, "左右");

        //移動先がないならNullを返却
        if (!isMove) { print("移動先がない"); return (target, isMove); }

        //一番近いオブジェクトを探索
        target = SearchNear(input, headTr.position);
        print("移動先ある" + target);

        //Listを全て削除
        _rayHits.Clear();

        //移動先を返却
        return (target, isMove);
    }

    /// <summary>
    /// 対象オブジェクトがプレイヤーと比較して左右上下どちらにあるか
    /// </summary>
    /// <param name="input">入力値</param>
    /// <param name="player">プレイヤーの座標</param>
    private bool SearchPos(float input, Transform headTr, string name)
    {

        //配列を全探索
        for (int i = _rayHits.Count - 1; i >= 0; i--)
        {

            //対象オブジェクトのx座標をローカル変換
            //対象オブジェクトx座標をローカル - プレイヤーx座標
            float dis = 0;
            if (name == "上下") { dis = _rayHits[i].transform.position.y - headTr.position.y; }
            else if(name == "左右") { dis = headTr.InverseTransformPoint(_rayHits[i].transform.position).x; }
            print(name + _rayHits[i] + dis);

            //入力値が(上,右),対象が(下,左)にある
            if (input >= 0 && dis < 0) { _rayHits.RemoveAt(i); }
            //入力値が(下,左),対象が(上,右)にある
            else if (input < 0 && dis >= 0) { _rayHits.RemoveAt(i);  }
        }

        //要素数が0ならfalse
        if (_rayHits.Count == 0) { return false; }
        return true;
    }

    /// <summary>
    /// 一番近いオブジェクトを探索
    /// </summary>
    /// <param name="input">入力値</param>
    /// <param name="playerPos">プレイヤーの座標</param>
    /// <returns>一番近いオブジェクト</returns>
    private GameObject SearchNear(Vector2 input, Vector3 playerPos)
    {

        /****[ローカル変数の初期化]****/
        //入力値の辺 = 三角形のtanθ(x入力を基準値1に設定)
        float inputTan = input.y * (1 / input.x);

        //一番近いオブジェクトの距離を初期化(値は適当)
        float disMin = 100;

        //一番近いオブジェクトを初期化
        GameObject nearObj = null;
        
        
        //配列を全探索
        for (int i = _rayHits.Count - 1; i >= 0; i--)
        {

            //オブジェクトの距離を初期化
            float dis = 0;

            //対象オブジェクトのVector2
            Vector2 targetPos = _rayHits[i].transform.position;

            //対象オブジェクトとプレイヤーの距離を計算
            if (-0.1f <=input.x && input.x <= 0.1f) { dis = targetPos.y; }//X軸の入力0
            else if (-0.1f <= input.y && input.y <= 0.1f) { dis = targetPos.x; }//Y軸の入力0
            else { dis = Mathf.Abs((targetPos.x + targetPos.y * inputTan) / inputTan); }
            print("距離" + _rayHits[i] + dis);

            //格納されているオブジェクトよりも近い距離があれば
            if (dis < disMin)
            {

                //一番近いオブジェクトを更新
                nearObj = _rayHits[i];

                //一番近いオブジェクトの距離を更新
                disMin = dis;
            }
        }

        //一番近いオブジェクトを返却
        return nearObj;
    }



    /// <summary>
    /// SphereCastの表示
    /// </summary>
    /// <param name="pos"></param>
    void OnDrawGizmos() { Gizmos.DrawWireSphere(transform.position + transform.up * _sRayPositionOffset, _sRayRange); }
}
