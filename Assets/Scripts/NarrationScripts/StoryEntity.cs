using UnityEngine;
using UnityEngine.TextCore.Text;

public class StoryEntity
{
    //Attributs
    public string Name { get; protected set; }
    public float MainCharacterBond { get; protected set; }
    private float MinBond = 0;
    private float MaxBond = 100;

    // Constructeur
    public StoryEntity(string name="generic name", float mainCharacterBond = 50)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
    }
    //Methodes de validation des criteres de l'enttie pour le prophetie
    public bool NameIs(string name) { if (name == Name) return true; else return false; }
    public bool BondMin(float bondMin) { if (bondMin <= MainCharacterBond) return true; else return false; }
    public bool BondMax(float bondMax) { if (bondMax >= MainCharacterBond) return true; else return false; }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void NewName(string name) {  Name = name; }
    public void BondPlus(float bondPlus) { MainCharacterBond = ValuePlus( MainCharacterBond, bondPlus,MinBond,MaxBond); }
    public void BondReverse(float boolean) { MainCharacterBond = MaxBond - MainCharacterBond; }

    protected float ValuePlus( float baseValue, float valuePlus, float minValue, float maxValue)
    {
        if (baseValue + valuePlus > maxValue)        { return  maxValue;    }
        else if (baseValue + valuePlus < minValue) { return minValue;    }
        else { return baseValue + valuePlus; }
    }
}
/*
public class StoryEntityValidator
{
    public float MainCharacterBond_min {  get; protected set; }
    public float MainCharacterBond_max {  get; protected set; }
}*/