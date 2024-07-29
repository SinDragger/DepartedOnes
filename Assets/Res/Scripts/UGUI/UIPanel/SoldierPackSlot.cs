using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierPackSlot : MonoBehaviour
{
    public SoldierPack soldierPack;
    public Text titleText;
    public Text costText;
    public ParticleSystem particle;
    public Button clickAbleArea;
    public TroopEntity troopEntity;
    public void Init(TroopEntity troopEntity, System.Action callback)
    {
        this.troopEntity = troopEntity;
        soldierPack.Init(troopEntity);
        titleText.text = troopEntity.originData.name;
        costText.text = troopEntity.originData.cost.ToString();
        clickAbleArea.SetBtnEvent(() =>
        {
            callback?.Invoke();
        });
        particle.Stop();
        soldierPack.SetColor(Color.white);
    }

    public void OnSelect(bool result)
    {
        if (result)
        {
            particle.Play();
            soldierPack.SetColor(Color.white);
        }
        else
        {
            particle.Stop();
            soldierPack.SetColor(Color.gray);
        }
    }
}
