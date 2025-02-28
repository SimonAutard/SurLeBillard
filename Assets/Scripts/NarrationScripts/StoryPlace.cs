public class StoryPlace : StoryEntity
{
    public string PlaceType { get; private set; } = "Nature";
    public float State { get; private set; } = 50;
    public bool Shore { get; private set; } = true;
    private float MinState = 0;
    private float MaxState = 100;

    //Constructeur naturel
    public StoryPlace(string name, float mainCharacterBond, string placeType, float state, bool shore) : base(name, mainCharacterBond)
    {
        PlaceType = placeType;
        State = state;
        Shore = shore;
    }

    //Constructeur par validateur
    public StoryPlace((string, object)[] validators) : base(validators)
    {
        Name = "somewhere";
        foreach (var validator in validators)
        {
            string methodName = validator.Item1; //nom de la fonction de check de lentite
            object methodValue = validator.Item2; // valeur associee
            if (methodName == "PlaceTypeIs")
            {
                PlaceType = (string)methodValue;
                if (PlaceType == "Nature") Name = "somewhere in the wild";
                if (PlaceType == "City") Name = "a city";
            }
            if (methodName == "StateMin") { State = (float)methodValue; }
            if (methodName == "StateMax") { State = (float)methodValue; }
            if (methodName == "IsLivingPlace") { if ((float)methodValue == 1) { BecomesLivingPlace(1); Name = "where he lives"; }; }
            if (methodName == "IsShore") { if ((float)methodValue == 1) { Shore = true; Name = "the sea"; } }
        }
    }
    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool PlaceTypeIs(string type) { if (PlaceType == type) return true; else return false; }
    public bool StateMin(float stateMin) { if (stateMin <= State) return true; else return false; }
    public bool StateMax(float stateMax) { if (stateMax >= State) return true; else return false; }
    public bool IsLivingPlace(float boolean)// Si boolean = 1, on demande si le cet ebdriut est l'habitat du MC. Sinon, on demande si ce n'est pas l'habitat du MC. 
    {
        if (this == NarrationManager.Instance.MainCharacter.LivingPlace)
        {
            return (boolean == 1) ? true : false;
        }
        else return (boolean == 1) ? false : true;
    }

    public bool IsShore(float boolean)
    {
        if (Shore) return (boolean == 1) ? true : false;
        else return (boolean == 1) ? false : true;
    }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void StatePlus(float statePlus) { State = ValuePlus(State, statePlus, MinState, MaxState); }
    public void PlaceTypeBecomes(string newPlaceType) { PlaceType = newPlaceType; }
    public void BecomesLivingPlace(float boolean) { NarrationManager.Instance.MainCharacter.LivingPlaceBecomes(boolean == 1 ? this : null); }
}
