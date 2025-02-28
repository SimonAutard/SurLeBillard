using System;

public class StoryCharacter : StoryEntity
{
    //Attributs
    public float Health { get; private set; } = 50;
    public float Money { get; private set; } = 50;
    public string CharacterType { get; private set; } = "Friend";
    private float MinHealth = 0;
    private float MaxHealth = 100;
    private float MinMoney = 0;
    private float MaxMoney = 100;

    //Constructeur
    public StoryCharacter(string name, float mainCharacterBond, string characterType, float health, float money) : base(name, mainCharacterBond)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;

        Health = health;
        Money = money;
        CharacterType = characterType;
    }

    // Constructeur par validateur
    public StoryCharacter((string, object)[] validators) : base(validators)
    {
        Name = "someone";
        foreach (var validator in validators)
        {
            string methodName = validator.Item1; //nom de la fonction de check de lentite
            object methodValue = validator.Item2; // valeur associee
            if (methodName == "CharacterTypeIs") { 
                CharacterType = (string)methodValue;
                if (CharacterType == "Friend") Name = "an acquaintance";
                if (CharacterType == "Family") Name = "a relative";
                if (CharacterType == "Crush") Name = "his crush";
            }
            if (methodName == "HealthMin") { Health = (float)methodValue; }
            if (methodName == "HealthMax") { Health = (float)methodValue; }
            if (methodName == "MoneyMin") { Money = (float)methodValue; }
            if (methodName == "MoneyMax") { Money = (float)methodValue; }
            if (methodName == "IsBoss") { if ((float)methodValue == 1) { BecomesBoss(1); Name = "his boss"; } }
            if (methodName == "IsColleague") { if ((float)methodValue == 1) { BecomesColleague(1); Name = "one of his colleagues"; } }
            if (methodName == "IsLover") { if ((float)methodValue == 1) { BecomesLover(1); Name = "his lover"; } }
        }
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
