using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MoveArrowControl : MonoBehaviour
{
    public float yMax = 2;

    public float originY = 1.5f;
    /// <summary>
    /// 动画曲线
    /// </summary>
    public AnimationCurve curve;
    CommandUnit commandUnit;
    // Start is called before the first frame update
    private void OnDisable()
    {
        // commandUnit = null;
        StopCoroutine(RecycleCoroutine());
        transform.DOKill();
    }

    public void StartActive(CommandUnit commandUnit, Vector3 position)
    {
        //在这里向Control 中注册command和自己  如果对应的键值对中有(command,gb) 那么需要先移除老的 值(gb) 
        transform.DOKill();

        this.commandUnit = commandUnit;
        transform.position = position;
        transform.DOMove(transform.position + new Vector3(0, yMax, 0), originY).SetEase(curve);

        StartCoroutine(RecycleCoroutine());
    }

    private void Update()
    {
        if (commandUnit != null)
        {
            if (Vector3.Distance(commandUnit.lastPosition, transform.position) < 2)
            {
                Recycle();
            }

        }
    }
    IEnumerator RecycleCoroutine()
    {
        yield return new WaitForSeconds(originY);
        Recycle();

    }
    /// <summary>
    /// 与Inspector窗口中的reset一样 不要点错咯
    /// </summary>
    public void Recycle()
    {
        gameObject.SetActive(false);
        //GameObjectPoolManager.Instance.Recycle(gameObject, DataBaseManager.Instance.configMap["MoveArrow"]);
    }
}
