using UnityEngine;
using System;



/// <summary>
/// プレイヤーのIK,Collider管理メソッド
/// </summary>
public class PlayerIK : MonoBehaviour
{

    [Header("取得コンポーネント")]
    //コンポーネント
    private GameObject _pl;//プレイヤーオブジェクト
    private Animator _anim;//プレイヤーのAnimator
    private CapsuleCollider _capCol;//プレイヤーのcollider


    [Header("足元のIK関連")]
    private static Vector3 _offset = new Vector3(0, 0.15f, 0);//足を置く位置のオフ設置値
    //足IK
    private const float _rayFootOffset = 0.15f;//Rayを飛ばす位置の調整値
    private const float _rayFootRange = 1.0f;//地上との距離を測るrayの長さ
    //手IK
    private const float _rayHandRange = 1.0f;//地上との距離を測るrayの長さ
    private const float _handIKOffset = 0.06f;//壁に手がかかっているようにアニメーションIKの座標を更新するときの距離
    private bool _isFootIK = false;//足IKを使用するかどうか
    private bool _isHandIK = false;//手IKを使用するかどうか


    [Header("Collider関連")]
    private Vector3 _colDefCenter = default;//コライダの中心位置
    private const float _colMoveSpd = 20;//Collider.center
    private const float _colPlus = 0.2f;//プレイヤーの移動先壁座標の加算


    [Header("段差判別関連")]
    //判別の許容距離
    [SerializeField, Tooltip("登れる階段の高さ"), Range(0.01f, 0.1f)] private float _stepOrStairs;
    [SerializeField, Tooltip("登れる低い壁の高さ"), Range(1, 5)] private float _lowWall;
    [SerializeField, Tooltip("登れる高い壁の高さ"), Range(10, 50)] private float _highWall;
    private float _stepOrStairsOffset = default;
    //Rayの長さ
    private const float _rayGroundRange = 1.5f;//地上(足元から下に飛ばす)
    private const float _rayStepRange = 1.0f;//段差(体から前に飛ばす)
    private const float __rayWallRange = 1.0f;//壁(体から前に飛ばす)
    //Rayを前に飛ばすY軸の調整値
    private const float _rayStepPos = 0.1f;//段差
    private const float _rayStairsPos = 0.2f;//階段
    private const float __rayWallPos = 0.8f;//壁
    //Rayを下に飛ばす位置の調整値
    private const float _rayGroundPos = 0.5f;//足元(Y軸)
    private const float _rayUpPos = 20f;//上方向の位置
    private const float _rayDown = 20f;//上から下に落とす位置(Y軸)


    [Header("高い壁Rayの探索関連")]
    //Rayの長さ
    private const float _rayHighRange = 1.0f;
    //Rayの飛ばす座標の調整値
    private static Vector3 _rayHighPos = new Vector3(2.0f, 2.0f, 1.0f);//飛ばす座標位置
    private const float _rayYPos = 1.0f;//中心のY座標
    //入力の許容値
    private const float _inputClamp = 0.4f;


    [Header("その他")]
    //小数点の切り上げ
    private const int _decimalPoint = 4;

    /// <summary>
    /// 段差enum
    /// </summary>
    public enum WallField
    {
        [InspectorName("平地")] FLAT,
        [InspectorName("坂")] SLOPE,
        [InspectorName("階段")] STARIS,
        [InspectorName("低い壁")] LOW_WALL,
        [InspectorName("高い壁")] HIGH_WALL,
        [InspectorName("登れない壁")] NOT_WALL,
    }
    [HideInInspector] public WallField _wallField = WallField.SLOPE;

    /// <summary>
    /// 高い壁の移動先探索enum
    /// </summary>
    public enum HighWallMotion
    {
        [InspectorName("上(終了)")] UP_END,
        [InspectorName("通常")] NOMAL,
        [InspectorName("下(終了)")] DOWN_END,
    }
    [HideInInspector] public HighWallMotion _hWallM = HighWallMotion.NOMAL;



    //処理部-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        //取得コンポーネント
        _pl = this.gameObject;
        _anim = _pl.GetComponent<Animator>();
        _capCol = _pl.GetComponent<CapsuleCollider>();

        //コライダの中心位置
        _colDefCenter = _capCol.center;

        //段差と会談判別rayの差分登れる会談の高さを加算
        _stepOrStairsOffset =  _stepOrStairs + _rayStairsPos - _rayStepPos;
    }



    //メソッド部--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*#####[外部参照]#####*/
    /// <summary>
    /// 設置判定のRay
    /// </summary>
    /// <param name="name">Ground,Fly</param>
    /// <returns></returns>
    public bool GroundRay()
    {

        //IKの足座標
        Vector3 rightIK = _anim.GetIKPosition(AvatarIKGoal.RightFoot);
        Vector3 leftIK = _anim.GetIKPosition(AvatarIKGoal.LeftFoot);

        //rayの左右位置
        Vector3 rightPos = transform.up * _rayGroundPos + new Vector3(rightIK.x, transform.position.y, rightIK.z);
        Vector3 leftPos = transform.up * _rayGroundPos + new Vector3(leftIK.x, transform.position.y, leftIK.z);

        //Rayを飛ばす処理
        Ray rayR = new Ray(rightPos, Vector3.down);
        Ray rayL = new Ray(leftPos, Vector3.down);

        //Rayの視覚化
        Debug.DrawRay(rightPos, Vector3.down * _rayGroundRange, Color.red);
        Debug.DrawRay(leftPos, Vector3.down * _rayGroundRange, Color.red);

        //Rayの判定フラグ
        RaycastHit hit;

        //Rayの判定がTrue
        if (Physics.Raycast(rayR, out hit, _rayGroundRange)) { return true; }
        else if (Physics.Raycast(rayL, out hit, _rayGroundRange)) { return true; }
        return false;
    }

    /// <summary>
    /// 前方にRayを飛ばす
    /// </summary>
    /// <returns>正面のオブジェクトを探索</returns>
    public (bool isHit, Vector3 hitRot, Vector3 hitPos) FrontRay()
    {

        //ローカル変数
        Vector3 rayPos = transform.position + transform.up * _rayStairsPos;
        Ray ray = new Ray(rayPos, transform.forward);//Rayを飛ばす処理
        RaycastHit hit;//Rayの判定オブジェクト        
        Vector3 hitRot = default;
        Vector3 hitPos = default;

        //RayのHit補完
        bool isHit = Physics.Raycast(ray, out hit, _rayStepRange);//フラグ
        if (isHit)
        {

            hitRot = hit.normal;
            hitPos = hit.point;
        }

        //Rayの視覚化
        Debug.DrawRay(rayPos, transform.forward * _rayStepRange, Color.green);

        //RayのHit判定と座標を返却
        return (isHit, hitRot, hitPos);
    }

    /// <summary>
    /// 前方に段差があるかのRayヒット処理
    /// Updteで処理をしてプレイヤーのenumの変更をする
    /// </summary>
    /// <returns>坂,階段,低い壁,高い壁</returns>
    public (Vector3 newPos, Vector3 nowPos) StepRay()
    {

        //前方に段差があるかどうか,段差RayのHitPos,段差の角度
        (bool isHit, Vector3 hitPos) stepItem = ThrowRay(_rayStepPos, _rayStepRange);


        //段差がある
        if (stepItem.isHit)
        {

            //登れる高さかどうか
            //前方に壁があるか,壁RayのHitPos,段差の角度
            (bool isHit, Vector3 hitPos) wallItem = ThrowRay(__rayWallPos, _rayStepRange);

            //前方の段差の高さ
            (bool isHit, Vector3 hitPos) heightItem = UpRay(_rayDown, __rayWallRange, _rayUpPos);


            //壁ではない
            if (!wallItem.Item1)
            {

                //前方に階段があるかどうか,階段RayのHitPos,階段の角度
                (bool isHit, Vector3 hitPos) stairsItem = ThrowRay(_rayStairsPos, _rayStepRange);

                //段差Rayと階段RayのHitPosの距離を計算
                if (stairsItem.isHit)
                {

                    //段差の奥行を計算(X,Z軸)
                    float disRay = Vector3.Distance(stepItem.hitPos, stairsItem.hitPos);

                    //坂
                    if (disRay >= _stepOrStairsOffset) { _wallField = WallField.SLOPE; }
                    //階段
                    else { _wallField = WallField.STARIS; }

                    return (heightItem.hitPos, stepItem.hitPos);
                }
            }

            //壁
            else if (wallItem.Item1)
            {

                //登れる壁
                if (heightItem.isHit)
                {

                    //前方の壁の高さ
                    float heightWall = Vector3.Distance(stepItem.Item2, heightItem.hitPos);

                    //低い壁
                    if (heightWall <= _lowWall) { _wallField = WallField.LOW_WALL; return (heightItem.hitPos, stepItem.hitPos); }
                    //高い壁
                    else if (_lowWall < heightWall && heightWall <= _highWall) { _wallField = WallField.HIGH_WALL; return (heightItem.hitPos, stepItem.hitPos); }
                }

                //登れない壁
                else { _wallField = WallField.NOT_WALL; return (new Vector3(0, 0, 0), stepItem.hitPos); }
            }
        }

        //段差なし
        //平地
        _wallField = WallField.FLAT;

        //Colliderをデフォルト位置に戻す
        return (new Vector3(0, 0, 0), stepItem.Item2);
    }

    /// <summary>
    /// 高い壁にrayを飛ばして移動先を探索
    /// </summary>
    /// <param name="input">入力値</param>
    /// <returns>移動先があるか,移動位置,現在の位置</returns>
    public (bool isHit, Vector3 hitPos, Vector2 moveInput) ClimbRay(Vector2 input)
    {

        //ローカル変数の初期化
        Vector3 nowPos = transform.position;//現在位置
        Vector3 rayInput = new Vector3(0, 0, 0);//入力方向

        //入力値に応じてrayの方向を変換
        //X軸
        if (-_inputClamp >= input.x || input.x >= _inputClamp)
        {

            if (input.x < 0) { rayInput.x = -_rayHighPos.x; }
            else { rayInput.x = _rayHighPos.x; }
        }
        //Y軸
        if (-_inputClamp >= input.y || input.y >= _inputClamp)
        {

            if (input.y < 0) { rayInput.y = -_rayHighPos.y; }
            else { rayInput.y = _rayHighPos.y; }
        }

        //周囲正面にrayを飛ばす
        (bool isHit, Vector3 hitPos) aroundItem = AroundRay(rayInput, _rayHighRange);

        //移動先があるか,移動位置,現在の位置を返却
        return (aroundItem.isHit, aroundItem.hitPos, new Vector2(rayInput.x / _rayHighPos.x, rayInput.y / _rayHighPos.y));
    }

    /// <summary>
    /// IKを使用するかどうかを変更
    /// </summary>
    /// <param name="isFlag">true,false</param>
    public void IsFootIK(bool isFlag)
    {

        if (isFlag) { _isFootIK = true; }
        else if (!isFlag) { _isFootIK = false; }
    }

    /// <summary>
    /// IKを使用するかどうかを変更
    /// </summary>
    /// <param name="isFlag">true,false</param>
    public void IsHandIK(bool isFlag)
    {

        if (isFlag) { _isHandIK = true; }
        else if (!isFlag) { _isHandIK = false; }
    }


    /*#####[内部処理]#####*/
    /****[Ray管理]****/
    /// <summary>
    /// 前方にRayを飛ばす
    /// </summary>
    /// <param name="rayYPos">RayのY座標</param>
    /// <param name="rayRange">Rayの長さ</param>
    /// <returns>RayがHitしたかどうか</returns>
    private (bool isHit, Vector3 hitPos) ThrowRay(float rayYVector, float rayRange)
    {

        //ローカル変数
        Vector3 rayPos = transform.position + transform.up * rayYVector;
        Ray ray = new Ray(rayPos, transform.forward);//Rayを飛ばす処理
        RaycastHit hit;//Rayの判定オブジェクト        

        //RayのHit補完
        bool isHit = Physics.Raycast(ray, out hit, rayRange);//フラグ
        Vector3 hitPos = hit.point;//Hit位置

        //Rayの視覚化
        //Rayの長さをHit位置に変更
        if (isHit) { rayRange = Vector3.Distance(rayPos, hitPos); }
        Debug.DrawRay(rayPos, transform.forward * rayRange, Color.green);

        //RayのHit判定と座標を返却
        return (isHit, hitPos);
    }

    /// <summary>
    /// 上からRayを飛ばしてHit位置の高さを計算
    /// </summary>
    /// <param name="rayYPos">RayのY座標</param>
    /// <param name="rayXPos">RayのX座標</param>
    /// <param name="rayRange">Rayの長さ</param>
    /// <returns></returns>
    private (bool isHit, Vector3 hitPos) UpRay(float rayYVector, float rayXVector, float rayRange)
    {

        //Rayの初期値
        Vector3 rayPos = transform.position + transform.up * rayYVector + transform.forward * rayXVector;
        Ray ray = new Ray(rayPos, Vector3.down);
        RaycastHit hit;

        //Rayの当たった位置と角度を補完
        //Rayの長さをHit位置に変更
        bool isHit = Physics.Raycast(ray, out hit, rayRange);
        Vector3 hitPos = hit.point;//位置

        //Rayの視覚化
        //Rayの長さをHit位置に変更
        if (isHit) { rayRange = Vector3.Distance(rayPos, hitPos); }
        Debug.DrawRay(rayPos, Vector3.down * rayRange, Color.green);

        //RayのHit判定と座標を返却
        return (isHit, hitPos);
    }

    /// <summary>
    /// 周囲正面にrayを飛ばす
    /// </summary>
    /// <param name="rayVector">入力に応じたrayの位置</param>
    /// <param name="rayRange">rayの長さ</param>
    /// <returns></returns>
    private (bool isHit, Vector3 hitPos) AroundRay(Vector3 rayVector, float rayRange)
    {

        //壁の移動座標を探索
        //Rayの初期値
        Vector3 rayPosF = transform.position + transform.up * rayVector.y + transform.right * rayVector.x;
        Ray rayF = new Ray(rayPosF, transform.forward);
        RaycastHit hitF;

        //Rayの当たった位置と角度を補完
        bool isHitF = Physics.Raycast(rayF, out hitF, rayRange);//Hit判定
        Debug.DrawRay(rayPosF, transform.forward * rayRange, Color.green);//Rayの視覚化
        Vector3 hitPosF = hitF.point;//位置


        //上方向に移動先がない時に最終位置を探索
        //上方向の入力
        if (!isHitF && rayVector.y >= 0)
        {

            //上方向の壁終了位置を補完
            //Rayの初期値
            Vector3 rayPosD = rayPosF + transform.forward * rayRange;
            Ray rayD = new Ray(rayPosD, -transform.up);
            RaycastHit hitD;

            //Rayの当たった位置と角度を補完
            bool isHitD = Physics.Raycast(rayD, out hitD, rayRange);//Hit判定
            Debug.DrawRay(rayPosD, -transform.up * rayRange, Color.green);//Rayの視覚化
            Vector3 hitPosD = hitD.point;//位置

            if (isHitD)
            {

                //上方向の終了
                _hWallM = HighWallMotion.UP_END;

                //RayのHit判定と座標を返却
                return (isHitD, hitPosD);
            }
        }


        //移動先に障害物がないかを探索
        //Rayの初期値
        Vector3 rayPosA = transform.position + transform.up * _rayYPos;

        //Rayの当たった位置と角度を補完
        RaycastHit hitA;
        bool isHitA = Physics.Linecast(rayPosA, rayPosF, out hitA);
        Debug.DrawLine(rayPosA, rayPosF, Color.green);//Rayの視覚化
        Vector3 hitPosA = hitA.point;//位置

        //下方向に移動先がない時に最終位置を探索
        if (isHitA)
        {

            //Rayの長さをHit位置に変更,hitしたらisHitをfalse
            isHitF = false;

            //上方向の入力
            if (rayVector.y < 0)
            {

                //下方向の終了
                _hWallM = HighWallMotion.DOWN_END;

                //RayのHit判定と座標を返却
                return (isHitA, hitPosA);
            }
        }


        //rayのhit座標をプレイヤーのxz軸に変更
        hitPosF = new Vector3(rayPosF.x, hitPosF.y, rayPosF.z);

        //通常の移動
        _hWallM = HighWallMotion.NOMAL;

        //RayのHit判定と座標を返却
        return (isHitF, hitPosF);
    }


    /****[IK管理]****/
    /// <summary>
    /// IKが機能した時のみ呼び出し
    /// </summary>
    void OnAnimatorIK()
    {

        //足IKがTrue中のみ処理
        if (_isFootIK)
        {

            /*[ローカル変数]*/
            //アニメーションパラメータから現在のIKのウエイトを取得
            float rightFootWeight = _anim.GetFloat("RightFootWeight");//右足のウエイト
            float leftFootWeight = _anim.GetFloat("LeftFootWeight");//左足のウエイト

            //両足のIK位置
            Vector3 rightFootIKPos = new Vector3(0, 0);//右足IKの位置
            Vector3 leftFootIKPos = new Vector3(0, 0);//左足IKの位置

            //足のRayが地面についているかどうか
            bool isRightFootRay = false;//右足のフラグ
            bool isLeftFootRay = false;//左足のフラグ


            /*[処理]*/
            //コライダの中心位置を移動
            ColliderMove(rightFootIKPos, leftFootIKPos);

            //Rayのヒット処理
            //ヒット位置,ヒットしたかどうか
            //AvatarIKGoal = ボディパーツのターゲット位置と角度
            (isRightFootRay, rightFootIKPos) = FootRay(AvatarIKGoal.RightFoot, rightFootWeight, rightFootIKPos, isRightFootRay);//右足
            (isLeftFootRay, leftFootIKPos) = FootRay(AvatarIKGoal.LeftFoot, leftFootWeight, leftFootIKPos, isLeftFootRay);//左足

            //体の重心を移動
            BodyMove(rightFootIKPos, leftFootIKPos, isRightFootRay, isLeftFootRay);
        }

        //手IKがTrue中のみ処理
        if(_isHandIK)
        {

                        /*[ローカル変数]*/
            //アニメーションパラメータから現在のIKのウエイトを取得
            float rightHandWeight = _anim.GetFloat("RightHandWeight");//右足のウエイト
            float leftHandWeight = _anim.GetFloat("LeftHandWeight");//左足のウエイト

            //両足のIK位置
            Vector3 rightHandIKPos = new Vector3(0, 0);//右足IKの位置
            Vector3 leftHandIKPos = new Vector3(0, 0);//左足IKの位置


            /*[処理]*/
            //Rayのヒット処理
            //ヒット位置,ヒットしたかどうか
            //AvatarIKGoal = ボディパーツのターゲット位置と角度
            HandRay(AvatarIKGoal.RightHand, rightHandWeight, rightHandIKPos);//右足
            HandRay(AvatarIKGoal.LeftHand, leftHandWeight, leftHandIKPos);//左足
        }
    }

    //FootIK
    /// <summary>
    /// Rayのヒット処理(IK)
    /// </summary>
    /// <param name="ikGoal">AvatarIKのname</param>
    /// <param name="footWeight">足のウエイト</param>
    /// <param name="footIKPos">足IKの位置</param>
    /// <param name="footBool">足のRayが地面にヒットしているかどうか</param>
    /// <returns>Rayのヒット位置,Rayがヒットしたかどうか</returns>
    private (bool isHit, Vector3 hitPos) FootRay(AvatarIKGoal ikGoal, float footWeight, Vector3 footIKPos, bool footBool)
    {

        //Rayを飛ばす処理
        Ray ray = new Ray(_anim.GetIKPosition(ikGoal) + Vector3.up * _rayFootOffset, Vector3.down);

        //Rayの視覚化
        Debug.DrawRay(_anim.GetIKPosition(ikGoal) + Vector3.up * _rayFootOffset, Vector3.down * _rayFootRange, Color.red);

        //Rayの判定フラグ
        RaycastHit hit;

        //Rayの判定がTrue
        if (Physics.Raycast(ray, out hit, _rayFootRange))
        {

            //足のRayが地面についている
            footBool = true;

            //Rayの当たった位置を補完
            footIKPos = hit.point;

            //足IKの位置
            _anim.SetIKPositionWeight(ikGoal, footWeight);//足IK位置ウエイトの設定
            _anim.SetIKPosition(ikGoal, footIKPos + _offset);//足IK位置の設定

            //Rayの当たった角度を補完
            Quaternion footIKRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            //足IKの角度
            _anim.SetIKRotationWeight(ikGoal, footWeight);//足IK角度ウエイトの設定
            _anim.SetIKRotation(ikGoal, footIKRot);//足IK角度の設定
        }

        //Rayの判定がFalse
        //足のRayが地面についていない
        else { footBool = false; }

        //Rayのヒット位置,Rayがヒットしたかどうか
        return (footBool, footIKPos);
    }

    /// <summary>
    /// コライダーの中心位置を移動(Collider,Transform,Default)
    /// </summary>
    /// <param name="rightIkVector3">右足IKの位置</param>
    /// <param name="leftIkVector3">左足IKの位置</param>
    private void ColliderMove(Vector3 rightIkVector3, Vector3 leftIkVector3)
    {

        //両足のY軸の高さの差を計算
        //両足の高さ/2(四捨五入)の差を絶対値にする
        float moveYPos = 0;

        //コライダーの初期位置にdistanceを加算
        float disFoot = Mathf.Abs((float)Math.Round(rightIkVector3.y, _decimalPoint) - (float)Math.Round(leftIkVector3.y, _decimalPoint));
        moveYPos = _colDefCenter.y + disFoot - _colPlus;

        //Colliderの位置を変更
        _capCol.center = Vector3.Lerp(_capCol.center, new Vector3(0f, moveYPos, 0f), Time.deltaTime * _colMoveSpd);
    }

    /// <summary>
    /// 体の重心を移動
    /// </summary>
    /// <param name="rightVector3">右足IKの位置</param>
    /// <param name="leftVector3">左足IKの位置</param>
    /// <param name="rightBool">右足のフラグ</param>
    /// <param name="leftBool">左足のフラグ</param>
    private void BodyMove(Vector3 rightIkVector3, Vector3 leftIkVector3, bool rightBool, bool leftBool)
    {

        //両足のRayがヒットしていないなら処理しない
        if (!rightBool || !leftBool) { return; }

        //左右の足とキャラクターの足元の位置との距離を計算
        float rightFootDistance = rightIkVector3.y - transform.position.y;//右足の距離
        float leftFootDistance = leftIkVector3.y - transform.position.y;//左足の距離

        //左右の足の位置がより下にある方を距離として使う
        //右足 < 左足 = 右足の距離 : 左足の距離
        float dis = rightFootDistance < leftFootDistance ? rightFootDistance : leftFootDistance;

        //体の重心を下にある方の足に合わせて下げる
        //現在の重心位置 + 足の距離
        Vector3 newBodyPosition = _anim.bodyPosition + Vector3.up * dis;
        _anim.bodyPosition = newBodyPosition;
    }

    //HandIK
    /// <summary>
    /// Rayのヒット処理(IK)
    /// </summary>
    /// <param name="ikGoal">AvatarIKのname</param>
    /// <param name="footWeight">足のウエイト</param>
    /// <param name="footIKPos">足IKの位置</param>
    /// <returns>Rayのヒット位置,Rayがヒットしたかどうか</returns>
    private void HandRay(AvatarIKGoal ikGoal, float handWeight, Vector3 handIKPos)
    {

        //Rayを飛ばす処理
        Ray ray = new Ray(_anim.GetIKPosition(ikGoal), Vector3.right);

        //Rayの視覚化
        Debug.DrawRay(_anim.GetIKPosition(ikGoal), Vector3.right * _rayFootRange, Color.red);

        //Rayの判定フラグ
        RaycastHit hit;

        //Rayの判定がTrue
        if (Physics.Raycast(ray, out hit, _rayHandRange))
        {

            //Rayの当たった位置を補完
            handIKPos = hit.point;

            Vector3 newPos = WallDistance(handIKPos, _anim.GetIKPosition(ikGoal), _handIKOffset);
            handIKPos = new Vector3(newPos.x, handIKPos.y, newPos.z);

            //足IKの位置
            _anim.SetIKPositionWeight(ikGoal, handWeight);//足IK位置ウエイトの設定
            _anim.SetIKPosition(ikGoal, handIKPos);//足IK位置の設定

            //Rayの当たった角度を補完
            Quaternion footIKRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            //足IKの角度
            _anim.SetIKRotationWeight(ikGoal, handWeight);//足IK角度ウエイトの設定
            _anim.SetIKRotation(ikGoal, footIKRot);//足IK角度の設定
        }
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

        //比率 = ① / 壁とプレイヤーの直線距離(移動目標の固定位置) ,,, ③
        float percentage = wallRange / range;

        //円の中心点(wallPos)から現在のプレイヤー座標の距離 = 壁座標 / プレイヤー座標 ... ④
        float xDisPlayer = targetPos.x - nowPos.x;//x軸
        float zDisPlayer = targetPos.z - nowPos.z;//z軸

        //円の中心点から固定Rangeの距離 = ④ / ③ ... ⑤
        float xDis = xDisPlayer / percentage;//cosθ
        float zDis = zDisPlayer / percentage;//sinθ

        //目標座標 = 壁座標 + ⑤
        Vector3 moveFrontPos = new Vector3(targetPos.x - xDis, 0, targetPos.z - zDis);

        return moveFrontPos;
    }
}
