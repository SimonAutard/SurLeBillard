using UnityEngine;
using UnityEngine.TextCore.Text;


public class Place : StoryEntity
{
    public string PlaceType { get; private set; }
    public float State { get; private set; }

    public Place (string name="generic place", float mainCharacterBond=50, string placeType = "generic type", float state = 50)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
        
        PlaceType = placeType;
        State = state;
    }

    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool PlaceTypeIs(string type) { if (PlaceType == type) return true; else return false; }
    public bool StateMin(float stateMin) { if (stateMin <= State) return true; else return false; }
    public bool StateMax(float stateMax) { if (stateMax >= State) return true; else return false; }
}
