using UnityEngine;
using UnityEngine.TextCore.Text;


public class StoryItem : StoryEntity
{
    public string ItemType { get; private set; }
    public float State { get; private set; }
    public StoryItem(string name, float mainCharacterBond, string itemType, float state)
    {
        Name = name;
        MainCharacterBond = mainCharacterBond;
        ItemType = itemType;
        State = state;
    }
}
