using System;

public class StoryCharacter : StoryEntity
{
    //Attributs
    public float Health { get; private set; }
    public float Money { get; private set; }
    public string CharacterType { get; private set; }
    private float MinHealth = 0;
    private float MaxHealth = 100;
    private float MinMoney = 0;
    private float MaxMoney = 100;

    //Constructeur
    public StoryCharacter(string name = "generic character", float mainCharacterBond = 50, string characterType = "generic type", float health = 50, float money = 50)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;

        Health = health;
        Money = money;
        CharacterType = characterType;
    }
    //Methodes de validation des criteres de l'entite pour la prophetie
    public bool CharacterTypeIs(string type) { if (CharacterType == type) return true; else return false; }
    public bool HealthMin(float healthMin) { if (healthMin <= Health) return true; else return false; }
    public bool HealthMax(float healthMax) { if (healthMax >= Health) return true; else return false; }
    public bool MoneyMin(float moneyMin) { if (moneyMin <= Money) return true; else return false; }
    public bool MoneyMax(float moneyMax) { if (moneyMax >= Money) return true; else return false; }
    public bool IsBoss(float boolean)// Si boolean = 1, on demande si le ce perso est le boss du MC. Sinon, on demande si ce n'est pas le boss du MC. 
    {
        if (NarrationManager.Instance.MainCharacter.Boss != null && this == NarrationManager.Instance.MainCharacter.Boss)
        {
            return (boolean == 1) ? true : false;
        }
        else return (boolean == 1) ? false : true;
    }
    public bool IsColleague(float boolean)// Si boolean = 1, on demande si le ce perso est un collegue du MC. Sinon, on demande si ce n'est pas un collegue du MC. 
    {
        if (NarrationManager.Instance.MainCharacter.Colleagues != null && NarrationManager.Instance.MainCharacter.Colleagues.Contains(this))
        {
            return (boolean == 1) ? true : false;
        }
        else return (boolean == 1) ? false : true;
    }
    public bool IsLover(float boolean)// Si boolean = 1, on demande si le ce perso est l'amante  du MC. Sinon, on demande si ce n'est pas l'amante du MC. 
    {
        if (NarrationManager.Instance.MainCharacter.Lover != null && this == NarrationManager.Instance.MainCharacter.Lover)
        {
            return (boolean == 1) ? true : false;
        }
        else return (boolean == 1) ? false : true;
    }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void HealthPlus(float healthPlus) { Health = ValuePlus(Health, healthPlus, MinHealth, MaxHealth); }
    public void MoneyPlus(float moneyPlus) { Money = ValuePlus(Money, moneyPlus, MinMoney, MaxMoney); }
    public void CharacterTypeBecomes(string newCharacterType) { CharacterType = newCharacterType; }
    public void BecomesColleague(float boolean) { if (boolean == 1) { NarrationManager.Instance.MainCharacter.AddColleague(this); } else { NarrationManager.Instance.MainCharacter.RemoveColleague(this); } }
    public void BecomesBoss(float boolean) { NarrationManager.Instance.MainCharacter.BossBecomes(boolean == 1 ? this : null); }
    public void BecomesLover(float boolean) { NarrationManager.Instance.MainCharacter.LoverBecomes(boolean == 1 ? this : null); }
}
/*
public class CharacterValidator : StoryEntityValidator
{
    public float Health_min { get; private set; } public float Health_max { get; private set; }
    public float Money_min { get; private set; }    public float Money_max { get; private set; }
    public string CharacterType { get; private set; }
    /// <summary>
    /// les critères non renseignés seront ignorés
    /// </summary>
    /// <param name="mainCharacterBond_min"></param>
    /// <param name="mainCharacterBond_max"></param>
    /// <param name="health_min"></param>
    /// <param name="health_max"></param>
    /// <param name="money_min"></param>
    /// <param name="money_max"></param>
    /// <param name="characterType"></param>
    public CharacterValidator(float mainCharacterBond_min = -1, float mainCharacterBond_max = -1, 
        float health_min = -1, float health_max = -1, 
        float money_min = -1, float money_max = -1, 
        string characterType = "Unspecified")
    {
        MainCharacterBond_max = mainCharacterBond_max;
        MainCharacterBond_min = mainCharacterBond_min;
        Health_max = health_max;         Health_min = health_min;
        Money_max = money_max; Money_min = money_min;
        CharacterType = characterType;
        
    }
    /// <summary>
    /// Renvoie vrai seulement si le personnage fourni respecte tous les critères de l'instance
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public bool ValidateCharacter(Character character)
    {
        if (MainCharacterBond_min != -1 && MainCharacterBond_min > character.MainCharacterBond) return false;
        if (MainCharacterBond_max != -1 && character.MainCharacterBond > MainCharacterBond_max) return false;
        if (Health_min != -1 && Health_min > character.Health) return false;
        if (Health_max != -1 && character.Health > Health_max) return false;
        if (Money_min != -1 && Money_min > character.Money) return false;
        if (Money_max !=-1 &&   character.Money > Money_max)return false;
        if (CharacterType != "Unspecified" &&    CharacterType != character.CharacterType) return false;
        return true;
    }
}*/