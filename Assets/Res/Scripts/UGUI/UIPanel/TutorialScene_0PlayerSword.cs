using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene_0PlayerSword : MonoBehaviour
{
    public AnimationCurve throwCurve;
    Vector3 originalV3;
    // Start is called before the first frame update
    void Start()
    {
        originalV3 = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator CurveCoroutine()
    {
        Time.timeScale = 1.25f;
        yield return new WaitForSeconds(0.5f);
        transform.DORotate(new Vector3(0, 0, -120), 0.25f).SetEase(Ease.Linear);
        float time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime / 0.5f;
            if (time > 0.5f) time = 0.5f;
            transform.DOMove(originalV3 + new Vector3(8 * time, 5.5f * throwCurve.Evaluate(time), 0), Time.deltaTime / 0.5f);

            yield return null;

        }
        Time.timeScale = 1f;

    }

    public void StartThePlot()
    {

        StartCoroutine(CurveCoroutine());

    }

    public void CollSword()
    {
        //transform.DOMove(new Vector3(148.8f,3.65f,150),0.5f);
        transform.DOMove(new Vector3(-1f, 3.6f, 0), 0.5f);
        transform.DORotate(new Vector3(0, 0, 60), 0.5f).SetEase(Ease.Linear);
        StartCoroutine(ClosGB());
    }
    IEnumerator ClosGB()
    {

        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
    }

}
