using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
/// <summary>
/// 底层控制动画状态机——SpineAnimation版本
/// 后续通过接口进行更替
/// </summary>
public class SpineUnityControl : MonoBehaviour
{
    public SkeletonAnimation selfSkelAni;
    AnimState nowState;
    AnimState nextRestoreState;
    [HideInInspector]
    public bool isAttacking;
    [HideInInspector]
    public bool isMoving;
    bool isDying;
    public MaterialPropertyBlock mpb;
    public MeshRenderer meshRenderer;
    public float timer;
    float moveSpeedAnimPercent = 1f;

    public void SetMoveSpeedAnimPercent(float value)
    {
        moveSpeedAnimPercent = value;
        if (nowState == AnimState.MOVE_BEGIN || nowState == AnimState.MOVE_LOOP)
            selfSkelAni.timeScale = moveSpeedAnimPercent;
    }

    private void Awake()
    {
        if (meshRenderer == null) meshRenderer = selfSkelAni.GetComponent<MeshRenderer>();
        meshRenderer.SetPropertyBlock(SpineAtlasManager.Instance.BasicBlock);

    }
    public void PlayIdle()
    {
        selfSkelAni.valid = true;
        PlayAnim(AnimState.IDLE);
    }
    public void PlayMoveStart(bool isRunning = false)
    {
        isMoving = true;
        //TODO:会打断攻击的播放
        if (isRunning)
        {
            PlayAnim(AnimState.RUN_BEGIN, AnimState.RUN_LOOP);
        }
        else
        {
            if (nowState.Equals(AnimState.MOVE_LOOP) || nowState.Equals(AnimState.MOVE_BEGIN)) return;
            PlayAnim(AnimState.MOVE_BEGIN);
        }
    }

    public void PlayMoveEnd(bool isRunning = false)
    {
        isMoving = false;
        if (isAttacking) return;
        if (isRunning)
        {
            PlayAnim(AnimState.RUN_END);
        }
        else
        {
            PlayAnim(AnimState.MOVE_END);
        }
    }


    public void AttachEventListener(string eventName, Action callback)
    {
        Spine.AnimationState.TrackEntryEventDelegate delegateAction = default;
        delegateAction = (s, e) =>
        {
            if (e.Data.Name.Equals(eventName))
            {
                selfSkelAni.state.Event -= delegateAction;
                //Debug.LogError("Remove");
                callback?.Invoke();
            }
        };
        selfSkelAni.state.Event -= delegateAction;
        selfSkelAni.state.Event += delegateAction;
    }
    /// <summary>
    /// TODO:增加AttackSpeed
    /// </summary>
    /// <param name="callback"></param>
    public void PlayAttack(Action callback = null)
    {
        isAttacking = true;
        if (PlayAnim(AnimState.ATTACK))
        {
            Spine.AnimationState.TrackEntryDelegate delegateAction = default;
            delegateAction = (g) =>
            {
                selfSkelAni.state.Complete -= delegateAction;
                nowState = AnimState.End;
                isAttacking = false;
                callback?.Invoke();
            };
            selfSkelAni.state.Complete += delegateAction;
        }
        else
        {
            if (AnimState.ATTACK == nowState) return;
            isAttacking = false;
            callback?.Invoke();
        }
    }
    public void PlayDie(Action callback = null)
    {
        PlayAnim(AnimState.DIE, AnimState.End);
        AttachEventListener("OnDie", () =>
        {
            //selfSkelAni.AnimationState.GetCurrent(0).TrackTime = selfSkelAni.state.GetCurrent(0).AnimationEnd;
            var updateMode = selfSkelAni.UpdateMode;
            selfSkelAni.UpdateMode = UpdateMode.FullUpdate;
            selfSkelAni.Update(0f);
            selfSkelAni.LateUpdate();
            selfSkelAni.UpdateMode = updateMode;
            selfSkelAni.valid = false;
        });
        selfSkelAni.state.Complete += (t) =>
        {
            callback?.Invoke();
        };
        isDying = true;
        timer = 1f;
    }

    public void SetAnimSpeed(float speed)
    {
        selfSkelAni.timeScale = speed;
    }

    public bool PlayAnim(AnimState state, AnimState nextState = default)// bool usingStanby = false
    {
        if (state == nowState || selfSkelAni == null || selfSkelAni.skeletonDataAsset == null) return false;
        if (selfSkelAni.state == null) return false;
        selfSkelAni.timeScale = 1f;
        selfSkelAni.state.SetAnimation(0, GetAnimationName(state), GetAnimationLoop(state));
        if (state == AnimState.ATTACK)
        {
            float weaponSpeed = GetComponentInParent<SoldierModel>().nowStatus.GetWeaponSpeed();
            selfSkelAni.timeScale = weaponSpeed;
        }
        else if (state == AnimState.MOVE_BEGIN || state == AnimState.MOVE_LOOP)
        {
            selfSkelAni.timeScale = moveSpeedAnimPercent;
        }
        if (nextState != AnimState.DEFAULT)
        {
            nextRestoreState = nextState;
        }
        nowState = state;
        return true;
    }

    public bool PlayAnim(string animName, AnimState nextState = default)// bool usingStanby = false
    {
        if (selfSkelAni == null || selfSkelAni.skeletonDataAsset == null) return false;
        if (selfSkelAni.state == null) return false;
        selfSkelAni.timeScale = 2f;
        selfSkelAni.state.SetAnimation(0, animName, false);
        if (nextState != AnimState.DEFAULT)
        {
            nextRestoreState = nextState;
        }
        nowState = AnimState.ATTACK;

        Spine.AnimationState.TrackEntryDelegate delegateAction = default;
        delegateAction = (g) =>
        {
            selfSkelAni.state.Complete -= delegateAction;
            nowState = AnimState.End;
            isAttacking = false;
        };
        selfSkelAni.state.Complete += delegateAction;
        return true;
    }

    public void OnReset()
    {
        isDying = false;
        mpb = null;
        timer = 0f;
        isAttacking = false;
        isMoving = false;
        nextRestoreState = default;
        nowState = AnimState.DEFAULT;
        selfSkelAni.Initialize(true, true);
        meshRenderer.SetPropertyBlock(SpineAtlasManager.Instance.BasicBlock);
    }
    public void BeenHit()
    {
        timer = 0.6f;
    }
    private void Update()
    {
        if (isDying)
        {
            if (timer > 0f)
            {
                timer -= Time.fixedDeltaTime;
                if (timer < 0f)
                {
                    timer = 0f;
                }
                if (timer > 0.5f)
                {
                    ChangeColor((1f - timer), Color.black, Color.white);
                }
                //else
                //{
                //    ChangeColor((0.5f - timer) * 2f, Color.clear, Color.black);
                //}
            }
        }
        else
        {
            if (timer > 0f)
            {
                timer -= Time.fixedDeltaTime;
                if (timer < 0f) timer = 0f;
                ChangeColor(timer / 0.6f, Color.red, Color.white);
            }
        }

        if (selfSkelAni == null || selfSkelAni.skeletonDataAsset == null || selfSkelAni.state == null || selfSkelAni.state.GetCurrent(0) == null) return;
        if (nextRestoreState == AnimState.End) return;
        //Debug.LogError($"{selfSkelAni.state.GetCurrent(0).AnimationEnd} {selfSkelAni.state.GetCurrent(0).AnimationTime}");
        if (selfSkelAni.state.GetCurrent(0).AnimationEnd <= selfSkelAni.state.GetCurrent(0).AnimationTime)
        {
            if (nextRestoreState != AnimState.DEFAULT)
            {
                PlayAnim(nextRestoreState);
                nextRestoreState = AnimState.DEFAULT;
            }
            else
            {
                PlayAnim(isMoving ? AnimState.MOVE_LOOP : AnimState.IDLE);
            }
        }


    }

    private void ChangeColor(float value, Color targetColor, Color basicColor)
    {
        if (meshRenderer == null) meshRenderer = selfSkelAni.GetComponent<MeshRenderer>();
        if (value <= 0f)// || meshRenderer.material.color.a <= 0.1f)
        {

            meshRenderer.SetPropertyBlock(SpineAtlasManager.Instance.BasicBlock);
        }
        else
        {
            if (mpb == null || mpb == SpineAtlasManager.Instance.BasicBlock)
            {
                mpb = new MaterialPropertyBlock();
                meshRenderer.GetPropertyBlock(mpb);
            }
            mpb.SetColor("_Color", targetColor);
            Color c = Color.Lerp(targetColor, basicColor, (1f - value));
            mpb.SetColor("_Color", c);
            meshRenderer.SetPropertyBlock(mpb);
        }
    }


    public virtual bool GetAnimationLoop(AnimState state)
    {
        switch (state)
        {
            case AnimState.IDLE:
            case AnimState.MOVE_LOOP:
            case AnimState.RUN_LOOP:
                return true;
        }
        return false;
    }
    public virtual string GetAnimationName(AnimState state)
    {
        return "UsingWrong";
    }
}
public enum AnimState
{
    End = -1,
    DEFAULT,
    IDLE,
    ATTACK,
    COMBAT,//被迫近战
    MOVE_BEGIN,
    MOVE_LOOP,
    MOVE_END,
    RUN_BEGIN,//动画
    RUN_LOOP,//动画
    RUN_END,//动画
    DIE,
}