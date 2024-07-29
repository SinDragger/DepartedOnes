using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class LeapSlashImpact : IImpactEffect
{
    public static Vector3 OnceTargetPos;
    //需要重写 可跳跃逻辑
    public static Vector3 CannotLeappos;

    HashSet<SoldierModel> underAttack = new HashSet<SoldierModel>();
    ARPGControl control;
    const float basicSpeed = 20;
    float speed;
    const float maxHeight = 2;
    event Action moveAction;
    SkillDeployer deployer;
    Vector3 mousePos;
    float totalTime;
    float nowTime;
    NavMeshHit hit;
    Vector3 startPos;
    Vector3 endPos;
    Transform shadow;
    /// <summary>
    /// 到达目标地点
    /// </summary>
    bool atTheTargetLocation;
    public LeapSlashImpact()
    {
        Debug.Log("LeapSlashImpact");
    }
    public void Execute(SkillDeployer deployer)
    {
        this.deployer = deployer;
        control = deployer.SkillData.owner.GetComponent<ARPGControl>();
        shadow = control.GetComponentsInChildren<Transform>()[3];
        control.characterSkillManager.canCastSkill = false;
        control.isItAFastMove = true;
        //剔除自生
        underAttack.Add(control.GetComponent<SoldierModel>());


        //判断落脚点 通过角色的头顶距离做射线判断 是否可以跳过去如果可以 就直接飞过去 
        //如果有障碍 还是起跳但是需要一直通过NavMesh.SamplePosition来判断跳跃方向的前进是否可以移动
        //角色先有一个上升的表现 后又一个下降的表现
        //当遇见不可移动的点时 应该直接落地
        //不可释放技能 且不可进行攻击
        //不一定会执行到看退出的情况但是还是要写上 防止没有退出的情况
        SetTargetMovePos();


        deployer.updateEvent += DeployerUpdate;
        deployer.fixedUpdateEvent += DeployerFixedUpdate;


    }

    Spine.Unity.SkeletonAnimation animation;
    Spine.AnimationState.TrackEntryEventDelegate trackEvent = default;
    private void SetTargetMovePos()
    {
        //control.control.PlayAnim(AnimState.跳越动画);
        //禁用移动 并且关闭NavMeshAgent
        startPos = control.transform.position;
        control.CannotSetDestantion();
        control.m_Agent.enabled = false;
        //调整朝向
        if (OnceTargetPos == default)
        {
            mousePos = InputManager.Instance.mouseWorldPos;
        }
        else
        {
            mousePos = OnceTargetPos;
            OnceTargetPos = default;
        }
        Vector2 ownerXZ = new Vector2(deployer.SkillData.owner.transform.position.x, deployer.SkillData.owner.transform.position.z);
        Vector2 targetXZ = new Vector2(mousePos.x, mousePos.z);
        float distance = Vector2.Distance(ownerXZ, targetXZ);
        float time = distance / basicSpeed;
        speed = basicSpeed;
        totalTime = time;
        nowTime = 0;
        //获取最大跳跃距离 才是落脚点
        //判断目标点是否是可移动 可移动给委托添加
        if (NavMesh.SamplePosition(mousePos, out hit, 0.3f, 1))
        {
            //Debug.LogError(hit.position);
            Vector3 startPos = control.transform.position + new Vector3(0, GameConfig.RangeWeaponYFix, 0);
            endPos = hit.position;
            //Vector3 endPos = hit.position;
            float height = distance / 3;
            height = Mathf.Clamp(height, 2f, float.MaxValue);
            time = Mathf.Clamp(time, 0.45f, float.MaxValue);
            totalTime = time;
            Vector3 middlePos = (startPos + endPos) / 2 + new Vector3(0, height, 0f);
            middlePos.x = startPos.x / 4 + endPos.x / 4 * 3;
            middlePos.z = startPos.z / 4 + endPos.z / 4 * 3;
            Vector3[] path =
            {
                endPos,
                middlePos,
                endPos,
            };
            control.transform.DOPath(path, time, PathType.CubicBezier).SetEase(GameManager.instance.arrowCurve).OnComplete(() =>
            {
            });
        }
        else
        {
            endPos = startPos;
            //不可移动
            moveAction += DestinationCannotBeMoved;
        }

        //人物朝向设置
        Vector2 mouseScreenPos = RectTransformUtility.WorldToScreenPoint(CameraControl.Instance.mainCamera, mousePos);
        Vector2 selfScreenPos = RectTransformUtility.WorldToScreenPoint(CameraControl.Instance.mainCamera, control.transform.position);
        control.moveModule.InputMoveDirection(mouseScreenPos - selfScreenPos);

        animation = control.m_SoldierModel.actionModel.control.selfSkelAni;
        control.m_SoldierModel.actionModel.control.forceDamageType = 1;
        trackEvent = (s, e) =>
       {
           if (e.Data.Name.Equals("HitPrepare"))
           {
               animation.state.Event -= trackEvent;
               animation.timeScale = 0f;
               //m_Model.OnAttackEnermy();
           }
       };
        animation.state.Event -= trackEvent;
        animation.state.Event += trackEvent;

        atTheTargetLocation = false;
        //播放起步动画
    }

    Vector2 targetV2XZ { get { return new Vector2(mousePos.x - control.transform.position.x, mousePos.z - control.transform.position.z); } }

    private void DeployerFixedUpdate()
    {


    }

    Ray ray;
    RaycastHit m_HitInfo;
    Vector3 originV3 = new Vector3(0, 40, 0);
    Vector3 vY = Vector3.zero;

    public void MoveToPosition(Vector2 targetMoveTo, float velocity)
    {
        nowTime += Time.deltaTime;
        if (animation.timeScale == 0f && nowTime / totalTime > 0.784f)//下落转折点
        {
            //调整Animation.TimeScale至后半部分播完的反比例
            float leftTime = (1 - 0.784f) * totalTime;
            float animTime = 0.5f;
            animation.timeScale = animTime / (leftTime + 0.2f);
            // Debug.LogError(animTime / leftTime);
        }

        if (nowTime >= totalTime)
        {
            atTheTargetLocation = true;
            animation.timeScale = 1f;
            shadow.localPosition = Vector3.zero;
            animation.state.Event -= trackEvent;
        }
        originV3.x = targetMoveTo.x;
        originV3.z = targetMoveTo.y;

        ray = new Ray(control.transform.position + originV3 * Time.deltaTime * velocity, Vector3.down);

        //TODO 做成事件抽出来 
        //三种情况：第一种目的地是可移动的移动的过程中没有不可移动的地方

        //​          第二种目的地是可移动的 移动的过程中有不可移动的地方

        //​          第三种目的地是不可移动的

        //第一步 检测目的地是否是不可移动的

        //可移动 第二步 按照角色与目的地的 距离做位移

        //不可移动 第二步 按照角色与目的地的方向 做跳跃 当遇见不可移动的物体时，坐标停滞 但是动画继续

        //if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
        //{
        //    if (NavMesh.SamplePosition(m_HitInfo.point, out hit, 0.5f, 1))
        //    {
        //        //TODO 将角色的Y于设定的曲线相结合 通过当前时间在总体时间的比例确定x方向的值获取到y方向的值
        //        vY.y = GameManager.instance.jumpCurve.Evaluate(nowTime / totalTime) * maxHeight;
        //        control.transform.DOMove(hit.position + vY, Time.deltaTime);
        //        return hit.position;
        //    }
        //}
        //else
        //{
        //    vY.y = GameManager.instance.jumpCurve.Evaluate(nowTime / totalTime) * maxHeight;
        //    control.transform.DOMove(control.transform.position + vY, Time.deltaTime);

        //}
        moveAction?.Invoke();


    }

    void DestinationMovable()
    {
        return;
        if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
        {
            if ("BlockCollider" == m_HitInfo.collider.gameObject.tag) return;
            //TODO 将角色的Y于设定的曲线相结合 通过当前时间在总体时间的比例确定x方向的值获取到y方向的值
            vY.y = GameManager.instance.jumpCurve.Evaluate(nowTime / totalTime) * maxHeight;
            //control.transform.DOMove(m_HitInfo.point + vY, Time.deltaTime);
            Debug.Log("改变点");
            //control.transform.DOMove(new Vector3(m_HitInfo.point.x, vY.y, m_HitInfo.point.z), Time.deltaTime);
            control.transform.DOMove(m_HitInfo.point + vY, Time.deltaTime);
        }
        //else
        //{
        //    vY.y = GameManager.instance.jumpCurve.Evaluate(nowTime / totalTime) * maxHeight;
        //    control.transform.DOMove(control.transform.position + vY, Time.deltaTime);

        //}
    }
    void DestinationCannotBeMoved()
    {

        if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
        {
            if (NavMesh.SamplePosition(m_HitInfo.point, out hit, 0.5f, 1))
            {
                //TODO 将角色的Y于设定的曲线相结合 通过当前时间在总体时间的比例确定x方向的值获取到y方向的值
                vY.y = GameManager.instance.jumpCurve.Evaluate(nowTime / totalTime) * maxHeight;
                control.transform.DOMove(m_HitInfo.point + vY, Time.deltaTime);
            }
        }
        else
        {
            //y在变 x和z不再变了
            vY.y = GameManager.instance.jumpCurve.Evaluate(nowTime / totalTime) * maxHeight;
            control.transform.DOMove(m_HitInfo.point + vY, Time.deltaTime);
        }
    }

    private void DeployerUpdate()
    {
        RaycastHit hit;
        Vector3 localPoint = shadow.localPosition;
        if (Physics.Raycast(new Vector3(control.transform.position.x, 100, control.transform.position.z), Vector3.down, out hit, 150, LayerMask.GetMask("Map")))
        {
            localPoint.y = hit.point.y - control.transform.position.y + 0.02f;
        }
        shadow.localPosition = localPoint;
        float percent = nowTime / totalTime;
        Vector3 target = shadow.position;
        target.y = Vector3.Lerp(startPos, endPos, percent).y;
        CameraControl.Instance.LookAtV3(target);
        //当到达鼠标地点和当遇见不可移动区域时 直接落地
        MoveToPosition(targetV2XZ.normalized, speed);
        if (atTheTargetLocation)
        {
            //释放技能特效并且计算当前角色周围敌人 照成伤害并且击退
            PushAndDamageEnemy();
            deployer.updateEvent -= DeployerUpdate;
            CoroutineManager.DelayedCoroutine(0.15f, () =>
            {
                ResetCanSetDestantion();
            });
        }

    }
    void PushAndDamageEnemy()
    {
        var effectName = deployer.SkillData.relatedEffectName;
        var burstEffect = GameObjectPoolManager.Instance.Spawn("Prefab/" + effectName);
        //Quaternion rotation = Quaternion.FromToRotation(-burstEffect.transform.forward, CameraControl.Instance.CameraToward(targetMoveTo));
        //// 将旋转应用到物体的朝向
        //burstEffect.transform.rotation = rotation * burstEffect.transform.rotation;
        burstEffect.transform.position = control.transform.position;
        burstEffect.transform.localScale = Vector3.one;
        //burstEffect.GetComponent<SpecialEffects>().Execute();
        //将释放器移动到玩家身上 并且启动释放器的查询周围敌人的方法
        deployer.transform.position = control.transform.position;
        deployer.CalculateTargets();
        //需要循环的目标
        foreach (var sModel in deployer.SkillData.attackTargets)
        {
            //如果trs在hashset中不做操作
            if (!underAttack.Contains(sModel))
            {
                float distance = UnitControlManager.instance.GetSoldierSpaceDistance(sModel, control.m_SoldierModel);
                if (distance > deployer.SkillData.atackDistance)
                {
                    continue;
                }
                //如果trs不在hashset中将transfrom于control.transfrom.position作差值向量 方向取control到trs的方向

                Vector3 pushDirection = sModel.transform.position - control.transform.position;
                pushDirection.y = 0;
                //TODO 推开敌人效果不理想
                sModel.transform.position += pushDirection.normalized * 1.5f;

                //获取敌人数据
                if (sModel.nowStatus.Belong != control.m_SoldierModel.nowStatus.Belong)
                    sModel.nowStatus.GetDamage((int)deployer.SkillData.atkRatio);

                if (sModel.nowStatus.isDead)
                {
                    var kNumber = deployer.SkillData.owner.GetComponent<SoldierModel>().nowStatus.GetIntValue("SkillKill");
                    kNumber++;
                    deployer.SkillData.owner.GetComponent<SoldierModel>().nowStatus.SetIntValue("SkillKill",kNumber);

                }

                underAttack.Add(sModel);
            }
        }
    }



    private void ResetCanSetDestantion()
    {
        underAttack.Clear();
        control.CanSetDestantion();
        //TODO 将下面两行代码写道control中直接进行调用
        control.m_Agent.enabled = true;
        control.characterSkillManager.canCastSkill = true;
        control.isItAFastMove = false;
        control.transform.DOKill();
        deployer.exitEvent -= ResetCanSetDestantion;
        deployer.updateEvent -= DeployerUpdate;
        deployer.fixedUpdateEvent -= DeployerFixedUpdate;
        deployer.SkillData.coolRemain = deployer.SkillData.coolTime;
        CameraControl.Instance.ChangeCameraToARPG(ARPGManager.Instance.currentGeneralControl);
        deployer.DestorySkillDeployer();
    }

    public bool Releasable()
    {
        if (CannotLeappos != default && InputManager.Instance.mouseWorldPos.x > CannotLeappos.x)
        {
            return false;
        }
        return true;
    }
}
