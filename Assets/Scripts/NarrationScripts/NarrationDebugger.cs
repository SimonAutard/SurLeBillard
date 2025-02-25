using System.Collections.Generic;
using UnityEngine;

public class NarrationDebugger : MonoBehaviour
{
    List<Prophecy>[,] positiveProphecyMasterTable;
    List<Prophecy>[,] negativeProphecyMasterTable;
    MainCharacter mainCharacter;
    public void ScanForBugs(List<Prophecy>[,] table1, List<Prophecy>[,] table2, MainCharacter chara)
    {
        positiveProphecyMasterTable = table1;
        negativeProphecyMasterTable = table2;
        for (int i = 0;i<= positiveProphecyMasterTable.GetUpperBound(0); i++)
        {
            for (int j = 0; j < i; j++)
            {
                Debug.Log("current index ="+i+ " "+j);
                for (int k = 0; k<positiveProphecyMasterTable[i,j].Count; k++)
                {
                    LegoProphecy legoProphecy = NarrationManager.Instance.WriteStoryAndMakeItTrue(i, j, true,k,false);
                }
            }
        }
        for (int i = 0; i <= negativeProphecyMasterTable.GetUpperBound(0); i++)
        {
            for (int j = 0; j < i; j++)
            {
                Debug.Log("current index =" + i + " " + j);
                for (int k = 0; k < negativeProphecyMasterTable[i, j].Count; k++)
                {
                    LegoProphecy legoProphecy = NarrationManager.Instance.WriteStoryAndMakeItTrue(i, j, false, k, false);
                }
            }
        }

    }

    /// <summary>
    /// En mode debug, affiche letat de toutes les entites impliquees dans une prophetie
    /// </summary>
    /// <param name="legoProphecy"></param>
    /// <param name="stateName">Indique si on est avant ou apres lexecution du validator</param>
    public void DebugLogEntitiesState(LegoProphecy legoProphecy, string stateName)
    {
        foreach (StoryEntity entity in legoProphecy.StoryEntities)
        {
            if (entity is StoryCharacter storyCharacter)
            {
                Debug.Log("etat " + stateName + " de " + storyCharacter.Name +
                    ": bond = " + storyCharacter.MainCharacterBond +
                    "; Money = " + storyCharacter.Money +
                    "; health = " + storyCharacter.Health);

            }
            else if (entity is StoryPlace storyPlace)
            {
                Debug.Log("etat " + stateName + " de " + storyPlace.Name +
                    ": bond = " + storyPlace.MainCharacterBond +
                    "; type = " + storyPlace.PlaceType +
                    "; state = " + storyPlace.State);

            }
            else if (entity is StoryItem storyItem)
            {
                Debug.Log("etat " + stateName + " de " + storyItem.Name +
                    ": bond = " + storyItem.MainCharacterBond +
                    "; type = " + storyItem.ItemType +
                    "; state = " + storyItem.State);

            }
            else if (entity is StoryActivity storyActivity)
            {
                Debug.Log("etat " + stateName + " de " + storyActivity.Name +
                    " : bond = " + storyActivity.MainCharacterBond +
                    "; type = " + storyActivity.ActivityType +
                    "; popularity = " + storyActivity.Popularity);

            }
        }
    }
}
