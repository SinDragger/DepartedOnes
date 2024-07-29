using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownTrigger : MonoBehaviour
{
    public GameObject targetGameObject;
    public Vector3 startLocalPos;
    public Vector3 endLocalPos;
    public Ease easeLine;
    public float moveTime;
    private void OnEnable()
    {
        targetGameObject.SetActive(true);
        targetGameObject.transform.localPosition = startLocalPos;
        transform.RotateAround(targetGameObject.transform.position,Vector3.up,Random.Range(-180,180));// * Time.deltaTime
        targetGameObject.transform.DOKill();
        targetGameObject.GetComponentInChildren<ParticleSystem>().Play();
        targetGameObject.transform.DOLocalMove(endLocalPos, moveTime).SetEase(easeLine);
    }
}
