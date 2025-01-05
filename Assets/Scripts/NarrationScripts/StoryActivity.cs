using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class StoryActivity : StoryEntity
{
    public string ActivityType { get; private set; }
    public string[] AssociatedPlaceTypes { get; private set; } 
    public StoryActivity(string name, float mainCharacterBond, string activityType, string[] associatedPlaces)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
        ActivityType = activityType;
        AssociatedPlaceTypes = associatedPlaces;
    }
    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool ActivityTypeIs(string type) { if (ActivityType == type) return true; else return false; }
    public bool AssociatedPlacesComprise(string placeType) { if (AssociatedPlaceTypes.Contains(placeType)) return true; else return false; }

}

