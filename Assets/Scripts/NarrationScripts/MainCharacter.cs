using UnityEngine;
using UnityEngine.TextCore.Text;


public class MainCharacter : StoryEntity
{
    // Attributs
    public StoryPlace LivingPlace { get; private set; }
    public StoryActivity Job { get; private set; }
    public StoryCharacter Boss { get; private set; }
    public StoryCharacter Lover { get; private set; }
    public float Health { get; private set; }
    public float Money { get; private set; }
    private float MinMoney = 0;
    private float MaxMoney = 100;
    private float MinHealth = 0;
    private float MaxHealth = 100;

    public MainCharacter(string name, StoryPlace livingPlace, StoryActivity job, StoryCharacter boss, StoryCharacter lover, float health, float money)
    { 
        Name = name;

        LivingPlace = livingPlace;
        Job = job;
        Boss = boss;
        Health = health;
        Money = money;
        Lover = lover;
    }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void HealthPlus(float healthPlus) { Health = ValuePlus(Health, healthPlus, MinHealth, MaxHealth); }
    public void MoneyPlus(float moneyPlus) { Money = ValuePlus(Money, moneyPlus, MinMoney, MaxMoney); }
    public void LivingPlaceBecomes(StoryPlace newPlace) { LivingPlace = newPlace; }
    public void JobBecomes(StoryActivity newJob) { Job = newJob; }
    public void BossBecomes(StoryCharacter newBoss) { Boss = newBoss; }
    public void LoverBecomes(StoryCharacter newLover) { Lover = newLover; }
}
