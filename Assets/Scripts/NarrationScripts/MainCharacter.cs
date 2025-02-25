using System.Collections.Generic;
public class MainCharacter : StoryEntity
{
    // Attributs
    public StoryPlace LivingPlace { get; private set; }
    public StoryActivity Job { get; private set; }
    public StoryCharacter Boss { get; private set; }
    public List<StoryCharacter> Colleagues { get; private set; }
    public StoryCharacter Lover { get; private set; }
    public float Health { get; private set; }
    public float Money { get; private set; }
    private float MinMoney = 0;
    private float MaxMoney = 100;
    private float MinHealth = 0;
    private float MaxHealth = 100;

    public MainCharacter(string name, StoryPlace livingPlace, StoryActivity job, StoryCharacter boss, List<StoryCharacter> colleagues, StoryCharacter lover, float health, float money)
    {
        Name = name;
        Health = health;
        Money = money;
        LivingPlace = livingPlace;
        Job = job;
        Colleagues = colleagues;
        Boss = boss;
        Lover = lover;
    }

    //Methodes de changement des attributs de l'tentite pour leffet de la prophetie
    public void HealthPlus(float healthPlus) { Health = ValuePlus(Health, healthPlus, MinHealth, MaxHealth); }
    public void MoneyPlus(float moneyPlus) { Money = ValuePlus(Money, moneyPlus, MinMoney, MaxMoney); }
    public void LivingPlaceBecomes(StoryPlace newPlace) { LivingPlace = newPlace; }
    public void JobBecomes(StoryActivity newJob)
    {
        Job = newJob;
        if(newJob != null) {
            StoryCharacter newBoss = NarrationManager.Instance.ReplaceBoss(newJob, Boss);
            BossBecomes(newBoss);
        }
        else { BossBecomes(null); }
    }
    public void BossBecomes(StoryCharacter newBoss) { Boss = newBoss; AddColleague(newBoss); }
    public void AddColleague(StoryCharacter newColleague) { if (Colleagues.Contains(newColleague)) { return; } else { Colleagues.Add(newColleague); } }
    public void RemoveColleague(StoryCharacter newColleague) { if (Colleagues.Contains(newColleague)) { Colleagues.Remove(newColleague); } else { return; } }
    public void LoverBecomes(StoryCharacter newLover) { Lover = newLover; }
}
