using UnityEngine;
using System;



/// <summary>
/// �v���C���[��IK,Collider�Ǘ����\�b�h
/// </summary>
public class PlayerIK : MonoBehaviour
{

    [Header("�擾�R���|�[�l���g")]
    //�R���|�[�l���g
    private GameObject _pl;//�v���C���[�I�u�W�F�N�g
    private Animator _anim;//�v���C���[��Animator
    private CapsuleCollider _capCol;//�v���C���[��collider


    [Header("������IK�֘A")]
    private static Vector3 _offset = new Vector3(0, 0.15f, 0);//����u���ʒu�̃I�t�ݒu�l
    //��IK
    private const float _rayFootOffset = 0.15f;//Ray���΂��ʒu�̒����l
    private const float _rayFootRange = 1.0f;//�n��Ƃ̋����𑪂�ray�̒���
    //��IK
    private const float _rayHandRange = 1.0f;//�n��Ƃ̋����𑪂�ray�̒���
    private const float _handIKOffset = 0.06f;//�ǂɎ肪�������Ă���悤�ɃA�j���[�V����IK�̍��W���X�V����Ƃ��̋���
    private bool _isFootIK = false;//��IK���g�p���邩�ǂ���
    private bool _isHandIK = false;//��IK���g�p���邩�ǂ���


    [Header("Collider�֘A")]
    private Vector3 _colDefCenter = default;//�R���C�_�̒��S�ʒu
    private const float _colMoveSpd = 20;//Collider.center
    private const float _colPlus = 0.2f;//�v���C���[�̈ړ���Ǎ��W�̉��Z


    [Header("�i�����ʊ֘A")]
    //���ʂ̋��e����
    [SerializeField, Tooltip("�o���K�i�̍���"), Range(0.01f, 0.1f)] private float _stepOrStairs;
    [SerializeField, Tooltip("�o���Ⴂ�ǂ̍���"), Range(1, 5)] private float _lowWall;
    [SerializeField, Tooltip("�o��鍂���ǂ̍���"), Range(10, 50)] private float _highWall;
    private float _stepOrStairsOffset = default;
    //Ray�̒���
    private const float _rayGroundRange = 1.5f;//�n��(�������牺�ɔ�΂�)
    private const float _rayStepRange = 1.0f;//�i��(�̂���O�ɔ�΂�)
    private const float __rayWallRange = 1.0f;//��(�̂���O�ɔ�΂�)
    //Ray��O�ɔ�΂�Y���̒����l
    private const float _rayStepPos = 0.1f;//�i��
    private const float _rayStairsPos = 0.2f;//�K�i
    private const float __rayWallPos = 0.8f;//��
    //Ray�����ɔ�΂��ʒu�̒����l
    private const float _rayGroundPos = 0.5f;//����(Y��)
    private const float _rayUpPos = 20f;//������̈ʒu
    private const float _rayDown = 20f;//�ォ�牺�ɗ��Ƃ��ʒu(Y��)


    [Header("������Ray�̒T���֘A")]
    //Ray�̒���
    private const float _rayHighRange = 1.0f;
    //Ray�̔�΂����W�̒����l
    private static Vector3 _rayHighPos = new Vector3(2.0f, 2.0f, 1.0f);//��΂����W�ʒu
    private const float _rayYPos = 1.0f;//���S��Y���W
    //���͂̋��e�l
    private const float _inputClamp = 0.4f;


    [Header("���̑�")]
    //�����_�̐؂�グ
    private const int _decimalPoint = 4;

    /// <summary>
    /// �i��enum
    /// </summary>
    public enum WallField
    {
        [InspectorName("���n")] FLAT,
        [InspectorName("��")] SLOPE,
        [InspectorName("�K�i")] STARIS,
        [InspectorName("�Ⴂ��")] LOW_WALL,
        [InspectorName("������")] HIGH_WALL,
        [InspectorName("�o��Ȃ���")] NOT_WALL,
    }
    [HideInInspector] public WallField _wallField = WallField.SLOPE;

    /// <summary>
    /// �����ǂ̈ړ���T��enum
    /// </summary>
    public enum HighWallMotion
    {
        [InspectorName("��(�I��)")] UP_END,
        [InspectorName("�ʏ�")] NOMAL,
        [InspectorName("��(�I��)")] DOWN_END,
    }
    [HideInInspector] public HighWallMotion _hWallM = HighWallMotion.NOMAL;



    //������-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {
        //�擾�R���|�[�l���g
        _pl = this.gameObject;
        _anim = _pl.GetComponent<Animator>();
        _capCol = _pl.GetComponent<CapsuleCollider>();

        //�R���C�_�̒��S�ʒu
        _colDefCenter = _capCol.center;

        //�i���Ɖ�k����ray�̍����o����k�̍��������Z
        _stepOrStairsOffset =  _stepOrStairs + _rayStairsPos - _rayStepPos;
    }



    //���\�b�h��--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*#####[�O���Q��]#####*/
    /// <summary>
    /// �ݒu�����Ray
    /// </summary>
    /// <param name="name">Ground,Fly</param>
    /// <returns></returns>
    public bool GroundRay()
    {

        //IK�̑����W
        Vector3 rightIK = _anim.GetIKPosition(AvatarIKGoal.RightFoot);
        Vector3 leftIK = _anim.GetIKPosition(AvatarIKGoal.LeftFoot);

        //ray�̍��E�ʒu
        Vector3 rightPos = transform.up * _rayGroundPos + new Vector3(rightIK.x, transform.position.y, rightIK.z);
        Vector3 leftPos = transform.up * _rayGroundPos + new Vector3(leftIK.x, transform.position.y, leftIK.z);

        //Ray���΂�����
        Ray rayR = new Ray(rightPos, Vector3.down);
        Ray rayL = new Ray(leftPos, Vector3.down);

        //Ray�̎��o��
        Debug.DrawRay(rightPos, Vector3.down * _rayGroundRange, Color.red);
        Debug.DrawRay(leftPos, Vector3.down * _rayGroundRange, Color.red);

        //Ray�̔���t���O
        RaycastHit hit;

        //Ray�̔��肪True
        if (Physics.Raycast(rayR, out hit, _rayGroundRange)) { return true; }
        else if (Physics.Raycast(rayL, out hit, _rayGroundRange)) { return true; }
        return false;
    }

    /// <summary>
    /// �O����Ray���΂�
    /// </summary>
    /// <returns>���ʂ̃I�u�W�F�N�g��T��</returns>
    public (bool isHit, Vector3 hitRot, Vector3 hitPos) FrontRay()
    {

        //���[�J���ϐ�
        Vector3 rayPos = transform.position + transform.up * _rayStairsPos;
        Ray ray = new Ray(rayPos, transform.forward);//Ray���΂�����
        RaycastHit hit;//Ray�̔���I�u�W�F�N�g        
        Vector3 hitRot = default;
        Vector3 hitPos = default;

        //Ray��Hit�⊮
        bool isHit = Physics.Raycast(ray, out hit, _rayStepRange);//�t���O
        if (isHit)
        {

            hitRot = hit.normal;
            hitPos = hit.point;
        }

        //Ray�̎��o��
        Debug.DrawRay(rayPos, transform.forward * _rayStepRange, Color.green);

        //Ray��Hit����ƍ��W��ԋp
        return (isHit, hitRot, hitPos);
    }

    /// <summary>
    /// �O���ɒi�������邩��Ray�q�b�g����
    /// Updte�ŏ��������ăv���C���[��enum�̕ύX������
    /// </summary>
    /// <returns>��,�K�i,�Ⴂ��,������</returns>
    public (Vector3 newPos, Vector3 nowPos) StepRay()
    {

        //�O���ɒi�������邩�ǂ���,�i��Ray��HitPos,�i���̊p�x
        (bool isHit, Vector3 hitPos) stepItem = ThrowRay(_rayStepPos, _rayStepRange);


        //�i��������
        if (stepItem.isHit)
        {

            //�o��鍂�����ǂ���
            //�O���ɕǂ����邩,��Ray��HitPos,�i���̊p�x
            (bool isHit, Vector3 hitPos) wallItem = ThrowRay(__rayWallPos, _rayStepRange);

            //�O���̒i���̍���
            (bool isHit, Vector3 hitPos) heightItem = UpRay(_rayDown, __rayWallRange, _rayUpPos);


            //�ǂł͂Ȃ�
            if (!wallItem.Item1)
            {

                //�O���ɊK�i�����邩�ǂ���,�K�iRay��HitPos,�K�i�̊p�x
                (bool isHit, Vector3 hitPos) stairsItem = ThrowRay(_rayStairsPos, _rayStepRange);

                //�i��Ray�ƊK�iRay��HitPos�̋������v�Z
                if (stairsItem.isHit)
                {

                    //�i���̉��s���v�Z(X,Z��)
                    float disRay = Vector3.Distance(stepItem.hitPos, stairsItem.hitPos);

                    //��
                    if (disRay >= _stepOrStairsOffset) { _wallField = WallField.SLOPE; }
                    //�K�i
                    else { _wallField = WallField.STARIS; }

                    return (heightItem.hitPos, stepItem.hitPos);
                }
            }

            //��
            else if (wallItem.Item1)
            {

                //�o����
                if (heightItem.isHit)
                {

                    //�O���̕ǂ̍���
                    float heightWall = Vector3.Distance(stepItem.Item2, heightItem.hitPos);

                    //�Ⴂ��
                    if (heightWall <= _lowWall) { _wallField = WallField.LOW_WALL; return (heightItem.hitPos, stepItem.hitPos); }
                    //������
                    else if (_lowWall < heightWall && heightWall <= _highWall) { _wallField = WallField.HIGH_WALL; return (heightItem.hitPos, stepItem.hitPos); }
                }

                //�o��Ȃ���
                else { _wallField = WallField.NOT_WALL; return (new Vector3(0, 0, 0), stepItem.hitPos); }
            }
        }

        //�i���Ȃ�
        //���n
        _wallField = WallField.FLAT;

        //Collider���f�t�H���g�ʒu�ɖ߂�
        return (new Vector3(0, 0, 0), stepItem.Item2);
    }

    /// <summary>
    /// �����ǂ�ray���΂��Ĉړ����T��
    /// </summary>
    /// <param name="input">���͒l</param>
    /// <returns>�ړ��悪���邩,�ړ��ʒu,���݂̈ʒu</returns>
    public (bool isHit, Vector3 hitPos, Vector2 moveInput) ClimbRay(Vector2 input)
    {

        //���[�J���ϐ��̏�����
        Vector3 nowPos = transform.position;//���݈ʒu
        Vector3 rayInput = new Vector3(0, 0, 0);//���͕���

        //���͒l�ɉ�����ray�̕�����ϊ�
        //X��
        if (-_inputClamp >= input.x || input.x >= _inputClamp)
        {

            if (input.x < 0) { rayInput.x = -_rayHighPos.x; }
            else { rayInput.x = _rayHighPos.x; }
        }
        //Y��
        if (-_inputClamp >= input.y || input.y >= _inputClamp)
        {

            if (input.y < 0) { rayInput.y = -_rayHighPos.y; }
            else { rayInput.y = _rayHighPos.y; }
        }

        //���͐��ʂ�ray���΂�
        (bool isHit, Vector3 hitPos) aroundItem = AroundRay(rayInput, _rayHighRange);

        //�ړ��悪���邩,�ړ��ʒu,���݂̈ʒu��ԋp
        return (aroundItem.isHit, aroundItem.hitPos, new Vector2(rayInput.x / _rayHighPos.x, rayInput.y / _rayHighPos.y));
    }

    /// <summary>
    /// IK���g�p���邩�ǂ�����ύX
    /// </summary>
    /// <param name="isFlag">true,false</param>
    public void IsFootIK(bool isFlag)
    {

        if (isFlag) { _isFootIK = true; }
        else if (!isFlag) { _isFootIK = false; }
    }

    /// <summary>
    /// IK���g�p���邩�ǂ�����ύX
    /// </summary>
    /// <param name="isFlag">true,false</param>
    public void IsHandIK(bool isFlag)
    {

        if (isFlag) { _isHandIK = true; }
        else if (!isFlag) { _isHandIK = false; }
    }


    /*#####[��������]#####*/
    /****[Ray�Ǘ�]****/
    /// <summary>
    /// �O����Ray���΂�
    /// </summary>
    /// <param name="rayYPos">Ray��Y���W</param>
    /// <param name="rayRange">Ray�̒���</param>
    /// <returns>Ray��Hit�������ǂ���</returns>
    private (bool isHit, Vector3 hitPos) ThrowRay(float rayYVector, float rayRange)
    {

        //���[�J���ϐ�
        Vector3 rayPos = transform.position + transform.up * rayYVector;
        Ray ray = new Ray(rayPos, transform.forward);//Ray���΂�����
        RaycastHit hit;//Ray�̔���I�u�W�F�N�g        

        //Ray��Hit�⊮
        bool isHit = Physics.Raycast(ray, out hit, rayRange);//�t���O
        Vector3 hitPos = hit.point;//Hit�ʒu

        //Ray�̎��o��
        //Ray�̒�����Hit�ʒu�ɕύX
        if (isHit) { rayRange = Vector3.Distance(rayPos, hitPos); }
        Debug.DrawRay(rayPos, transform.forward * rayRange, Color.green);

        //Ray��Hit����ƍ��W��ԋp
        return (isHit, hitPos);
    }

    /// <summary>
    /// �ォ��Ray���΂���Hit�ʒu�̍������v�Z
    /// </summary>
    /// <param name="rayYPos">Ray��Y���W</param>
    /// <param name="rayXPos">Ray��X���W</param>
    /// <param name="rayRange">Ray�̒���</param>
    /// <returns></returns>
    private (bool isHit, Vector3 hitPos) UpRay(float rayYVector, float rayXVector, float rayRange)
    {

        //Ray�̏����l
        Vector3 rayPos = transform.position + transform.up * rayYVector + transform.forward * rayXVector;
        Ray ray = new Ray(rayPos, Vector3.down);
        RaycastHit hit;

        //Ray�̓��������ʒu�Ɗp�x��⊮
        //Ray�̒�����Hit�ʒu�ɕύX
        bool isHit = Physics.Raycast(ray, out hit, rayRange);
        Vector3 hitPos = hit.point;//�ʒu

        //Ray�̎��o��
        //Ray�̒�����Hit�ʒu�ɕύX
        if (isHit) { rayRange = Vector3.Distance(rayPos, hitPos); }
        Debug.DrawRay(rayPos, Vector3.down * rayRange, Color.green);

        //Ray��Hit����ƍ��W��ԋp
        return (isHit, hitPos);
    }

    /// <summary>
    /// ���͐��ʂ�ray���΂�
    /// </summary>
    /// <param name="rayVector">���͂ɉ�����ray�̈ʒu</param>
    /// <param name="rayRange">ray�̒���</param>
    /// <returns></returns>
    private (bool isHit, Vector3 hitPos) AroundRay(Vector3 rayVector, float rayRange)
    {

        //�ǂ̈ړ����W��T��
        //Ray�̏����l
        Vector3 rayPosF = transform.position + transform.up * rayVector.y + transform.right * rayVector.x;
        Ray rayF = new Ray(rayPosF, transform.forward);
        RaycastHit hitF;

        //Ray�̓��������ʒu�Ɗp�x��⊮
        bool isHitF = Physics.Raycast(rayF, out hitF, rayRange);//Hit����
        Debug.DrawRay(rayPosF, transform.forward * rayRange, Color.green);//Ray�̎��o��
        Vector3 hitPosF = hitF.point;//�ʒu


        //������Ɉړ��悪�Ȃ����ɍŏI�ʒu��T��
        //������̓���
        if (!isHitF && rayVector.y >= 0)
        {

            //������̕ǏI���ʒu��⊮
            //Ray�̏����l
            Vector3 rayPosD = rayPosF + transform.forward * rayRange;
            Ray rayD = new Ray(rayPosD, -transform.up);
            RaycastHit hitD;

            //Ray�̓��������ʒu�Ɗp�x��⊮
            bool isHitD = Physics.Raycast(rayD, out hitD, rayRange);//Hit����
            Debug.DrawRay(rayPosD, -transform.up * rayRange, Color.green);//Ray�̎��o��
            Vector3 hitPosD = hitD.point;//�ʒu

            if (isHitD)
            {

                //������̏I��
                _hWallM = HighWallMotion.UP_END;

                //Ray��Hit����ƍ��W��ԋp
                return (isHitD, hitPosD);
            }
        }


        //�ړ���ɏ�Q�����Ȃ�����T��
        //Ray�̏����l
        Vector3 rayPosA = transform.position + transform.up * _rayYPos;

        //Ray�̓��������ʒu�Ɗp�x��⊮
        RaycastHit hitA;
        bool isHitA = Physics.Linecast(rayPosA, rayPosF, out hitA);
        Debug.DrawLine(rayPosA, rayPosF, Color.green);//Ray�̎��o��
        Vector3 hitPosA = hitA.point;//�ʒu

        //�������Ɉړ��悪�Ȃ����ɍŏI�ʒu��T��
        if (isHitA)
        {

            //Ray�̒�����Hit�ʒu�ɕύX,hit������isHit��false
            isHitF = false;

            //������̓���
            if (rayVector.y < 0)
            {

                //�������̏I��
                _hWallM = HighWallMotion.DOWN_END;

                //Ray��Hit����ƍ��W��ԋp
                return (isHitA, hitPosA);
            }
        }


        //ray��hit���W���v���C���[��xz���ɕύX
        hitPosF = new Vector3(rayPosF.x, hitPosF.y, rayPosF.z);

        //�ʏ�̈ړ�
        _hWallM = HighWallMotion.NOMAL;

        //Ray��Hit����ƍ��W��ԋp
        return (isHitF, hitPosF);
    }


    /****[IK�Ǘ�]****/
    /// <summary>
    /// IK���@�\�������̂݌Ăяo��
    /// </summary>
    void OnAnimatorIK()
    {

        //��IK��True���̂ݏ���
        if (_isFootIK)
        {

            /*[���[�J���ϐ�]*/
            //�A�j���[�V�����p�����[�^���猻�݂�IK�̃E�G�C�g���擾
            float rightFootWeight = _anim.GetFloat("RightFootWeight");//�E���̃E�G�C�g
            float leftFootWeight = _anim.GetFloat("LeftFootWeight");//�����̃E�G�C�g

            //������IK�ʒu
            Vector3 rightFootIKPos = new Vector3(0, 0);//�E��IK�̈ʒu
            Vector3 leftFootIKPos = new Vector3(0, 0);//����IK�̈ʒu

            //����Ray���n�ʂɂ��Ă��邩�ǂ���
            bool isRightFootRay = false;//�E���̃t���O
            bool isLeftFootRay = false;//�����̃t���O


            /*[����]*/
            //�R���C�_�̒��S�ʒu���ړ�
            ColliderMove(rightFootIKPos, leftFootIKPos);

            //Ray�̃q�b�g����
            //�q�b�g�ʒu,�q�b�g�������ǂ���
            //AvatarIKGoal = �{�f�B�p�[�c�̃^�[�Q�b�g�ʒu�Ɗp�x
            (isRightFootRay, rightFootIKPos) = FootRay(AvatarIKGoal.RightFoot, rightFootWeight, rightFootIKPos, isRightFootRay);//�E��
            (isLeftFootRay, leftFootIKPos) = FootRay(AvatarIKGoal.LeftFoot, leftFootWeight, leftFootIKPos, isLeftFootRay);//����

            //�̂̏d�S���ړ�
            BodyMove(rightFootIKPos, leftFootIKPos, isRightFootRay, isLeftFootRay);
        }

        //��IK��True���̂ݏ���
        if(_isHandIK)
        {

                        /*[���[�J���ϐ�]*/
            //�A�j���[�V�����p�����[�^���猻�݂�IK�̃E�G�C�g���擾
            float rightHandWeight = _anim.GetFloat("RightHandWeight");//�E���̃E�G�C�g
            float leftHandWeight = _anim.GetFloat("LeftHandWeight");//�����̃E�G�C�g

            //������IK�ʒu
            Vector3 rightHandIKPos = new Vector3(0, 0);//�E��IK�̈ʒu
            Vector3 leftHandIKPos = new Vector3(0, 0);//����IK�̈ʒu


            /*[����]*/
            //Ray�̃q�b�g����
            //�q�b�g�ʒu,�q�b�g�������ǂ���
            //AvatarIKGoal = �{�f�B�p�[�c�̃^�[�Q�b�g�ʒu�Ɗp�x
            HandRay(AvatarIKGoal.RightHand, rightHandWeight, rightHandIKPos);//�E��
            HandRay(AvatarIKGoal.LeftHand, leftHandWeight, leftHandIKPos);//����
        }
    }

    //FootIK
    /// <summary>
    /// Ray�̃q�b�g����(IK)
    /// </summary>
    /// <param name="ikGoal">AvatarIK��name</param>
    /// <param name="footWeight">���̃E�G�C�g</param>
    /// <param name="footIKPos">��IK�̈ʒu</param>
    /// <param name="footBool">����Ray���n�ʂɃq�b�g���Ă��邩�ǂ���</param>
    /// <returns>Ray�̃q�b�g�ʒu,Ray���q�b�g�������ǂ���</returns>
    private (bool isHit, Vector3 hitPos) FootRay(AvatarIKGoal ikGoal, float footWeight, Vector3 footIKPos, bool footBool)
    {

        //Ray���΂�����
        Ray ray = new Ray(_anim.GetIKPosition(ikGoal) + Vector3.up * _rayFootOffset, Vector3.down);

        //Ray�̎��o��
        Debug.DrawRay(_anim.GetIKPosition(ikGoal) + Vector3.up * _rayFootOffset, Vector3.down * _rayFootRange, Color.red);

        //Ray�̔���t���O
        RaycastHit hit;

        //Ray�̔��肪True
        if (Physics.Raycast(ray, out hit, _rayFootRange))
        {

            //����Ray���n�ʂɂ��Ă���
            footBool = true;

            //Ray�̓��������ʒu��⊮
            footIKPos = hit.point;

            //��IK�̈ʒu
            _anim.SetIKPositionWeight(ikGoal, footWeight);//��IK�ʒu�E�G�C�g�̐ݒ�
            _anim.SetIKPosition(ikGoal, footIKPos + _offset);//��IK�ʒu�̐ݒ�

            //Ray�̓��������p�x��⊮
            Quaternion footIKRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            //��IK�̊p�x
            _anim.SetIKRotationWeight(ikGoal, footWeight);//��IK�p�x�E�G�C�g�̐ݒ�
            _anim.SetIKRotation(ikGoal, footIKRot);//��IK�p�x�̐ݒ�
        }

        //Ray�̔��肪False
        //����Ray���n�ʂɂ��Ă��Ȃ�
        else { footBool = false; }

        //Ray�̃q�b�g�ʒu,Ray���q�b�g�������ǂ���
        return (footBool, footIKPos);
    }

    /// <summary>
    /// �R���C�_�[�̒��S�ʒu���ړ�(Collider,Transform,Default)
    /// </summary>
    /// <param name="rightIkVector3">�E��IK�̈ʒu</param>
    /// <param name="leftIkVector3">����IK�̈ʒu</param>
    private void ColliderMove(Vector3 rightIkVector3, Vector3 leftIkVector3)
    {

        //������Y���̍����̍����v�Z
        //�����̍���/2(�l�̌ܓ�)�̍����Βl�ɂ���
        float moveYPos = 0;

        //�R���C�_�[�̏����ʒu��distance�����Z
        float disFoot = Mathf.Abs((float)Math.Round(rightIkVector3.y, _decimalPoint) - (float)Math.Round(leftIkVector3.y, _decimalPoint));
        moveYPos = _colDefCenter.y + disFoot - _colPlus;

        //Collider�̈ʒu��ύX
        _capCol.center = Vector3.Lerp(_capCol.center, new Vector3(0f, moveYPos, 0f), Time.deltaTime * _colMoveSpd);
    }

    /// <summary>
    /// �̂̏d�S���ړ�
    /// </summary>
    /// <param name="rightVector3">�E��IK�̈ʒu</param>
    /// <param name="leftVector3">����IK�̈ʒu</param>
    /// <param name="rightBool">�E���̃t���O</param>
    /// <param name="leftBool">�����̃t���O</param>
    private void BodyMove(Vector3 rightIkVector3, Vector3 leftIkVector3, bool rightBool, bool leftBool)
    {

        //������Ray���q�b�g���Ă��Ȃ��Ȃ珈�����Ȃ�
        if (!rightBool || !leftBool) { return; }

        //���E�̑��ƃL�����N�^�[�̑����̈ʒu�Ƃ̋������v�Z
        float rightFootDistance = rightIkVector3.y - transform.position.y;//�E���̋���
        float leftFootDistance = leftIkVector3.y - transform.position.y;//�����̋���

        //���E�̑��̈ʒu����艺�ɂ�����������Ƃ��Ďg��
        //�E�� < ���� = �E���̋��� : �����̋���
        float dis = rightFootDistance < leftFootDistance ? rightFootDistance : leftFootDistance;

        //�̂̏d�S�����ɂ�����̑��ɍ��킹�ĉ�����
        //���݂̏d�S�ʒu + ���̋���
        Vector3 newBodyPosition = _anim.bodyPosition + Vector3.up * dis;
        _anim.bodyPosition = newBodyPosition;
    }

    //HandIK
    /// <summary>
    /// Ray�̃q�b�g����(IK)
    /// </summary>
    /// <param name="ikGoal">AvatarIK��name</param>
    /// <param name="footWeight">���̃E�G�C�g</param>
    /// <param name="footIKPos">��IK�̈ʒu</param>
    /// <returns>Ray�̃q�b�g�ʒu,Ray���q�b�g�������ǂ���</returns>
    private void HandRay(AvatarIKGoal ikGoal, float handWeight, Vector3 handIKPos)
    {

        //Ray���΂�����
        Ray ray = new Ray(_anim.GetIKPosition(ikGoal), Vector3.right);

        //Ray�̎��o��
        Debug.DrawRay(_anim.GetIKPosition(ikGoal), Vector3.right * _rayFootRange, Color.red);

        //Ray�̔���t���O
        RaycastHit hit;

        //Ray�̔��肪True
        if (Physics.Raycast(ray, out hit, _rayHandRange))
        {

            //Ray�̓��������ʒu��⊮
            handIKPos = hit.point;

            Vector3 newPos = WallDistance(handIKPos, _anim.GetIKPosition(ikGoal), _handIKOffset);
            handIKPos = new Vector3(newPos.x, handIKPos.y, newPos.z);

            //��IK�̈ʒu
            _anim.SetIKPositionWeight(ikGoal, handWeight);//��IK�ʒu�E�G�C�g�̐ݒ�
            _anim.SetIKPosition(ikGoal, handIKPos);//��IK�ʒu�̐ݒ�

            //Ray�̓��������p�x��⊮
            Quaternion footIKRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            //��IK�̊p�x
            _anim.SetIKRotationWeight(ikGoal, handWeight);//��IK�p�x�E�G�C�g�̐ݒ�
            _anim.SetIKRotation(ikGoal, footIKRot);//��IK�p�x�̐ݒ�
        }
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

        //�䗦 = �@ / �ǂƃv���C���[�̒�������(�ړ��ڕW�̌Œ�ʒu) ,,, �B
        float percentage = wallRange / range;

        //�~�̒��S�_(wallPos)���猻�݂̃v���C���[���W�̋��� = �Ǎ��W / �v���C���[���W ... �C
        float xDisPlayer = targetPos.x - nowPos.x;//x��
        float zDisPlayer = targetPos.z - nowPos.z;//z��

        //�~�̒��S�_����Œ�Range�̋��� = �C / �B ... �D
        float xDis = xDisPlayer / percentage;//cos��
        float zDis = zDisPlayer / percentage;//sin��

        //�ڕW���W = �Ǎ��W + �D
        Vector3 moveFrontPos = new Vector3(targetPos.x - xDis, 0, targetPos.z - zDis);

        return moveFrontPos;
    }
}
