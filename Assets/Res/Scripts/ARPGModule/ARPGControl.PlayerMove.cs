using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class ARPGControl : MonoBehaviour
{
    public bool startMove;
    bool isWDown = false;
    bool isADown = false;
    bool isSDown = false;
    bool isDDown = false;
    public Vector2 targetMoveTo;
    public bool isItAFastMove = false;
    public float moveSpeed = 1f;
    public void Move()
    {

        //Debug.LogError("现在强行进入arpg模式update刷新commander不可设置敌人 被敌人吸附的逻辑 ");
        //TODO 现在强行进入arpg模式update刷新该逻辑 
        m_SoldierModel.commander.canSetTarget = false;
        if (canSetDestantion)
        {
            MoveToPosition(targetMoveTo, m_Agent.speed * moveSpeed);
        }
        else
        {
            return;
        }
        targetMoveTo = Vector2.zero;
        if (isWDown) targetMoveTo += Vector2.up;
        if (isADown) targetMoveTo += Vector2.left;
        if (isSDown) targetMoveTo += Vector2.down;
        if (isDDown) targetMoveTo += Vector2.right;
        if (targetMoveTo != Vector2.zero)
        {
            if (!startMove)
            {
                control.PlayMoveStart();
                startMove = true;
            }
        }
        else
        {
            if (startMove)
            {
                control.PlayMoveEnd();
                startMove = false;
            }
        }
        isWDown = false;
        isADown = false;
        isSDown = false;
        isDDown = false;
        //不适用NavMeahAgent
        if (canSetDestantion)
            moveModule.InputMoveDirection(targetMoveTo.normalized);
    }

    RaycastHit m_HitInfo;
    Ray ray;
    NavMeshHit hit;
    Vector3 originV3 = new Vector3(0, 40, 0);
    private void FixedUpdate()
    {
    }
    /// <summary>
    /// 移动方法通过传入移动的方向 和移动速度设置移动
    /// </summary>
    /// <param name="targetMoveTo">移动方向</param>
    /// <param name="velocity">移动的速度</param>
    public Vector3 MoveToPosition(Vector2 targetMoveTo, float velocity)
    {
        Vector3 cameraToward = CameraControl.Instance.CameraToward(targetMoveTo).normalized;
        originV3.x = cameraToward.x;
        originV3.z = cameraToward.z;
        var value = transform.position + originV3 * Time.deltaTime * velocity;
        value.y = transform.position.y;
        if (NavMesh.SamplePosition(value, out hit, 0.5f, 1))
        {
            transform.position = hit.position;
            return hit.position;
        }
        ray = new Ray(transform.position + originV3 * Time.deltaTime * velocity, Vector3.down);
        //CameraControl.instance.SetDesiredPosition(targetMoveTo);
        if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
        {
            if (NavMesh.SamplePosition(m_HitInfo.point, out hit, 0.5f, 1))
            {
                //TODO 如果要计算三角函数需要用
                //transform.DOMove(hit.position, Time.deltaTime);
                transform.position = hit.position;
                return hit.position;
            }
        }
        return Vector3.zero;
    }


    /// <summary>
    /// 可设置移动
    /// </summary>
    public void CanSetDestantion()
    {

        canSetDestantion = true;
    }
    /// <summary>
    /// 将NavMeshAgent组件关闭 启动
    /// </summary>
    public void CannotSetDestantion()
    {
        canSetDestantion = false;

    }
    public void ResetMoveVector()
    {
        isWDown = false;
        isADown = false;
        isSDown = false;
        isDDown = false;
    }

    public void WDown()
    {
        isWDown = true;

    }
    public void ADown()
    {
        isADown = true;

    }
    public void SDown()
    {
        isSDown = true;

    }
    public void DDown()
    {
        isDDown = true;

    }

    public void WUp()
    {
        //isWDown = false;

    }
    public void AUp()
    {
        //isADown = false;

    }

    public void SUp()
    {
        //isSDown = false;

    }
    public void DUp()
    {
        //isDDown = false;
    }
}
