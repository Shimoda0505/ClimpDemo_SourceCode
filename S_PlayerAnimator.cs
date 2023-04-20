using UnityEngine;



/// <summary>
/// アニメーション管理
/// </summary>
public struct S_PlayerAnimator
{

    /// <summary>
    /// 移動アニメーションBool
    /// </summary>
    /// <param name="name">Idle,Walk,Run,FallEnd,Climb</param>
    /// <param name="anim">Animator</param>
    public void MoveBoolAnim(string name, Animator anim)
    {

        //アニメーション再生
        anim.SetBool(name, true);

        //アニメーションのfalse
        ResetAnim(name, anim);
    }

    /// <summary>
    /// アニメーションのfalse
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
    /// カメのぼりアニメーションFloat
    /// </summary>
    /// <param name="anim">Animator</param>
    /// <param name="input">入力値</param>
    public void HighWallFloatAnim(Vector2 input, Animator anim)
    {

        //アニメーションBrendTree
        anim.SetFloat("ClimbH", input.x);
        anim.SetFloat("ClimbV", input.y);
    }

    /// <summary>
    /// 移動アニメーションFloat
    /// </summary>
    public void MoveFloatAnim(Animator anim, Vector2 input)
    {

        //アニメーションBrendTree
        anim.SetFloat("Horizontal", input.x);
        anim.SetFloat("Vertical", input.y);
    }

    /// <summary>
    /// アニメーションに同期したcolliderサイズ
    /// </summary>
    /// <param name="anim">Animator</param>
    /// <returns>高さ,位置</returns>
    public (float, float) ChangeCollider(Animator anim)
    {

        float height = anim.GetFloat("ChangeColliderH");
        float center = anim.GetFloat("ChangeColliderC");

        return (height, center);
    }

    /// <summary>
    /// パルクールアニメーション(Start,Fall,End,Move)
    /// </summary>
    public void WallAnim(string name, Animator anim, Vector2 input)
    {

    }
}

