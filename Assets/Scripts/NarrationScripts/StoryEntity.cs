using UnityEngine;
using UnityEngine.TextCore.Text;

public class StoryEntity
{
    //Attributs
    public string Name { get; protected set; }
    public float MainCharacterBond { get; protected set; }

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
    public void BondPlus(float bondPlus) { MainCharacterBond += bondPlus; }
}
/*
public class StoryEntityValidator
{
    public float MainCharacterBond_min {  get; protected set; }
    public float MainCharacterBond_max {  get; protected set; }
}*/