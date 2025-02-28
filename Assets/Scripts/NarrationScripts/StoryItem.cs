public class StoryItem : StoryEntity
{
    public string ItemType { get; private set; } = "Tool";
    public float State { get; private set; } = 50;
    private float MinState = 0;
    private float MaxState = 100;

    //Constructeur naturel
    public StoryItem(string name , float mainCharacterBond, string itemType, float state) : base(name, mainCharacterBond)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;

        ItemType = itemType;
        State = state;
    }

    //Constructeur par validateur
    public StoryItem((string, object)[] validators) : base(validators)
    {
        Name = "something";
        foreach (var validator in validators)
        {
            string methodName = validator.Item1; //nom de la fonction de check de lentite
            object methodValue = validator.Item2; // valeur associee
            if (methodName == "ItemTypeIs") { 
                ItemType = (string)methodValue;
                if (ItemType == "Artefact") Name = "a curio";
                if (ItemType == "Tool") Name = "some tools";
                if (ItemType == "Food") Name = "some food";
                if (ItemType == "Cloth") Name = "comes clothes";
                if (ItemType == "Concept") Name = "some notions";
                if (ItemType == "Event") Name = "an event";
            }
            if (methodName == "StateMin") { State = (float)methodValue; }
            if (methodName == "StateMax") { State = (float)methodValue; }
        }
    }

    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool ItemTypeIs(string type) { if (ItemType == type) return true; else return false; }
    public bool ItemTypeIsNot(string type) { if (ItemType != type) return true; else return false; }
    public bool StateMin(float stateMin) { if (stateMin <= State) return true; else return false; }
    public bool StateMax(float stateMax) { if (stateMax >= State) return true; else return false; }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void StatePlus(float statePlus) { State = ValuePlus(State, statePlus, MinState, MaxState); }
    public void ItemTypeBecomes(string newItemType) { ItemType = newItemType; }
}
