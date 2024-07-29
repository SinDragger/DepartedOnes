using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene_0 : MonoBehaviour, ITutorialScene
{
    int nowState = 0;
    public GameObject effect;
    public ParticleSystem[] particles;
    public Transform[] positions;
    public Light pointLight;
    public Transform beam;
    public ParticleSystem burst;
    // Start is called before the first frame update
    TutorialPanel tipPanel;
    public void PlayTutorial()
    {
        UIManager.Instance.ShowUI("TutorialPanel", (ui) =>
        {
            tipPanel = (ui as TutorialPanel);
        });
        nowState = PlayerPrefs.GetInt("NowTutorialState", 0);
        if (nowState == 0)
        {
            CoroutineManager.StartFrameDelayedCoroutine(() =>
            {
                ARPGManager.Instance.SetCurrentGeneralControl();
                ARPGManager.Instance.Active();
                ARPGManager.Instance.currentGeneralControl.m_SoldierModel.SetToFloat(true);
                ARPGManager.Instance.inputModule.Deactive();
                ARPGManager.Instance.tabTrigger.Deactive();
                //ARPGManager.Instance.HideARPGCharacterUI();
                ARPGManager.Instance.HideARPGSkillListUI();
            });
            //进行显示
            effect.gameObject.SetActive(true);
            tipPanel.SetShow(0);
        }
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => ARPGManager.Instance.currentGeneralControl != null);
    }

    // Update is called once per frame
    void Update()
    {
        if (nowState == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            nowState++;
            tipPanel.EndShow();
            PlayerPrefs.SetInt("NowTutorialState", nowState);
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.maxParticles = 0;
            }
            UIManager.Instance.ShowUI("TutorialPanel", (ui) =>
            {
                (ui as TutorialPanel).EndShow();
            });
            CoroutineManager.DelayedCoroutine(1.6f, () => { nowState++; });
            CoroutineManager.DelayedCoroutine(1f, () =>
            {
                effect.gameObject.SetActive(false);
                burst.Play();
                ARPGManager.Instance.currentGeneralControl.m_SoldierModel.ResetFloat();
                LeapSlashImpact.OnceTargetPos = ARPGManager.Instance.currentGeneralControl.m_SoldierModel.transform.position + new Vector3(5, 0, -3);
                ARPGManager.Instance.currentGeneralControl.characterSkillManager.OnSkillDown2();
                ARPGManager.Instance.inputModule.Active();
            });
        }
        else if (nowState == 2)
        {
            nowState++;
            CameraControl.Instance.ChangeCameraToARPG(ARPGManager.Instance.currentGeneralControl);
            //ARPGManager.Instance.ShowARPGCharacterUI();
            tipPanel.SetShow(1);
        }
        if (nowState > 0)
        {
            pointLight.intensity = Mathf.Lerp(pointLight.intensity, 0, 3f * Time.deltaTime);
            float target = 0.3f;
            beam.localScale = Vector3.Lerp(beam.localScale, new Vector3(beam.localScale.x, beam.localScale.y, target), 3f * Time.deltaTime);
        }
    }
    public Vector3 GetPosition(string target)
    {
        if (target == "StartTutorialPosition")
        {
            return positions[0].position;
        }
        if (target == "StartTutorialFaceTo")
        {
            return Vector3.right;
        }
        return default;
    }

}
public interface ITutorialScene
{
    Vector3 GetPosition(string target);
    void PlayTutorial();
}