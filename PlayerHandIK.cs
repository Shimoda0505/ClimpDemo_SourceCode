using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//�ړ���̒T�������܂������Ȃ��������ߖv


/// <summary>
/// �v���C���[�̎�IK
/// </summary>
public class PlayerHandIK : MonoBehaviour
{

    [Header("RayCast�֘A")]
    //Ray�̒���
    private static float _rayRange = 1.0f;

    //Ray���΂��ʒu�̒����l
    private static float _rayPositionOffset = 1.5f;
    private static float _sRayPositionOffset = 3.3f;


    [Header("SphereCast�֘A")]
    //Ray�̒���
    private float _sRayRange = 2;

    //�i�[����tag��
    private string _rayCatchTag = "WallPoint";

    //�ړ�����i�[����List
    private List<GameObject> _rayHits = new List<GameObject>();
   


    //���\�b�h��--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /****[�O���ɕǂ����邩]****/
    /// <summary>
    /// �O���ɕǂ����邩��Ray�q�b�g����
    /// </summary>
    /// <returns>�O���ɕǂ����邩�ǂ���</returns>
    public bool HandRay()
    {

        //Ray���΂�����
        Ray ray = new Ray(transform.position + transform.up * _rayPositionOffset, transform.forward);

        //Ray�̎��o��
        Debug.DrawRay(transform.position + transform.up * _rayPositionOffset, transform.forward * _rayRange, Color.blue);

        //Ray�̔���I�u�W�F�N�g
        RaycastHit hit;

        //Ray�̔����ԋp
        return Physics.Raycast(ray, out hit, _rayRange);

    }


    /****[�͈͓��Ɉړ��悪���邩��T�����Ĉړ�]****/
    /// <summary>
    /// �ړ���̃|�C���g��T��
    /// </summary>
    public (GameObject, bool) SearchPoint(GameObject lastObj,Vector2 input,Transform headTr)
    {

        /****[���[�J���ϐ��̏�����]****/
        //�ړ���I�u�W�F�N�g�̏�����
        GameObject target = null;

        //�ړ��悪���邩�ǂ���
        bool isMove = false;


        /****[SphereCast���g�p�����͈͒T��]****/
        //SphereCast�Ŕ͈͂�T�����Ċi�[(���_,���a,����)
        RaycastHit[] clampHits = Physics.SphereCastAll(transform.position + transform.up * _sRayPositionOffset, _sRayRange, transform.forward);

        //Ray�z��̗v�f����0�Ȃ�null��ԋp
        if (clampHits.Length < 0) { print("Ray�z��̗v�f����0"); return (target, isMove);  }

        //clampHits�Ɋi�[����Ă���I�u�W�F�N�g����WallPoint�^�O�̃I�u�W�F�N�g���Ċi�[
        for (int i = 0; i <= clampHits.Length - 1; i++)
        {

            //clampHits�̊e�I�u�W�F�N�g��ݒ�
            GameObject newObj = null;
            newObj = clampHits[i].collider.gameObject;

            //tag������v && �O��ƈႤ�I�u�W�F�N�g�Ȃ�List�Ɋi�[
            if (newObj.tag == _rayCatchTag && newObj != lastObj) { _rayHits.Add(newObj); }
        }

        //�z��̗v�f����0�Ȃ�null��ԋp
        if (_rayHits.Count < 0) { print("�z��̗v�f����0"); return (target, isMove); }


        /*****[���͕��� && �z�񂩂�ł��߂��I�u�W�F�N�g��T��]****/
        //�ΏۃI�u�W�F�N�g���v���C���[�Ɣ�r���ď㉺���E�ǂ���ɂ��邩
        isMove = SearchPos(input.y, headTr, "�㉺");
        isMove = SearchPos(input.x, headTr, "���E");

        //�ړ��悪�Ȃ��Ȃ�Null��ԋp
        if (!isMove) { print("�ړ��悪�Ȃ�"); return (target, isMove); }

        //��ԋ߂��I�u�W�F�N�g��T��
        target = SearchNear(input, headTr.position);
        print("�ړ��悠��" + target);

        //List��S�č폜
        _rayHits.Clear();

        //�ړ����ԋp
        return (target, isMove);
    }

    /// <summary>
    /// �ΏۃI�u�W�F�N�g���v���C���[�Ɣ�r���č��E�㉺�ǂ���ɂ��邩
    /// </summary>
    /// <param name="input">���͒l</param>
    /// <param name="player">�v���C���[�̍��W</param>
    private bool SearchPos(float input, Transform headTr, string name)
    {

        //�z���S�T��
        for (int i = _rayHits.Count - 1; i >= 0; i--)
        {

            //�ΏۃI�u�W�F�N�g��x���W�����[�J���ϊ�
            //�ΏۃI�u�W�F�N�gx���W�����[�J�� - �v���C���[x���W
            float dis = 0;
            if (name == "�㉺") { dis = _rayHits[i].transform.position.y - headTr.position.y; }
            else if(name == "���E") { dis = headTr.InverseTransformPoint(_rayHits[i].transform.position).x; }
            print(name + _rayHits[i] + dis);

            //���͒l��(��,�E),�Ώۂ�(��,��)�ɂ���
            if (input >= 0 && dis < 0) { _rayHits.RemoveAt(i); }
            //���͒l��(��,��),�Ώۂ�(��,�E)�ɂ���
            else if (input < 0 && dis >= 0) { _rayHits.RemoveAt(i);  }
        }

        //�v�f����0�Ȃ�false
        if (_rayHits.Count == 0) { return false; }
        return true;
    }

    /// <summary>
    /// ��ԋ߂��I�u�W�F�N�g��T��
    /// </summary>
    /// <param name="input">���͒l</param>
    /// <param name="playerPos">�v���C���[�̍��W</param>
    /// <returns>��ԋ߂��I�u�W�F�N�g</returns>
    private GameObject SearchNear(Vector2 input, Vector3 playerPos)
    {

        /****[���[�J���ϐ��̏�����]****/
        //���͒l�̕� = �O�p�`��tan��(x���͂���l1�ɐݒ�)
        float inputTan = input.y * (1 / input.x);

        //��ԋ߂��I�u�W�F�N�g�̋�����������(�l�͓K��)
        float disMin = 100;

        //��ԋ߂��I�u�W�F�N�g��������
        GameObject nearObj = null;
        
        
        //�z���S�T��
        for (int i = _rayHits.Count - 1; i >= 0; i--)
        {

            //�I�u�W�F�N�g�̋�����������
            float dis = 0;

            //�ΏۃI�u�W�F�N�g��Vector2
            Vector2 targetPos = _rayHits[i].transform.position;

            //�ΏۃI�u�W�F�N�g�ƃv���C���[�̋������v�Z
            if (-0.1f <=input.x && input.x <= 0.1f) { dis = targetPos.y; }//X���̓���0
            else if (-0.1f <= input.y && input.y <= 0.1f) { dis = targetPos.x; }//Y���̓���0
            else { dis = Mathf.Abs((targetPos.x + targetPos.y * inputTan) / inputTan); }
            print("����" + _rayHits[i] + dis);

            //�i�[����Ă���I�u�W�F�N�g�����߂������������
            if (dis < disMin)
            {

                //��ԋ߂��I�u�W�F�N�g���X�V
                nearObj = _rayHits[i];

                //��ԋ߂��I�u�W�F�N�g�̋������X�V
                disMin = dis;
            }
        }

        //��ԋ߂��I�u�W�F�N�g��ԋp
        return nearObj;
    }



    /// <summary>
    /// SphereCast�̕\��
    /// </summary>
    /// <param name="pos"></param>
    void OnDrawGizmos() { Gizmos.DrawWireSphere(transform.position + transform.up * _sRayPositionOffset, _sRayRange); }
}
