using UnityEngine;



/// <summary>
/// カメラの挙動メソッド
/// </summary>
public class CameraController : MonoBehaviour
{


    [Header("カメラ関連")]
    //カメラの直視オブジェクトの位置
    private Transform _headTr;
    private Vector3 _camLookPos = default;

    [SerializeField, Tooltip("カメラの移動速度"),Range(1,20)]
    private float _camRotateSpd;
    private const float _camRotateMag = 10;//カメラの回転速度を100倍率から10倍率に変更

    //カメラの初期位置
    private const float _camDefDepth = -4f;

    //カメラの追従オブジェクト名
    private string _camLookPosName = "HeadPos";

    /// <summary>
    /// カメラの状態enum
    /// </summary>
    enum CameraMotion
    {
        [InspectorName("移動")] IDLE,
        [InspectorName("待機")] MOVE,
    }
    private CameraMotion _cameraMotion = CameraMotion.IDLE;



    //処理部------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {

        //カメラの直視オブジェクトの位置
        _headTr = GameObject.FindGameObjectWithTag(_camLookPosName).gameObject.transform;

        //カメラの直視オブジェクト
        _camLookPos = _headTr.position;

        //カメラをプレイヤーの背後に初期設定
        Resetting(_camDefDepth, _headTr.position);

        //カメラの回転速度を10倍率から100倍率に変更
        _camRotateSpd *= _camRotateMag;
    }



    //メソッド部--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*#####[外部参照]#####*/
    /// <summary>
    /// カメラの入力管理
    /// </summary>
    /// <param name="Right_S">RightStickの入力</param>
    public void CameraInput(bool Right_S)
    {

        if (Right_S) { _cameraMotion = CameraMotion.MOVE; }//移動
        else { _cameraMotion = CameraMotion.IDLE; }//待機
    }

    /// <summary>
    /// カメラの移動管理
    /// </summary>
    public void CameraMove(Vector2 Right_S)
    {

        switch (_cameraMotion)
        {

            case CameraMotion.IDLE:

                break;


            case CameraMotion.MOVE:

                //回転計算
                Rotate(_camLookPos, Right_S.normalized, _camRotateSpd);
                break;
        }

        //追従計算
        _camLookPos = Move(_headTr.position, _camLookPos);
    }


    /*#####[内部処理]#####*/
    /// <summary>
    /// カメラを初期位置に移動
    /// </summary>
    private void Resetting(float depthFloat, Vector3 targetPos)
    {

        //加算距離(float)をVector3に変換
        Vector3 depthPos = new Vector3(0, 0, depthFloat);

        //ターゲット + 加算位置に移動
        Camera.main.transform.position = targetPos + depthPos;
    }

    /// <summary>
    /// カメラの追従
    /// </summary>
    private Vector3 Move(Vector3 nowPos, Vector3 lastPos)
    {

        //カメラをターゲットの移動量分移動させる
        Camera.main.transform.position += nowPos - lastPos;

        //移動先の保管と更新を返却
        return lastPos = nowPos;
    }

    /// <summary>
    /// カメラの回転
    /// </summary>
    private void Rotate(Vector3 targetPos, Vector2 input, float speed)
    {

        //カメラの位置
        Transform camTr = Camera.main.transform;

        //カメラの横回転
        camTr.RotateAround(targetPos, Vector3.up, input.x * Time.deltaTime * speed);

        //カメラの縦回転
        camTr.RotateAround(targetPos, camTr.right, -input.y * Time.deltaTime * speed);
    }
}
