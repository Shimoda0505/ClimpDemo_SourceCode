using UnityEngine;



/// <summary>
/// 入力管理
/// </summary>
public class InputManager : MonoBehaviour
{

    [Header("取得コンポーネント")]
    //スクリプト
    private S_InputData _inputData;//InputSystemメソッド
    private CameraController _camCon;//カメラの挙動メソッド
    private PlayerController _plCon;//プレイヤーの挙動メソッド


    [Header("入力関連")]
    //入力のデットゾーン
    [SerializeField,Tooltip("入力のデットゾーン"),Range(0,1)]
    private float _deadZoneStick = 0.1f;


    [Header("Nullチェック")]
    private bool _isPl = false;//プレイヤー
    private bool _isCame = false;//カメラ


    //処理部------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {

        //取得コンポーネント
        _camCon = Camera.main.GetComponent<CameraController>();
        _plCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


        //Nullチェック
        if (_camCon == null) { _isCame = true; print("カメラがNull(InputManager)"); }
        if (_plCon == null) { _isPl = true; print("プレイヤーがNull(InputManager)"); }
    }

    private void Update()
    {

        /****[プレイヤー関連]****/
        //プレイヤーがNullではないなら処理
        if (!_isPl)
        {

            //LStick入力中
            _plCon.PlayerInput(_inputData.StickLeftBool(_deadZoneStick), _inputData.StickLeftValue(_deadZoneStick), _inputData.ButtonSouth(), _inputData.ButtonSouthDown(), _inputData.ButtonEast());
        }


        /****[カメラ関連]****/
        //カメラがNullではないなら処理
        if (!_isCame)
        {

            //カメラの状態enumの遷移
            _camCon.CameraInput(_inputData.StickRightBool(_deadZoneStick));
        }
    }

    private void FixedUpdate()
    {

        /****[プレイヤー関連]****/
        //プレイヤーがNullならreturn
        if (_isPl) { return; }

        //プレイヤーの移動管理
        _plCon.PlayerMove(_inputData.StickLeftValue(_deadZoneStick));
    }

    private void LateUpdate() 
    {

        /****[カメラ関連]****/
        //カメラがNullならreturn
        if (_isCame) { return; }

        //カメラの移動管理
        _camCon.CameraMove(_inputData.StickRightValue(_deadZoneStick)); 
    }
}
