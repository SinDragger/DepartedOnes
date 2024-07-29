using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色控制系统
/// </summary>
public class CharacterManager : Singleton<CharacterManager>
{
    Dictionary<string, Character> characterDic = new Dictionary<string, Character>();
    List<Character> characterList = new List<Character>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="unitIdName">模型</param>
    /// <param name="characterIdName">角色ID</param>
    /// <param name="speciesType">角色类型</param>
    /// <param name="belong">角色势力</param>
    public void DeployCharacterToScene(Vector3 pos, string characterIdName, string unitIdName, int belong = 1)
    {
        var refData = UnitControlManager.instance.DeployUnit(pos, unitIdName, belong);
        Character newCharacter = new Character();
        newCharacter.idName = characterIdName;
        newCharacter.refData = refData;
        characterList.Add(newCharacter);
    }

    public Character GetCharacter((string, string, string) castMember)
    {
        if (characterList.Find((c) => c.idName == castMember.Item1) == null)
        {
            if (int.TryParse(castMember.Item3, out int belong))
                DeployCharacterToScene(new Vector3(5,0,0), castMember.Item1, castMember.Item2, belong);
            else
                DeployCharacterToScene(Vector3.zero, castMember.Item1, castMember.Item1);
        }
        return characterList.Find((c) => c.idName == castMember.Item1);
    }

    public void RegistCharacter(SoldierStatus soldier, string characterIdName)
    {
        Character newCharacter = new Character();
        newCharacter.idName = characterIdName;
        newCharacter.refData = soldier;
        characterList.Add(newCharacter);
    }
}
