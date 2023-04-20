using UnityEngine;



/// <summary>
/// ���͊Ǘ�
/// </summary>
public class InputManager : MonoBehaviour
{

    [Header("�擾�R���|�[�l���g")]
    //�X�N���v�g
    private S_InputData _inputData;//InputSystem���\�b�h
    private CameraController _camCon;//�J�����̋������\�b�h
    private PlayerController _plCon;//�v���C���[�̋������\�b�h


    [Header("���͊֘A")]
    //���͂̃f�b�g�]�[��
    [SerializeField,Tooltip("���͂̃f�b�g�]�[��"),Range(0,1)]
    private float _deadZoneStick = 0.1f;


    [Header("Null�`�F�b�N")]
    private bool _isPl = false;//�v���C���[
    private bool _isCame = false;//�J����


    //������------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {

        //�擾�R���|�[�l���g
        _camCon = Camera.main.GetComponent<CameraController>();
        _plCon = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


        //Null�`�F�b�N
        if (_camCon == null) { _isCame = true; print("�J������Null(InputManager)"); }
        if (_plCon == null) { _isPl = true; print("�v���C���[��Null(InputManager)"); }
    }

    private void Update()
    {

        /****[�v���C���[�֘A]****/
        //�v���C���[��Null�ł͂Ȃ��Ȃ珈��
        if (!_isPl)
        {

            //LStick���͒�
            _plCon.PlayerInput(_inputData.StickLeftBool(_deadZoneStick), _inputData.StickLeftValue(_deadZoneStick), _inputData.ButtonSouth(), _inputData.ButtonSouthDown(), _inputData.ButtonEast());
        }


        /****[�J�����֘A]****/
        //�J������Null�ł͂Ȃ��Ȃ珈��
        if (!_isCame)
        {

            //�J�����̏��enum�̑J��
            _camCon.CameraInput(_inputData.StickRightBool(_deadZoneStick));
        }
    }

    private void FixedUpdate()
    {

        /****[�v���C���[�֘A]****/
        //�v���C���[��Null�Ȃ�return
        if (_isPl) { return; }

        //�v���C���[�̈ړ��Ǘ�
        _plCon.PlayerMove(_inputData.StickLeftValue(_deadZoneStick));
    }

    private void LateUpdate() 
    {

        /****[�J�����֘A]****/
        //�J������Null�Ȃ�return
        if (_isCame) { return; }

        //�J�����̈ړ��Ǘ�
        _camCon.CameraMove(_inputData.StickRightValue(_deadZoneStick)); 
    }
}
