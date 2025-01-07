using UnityEngine;
using UnityEngine.TextCore.Text;


public class StoryActivity : StoryEntity
{
    public string ActivityType { get; private set; }
    public Place[] AssociatedPlaces { get; private set; } 
    public StoryActivity(string name, float mainCharacterBond, string activityType, Place[] associatedPlaces)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
        ActivityType = activityType;
        AssociatedPlaces = associatedPlaces;
    }
}

