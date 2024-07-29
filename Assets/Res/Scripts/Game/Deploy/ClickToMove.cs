using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

///负责移动和碰撞的管理类 TODO:改名成模型管理
///寻路管理
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour, ITakeOverAbleMove
{
    public NavMeshAgent m_Agent;
    SoldierModel m_Model;
    [HideInInspector]
    public SC_Common control;
    [HideInInspector]
    public bool agentMoving;
    public bool holdMode
    {
        set
        {
            if (value)
            {
                //近战不会避障，进行阵地战
                m_Agent.avoidancePriority = 40;
            }
            else
            {
                m_Agent.avoidancePriority = 50;
            }

        }
    }

    /// <summary>
    /// 设置行为最高值
    /// </summary>
    public void SetMotionPriority(int value)
    {
        m_Agent.avoidancePriority = value;
    }

    /// <summary>
    /// 是否在进攻
    /// </summary>
    bool isAttacking;
    public bool IsAttacking
    {
        get
        {
            return isAttacking;
        }
        set
        {
            isAttacking = value;
        }
    }
    public string attackEventName;
    void OnEnable()
    {
        if (m_Agent == null)
            m_Agent = GetComponent<NavMeshAgent>();
        if (m_Model == null)
            m_Model = GetComponent<SoldierModel>();
        if (control == null)
            control = GetComponent<SC_Common>();
        //配置Control使用的攻防移动逻辑
    }
    public void Init(SoldierStatus data)
    {
        if (control == null)
            control = GetComponent<SC_Common>();
        control.Init(data);
    }
    Vector3 movePosition;
    NavMeshHit hit;
    public void SetPositionMoveTo(Vector3 position)
    {
        //攻击状态且已经出现阻挡关系的单位不影响
        if (NavMesh.SamplePosition(position, out hit, 30f, 1))
        {
            movePosition = hit.position;
        }
        else if (NavMesh.SamplePosition(position, out hit, 50f, 1))
        {
            movePosition = hit.position;
        }
        //TODO


        if (Vector3.Distance(new Vector3(movePosition.x, transform.position.y, movePosition.z), transform.position) > 0.05f * m_Agent.speed)
        {
            m_Agent.destination = movePosition;
            m_Agent.avoidancePriority = 25;
            agentMoving = true;
            control.PlayMoveStart();
        }
    }
    /// <summary>
    /// 设置移动目标点邻近移动点
    /// </summary>
    public void SetPositionAttackRangeCloseTo(SoldierModel target)
    {
        float distance = Vector3.Distance(m_Model.lastPosition, target.lastPosition);
        if (distance > m_Model.nowStatus.nowAttackRange)
        {
            SetPositionMoveTo(Vector3.MoveTowards(m_Model.lastPosition, target.lastPosition, distance - m_Model.nowStatus.nowAttackRange));
        }
        //Vector3.MoveTowards(m_Model.lastPosition, target.lastPosition, m_Model.nowStatus.nowAttackRange);
    }
    /// <summary>
    /// 考虑更替接口？
    /// 由父级调用执行，移动向对方中心位置，并建立阻挡对战关系
    /// </summary>
    public void AttackTarget(Transform target)
    {
        FixFaceTarget(target.transform.position - transform.position);//修正朝向
        IsAttacking = true;
        m_Agent.SetDestination(transform.position);
        var oldTarget = m_Model.nowStatus.focusTarget;
        control.AttachEventListener("Hit", () =>
        {
            m_Model.nowStatus.OnAttackEnermy(oldTarget);
        });
        control.PlayAttack(() =>
        {
            IsAttacking = false;
        });
    }


    public void BeenHit()
    {
        control.BeenHit();
    }

    public void OnInit()
    {
        control.OnReset();
        IsAttacking = false;
        agentMoving = false;
        m_Agent.enabled = true;
    }

    public void OnReset()
    {
        OnInit();
        SetMotionPriority(50);
    }

    public void OnDying()
    {
        m_Agent.enabled = false;
        control.PlayDie(() =>
        {
            m_Model.gameObject.SetActive(true);

        }
        );
    }

    bool isShifting;
    bool isRight;

    public bool IsRight { get => isRight; }

    void LateUpdate()
    {
        if (IsAttacking)
        {
            if (m_Model.nowStatus.focusTarget == null || m_Model.nowStatus.focusTarget.model == null)
            {
                IsAttacking = false;
            }
            else
            {
                //m_Model.ChangeFaceTo(m_Model.nowStatus.focusTarget.model.transform.position - transform.position);
                FixFaceTarget(m_Model.nowStatus.focusTarget.model.transform.position - transform.position);
                return;
            }
        }
        if (agentMoving)
        {
            if (m_Model.nowStatus.focusTarget != null && m_Model.nowStatus.focusTarget.model != null)
            {
                FixFaceTarget(m_Model.nowStatus.focusTarget.model.transform.position - transform.position);
            }
            else
            {
                FixFaceTarget(transform.forward);
            }
            if (Vector3.Distance(new Vector3(m_Agent.destination.x, transform.position.y, m_Agent.destination.z), transform.position) < 0.01f * m_Agent.speed)
            {

                if (m_Model.commander == null || m_Model.commander.moveList.Count <= 1)
                {
                    agentMoving = false;
                    m_Agent.avoidancePriority = 50;
                    control.PlayMoveEnd();
                    m_Agent.ResetPath();
                    //if (m_Model.commander != null)
                    //{
                    //    Debug.LogError("Trigger");
                    //    FixFaceTarget(m_Model.commander.troopfaceTo);
                    //}
                    FixFaceTarget(m_Model.commander.troopfaceTo);
                }
            }
        }
        //TODO 重新弄逻辑

    }




    void Update()
    {
        CheckMoveList();
    }

    public void CheckMoveList()
    {
        if (m_Model.commander == null || m_Model.commander.moveList.Count == 0) return;
        float distance = Vector3.Distance(movePosition, transform.position);
        if (distance < 0.1f * m_Agent.speed)
        {
            m_Model.commander.ReachNumberIncrease();
        }
       
    }

    public void FixFaceTarget(Vector3 target)
    {
        if (!isShifting && !IsAttacking)
        {
            bool wantRight = Vector3.Dot(target, CameraControl.right) > 0;
            if (isRight != wantRight)
            {
                ShiftModel();
            }
        }
    }

    public void ShiftModelFaceTo(bool right)
    {
        if (isRight && right) return;
        if (!isRight && !right) return;
        //if (right)
        //{
        //    m_Model.transform.forward = CameraControl.right;
        //}
        //else
        //{
        //    m_Model.transform.forward = -CameraControl.right;
        //}
        ShiftModel();
    }

    void ShiftModel()
    {
        isShifting = true;
        //不增加转身动作 时间设为0.01f
        isShifting = false;
        isRight = !isRight;
        m_Model.flipRoot.GetComponent<ForceFaceCamera>().Flip();
    }

    public void TakeOver()
    {
        enabled = false;
    }

    public void ReturnBack()
    {
        enabled = true;
    }
    /// <summary>
    /// 接入的是相对的摄像机屏幕坐标
    /// </summary>
    /// <param name="vec"></param>
    public void InputMoveDirection(Vector2 vec)
    {
        if (vec == Vector2.zero)
        {
            // m_Agent.ResetPath();
        }
        else
        {
            if (vec.x < 0)
            {
                ShiftModelFaceTo(false);
            }
            else if (vec.x > 0)
            {
                ShiftModelFaceTo(true);
            }
        }
    }
}