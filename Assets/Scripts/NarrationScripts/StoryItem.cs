using UnityEngine;
using UnityEngine.TextCore.Text;


public class StoryItem : StoryEntity
{
    public string ItemType { get; private set; }
    public float State { get; private set; }
    private float MinState = 0;
    private float MaxState = 100;
    public StoryItem(string name, float mainCharacterBond, string itemType, float state)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;

        ItemType = itemType;
        State = state;
    }

    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool ItemTypeIs(string type) { if (ItemType == type) return true; else return false; }
    public bool StateMin(float stateMin) { if (stateMin <= State) return true; else return false; }
    public bool StateMax(float stateMax) { if (stateMax >= State) return true; else return false; }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void StatePlus(float statePlus) { State = ValuePlus(State, statePlus, MinState, MaxState); }
    public void ItemTypeBecomes(string newPlaceType) { ItemType = newPlaceType; }
}
