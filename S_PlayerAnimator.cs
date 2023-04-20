using UnityEngine;



/// <summary>
/// �A�j���[�V�����Ǘ�
/// </summary>
public struct S_PlayerAnimator
{

    /// <summary>
    /// �ړ��A�j���[�V����Bool
    /// </summary>
    /// <param name="name">Idle,Walk,Run,FallEnd,Climb</param>
    /// <param name="anim">Animator</param>
    public void MoveBoolAnim(string name, Animator anim)
    {

        //�A�j���[�V�����Đ�
        anim.SetBool(name, true);

        //�A�j���[�V������false
        ResetAnim(name, anim);
    }

    /// <summary>
    /// �A�j���[�V������false
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="anim">Animator</param>
    private void ResetAnim(string name, Animator anim)
    {

        if (name != "Idle") { anim.SetBool("Idle", false); }
        if (name != "Walk") { anim.SetBool("Walk", false); }
        if (name != "Run") { anim.SetBool("Run", false); }
        if (name != "FallEnd") { anim.SetBool("FallEnd", false); }
        if (name != "Climb") { anim.SetBool("Climb", false); }
        if (name != "LowWall") { anim.SetBool("LowWall", false); }
        if (name != "FallRoop") { anim.SetBool("FallRoop", false); }
        if (name != "LowWall") { anim.SetBool("LowWall", false); }
        if (name != "HighWall") { anim.SetBool("HighWall", false); }
    }

    /// <summary>
    /// �J���̂ڂ�A�j���[�V����Float
    /// </summary>
    /// <param name="anim">Animator</param>
    /// <param name="input">���͒l</param>
    public void HighWallFloatAnim(Vector2 input, Animator anim)
    {

        //�A�j���[�V����BrendTree
        anim.SetFloat("ClimbH", input.x);
        anim.SetFloat("ClimbV", input.y);
    }

    /// <summary>
    /// �ړ��A�j���[�V����Float
    /// </summary>
    public void MoveFloatAnim(Animator anim, Vector2 input)
    {

        //�A�j���[�V����BrendTree
        anim.SetFloat("Horizontal", input.x);
        anim.SetFloat("Vertical", input.y);
    }

    /// <summary>
    /// �A�j���[�V�����ɓ�������collider�T�C�Y
    /// </summary>
    /// <param name="anim">Animator</param>
    /// <returns>����,�ʒu</returns>
    public (float, float) ChangeCollider(Animator anim)
    {

        float height = anim.GetFloat("ChangeColliderH");
        float center = anim.GetFloat("ChangeColliderC");

        return (height, center);
    }

    /// <summary>
    /// �p���N�[���A�j���[�V����(Start,Fall,End,Move)
    /// </summary>
    public void WallAnim(string name, Animator anim, Vector2 input)
    {

    }
}

