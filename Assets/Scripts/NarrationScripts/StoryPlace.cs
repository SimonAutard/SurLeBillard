using UnityEngine;
using UnityEngine.TextCore.Text;


public class StoryPlace : StoryEntity
{
    public string PlaceType { get; private set; }
    public float State { get; private set; }
    private float MinState = 0;
    private float MaxState = 100;

    public StoryPlace (string name="generic place", float mainCharacterBond=50, string placeType = "generic type", float state = 50)
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
    public bool IsLivingPlace(float useless) { if (this == NarrationManager.Instance.MainCharacter.LivingPlace) return true; else return false; }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void StatePlus(float statePlus) { State = ValuePlus(State, statePlus, MinState, MaxState); }
    public void PlaceTypeBecomes(string newPlaceType) { PlaceType = newPlaceType; }
}
