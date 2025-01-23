using UnityEngine;
using UnityEngine.TextCore.Text;
using System;

public class StoryActivity : StoryEntity
{
    public string ActivityType { get; private set; }
    public string[] AssociatedPlaceTypes { get; private set; }
    public float Popularity { get; private set; }
    public StoryActivity(string name = "generic activity", float mainCharacterBond = 50, string activityType = "generic type", string[] associatedPlaces = null, float popularity = 50)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
        ActivityType = activityType;
        AssociatedPlaceTypes = associatedPlaces ?? new string[] { "generic place" };
        Popularity = popularity;
    }
    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool ActivityTypeIs(string type) { if (ActivityType == type) return true; else return false; }
    public bool AssociatedPlacesComprise(string placeType) { if (Array.IndexOf(AssociatedPlaceTypes,placeType)>-1) return true; else return false; }

}

