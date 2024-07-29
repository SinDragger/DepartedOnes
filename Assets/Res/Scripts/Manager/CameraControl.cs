using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraControl : MonoSingleton_AutoCreate<CameraControl>
{
    public float xMax;
    //实际是y
    public float yMax;

    public float[] maxChange;
    public float speed = 10f;
    public GameObject rotateCenter;
    public GameObject moveAssist;
    public Camera mainCamera;
    public int cameraLevel;
    int cameraLevelMin = 0;
    public int cameraLevelMax => cameraLevels.Length;
    Coroutine frontPull;
    InputModule inputModule = new CameraManager_InputModule();
    public CameraStatus status = new CameraStatus();
    public Vector3 CameraForward { get { return mainCamera.transform.forward; } }

    CameraLevel[] cameraLevels;
    public CameraLevel[] battleCameraLevels;
    public CameraLevel[] mapCameraLevels;
    public CameraLevel[] arpgCameraLevels;
    public bool activeLimited;
    public Func<bool> unlimitedCheck;
    public static Vector3 right;

    //TODO Arpg控制摄像头依旧需要覆写跟随玩家内容 以及 根据鼠标方向偏移摄像头

    protected override void Init()
    {
        if (hasInit) return;
        hasInit = true;
        //设置成SectorBlock状态
        if (activeLimited)
        {

        }
        else
        {
            SetCameraToMap();
        }
        //注册进去
        inputModule.Init();
        inputModule.RegistInputModuleToSet("Normal");
        inputModule.Active();
        transform.localEulerAngles = new Vector3(45, 0, 0);
    }

    public void BlockInput() { inputModule.Deactive(); }
    public void RestoreInput() { inputModule.Active(); }

    /// <summary>
    /// 重写到CameraStatus
    /// </summary>
    public ARPGControl target; // 玩家的 Transform 组件


    public float smoothSpeed = 10f; // 平滑移动的速度

    Vector3 targetMoveToZero = Vector3.zero;
    bool targetMoveToIsFirstZero = false;
    Vector3 desiredPosition;
    public void SetDesiredPosition(Vector3 desiredPosition)
    {

        this.desiredPosition = GetClamp(desiredPosition);
        status = new CS_LookAtTroop(this.desiredPosition);
        //rotateCenter.transform.position = this.desiredPosition;
    }
    public void SetTargetMoveTo(Vector2 targetMoveTo)
    {
        if (targetMoveTo == Vector2.zero)
        {

            if (targetMoveToIsFirstZero)
            {

                targetMoveToZero = (transform.parent.position - target.transform.position);
                targetMoveToIsFirstZero = false;
            }

            desiredPosition = target.transform.position + targetMoveToZero;

        }
        else
        {
            Vector3 cameraToward = CameraControl.Instance.CameraToward(targetMoveTo).normalized;
            targetMoveToIsFirstZero = true;
            desiredPosition = target.transform.position + cameraToward * 3;
        }

    }

    private void LateUpdate()
    {
        //CameraForward = mainCamera.transform.forward;

        //状态的执行方法 通过改变CS来改变谁相机的运行模式
        status.Execute();


        //if (!hasLockTarget)
        //{
        //    if (target != null)
        //    {
        //        desiredPosition = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        //    }
        //    transform.parent.position = Vector3.Lerp(transform.parent.position, desiredPosition, smoothSpeed * Time.deltaTime);
        //    float distance = Vector3.Distance(transform.parent.position, desiredPosition);
        //    if (distance < 0.05f)
        //    {
        //        if (target != null)
        //        {
        //            transform.parent.position = target.transform.position;
        //        }
        //        hasLockTarget = true;
        //        desiredPosition = default;
        //    }
        //    else if (needImmediateLockTarget)
        //    {
        //        needImmediateLockTarget = false;
        //        if (target != null)
        //        {
        //            transform.parent.position = target.transform.position;
        //        }
        //        hasLockTarget = true;
        //        desiredPosition = default;
        //    }
        //}
        //else
        //{
        //    if (target != null)
        //    {
        //        SetTargetMoveTo(target.targetMoveTo);
        //        if (needImmediateLockTarget)
        //        {
        //            needImmediateLockTarget = false;
        //            transform.parent.position = target.transform.position;
        //        }
        //        else
        //        {
        //            transform.parent.position += Fix(desiredPosition);
        //        }
        //        return;
        //    }
        //    if (desiredPosition != default)
        //    {
        //        transform.parent.position = Vector3.Lerp(transform.parent.position, desiredPosition, smoothSpeed * Time.deltaTime);
        //        float distance = Vector3.Distance(transform.parent.position, desiredPosition);
        //        if (distance < 0.05f)
        //        {
        //            desiredPosition = default;
        //        }

        //     if (unlockTargetTime > 0f)
        //        {
        //            unlockTargetTime -= Time.deltaTime;
        //            if (unlockTargetTime <= 0f)
        //            {
        //                desiredPosition = default;
        //            }
        //        }
        //    }
        //}
    }

    public Vector3 Fix()
    {
        //TODO 摄像机抖动目前不好解决 ARPG模式直接改成跟随玩家transfrom 
        return target.transform.position;
        return desiredPosition;
        float distance = Mathf.Round(Vector3.Distance(transform.parent.position, desiredPosition) * 100) / 100;
        if (target.isItAFastMove)
        {
            return (desiredPosition - transform.parent.position).normalized * distance * 3f * Time.deltaTime;
        }
        else
        {
            if (Mathf.Abs(distance - 2.5f) < 0.15f)
            {
                //为什么 因为 人物的移动速度是5 2*2.5=5 在两者接近相等时需要对其进行修正 防止抖动
                distance = 2.5f;
            }
            // Debug.Log((desiredPosition - transform.parent.position).normalized * (distance - maxDistance) * 2f * Time.deltaTime);
            return (desiredPosition - transform.parent.position).normalized * distance * 2 * Time.deltaTime;
        }
    }
    public void SetCameraToBattle()
    {
        cameraLevels = battleCameraLevels;
        SetToTargetLevel(cameraLevels.Length - 1);
        CameraReset();
    }
    public void SetCameraToMap()
    {
        cameraLevels = mapCameraLevels;
        SetToTargetLevel(1);
        // CameraReset();
    }
    public void LookAtV3(Vector3 target)
    {
        status = new CS_LookAtPos(target);
    }
    public void LookAtV3Immediate(Vector3 target)
    {
        status = new CS_LookAtPosImmediate(target);
    }
    public void LookAtTransfrom(Transform transform)
    {
        status = new CS_LookAtTransfrom(transform);
    }

    public void LookAtTransfrom(Transform transform, Vector3 deviation)
    {
        status = new CS_LookAtTransfrom(transform, deviation);
    }

    public void SetCameraToArpg()
    {
        cameraLevels = arpgCameraLevels;
        targetMoveToZero = Vector3.zero;
        SetToTargetLevel(0);
        //CameraReset();

    }

    //将相机设置为跟随模式
    public void ChangeCameraToARPG(ARPGControl target)
    {
        //inputModule.Deactive();
        //设置跟随玩家
        status = new CS_FollowARPGControl(target);

        SetCameraToArpg();

        //

        this.target = target;

    }

    public void ChangeCameraToBattle()
    {
        //inputModule.Active();
        //停止跟随玩家
        SetCameraToMap();
        status = new CameraStatus();
        target = null;
    }


    private void OnEnable()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        right = transform.right;
        Init();
        //cameraLevel = 4;
        SetCameraToMap();
        frontPull = StartCoroutine(CameraPull(cameraLevels[cameraLevel].moveInLevel, 0f + 0.2f * cameraLevel, GetFlagDelta()));
        //ControlFlag.canvasAlpha = 0f + 0.2f * cameraLevel;
        //ControlFlag.upDelta = 150f - 10 * cameraLevel;
    }

    float GetFlagDelta()
    {
        return 100f - 4 * cameraLevel;
    }

    public event Action forceSynAction;
    void Update()
    {
        if (forceSynCamera)
        {
            forceSynAction?.Invoke();
        }
    }

    int rotateDegree = 0;
    public bool forceSynCamera = false;
    public void CameraSizeChange(bool isCloser)
    {
        if (forceSynCamera) return;
        if (isCloser)
        {
            cameraLevel = Mathf.Clamp(cameraLevel - 1, cameraLevelMin, cameraLevelMax - 1);
        }
        else
        {
            cameraLevel = Mathf.Clamp(cameraLevel + 1, cameraLevelMin, cameraLevelMax - 1);
        }
        if (frontPull != null) StopCoroutine(frontPull);
        frontPull = StartCoroutine(CameraPull(cameraLevels[cameraLevel].moveInLevel, 0f + 0.2f * cameraLevel, GetFlagDelta()));
    }

    public void SetToTargetLevel(int level)
    {
        cameraLevel = level;
        if (cameraLevel < cameraLevelMin)
        {
            cameraLevel = cameraLevelMin;
        }
        if (frontPull != null) StopCoroutine(frontPull);
        frontPull = StartCoroutine(CameraPull(cameraLevels[cameraLevel].moveInLevel, 0f + 0.2f * cameraLevel, GetFlagDelta(), 1f));
    }

    /// <summary>
    /// 镜头的远近
    /// </summary>
    IEnumerator CameraPull(float target, float canvasAlpha, float flagDleta, float speed = 0.3f)
    {
        float from = 0f;
        float to = 1f;
        Vector3 start = mainCamera.transform.parent.eulerAngles;
        if (start.x > 180) start.x = start.x - 360;
        while (!Mathf.Approximately(from, to))
        {
            from = Mathf.Lerp(from, to, speed);
            right = transform.right;
            //mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, target, 0.3f);
            if (cameraLevel >= cameraLevels.Length) cameraLevel = cameraLevels.Length - 1;
            mainCamera.transform.parent.eulerAngles = Vector3.Lerp(start, new Vector3(cameraLevels[cameraLevel].cameraRotation, mainCamera.transform.parent.eulerAngles.y, start.z), from);
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, new Vector3(0f, target, -target), speed);
            ControlFlag.canvasAlpha = Mathf.Lerp(ControlFlag.canvasAlpha, canvasAlpha, speed);
            ControlFlag.upDelta = Mathf.Lerp(ControlFlag.upDelta, flagDleta, speed);
            CameraMove(Vector2.zero);
            yield return 0;
        }
        right = transform.right;
        //mainCamera.fieldOfView = target;
        mainCamera.transform.localPosition = new Vector3(0f, target, -target);
        CameraMove(Vector2.zero);
        ControlFlag.canvasAlpha = canvasAlpha;
        ControlFlag.upDelta = flagDleta;
    }
    public void CameraReset()
    {
        if (frontPull != null) StopCoroutine(frontPull);
        rotateDegree = 0;
        rotateCenter.transform.localEulerAngles = new Vector3(0, 0, rotateCenter.transform.eulerAngles.z);
        right = transform.right;
    }
    public void CameraRotate(bool forward)
    {
        if (forceSynCamera) return;
        rotateDegree += forward ? 1 : -1;
        if (rotateDegree < 0) rotateDegree = 3;
        if (rotateDegree > 3) rotateDegree = 0;
        forceSynCamera = true;
        float z = rotateCenter.transform.eulerAngles.z;
        rotateCenter.transform.DOKill();
        rotateCenter.transform.DORotate(new Vector3(rotateCenter.transform.eulerAngles.x, rotateDegree * 90, z), 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            forceSynCamera = false;
            right = transform.right;
        }).SetUpdate(true);
        CameraMove(Vector2.zero);
        right = transform.right;
    }
    public Vector3 CameraToward(Vector2 inputDelta)
    {
        return moveAssist.transform.right * inputDelta.x + moveAssist.transform.forward * inputDelta.y;
    }
    public void CameraMove(Vector2 moveDelta)//,bool isHorizon
    {
        //先进行摄像机角度的变换
        Vector3 c = moveAssist.transform.right * moveDelta.x + moveAssist.transform.forward * moveDelta.y;
        moveDelta = new Vector2(c.x, c.z);
        Vector3 selfPosition = rotateCenter.transform.position;
        selfPosition += new Vector3(moveDelta.x, 0, moveDelta.y) * (speed * (cameraLevel + 2));//镜头近的情况下speed慢速
        //关联角度
        //float length = cameraLevels[cameraLevel].fieldOfViewLevel;
        //float limited = length / Screen.height * Screen.width;
        float xCameraSize = 0;// length;
        float yCameraSize = 0;// length;
        //TODO:一个角度计算还有问题
        if (unlimitedCheck != null)
        {
            if (unlimitedCheck.Invoke())
            {
                rotateCenter.transform.position = selfPosition;
                return;
            }
        }
        if (activeLimited)
        {
            selfPosition.x = Mathf.Clamp(selfPosition.x, -GetXMax() + xCameraSize, GetXMax() - xCameraSize);
            selfPosition.z = Mathf.Clamp(selfPosition.z, -GetYMax() + yCameraSize, GetYMax() - yCameraSize);
        }
        else//Map界面使用底图大小的范围
        {
            selfPosition.x = Mathf.Clamp(selfPosition.x, 0, 128);
            selfPosition.z = Mathf.Clamp(selfPosition.z, 0, 128);
        }
        rotateCenter.transform.position = selfPosition;
    }

    Vector3 GetClamp(Vector3 selfPosition)
    {
        float xCameraSize = 0;// length;
        float yCameraSize = 0;// length;
        if (activeLimited)
        {
            selfPosition.x = Mathf.Clamp(selfPosition.x, -GetXMax() + xCameraSize, GetXMax() - xCameraSize);
            selfPosition.z = Mathf.Clamp(selfPosition.z, -GetYMax() + yCameraSize, GetYMax() - yCameraSize);
        }
        else//Map界面使用底图大小的范围
        {
            selfPosition.x = Mathf.Clamp(selfPosition.x, 0, 128);
            selfPosition.z = Mathf.Clamp(selfPosition.z, 0, 128);
        }
        return selfPosition;
    }
    float GetXMax()
    {
        return xMax + maxChange[cameraLevel];
    }
    float GetYMax()
    {
        return yMax + maxChange[cameraLevel];
    }
    public void SetCameraPos(float x, float y)
    {
        rotateCenter.transform.position = new Vector3(x, 0, y);
    }
    public bool recordFOV = false;
    float replaceFOV;
    public void CloseUp(float deltaTime = 0f)
    {
        if (!recordFOV)
        {
            replaceFOV = mainCamera.fieldOfView;
            recordFOV = true;
        }
        StartCoroutine(ChangeFOV(deltaTime));
    }
    IEnumerator ChangeFOV(float deltaTime)
    {
        if (deltaTime == 0f)
        {
            mainCamera.fieldOfView = 5;
            yield break;
        }
        while (Mathf.Abs(mainCamera.fieldOfView - 5) > 0.1f)
        {
            mainCamera.fieldOfView -= 0.2f;
            yield return null;
        }
        mainCamera.fieldOfView = 5;
    }
    public void RestoreFOV()
    {
        //mainCamera.fieldOfView = replaceFOV;
        StartCoroutine(ReplaceFOV());
        recordFOV = false;
    }
    IEnumerator ReplaceFOV()
    {
        while (Mathf.Abs(mainCamera.fieldOfView - replaceFOV) > 0.1f)
        {
            mainCamera.fieldOfView += 0.1f;
            yield return null;
        }
        mainCamera.fieldOfView = replaceFOV;
    }


    public void ChangeCameraFOV(float view)
    {
        if (!recordFOV)
        {
            replaceFOV = mainCamera.fieldOfView;
            recordFOV = true;
        }
        mainCamera.fieldOfView = view;

    }

}
[System.Serializable]
public struct CameraLevel
{
    public float fieldOfViewLevel;
    public float moveInLevel;
    public float cameraRotation;
}


