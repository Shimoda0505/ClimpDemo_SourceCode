using UnityEngine;
using System;



/// <summary>
/// プレイヤー挙動メソッド
/// </summary>
public class PlayerController : MonoBehaviour
{

    [Header("取得コンポーネント")]
    //スクリプト
    private S_PlayerAnimator _plAnim;//アニメーション管理メソッド
    private PlayerIK _plIK;//IK

    //コンポーネント
    private GameObject _pl;//プレイヤーオブジェクト
    private Rigidbody _rb;//プレイヤーのRigidbody
    private Animator _anim;//プレイヤーのAnimator
    private CapsuleCollider _col;//プレイヤーのcollider


    [Header("時間関連")]
    private float _count;//時間計測
    private const float _fallTime = 0.5f;//落下後のインターバル
    private const float _highWallTime = 0.5f;//高い壁開始インターバル


    [Header("速度関連")]
    [SerializeField, Tooltip("プレイヤーの歩き速度"),Range(0.1f,10f)] private float _moveWalkSpd;
    [SerializeField, Tooltip("プレイヤーの走り速度"), Range(0.1f, 10f)] private float _moveRunSpd;
    [SerializeField, Tooltip("階段を上り下りする速度"), Range(1f, 10f)] private float _stairsMoveSpd;
    [SerializeField, Tooltip("低い壁(上)移動速度"), Range(1f, 10f)] private float _lowWallUpMoveSpd;
    [SerializeField, Tooltip("低い壁(正面)移動速度"), Range(1f, 10f)] private float _lowWallForwardMoveSpd;
    [SerializeField, Tooltip("高い壁移動速度"), Range(1f, 10f)] private float _highWallMoveSpd;
    [SerializeField, Tooltip("壁の方向を向く回転速度"), Range(1f, 10f)] private float _wallRoteSpd;


    [Header("Collider関連")]
    private Vector3 _colDefCenter = default;//基準位置
    private float _colDefHeight = default;//基準サイズ
    private const float _colMoveSpd = 2f;//位置を調整する時のスピード


    [Header("パルクール関連")]
    //段差の移動速度
    //段差の座標
    private static Vector3 _stepAd = new Vector3(0.001f, 0.65f, 0.2f);//プレイヤーの移動先座標の加算
    private Vector3 _wallPos = new Vector3(0, 0, 0);//移動先の壁座標
    private Vector3 _wallMovePos = new Vector3(0, 0, 0);//移動先の壁座標
    private float _wallNearRange = 0.5f;//壁との距離を一定にする座標(手前)
    private float _wallBackRange = -0.35f;//壁との距離を一定にする座標(奥)
    private const float _wallEndOffset = 0.4f;//低い壁のY軸座標
    //その他
    private float _timePer = default;//移動座標に応じて一定の速度で登るための時間調整値
    private const float _disWall = 0.1f;//移動位置との許容範囲
    private const float _lowWallOffset = 1.2f;//低い壁のY軸座標


    [Header("その他")]
    //小数点の切り上げ
    private static int _decimalPoint = 4;

    /// <summary>
    /// プレイヤーのいるフィールドenum
    /// </summary>
    private enum PlayerField
    {
        [InspectorName("地上")] GROUND,
        [InspectorName("空中")] FLY,
        [InspectorName("壁")] WALL,
    }
    private PlayerField _plField = PlayerField.GROUND;

    /// <summary>
    /// 地上enum
    /// </summary>
    enum GroundMotion
    {
        [InspectorName("初期")] NOT_MOTION,
        [InspectorName("待機")] IDLE,
        [InspectorName("歩き")] WALK,
        [InspectorName("走り")] RUN,
    }
    private GroundMotion _groundMotion = GroundMotion.NOT_MOTION;

    /// <summary>
    /// 低い壁enum
    /// </summary>
    private enum LowWallMotion
    {
        [InspectorName("開始")] ENTER,
        [InspectorName("正面移動")] FRONT_MOVE,
        [InspectorName("上昇移動")] UP_MOVE,
        [InspectorName("前方移動")] FORWARD_MOVE,
    }
    private LowWallMotion _lowWallMotion = LowWallMotion.ENTER;

    /// <summary>
    /// 高い壁enum 
    /// </summary>
    private enum HighWallMotion
    {
        [InspectorName("開始")] ENTER,
        [InspectorName("待機")] IDLE,
        [InspectorName("移動")] MOVE,
        [InspectorName("飛び移動")] JUMP,
    }
    private HighWallMotion _highWallMotion = HighWallMotion.ENTER;



    //処理部-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {

        //取得コンポーネント
        _pl = this.gameObject;
        _rb = _pl.GetComponent<Rigidbody>();
        _anim = _pl.GetComponent<Animator>();
        _col = _pl.GetComponent<CapsuleCollider>();

        //コライダの中心位置
        _colDefCenter = _col.center;
        _colDefHeight = _col.height;
        _plIK = _pl.GetComponent<PlayerIK>();
        _plIK.IsFootIK(false);//IK停止
    }



    //メソッド部--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*#####[外部参照]#####*/
    /// <summary>
    /// プレイヤーの入力管理
    /// </summary>
    /// <param name="Left_S">LeftStickの入力</param>
    /// <param name="South_B">SouthBottunの入力</param>
    public void PlayerInput(bool Left_S_bool, Vector2 Left_S_Vector, bool South_B, bool South_B_D, bool East_B)
    {

        //プレイヤーのいるフィールド
        switch (_plField)
        {

            #region 地上case
            case PlayerField.GROUND:

                //地面に接していないなら
                if (!_plIK.GroundRay())
                {

                    //状態の更新
                    //_plAnim.MoveBoolAnim("FallRoop", _anim);//落下アニメーション
                    _plIK.IsFootIK(false);//IK停止

                    //Enumの遷移
                    _plField = PlayerField.FLY;//空中(フィールドenum)
                    _groundMotion = GroundMotion.NOT_MOTION;//初期化(地上enum)
                    break;
                }


                //LeftStick入力中
                if (!Left_S_bool)
                {

                    //Idle中は処理しない
                    if (_groundMotion == GroundMotion.IDLE) { return; }

                    //状態の更新
                    _plAnim.MoveBoolAnim("Idle", _anim);//Idleのアニメーション
                    _plIK.IsFootIK(true);//IK開始

                    //Enumの遷移
                    _groundMotion = GroundMotion.IDLE;//待機(地上enum)
                }

                //LeftStick未入力
                else if (Left_S_bool)
                {

                    //地上移動,壁があれば遷移
                    if (MoveGround(South_B_D))
                    {

                        //モーションの更新
                        //低い壁
                        if (_plIK._wallField == PlayerIK.WallField.LOW_WALL) {  }
                        //高い壁
                        else if (_plIK._wallField == PlayerIK.WallField.HIGH_WALL) { _plAnim.MoveBoolAnim("HighWall", _anim); _plAnim.HighWallFloatAnim(new Vector2(0, 1), _anim); }

                        //Enumの遷移
                        _plField = PlayerField.WALL;//壁(フィールドenum)
                        _groundMotion = GroundMotion.NOT_MOTION;//初期化(地上enum)
                        break;
                    }

                    //SouthButton入力中
                    if (South_B)
                    {

                        //Run中は処理しない
                        if (_groundMotion == GroundMotion.RUN) { return; }

                        //モーションの更新
                        _plAnim.MoveBoolAnim("Run", _anim);//Runアニメーション
                        _plIK.IsFootIK(false);//IK停止

                        //Enumの遷移
                        _groundMotion = GroundMotion.RUN;//走る(地上enum)
                    }
                    else if (!South_B)
                    {

                        //Walk中は処理しない
                        if (_groundMotion == GroundMotion.WALK) { return; }

                        //モーションの更新
                        _plAnim.MoveBoolAnim("Walk", _anim);//Walkのアニメーション
                        _plIK.IsFootIK(false);//IK停止

                        //Enumの遷移
                        _groundMotion = GroundMotion.WALK;//歩き(地上enum)
                    }
                }
                break;
            #endregion

            #region 空中case
            case PlayerField.FLY:

                //地面に接していないなら処理しない
                if (!_plIK.GroundRay()) { return; }


                //着地アニメーション
                _plAnim.MoveBoolAnim("FallEnd", _anim);

                //着地の姿勢にColliderの形を変更
                _col.height = _plAnim.ChangeCollider(_anim).Item1;
                _col.center = new Vector3(0, _plAnim.ChangeCollider(_anim).Item2, 0);

                //時間計測中は処理しない
                _count += Time.deltaTime;
                if (_count <= _fallTime) { return; }

                //colliderのheighがデフォルト位置に戻ったら
                if (_col.height >= _colDefHeight)
                {

                    _count = 0;

                    //Enumの遷移
                    _plField = PlayerField.GROUND;//地上(フィールドenum)
                    _groundMotion = GroundMotion.NOT_MOTION;//初期化(地上enum)
                }
                break;
            #endregion

            #region 壁case
            case PlayerField.WALL:

                //高い壁(フィールドenum)ではない || 待機(高い壁enum)ではないなら処理しない
                if (_plIK._wallField != PlayerIK.WallField.HIGH_WALL || _highWallMotion != HighWallMotion.IDLE) { return; }


                //移動先の探索
                if (Left_S_bool)
                {

                    //移動先の壁があるなら
                    //ヒットしたかどうか,ヒット座標,移動方向
                    (bool isHit, Vector3 hitPos, Vector2 moveAngle) _climbItem = _plIK.ClimbRay(Left_S_Vector);

                    //移動先の位置を補完
                    if (_climbItem.isHit)
                    {

                        //移動先の壁座標を計算
                        WallCal(_climbItem.hitPos, _pl.transform.position, _pl.transform.position);

                        //移動入力していれば
                        if (South_B_D)
                        {

                            //モーションの更新
                            _plAnim.MoveBoolAnim("HighWall", _anim);
                            _plAnim.HighWallFloatAnim(_climbItem.moveAngle, _anim);

                            //Enumの遷移
                            _highWallMotion = HighWallMotion.JUMP;//移動(高い壁enum)
                            break;
                        }
                    }
                }
                //落下
                if (East_B)
                {

                    //物理開始
                    _rb.isKinematic = false;

                    //手のIK停止
                    _plIK.IsHandIK(false);

                    //Enumの遷移
                    _highWallMotion = HighWallMotion.ENTER;//開始(高い壁enum)
                    _plField = PlayerField.GROUND;//地上(フィールドenum)
                }
                break;
            #endregion

            default:
                print("PlayerFieldのCaseなし");
                break;
        }
    }

    /// <summary>
    /// プレイヤーの移動管理
    /// </summary>
    public void PlayerMove(Vector2 Left_S)
    {

        switch (_plField)
        {

            case PlayerField.GROUND://地上

                //プレイヤーの移動管理(地上)
                switch (_groundMotion)
                {

                    case GroundMotion.NOT_MOTION://挙動の初期化
                        break;


                    case GroundMotion.IDLE://待機

                        //移動停止
                        _rb.velocity = new Vector3(0, 0, 0);
                        break;


                    case GroundMotion.WALK://歩き

                        //移動計算
                        GroundCal(_rb, _pl.transform, Left_S.normalized, _moveWalkSpd);
                        break;


                    case GroundMotion.RUN:

                        //移動計算
                        GroundCal(_rb, _pl.transform, Left_S.normalized, _moveRunSpd);
                        break;


                    default:
                        print("playerMotionのCaseなし");
                        break;

                }
                break;


            case PlayerField.FLY://空中
                break;


            case PlayerField.WALL://壁

                //低い壁
                if (_plIK._wallField == PlayerIK.WallField.LOW_WALL) { LowWallMove(); }
                //高い壁
                else if (_plIK._wallField == PlayerIK.WallField.HIGH_WALL) { HighWallMove(); }
                break;


            default:
                print("playerFieldのCaseなし");
                break;
        }
    }


    /*#####[内部参照]#####*/
    /****[PlayerInput関連]****/
    /// <summary>
    /// 前方の壁を探索しながら地上を移動
    /// </summary>
    /// <param name="input">入力値</param>
    /// <param name="moveSpeed">移動速度</param>
    /// <returns>壁があるかどうか</returns>
    private bool MoveGround(bool South_B_D)
    {

        //前方に段差があるか
        (Vector3 newPos, Vector3 nowPos) pos = _plIK.StepRay();

        //壁ならWALLのFieldモードに移行
        //壁なし
        if (_plIK._wallField == PlayerIK.WallField.NOT_WALL) { return false; }
        //平地 
        if (_plIK._wallField == PlayerIK.WallField.FLAT) { ColliderCal(pos.newPos, pos.nowPos); return false; }
        //坂
        else if (_plIK._wallField == PlayerIK.WallField.SLOPE) { ColliderCal(pos.newPos, pos.nowPos); return false; }
        //階段
        else if (_plIK._wallField == PlayerIK.WallField.STARIS) { TransformCal(pos.newPos, pos.nowPos); return false; }
        //壁
        if (South_B_D)
        {

            //低い壁
            if (_plIK._wallField == PlayerIK.WallField.LOW_WALL) { WallCal(pos.newPos, pos.nowPos, _pl.transform.position); return true; }
            //高い壁
            else if (_plIK._wallField == PlayerIK.WallField.HIGH_WALL) { _rb.isKinematic = true; _plIK.IsHandIK(true); return true; }
        }

        return false;
    }

    /// <summary>
    /// 低い壁の移動
    /// </summary>
    private void LowWallMove()
    {

        switch (_lowWallMotion)
        {

            /*壁の方向に体を向ける*/
            case LowWallMotion.ENTER://開始

                //壁とプレイヤーの平行向きベクトルを計算
                Vector3 moverotate = WallRotate(_pl.transform);

                //徐々に壁と平行になるように回転
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, moverotate, _wallRoteSpd);


                //壁の方向に座標が向くまで処理しない
                if(transform.eulerAngles == moverotate)
                {

                    //壁のぼりアニメーション
                    _plAnim.MoveBoolAnim("LowWall", _anim);

                    //正面の壁との距離を一定にする
                    _wallMovePos = WallDistance(_plIK.FrontRay().hitPos, _pl.transform.position, _wallNearRange);

                    //Enumの遷移
                    _lowWallMotion = LowWallMotion.FRONT_MOVE;//正面移動(低い壁enum)
                }

                break;


            /*壁の方向に体を近づける*/
            case LowWallMotion.FRONT_MOVE://正面移動

                //移動先の正面壁座標
                Vector3 moveFrontPos = new Vector3(_wallMovePos.x, _pl.transform.position.y, _wallMovePos.z);

                //前方向への移動
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, moveFrontPos, Time.deltaTime * _lowWallForwardMoveSpd);

                //Enumの遷移
                if (_pl.transform.position == moveFrontPos)
                {

                    //壁と平行に登る位置の平面座標(X,Z)
                    Vector3 movaPos = WallDistance(_plIK.FrontRay().hitPos, _pl.transform.position, _wallBackRange);

                    //移動先の壁座標
                    _wallPos = new Vector3(movaPos.x, _wallPos.y - _wallEndOffset, movaPos.z);

                    //Enumの遷移
                    _lowWallMotion = LowWallMotion.UP_MOVE;//上昇移動(低い壁enum)
                }
                break;


            /*目標座標Yに移動*/
            case LowWallMotion.UP_MOVE://上昇移動

                //移動先の壁Y座標
                Vector3 moveUpPos = new Vector3(_pl.transform.position.x, _wallPos.y - _lowWallOffset, _pl.transform.position.z);

                //前方向への移動
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, moveUpPos, Time.deltaTime * _lowWallUpMoveSpd * _timePer);

                //Enumの遷移
                if (_pl.transform.position == moveUpPos)
                {
                    //Enumの遷移
                    _lowWallMotion = LowWallMotion.FORWARD_MOVE;//前方移動(低い壁enum)
                }

                break;


            /*目標座標に移動*/
            case LowWallMotion.FORWARD_MOVE://前方移動

                //前方向への移動
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, _wallPos, Time.deltaTime * _lowWallForwardMoveSpd * _timePer);


                //移動位置に近づいたら
                float disPos = Mathf.Abs((float)Vector3.Distance(_pl.transform.position, _wallPos));
                if (disPos <= _disWall)
                {

                    //物理開始
                    _rb.isKinematic = false;

                    //Enumの遷移
                    _lowWallMotion = LowWallMotion.ENTER;//開始(低い壁enum)
                    _highWallMotion = HighWallMotion.ENTER;//開始(高い壁enum)
                    _groundMotion = GroundMotion.NOT_MOTION;//初期化(地上enum)
                    _plField = PlayerField.GROUND;//地上(フィールドenum)
                }
                break;


            default:
                print("LowWallMotionのcaseなし");
                break;
        }
    }

    /// <summary>
    /// 高い壁の移動
    /// </summary>
    private void HighWallMove()
    {

        switch (_highWallMotion)
        {

            case HighWallMotion.ENTER://開始

                //時間計測後に処理
                _count += Time.deltaTime;
                if (_count >= _highWallTime)
                {
                    
                    _count = 0;

                    //Enumの遷移
                    _highWallMotion = HighWallMotion.IDLE;//待機(高い壁enum)
                }
                break;


            case HighWallMotion.IDLE://待機

                //壁と平行に登る位置の平面座標(X,Z)
                Vector3 movaPos = WallDistance(_plIK.FrontRay().hitPos, _pl.transform.position, _wallNearRange);

                //移動先の壁座標
                Vector3 wallPos = new Vector3(movaPos.x, _pl.transform.position.y, movaPos.z);

                //壁とプレイヤーの平行向きベクトルを計算
                Vector3 moverotate = WallRotate(_pl.transform);

                //徐々に壁と平行になるように回転
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, moverotate, _wallRoteSpd);

                //前方向への移動
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, wallPos, Time.deltaTime * _highWallMoveSpd * _timePer);
                break;


            case HighWallMotion.JUMP://移動

                //通常の移動
                if (_plIK._hWallM == PlayerIK.HighWallMotion.NOMAL)
                {

                    //前方向への移動
                    _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, _wallPos - new Vector3(0, 1, 0), Time.deltaTime * _highWallMoveSpd * _timePer);

                    if (_pl.transform.position == _wallPos - new Vector3(0, 1, 0)) { _highWallMotion = HighWallMotion.IDLE;/*待機(高い壁enum)*/ }
                }
                //上方向の終了
                else if (_plIK._hWallM == PlayerIK.HighWallMotion.UP_END)
                {

                    //手のIK停止
                    _plIK.IsHandIK(false);

                    LowWallMove(); _plAnim.MoveBoolAnim("LowWall", _anim);
                }
                //下方向の終了
                else if (_plIK._hWallM == PlayerIK.HighWallMotion.DOWN_END)
                {

                    //物理開始
                    _rb.isKinematic = false;

                    //手のIK停止
                    _plIK.IsHandIK(false);

                    //Enumの遷移
                    _highWallMotion = HighWallMotion.ENTER;//開始(高い壁enum)
                    _plField = PlayerField.GROUND;//地上(フィールドenum)
                }
                break;


            default:
                print("HighWallMotionのcaseなし");
                break;
        }
    }

    /// <summary>
    /// コライダーの中心位置を移動(Collider,Transform,Default)
    /// </summary>
    /// <param name="movePos">移動先の位置</param>
    /// <param name="nowPos">現在の位置</param>
    private void ColliderCal(Vector3 movePos, Vector3 nowPos)
    {

        //両足のY軸の高さの差を計算
        //両足の高さ/2(四捨五入)の差を絶対値にする
        float moveYPos = 0;

        //平地
        if (_plIK._wallField == PlayerIK.WallField.FLAT) { moveYPos = _colDefCenter.y; }

        //坂
        else if (_plIK._wallField == PlayerIK.WallField.SLOPE)
        {

            //コライダーの初期位置にdistanceを加算
            float disFoot = Mathf.Abs((float)Math.Round(movePos.y, _decimalPoint) - (float)Math.Round(nowPos.y, _decimalPoint));
            moveYPos = _colDefCenter.y + disFoot - _stepAd.y;
        }

        //Colliderの位置を変更
        _col.center = Vector3.Lerp(_col.center, new Vector3(0f, moveYPos, 0f), Time.deltaTime * _colMoveSpd);
    }

    /// <summary>
    /// プレイヤーの中心位置を移動(Collider,Transform,Default)
    /// </summary>
    /// <param name="movePos">移動先の位置</param>
    /// <param name="nowPos">現在の位置</param>
    private void TransformCal(Vector3 movePos, Vector3 nowPos)
    {

        //両足のY軸の高さの差を計算
        //両足の高さ/2(四捨五入)の差を絶対値にする
        float disFoot = Mathf.Abs((float)Math.Round(movePos.y, _decimalPoint) - (float)Math.Round(nowPos.y, _decimalPoint));

        //Transformの初期位置にdistanceを加算
        _pl.transform.position = Vector3.Lerp(_pl.transform.position,
                                                  new Vector3(_pl.transform.position.x, _pl.transform.position.y + disFoot + _stepAd.y, _pl.transform.position.z),
                                                  Time.deltaTime * _stairsMoveSpd);
    }

    /// <summary>
    /// 壁の移動位置計算
    /// </summary>
    /// <param name="movePos">移動先の位置</param>
    /// <param name="nowPos">現在の位置</param>
    private void WallCal(Vector3 movePos, Vector3 nowPos, Vector3 playerPos)
    {

        //物理停止
        _rb.isKinematic = true;

        //２か所の座標のY軸差を求める
        float disFootY = (float)Math.Round(movePos.y, _decimalPoint) - (float)Math.Round(nowPos.y, _decimalPoint);

        //移動先の座標
        float _plPosY = playerPos.y + disFootY;

        //移動先の値を補完
        _wallPos = new Vector3(movePos.x, _plPosY, movePos.z);


        //移動先の距離に応じた移動速度の一定化
        //割合を100%にするための1
        if (disFootY != 0) { _timePer = 1 / Mathf.Abs(disFootY); }
        else
        {

            //目標地点の距離に対しての、移動時間を一定にする
            float disFoot = Vector3.Distance(movePos, nowPos);
            _timePer = 1 / Mathf.Abs(disFoot);
        }
    }


    /****[PlayerMove関連]****/
    /// <summary>
    /// プレイヤーの挙動(地面)
    /// </summary>
    /// <param name="rb">プレイヤーのRigidbody</param>
    /// <param name="tr">プレイヤーのTransform</param>
    /// <param name="input">コントローラーの入力値</param>
    /// <param name="speed">移動速度</param>
    private void GroundCal(Rigidbody playerRb, Transform playerTrans, Vector2 input, float speed)
    {

        //カメラの正面方向を所得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        //入力値ｔｐカメラの向きから移動方向を計算
        Vector3 moveForward = cameraForward * input.y + Camera.main.transform.right * input.x;

        //移動の反映
        playerRb.velocity = moveForward * speed + new Vector3(0, playerRb.velocity.y, 0);
        //rb.AddForce(input.x * speed, 0, input.y * speed, ForceMode.Impulse);

        //進行方向を向く
        playerTrans.rotation = Quaternion.LookRotation(moveForward);
    }

    /// <summary>
    /// 壁と平行に向きを変更する
    /// </summary>
    /// <returns>壁との平行向きベクトル</returns>
    private Vector3 WallRotate(Transform playerTrans)
    {

        //RayのHit面とプレイヤーの180度から減算した角度差
        float angleY = 180 - Quaternion.FromToRotation(playerTrans.forward, _plIK.FrontRay().hitRot).eulerAngles.y;

        //現在の角度との差を求める
        Vector3 targetAngle = playerTrans.eulerAngles - new Vector3(0, angleY, 0);

        return targetAngle;
    }

    /// <summary>
    /// 目標との距離を一定にする
    /// </summary>
    /// <param name="targetPos">目標座標</param>
    /// <param name="nowPos">現在の座標</param>
    /// <param name="range">距離の固定値</param>
    /// <returns>移動先の座標</returns>
    private Vector3 WallDistance(Vector3 targetPos, Vector3 nowPos, float range)
    {

        //距離
        float wallRange = Vector3.Distance(targetPos, nowPos);//壁とプレイヤーの直線距離 ... ①

        //比率 = ① / 壁とプレイヤーの直線距離(移動目標の固定位置) ,,, ②
        float percentage = wallRange / range;

        //円の中心点(wallPos)から現在のプレイヤー座標の距離 = 壁座標 / プレイヤー座標 ... ③
        float xDisPlayer = targetPos.x - nowPos.x;//x軸
        float zDisPlayer = targetPos.z - nowPos.z;//z軸

        //円の中心点から固定Rangeの距離 = ③ / ② ... ④
        float xDis = xDisPlayer / percentage;//cosθ
        float zDis = zDisPlayer / percentage;//sinθ

        //目標座標 = 壁座標 + ④
        Vector3 moveFrontPos = new Vector3(targetPos.x - xDis, 0, targetPos.z - zDis);

        return moveFrontPos;
    }
}
