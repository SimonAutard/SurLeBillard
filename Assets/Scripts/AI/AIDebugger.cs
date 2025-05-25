using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;

public class AIDebugger : MonoBehaviour
{
    public void UpdateAIDifficulty()
    {
        float fumbleChance = this.GetComponent<Slider>().value;
        AIManager.Instance.SetAIDifficulty(fumbleChance);
    }
}
