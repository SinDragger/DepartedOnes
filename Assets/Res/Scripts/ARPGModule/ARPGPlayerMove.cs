using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARPGPlayerMove : MonoBehaviour
{
    public NavMeshAgent m_Agent;
    public ClickToMove filp;
    public SpineUnityControl control;


    bool startMove;
    bool isWDown = false;
    bool isADown = false;
    bool isSDown = false;
    bool isDDown = false;
    Vector2 targetMoveTo;
    // Start is called before the first frame update
    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        control = GetComponent<SpineUnityControl>();
        filp = GetComponent<ClickToMove>();
    }

    // Update is called once per fraFme
    void Update()
    {
        targetMoveTo = Vector2.zero;
        if (isWDown) targetMoveTo += Vector2.up;
        if (isADown) targetMoveTo += Vector2.left;
        if (isSDown) targetMoveTo += Vector2.down;
        if (isDDown) targetMoveTo += Vector2.right;


        if (targetMoveTo != Vector2.zero)
        {
            m_Agent.destination = transform.position + new Vector3(targetMoveTo.x, 0, targetMoveTo.y).normalized;
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
            m_Agent.ResetPath();
        }
        if (targetMoveTo.x > 0)
        {
            filp.ShiftModelFaceTo(true);
        }
        else if (targetMoveTo.x < 0)
        {
            filp.ShiftModelFaceTo(false);
        }
    
    }
    public void SetPositionMoveTo(Vector3 position)
    {
        m_Agent.destination = position;
    }
    /// <summary>
    /// 保证移动方向只有-1 0 1 三个档位 确保up 和down的时候能够准确归位
    /// </summary>


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
        isWDown = false;

    }
    public void AUp()
    {
        isADown = false;

    }

    public void SUp()
    {
        isSDown = false;

    }
    public void DUp()
    {
        isDDown = false;

    }

}



