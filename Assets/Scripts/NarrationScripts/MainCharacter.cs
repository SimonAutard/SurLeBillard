using UnityEngine;
using UnityEngine.TextCore.Text;


public class MainCharacter : StoryEntity
{
    // Attributs
    public Place LivingPlace { get; private set; }
    public StoryActivity Job { get; private set; }
    public Character Boss { get; private set; }
    public float Health { get; private set; }
    public float Money { get; private set; }

    public MainCharacter(Place livingPlace, StoryActivity job, Character boss, float health, float money)
    { 
        LivingPlace = livingPlace;
        Job = job;
        Boss = boss;
        Health = health;
        Money = money;
    }
}
