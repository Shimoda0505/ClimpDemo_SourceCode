using UnityEngine;



/// <summary>
/// �J�����̋������\�b�h
/// </summary>
public class CameraController : MonoBehaviour
{


    [Header("�J�����֘A")]
    //�J�����̒����I�u�W�F�N�g�̈ʒu
    private Transform _headTr;
    private Vector3 _camLookPos = default;

    [SerializeField, Tooltip("�J�����̈ړ����x"),Range(1,20)]
    private float _camRotateSpd;
    private const float _camRotateMag = 10;//�J�����̉�]���x��100�{������10�{���ɕύX

    //�J�����̏����ʒu
    private const float _camDefDepth = -4f;

    //�J�����̒Ǐ]�I�u�W�F�N�g��
    private string _camLookPosName = "HeadPos";

    /// <summary>
    /// �J�����̏��enum
    /// </summary>
    enum CameraMotion
    {
        [InspectorName("�ړ�")] IDLE,
        [InspectorName("�ҋ@")] MOVE,
    }
    private CameraMotion _cameraMotion = CameraMotion.IDLE;



    //������------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    private void Awake()
    {

        //�J�����̒����I�u�W�F�N�g�̈ʒu
        _headTr = GameObject.FindGameObjectWithTag(_camLookPosName).gameObject.transform;

        //�J�����̒����I�u�W�F�N�g
        _camLookPos = _headTr.position;

        //�J�������v���C���[�̔w��ɏ����ݒ�
        Resetting(_camDefDepth, _headTr.position);

        //�J�����̉�]���x��10�{������100�{���ɕύX
        _camRotateSpd *= _camRotateMag;
    }



    //���\�b�h��--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*#####[�O���Q��]#####*/
    /// <summary>
    /// �J�����̓��͊Ǘ�
    /// </summary>
    /// <param name="Right_S">RightStick�̓���</param>
    public void CameraInput(bool Right_S)
    {

        if (Right_S) { _cameraMotion = CameraMotion.MOVE; }//�ړ�
        else { _cameraMotion = CameraMotion.IDLE; }//�ҋ@
    }

    /// <summary>
    /// �J�����̈ړ��Ǘ�
    /// </summary>
    public void CameraMove(Vector2 Right_S)
    {

        switch (_cameraMotion)
        {

            case CameraMotion.IDLE:

                break;


            case CameraMotion.MOVE:

                //��]�v�Z
                Rotate(_camLookPos, Right_S.normalized, _camRotateSpd);
                break;
        }

        //�Ǐ]�v�Z
        _camLookPos = Move(_headTr.position, _camLookPos);
    }


    /*#####[��������]#####*/
    /// <summary>
    /// �J�����������ʒu�Ɉړ�
    /// </summary>
    private void Resetting(float depthFloat, Vector3 targetPos)
    {

        //���Z����(float)��Vector3�ɕϊ�
        Vector3 depthPos = new Vector3(0, 0, depthFloat);

        //�^�[�Q�b�g + ���Z�ʒu�Ɉړ�
        Camera.main.transform.position = targetPos + depthPos;
    }

    /// <summary>
    /// �J�����̒Ǐ]
    /// </summary>
    private Vector3 Move(Vector3 nowPos, Vector3 lastPos)
    {

        //�J�������^�[�Q�b�g�̈ړ��ʕ��ړ�������
        Camera.main.transform.position += nowPos - lastPos;

        //�ړ���̕ۊǂƍX�V��ԋp
        return lastPos = nowPos;
    }

    /// <summary>
    /// �J�����̉�]
    /// </summary>
    private void Rotate(Vector3 targetPos, Vector2 input, float speed)
    {

        //�J�����̈ʒu
        Transform camTr = Camera.main.transform;

        //�J�����̉���]
        camTr.RotateAround(targetPos, Vector3.up, input.x * Time.deltaTime * speed);

        //�J�����̏c��]
        camTr.RotateAround(targetPos, camTr.right, -input.y * Time.deltaTime * speed);
    }
}
