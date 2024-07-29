using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMiniMap : MonoBehaviour
{
    //public RawImage image;
    public Camera nowCamera;
    public Transform mapBorder;
    public Transform pointsParent;
    List<BattleMiniMapControlFlag> controlFlagList = new List<BattleMiniMapControlFlag>();
    const float cameraSize = 125f;
    public float showScale = 1f;
    public float pointScale = 1f;
    public RectTransform[] cameraLines;
    float lineWidth = 2f;

    public bool needUpdateThisFrame;

    public void Trigger()
    {
#if UNITY_EDITOR
        lastshowScale = showScale;
#endif
        nowCamera.orthographicSize = cameraSize / showScale;
        nowCamera.Render();
    }

    public void RegistMiniMapCommandFlag(CommandUnit commander)
    {
        var flag = GameObjectPoolManager.Instance.Spawn("Prefab/BattleMiniMapControlFlag", pointsParent).GetComponent<BattleMiniMapControlFlag>();
        flag.gameObject.SetActive(true);
        flag.sourceCommand = commander;
        flag.UpdatePos(showScale);
        flag.Init(commander.belong);
        controlFlagList.Add(flag);
    }

    public void OnUpdate()
    {
        foreach (var flag in controlFlagList)
        {
            flag.pointScale = pointScale;
            flag.UpdatePos(showScale);
        }
        if (Input.GetKeyDown(KeyCode.M) || needUpdateThisFrame)
        {
            Trigger();
            needUpdateThisFrame = false;
        }
        var points = GetCameraRaycast();
        LineFix(cameraLines[0], points[0], points[1]);
        LineFix(cameraLines[1], points[1], points[2]);
        LineFix(cameraLines[2], points[2], points[3]);
        LineFix(cameraLines[3], points[3], points[0]);
    }

    private void LineFix(RectTransform line, Vector2 pointA, Vector2 pointB)
    {
        line.anchoredPosition = (pointA + pointB) / 2 * showScale;
        line.sizeDelta = new Vector2(Vector2.Distance(pointA, pointB) * showScale, lineWidth);
        float angle = Vector2.Angle(Vector2.right, pointA - pointB);
        if ((pointA - pointB).y < 0) angle = -angle;
        line.localEulerAngles = new Vector3(0, 0, angle);
    }

    private RaycastHit hit;
    private Ray ray;
    public Vector2[] GetCameraRaycast()
    {
        Vector2[] results = new Vector2[4];
        var camera = CameraControl.Instance.mainCamera;
        //左下
        ray = camera.ScreenPointToRay(Vector2.zero);
        if (Physics.Raycast(ray, out hit, float.MaxValue, InputManager.MapMask))
        {
            results[0] = new Vector2(hit.point.x, hit.point.z);
        }
        //左上
        ray = camera.ScreenPointToRay(new Vector2(0, Screen.height));
        if (Physics.Raycast(ray, out hit, float.MaxValue, InputManager.MapMask))
        {
            results[1] = new Vector2(hit.point.x, hit.point.z);
        }
        //右上
        ray = camera.ScreenPointToRay(new Vector2(Screen.width, Screen.height));
        if (Physics.Raycast(ray, out hit, float.MaxValue, InputManager.MapMask))
        {
            results[2] = new Vector2(hit.point.x, hit.point.z);
        }
        //右下
        ray = camera.ScreenPointToRay(new Vector2(Screen.width, 0));
        if (Physics.Raycast(ray, out hit,float.MaxValue, InputManager.MapMask))
        {
            results[3] = new Vector2(hit.point.x, hit.point.z);
        }
        return results;
    }

    /// <summary>
    /// 回收所有内容
    /// </summary>
    public void OnReset()
    {
        foreach (var flag in controlFlagList)
        {
            GameObjectPoolManager.Instance.Recycle(flag.gameObject, "Prefab/BattleMiniMapControlFlag");
        }
    }
#if UNITY_EDITOR
    float lastshowScale;
    private void OnValidate()
    {
        //if (!Application.isPlaying) return;
        //if (lastshowScale != showScale)
        //{
        //    lastshowScale = showScale;
        //    Trigger();
        //}
    }
#endif
}
