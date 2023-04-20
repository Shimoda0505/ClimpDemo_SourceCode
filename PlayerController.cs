using UnityEngine;
using System;



/// <summary>
/// �v���C���[�������\�b�h
/// </summary>
public class PlayerController : MonoBehaviour
{

    [Header("�擾�R���|�[�l���g")]
    //�X�N���v�g
    private S_PlayerAnimator _plAnim;//�A�j���[�V�����Ǘ����\�b�h
    private PlayerIK _plIK;//IK

    //�R���|�[�l���g
    private GameObject _pl;//�v���C���[�I�u�W�F�N�g
    private Rigidbody _rb;//�v���C���[��Rigidbody
    private Animator _anim;//�v���C���[��Animator
    private CapsuleCollider _col;//�v���C���[��collider


    [Header("���Ԋ֘A")]
    private float _count;//���Ԍv��
    private const float _fallTime = 0.5f;//������̃C���^�[�o��
    private const float _highWallTime = 0.5f;//�����ǊJ�n�C���^�[�o��


    [Header("���x�֘A")]
    [SerializeField, Tooltip("�v���C���[�̕������x"),Range(0.1f,10f)] private float _moveWalkSpd;
    [SerializeField, Tooltip("�v���C���[�̑��葬�x"), Range(0.1f, 10f)] private float _moveRunSpd;
    [SerializeField, Tooltip("�K�i����艺�肷�鑬�x"), Range(1f, 10f)] private float _stairsMoveSpd;
    [SerializeField, Tooltip("�Ⴂ��(��)�ړ����x"), Range(1f, 10f)] private float _lowWallUpMoveSpd;
    [SerializeField, Tooltip("�Ⴂ��(����)�ړ����x"), Range(1f, 10f)] private float _lowWallForwardMoveSpd;
    [SerializeField, Tooltip("�����ǈړ����x"), Range(1f, 10f)] private float _highWallMoveSpd;
    [SerializeField, Tooltip("�ǂ̕�����������]���x"), Range(1f, 10f)] private float _wallRoteSpd;


    [Header("Collider�֘A")]
    private Vector3 _colDefCenter = default;//��ʒu
    private float _colDefHeight = default;//��T�C�Y
    private const float _colMoveSpd = 2f;//�ʒu�𒲐����鎞�̃X�s�[�h


    [Header("�p���N�[���֘A")]
    //�i���̈ړ����x
    //�i���̍��W
    private static Vector3 _stepAd = new Vector3(0.001f, 0.65f, 0.2f);//�v���C���[�̈ړ�����W�̉��Z
    private Vector3 _wallPos = new Vector3(0, 0, 0);//�ړ���̕Ǎ��W
    private Vector3 _wallMovePos = new Vector3(0, 0, 0);//�ړ���̕Ǎ��W
    private float _wallNearRange = 0.5f;//�ǂƂ̋��������ɂ�����W(��O)
    private float _wallBackRange = -0.35f;//�ǂƂ̋��������ɂ�����W(��)
    private const float _wallEndOffset = 0.4f;//�Ⴂ�ǂ�Y�����W
    //���̑�
    private float _timePer = default;//�ړ����W�ɉ����Ĉ��̑��x�œo�邽�߂̎��Ԓ����l
    private const float _disWall = 0.1f;//�ړ��ʒu�Ƃ̋��e�͈�
    private const float _lowWallOffset = 1.2f;//�Ⴂ�ǂ�Y�����W


    [Header("���̑�")]
    //�����_�̐؂�グ
    private static int _decimalPoint = 4;

    /// <summary>
    /// �v���C���[�̂���t�B�[���henum
    /// </summary>
    private enum PlayerField
    {
        [InspectorName("�n��")] GROUND,
        [InspectorName("��")] FLY,
        [InspectorName("��")] WALL,
    }
    private PlayerField _plField = PlayerField.GROUND;

    /// <summary>
    /// �n��enum
    /// </summary>
    enum GroundMotion
    {
        [InspectorName("����")] NOT_MOTION,
        [InspectorName("�ҋ@")] IDLE,
        [InspectorName("����")] WALK,
        [InspectorName("����")] RUN,
    }
    private GroundMotion _groundMotion = GroundMotion.NOT_MOTION;

    /// <summary>
    /// �Ⴂ��enum
    /// </summary>
    private enum LowWallMotion
    {
        [InspectorName("�J�n")] ENTER,
        [InspectorName("���ʈړ�")] FRONT_MOVE,
        [InspectorName("�㏸�ړ�")] UP_MOVE,
        [InspectorName("�O���ړ�")] FORWARD_MOVE,
    }
    private LowWallMotion _lowWallMotion = LowWallMotion.ENTER;

    /// <summary>
    /// ������enum 
    /// </summary>
    private enum HighWallMotion
    {
        [InspectorName("�J�n")] ENTER,
        [InspectorName("�ҋ@")] IDLE,
        [InspectorName("�ړ�")] MOVE,
        [InspectorName("��шړ�")] JUMP,
    }
    private HighWallMotion _highWallMotion = HighWallMotion.ENTER;



    //������-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {

        //�擾�R���|�[�l���g
        _pl = this.gameObject;
        _rb = _pl.GetComponent<Rigidbody>();
        _anim = _pl.GetComponent<Animator>();
        _col = _pl.GetComponent<CapsuleCollider>();

        //�R���C�_�̒��S�ʒu
        _colDefCenter = _col.center;
        _colDefHeight = _col.height;
        _plIK = _pl.GetComponent<PlayerIK>();
        _plIK.IsFootIK(false);//IK��~
    }



    //���\�b�h��--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*#####[�O���Q��]#####*/
    /// <summary>
    /// �v���C���[�̓��͊Ǘ�
    /// </summary>
    /// <param name="Left_S">LeftStick�̓���</param>
    /// <param name="South_B">SouthBottun�̓���</param>
    public void PlayerInput(bool Left_S_bool, Vector2 Left_S_Vector, bool South_B, bool South_B_D, bool East_B)
    {

        //�v���C���[�̂���t�B�[���h
        switch (_plField)
        {

            #region �n��case
            case PlayerField.GROUND:

                //�n�ʂɐڂ��Ă��Ȃ��Ȃ�
                if (!_plIK.GroundRay())
                {

                    //��Ԃ̍X�V
                    //_plAnim.MoveBoolAnim("FallRoop", _anim);//�����A�j���[�V����
                    _plIK.IsFootIK(false);//IK��~

                    //Enum�̑J��
                    _plField = PlayerField.FLY;//��(�t�B�[���henum)
                    _groundMotion = GroundMotion.NOT_MOTION;//������(�n��enum)
                    break;
                }


                //LeftStick���͒�
                if (!Left_S_bool)
                {

                    //Idle���͏������Ȃ�
                    if (_groundMotion == GroundMotion.IDLE) { return; }

                    //��Ԃ̍X�V
                    _plAnim.MoveBoolAnim("Idle", _anim);//Idle�̃A�j���[�V����
                    _plIK.IsFootIK(true);//IK�J�n

                    //Enum�̑J��
                    _groundMotion = GroundMotion.IDLE;//�ҋ@(�n��enum)
                }

                //LeftStick������
                else if (Left_S_bool)
                {

                    //�n��ړ�,�ǂ�����ΑJ��
                    if (MoveGround(South_B_D))
                    {

                        //���[�V�����̍X�V
                        //�Ⴂ��
                        if (_plIK._wallField == PlayerIK.WallField.LOW_WALL) {  }
                        //������
                        else if (_plIK._wallField == PlayerIK.WallField.HIGH_WALL) { _plAnim.MoveBoolAnim("HighWall", _anim); _plAnim.HighWallFloatAnim(new Vector2(0, 1), _anim); }

                        //Enum�̑J��
                        _plField = PlayerField.WALL;//��(�t�B�[���henum)
                        _groundMotion = GroundMotion.NOT_MOTION;//������(�n��enum)
                        break;
                    }

                    //SouthButton���͒�
                    if (South_B)
                    {

                        //Run���͏������Ȃ�
                        if (_groundMotion == GroundMotion.RUN) { return; }

                        //���[�V�����̍X�V
                        _plAnim.MoveBoolAnim("Run", _anim);//Run�A�j���[�V����
                        _plIK.IsFootIK(false);//IK��~

                        //Enum�̑J��
                        _groundMotion = GroundMotion.RUN;//����(�n��enum)
                    }
                    else if (!South_B)
                    {

                        //Walk���͏������Ȃ�
                        if (_groundMotion == GroundMotion.WALK) { return; }

                        //���[�V�����̍X�V
                        _plAnim.MoveBoolAnim("Walk", _anim);//Walk�̃A�j���[�V����
                        _plIK.IsFootIK(false);//IK��~

                        //Enum�̑J��
                        _groundMotion = GroundMotion.WALK;//����(�n��enum)
                    }
                }
                break;
            #endregion

            #region ��case
            case PlayerField.FLY:

                //�n�ʂɐڂ��Ă��Ȃ��Ȃ珈�����Ȃ�
                if (!_plIK.GroundRay()) { return; }


                //���n�A�j���[�V����
                _plAnim.MoveBoolAnim("FallEnd", _anim);

                //���n�̎p����Collider�̌`��ύX
                _col.height = _plAnim.ChangeCollider(_anim).Item1;
                _col.center = new Vector3(0, _plAnim.ChangeCollider(_anim).Item2, 0);

                //���Ԍv�����͏������Ȃ�
                _count += Time.deltaTime;
                if (_count <= _fallTime) { return; }

                //collider��heigh���f�t�H���g�ʒu�ɖ߂�����
                if (_col.height >= _colDefHeight)
                {

                    _count = 0;

                    //Enum�̑J��
                    _plField = PlayerField.GROUND;//�n��(�t�B�[���henum)
                    _groundMotion = GroundMotion.NOT_MOTION;//������(�n��enum)
                }
                break;
            #endregion

            #region ��case
            case PlayerField.WALL:

                //������(�t�B�[���henum)�ł͂Ȃ� || �ҋ@(������enum)�ł͂Ȃ��Ȃ珈�����Ȃ�
                if (_plIK._wallField != PlayerIK.WallField.HIGH_WALL || _highWallMotion != HighWallMotion.IDLE) { return; }


                //�ړ���̒T��
                if (Left_S_bool)
                {

                    //�ړ���̕ǂ�����Ȃ�
                    //�q�b�g�������ǂ���,�q�b�g���W,�ړ�����
                    (bool isHit, Vector3 hitPos, Vector2 moveAngle) _climbItem = _plIK.ClimbRay(Left_S_Vector);

                    //�ړ���̈ʒu��⊮
                    if (_climbItem.isHit)
                    {

                        //�ړ���̕Ǎ��W���v�Z
                        WallCal(_climbItem.hitPos, _pl.transform.position, _pl.transform.position);

                        //�ړ����͂��Ă����
                        if (South_B_D)
                        {

                            //���[�V�����̍X�V
                            _plAnim.MoveBoolAnim("HighWall", _anim);
                            _plAnim.HighWallFloatAnim(_climbItem.moveAngle, _anim);

                            //Enum�̑J��
                            _highWallMotion = HighWallMotion.JUMP;//�ړ�(������enum)
                            break;
                        }
                    }
                }
                //����
                if (East_B)
                {

                    //�����J�n
                    _rb.isKinematic = false;

                    //���IK��~
                    _plIK.IsHandIK(false);

                    //Enum�̑J��
                    _highWallMotion = HighWallMotion.ENTER;//�J�n(������enum)
                    _plField = PlayerField.GROUND;//�n��(�t�B�[���henum)
                }
                break;
            #endregion

            default:
                print("PlayerField��Case�Ȃ�");
                break;
        }
    }

    /// <summary>
    /// �v���C���[�̈ړ��Ǘ�
    /// </summary>
    public void PlayerMove(Vector2 Left_S)
    {

        switch (_plField)
        {

            case PlayerField.GROUND://�n��

                //�v���C���[�̈ړ��Ǘ�(�n��)
                switch (_groundMotion)
                {

                    case GroundMotion.NOT_MOTION://�����̏�����
                        break;


                    case GroundMotion.IDLE://�ҋ@

                        //�ړ���~
                        _rb.velocity = new Vector3(0, 0, 0);
                        break;


                    case GroundMotion.WALK://����

                        //�ړ��v�Z
                        GroundCal(_rb, _pl.transform, Left_S.normalized, _moveWalkSpd);
                        break;


                    case GroundMotion.RUN:

                        //�ړ��v�Z
                        GroundCal(_rb, _pl.transform, Left_S.normalized, _moveRunSpd);
                        break;


                    default:
                        print("playerMotion��Case�Ȃ�");
                        break;

                }
                break;


            case PlayerField.FLY://��
                break;


            case PlayerField.WALL://��

                //�Ⴂ��
                if (_plIK._wallField == PlayerIK.WallField.LOW_WALL) { LowWallMove(); }
                //������
                else if (_plIK._wallField == PlayerIK.WallField.HIGH_WALL) { HighWallMove(); }
                break;


            default:
                print("playerField��Case�Ȃ�");
                break;
        }
    }


    /*#####[�����Q��]#####*/
    /****[PlayerInput�֘A]****/
    /// <summary>
    /// �O���̕ǂ�T�����Ȃ���n����ړ�
    /// </summary>
    /// <param name="input">���͒l</param>
    /// <param name="moveSpeed">�ړ����x</param>
    /// <returns>�ǂ����邩�ǂ���</returns>
    private bool MoveGround(bool South_B_D)
    {

        //�O���ɒi�������邩
        (Vector3 newPos, Vector3 nowPos) pos = _plIK.StepRay();

        //�ǂȂ�WALL��Field���[�h�Ɉڍs
        //�ǂȂ�
        if (_plIK._wallField == PlayerIK.WallField.NOT_WALL) { return false; }
        //���n 
        if (_plIK._wallField == PlayerIK.WallField.FLAT) { ColliderCal(pos.newPos, pos.nowPos); return false; }
        //��
        else if (_plIK._wallField == PlayerIK.WallField.SLOPE) { ColliderCal(pos.newPos, pos.nowPos); return false; }
        //�K�i
        else if (_plIK._wallField == PlayerIK.WallField.STARIS) { TransformCal(pos.newPos, pos.nowPos); return false; }
        //��
        if (South_B_D)
        {

            //�Ⴂ��
            if (_plIK._wallField == PlayerIK.WallField.LOW_WALL) { WallCal(pos.newPos, pos.nowPos, _pl.transform.position); return true; }
            //������
            else if (_plIK._wallField == PlayerIK.WallField.HIGH_WALL) { _rb.isKinematic = true; _plIK.IsHandIK(true); return true; }
        }

        return false;
    }

    /// <summary>
    /// �Ⴂ�ǂ̈ړ�
    /// </summary>
    private void LowWallMove()
    {

        switch (_lowWallMotion)
        {

            /*�ǂ̕����ɑ̂�������*/
            case LowWallMotion.ENTER://�J�n

                //�ǂƃv���C���[�̕��s�����x�N�g�����v�Z
                Vector3 moverotate = WallRotate(_pl.transform);

                //���X�ɕǂƕ��s�ɂȂ�悤�ɉ�]
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, moverotate, _wallRoteSpd);


                //�ǂ̕����ɍ��W�������܂ŏ������Ȃ�
                if(transform.eulerAngles == moverotate)
                {

                    //�ǂ̂ڂ�A�j���[�V����
                    _plAnim.MoveBoolAnim("LowWall", _anim);

                    //���ʂ̕ǂƂ̋��������ɂ���
                    _wallMovePos = WallDistance(_plIK.FrontRay().hitPos, _pl.transform.position, _wallNearRange);

                    //Enum�̑J��
                    _lowWallMotion = LowWallMotion.FRONT_MOVE;//���ʈړ�(�Ⴂ��enum)
                }

                break;


            /*�ǂ̕����ɑ̂��߂Â���*/
            case LowWallMotion.FRONT_MOVE://���ʈړ�

                //�ړ���̐��ʕǍ��W
                Vector3 moveFrontPos = new Vector3(_wallMovePos.x, _pl.transform.position.y, _wallMovePos.z);

                //�O�����ւ̈ړ�
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, moveFrontPos, Time.deltaTime * _lowWallForwardMoveSpd);

                //Enum�̑J��
                if (_pl.transform.position == moveFrontPos)
                {

                    //�ǂƕ��s�ɓo��ʒu�̕��ʍ��W(X,Z)
                    Vector3 movaPos = WallDistance(_plIK.FrontRay().hitPos, _pl.transform.position, _wallBackRange);

                    //�ړ���̕Ǎ��W
                    _wallPos = new Vector3(movaPos.x, _wallPos.y - _wallEndOffset, movaPos.z);

                    //Enum�̑J��
                    _lowWallMotion = LowWallMotion.UP_MOVE;//�㏸�ړ�(�Ⴂ��enum)
                }
                break;


            /*�ڕW���WY�Ɉړ�*/
            case LowWallMotion.UP_MOVE://�㏸�ړ�

                //�ړ���̕�Y���W
                Vector3 moveUpPos = new Vector3(_pl.transform.position.x, _wallPos.y - _lowWallOffset, _pl.transform.position.z);

                //�O�����ւ̈ړ�
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, moveUpPos, Time.deltaTime * _lowWallUpMoveSpd * _timePer);

                //Enum�̑J��
                if (_pl.transform.position == moveUpPos)
                {
                    //Enum�̑J��
                    _lowWallMotion = LowWallMotion.FORWARD_MOVE;//�O���ړ�(�Ⴂ��enum)
                }

                break;


            /*�ڕW���W�Ɉړ�*/
            case LowWallMotion.FORWARD_MOVE://�O���ړ�

                //�O�����ւ̈ړ�
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, _wallPos, Time.deltaTime * _lowWallForwardMoveSpd * _timePer);


                //�ړ��ʒu�ɋ߂Â�����
                float disPos = Mathf.Abs((float)Vector3.Distance(_pl.transform.position, _wallPos));
                if (disPos <= _disWall)
                {

                    //�����J�n
                    _rb.isKinematic = false;

                    //Enum�̑J��
                    _lowWallMotion = LowWallMotion.ENTER;//�J�n(�Ⴂ��enum)
                    _highWallMotion = HighWallMotion.ENTER;//�J�n(������enum)
                    _groundMotion = GroundMotion.NOT_MOTION;//������(�n��enum)
                    _plField = PlayerField.GROUND;//�n��(�t�B�[���henum)
                }
                break;


            default:
                print("LowWallMotion��case�Ȃ�");
                break;
        }
    }

    /// <summary>
    /// �����ǂ̈ړ�
    /// </summary>
    private void HighWallMove()
    {

        switch (_highWallMotion)
        {

            case HighWallMotion.ENTER://�J�n

                //���Ԍv����ɏ���
                _count += Time.deltaTime;
                if (_count >= _highWallTime)
                {
                    
                    _count = 0;

                    //Enum�̑J��
                    _highWallMotion = HighWallMotion.IDLE;//�ҋ@(������enum)
                }
                break;


            case HighWallMotion.IDLE://�ҋ@

                //�ǂƕ��s�ɓo��ʒu�̕��ʍ��W(X,Z)
                Vector3 movaPos = WallDistance(_plIK.FrontRay().hitPos, _pl.transform.position, _wallNearRange);

                //�ړ���̕Ǎ��W
                Vector3 wallPos = new Vector3(movaPos.x, _pl.transform.position.y, movaPos.z);

                //�ǂƃv���C���[�̕��s�����x�N�g�����v�Z
                Vector3 moverotate = WallRotate(_pl.transform);

                //���X�ɕǂƕ��s�ɂȂ�悤�ɉ�]
                transform.eulerAngles = Vector3.MoveTowards(transform.eulerAngles, moverotate, _wallRoteSpd);

                //�O�����ւ̈ړ�
                _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, wallPos, Time.deltaTime * _highWallMoveSpd * _timePer);
                break;


            case HighWallMotion.JUMP://�ړ�

                //�ʏ�̈ړ�
                if (_plIK._hWallM == PlayerIK.HighWallMotion.NOMAL)
                {

                    //�O�����ւ̈ړ�
                    _pl.transform.position = Vector3.MoveTowards(_pl.transform.position, _wallPos - new Vector3(0, 1, 0), Time.deltaTime * _highWallMoveSpd * _timePer);

                    if (_pl.transform.position == _wallPos - new Vector3(0, 1, 0)) { _highWallMotion = HighWallMotion.IDLE;/*�ҋ@(������enum)*/ }
                }
                //������̏I��
                else if (_plIK._hWallM == PlayerIK.HighWallMotion.UP_END)
                {

                    //���IK��~
                    _plIK.IsHandIK(false);

                    LowWallMove(); _plAnim.MoveBoolAnim("LowWall", _anim);
                }
                //�������̏I��
                else if (_plIK._hWallM == PlayerIK.HighWallMotion.DOWN_END)
                {

                    //�����J�n
                    _rb.isKinematic = false;

                    //���IK��~
                    _plIK.IsHandIK(false);

                    //Enum�̑J��
                    _highWallMotion = HighWallMotion.ENTER;//�J�n(������enum)
                    _plField = PlayerField.GROUND;//�n��(�t�B�[���henum)
                }
                break;


            default:
                print("HighWallMotion��case�Ȃ�");
                break;
        }
    }

    /// <summary>
    /// �R���C�_�[�̒��S�ʒu���ړ�(Collider,Transform,Default)
    /// </summary>
    /// <param name="movePos">�ړ���̈ʒu</param>
    /// <param name="nowPos">���݂̈ʒu</param>
    private void ColliderCal(Vector3 movePos, Vector3 nowPos)
    {

        //������Y���̍����̍����v�Z
        //�����̍���/2(�l�̌ܓ�)�̍����Βl�ɂ���
        float moveYPos = 0;

        //���n
        if (_plIK._wallField == PlayerIK.WallField.FLAT) { moveYPos = _colDefCenter.y; }

        //��
        else if (_plIK._wallField == PlayerIK.WallField.SLOPE)
        {

            //�R���C�_�[�̏����ʒu��distance�����Z
            float disFoot = Mathf.Abs((float)Math.Round(movePos.y, _decimalPoint) - (float)Math.Round(nowPos.y, _decimalPoint));
            moveYPos = _colDefCenter.y + disFoot - _stepAd.y;
        }

        //Collider�̈ʒu��ύX
        _col.center = Vector3.Lerp(_col.center, new Vector3(0f, moveYPos, 0f), Time.deltaTime * _colMoveSpd);
    }

    /// <summary>
    /// �v���C���[�̒��S�ʒu���ړ�(Collider,Transform,Default)
    /// </summary>
    /// <param name="movePos">�ړ���̈ʒu</param>
    /// <param name="nowPos">���݂̈ʒu</param>
    private void TransformCal(Vector3 movePos, Vector3 nowPos)
    {

        //������Y���̍����̍����v�Z
        //�����̍���/2(�l�̌ܓ�)�̍����Βl�ɂ���
        float disFoot = Mathf.Abs((float)Math.Round(movePos.y, _decimalPoint) - (float)Math.Round(nowPos.y, _decimalPoint));

        //Transform�̏����ʒu��distance�����Z
        _pl.transform.position = Vector3.Lerp(_pl.transform.position,
                                                  new Vector3(_pl.transform.position.x, _pl.transform.position.y + disFoot + _stepAd.y, _pl.transform.position.z),
                                                  Time.deltaTime * _stairsMoveSpd);
    }

    /// <summary>
    /// �ǂ̈ړ��ʒu�v�Z
    /// </summary>
    /// <param name="movePos">�ړ���̈ʒu</param>
    /// <param name="nowPos">���݂̈ʒu</param>
    private void WallCal(Vector3 movePos, Vector3 nowPos, Vector3 playerPos)
    {

        //������~
        _rb.isKinematic = true;

        //�Q�����̍��W��Y���������߂�
        float disFootY = (float)Math.Round(movePos.y, _decimalPoint) - (float)Math.Round(nowPos.y, _decimalPoint);

        //�ړ���̍��W
        float _plPosY = playerPos.y + disFootY;

        //�ړ���̒l��⊮
        _wallPos = new Vector3(movePos.x, _plPosY, movePos.z);


        //�ړ���̋����ɉ������ړ����x�̈�艻
        //������100%�ɂ��邽�߂�1
        if (disFootY != 0) { _timePer = 1 / Mathf.Abs(disFootY); }
        else
        {

            //�ڕW�n�_�̋����ɑ΂��ẮA�ړ����Ԃ����ɂ���
            float disFoot = Vector3.Distance(movePos, nowPos);
            _timePer = 1 / Mathf.Abs(disFoot);
        }
    }


    /****[PlayerMove�֘A]****/
    /// <summary>
    /// �v���C���[�̋���(�n��)
    /// </summary>
    /// <param name="rb">�v���C���[��Rigidbody</param>
    /// <param name="tr">�v���C���[��Transform</param>
    /// <param name="input">�R���g���[���[�̓��͒l</param>
    /// <param name="speed">�ړ����x</param>
    private void GroundCal(Rigidbody playerRb, Transform playerTrans, Vector2 input, float speed)
    {

        //�J�����̐��ʕ���������
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        //���͒l�����J�����̌�������ړ��������v�Z
        Vector3 moveForward = cameraForward * input.y + Camera.main.transform.right * input.x;

        //�ړ��̔��f
        playerRb.velocity = moveForward * speed + new Vector3(0, playerRb.velocity.y, 0);
        //rb.AddForce(input.x * speed, 0, input.y * speed, ForceMode.Impulse);

        //�i�s����������
        playerTrans.rotation = Quaternion.LookRotation(moveForward);
    }

    /// <summary>
    /// �ǂƕ��s�Ɍ�����ύX����
    /// </summary>
    /// <returns>�ǂƂ̕��s�����x�N�g��</returns>
    private Vector3 WallRotate(Transform playerTrans)
    {

        //Ray��Hit�ʂƃv���C���[��180�x���猸�Z�����p�x��
        float angleY = 180 - Quaternion.FromToRotation(playerTrans.forward, _plIK.FrontRay().hitRot).eulerAngles.y;

        //���݂̊p�x�Ƃ̍������߂�
        Vector3 targetAngle = playerTrans.eulerAngles - new Vector3(0, angleY, 0);

        return targetAngle;
    }

    /// <summary>
    /// �ڕW�Ƃ̋��������ɂ���
    /// </summary>
    /// <param name="targetPos">�ڕW���W</param>
    /// <param name="nowPos">���݂̍��W</param>
    /// <param name="range">�����̌Œ�l</param>
    /// <returns>�ړ���̍��W</returns>
    private Vector3 WallDistance(Vector3 targetPos, Vector3 nowPos, float range)
    {

        //����
        float wallRange = Vector3.Distance(targetPos, nowPos);//�ǂƃv���C���[�̒������� ... �@

        //�䗦 = �@ / �ǂƃv���C���[�̒�������(�ړ��ڕW�̌Œ�ʒu) ,,, �A
        float percentage = wallRange / range;

        //�~�̒��S�_(wallPos)���猻�݂̃v���C���[���W�̋��� = �Ǎ��W / �v���C���[���W ... �B
        float xDisPlayer = targetPos.x - nowPos.x;//x��
        float zDisPlayer = targetPos.z - nowPos.z;//z��

        //�~�̒��S�_����Œ�Range�̋��� = �B / �A ... �C
        float xDis = xDisPlayer / percentage;//cos��
        float zDis = zDisPlayer / percentage;//sin��

        //�ڕW���W = �Ǎ��W + �C
        Vector3 moveFrontPos = new Vector3(targetPos.x - xDis, 0, targetPos.z - zDis);

        return moveFrontPos;
    }
}
