public class StoryEntity
{
    //Attributs
    public string Name { get; protected set; } = "generic entity";
    public float MainCharacterBond { get; protected set; } = 50;
    
    private float MinBond = 0;
    private float MaxBond = 100;

    // Constructeur naturel
    public StoryEntity(string name, float mainCharacterBond)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
    }

    // Constructeur par validateur
    public StoryEntity((string, object)[] validators)
    {
        foreach (var validator in validators) {
            string methodName = validator.Item1; //nom de la fonction de check de lentite
            object methodValue = validator.Item2; // valeur associee
            if(methodName == "NameIs") { Name = (string)methodValue; }
            if(methodName == "BondMin") {MainCharacterBond = (float)methodValue; }
            if(methodName == "BondMax") {MainCharacterBond = (float)methodValue; }
        }
    }
    //Methodes de validation des criteres de l'enttie pour le prophetie
    public bool NameIs(string name) { if (name == Name) return true; else return false; }
    public bool NameIsNot(string name) { if (name != Name) return true; else return false; }
    public bool BondMin(float bondMin) { if (bondMin <= MainCharacterBond) return true; else return false; }
    public bool BondMax(float bondMax) { if (bondMax >= MainCharacterBond) return true; else return false; }

    //Methodes de changement des attributs de l'entite pour leffet de la prophetie
    public void NewName(string name) { Name = name; }
    public void BondPlus(float bondPlus) { MainCharacterBond = ValuePlus(MainCharacterBond, bondPlus, MinBond, MaxBond); }
    public void BondReverse(float boolean) { MainCharacterBond = MaxBond - MainCharacterBond; }

    protected float ValuePlus(float baseValue, float valuePlus, float minValue, float maxValue)
    {
        if (baseValue + valuePlus > maxValue) { return maxValue; }
        else if (baseValue + valuePlus < minValue) { return minValue; }
        else { return baseValue + valuePlus; }
    }
}