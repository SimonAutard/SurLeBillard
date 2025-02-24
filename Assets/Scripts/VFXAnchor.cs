using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class VFXAnchor : MonoBehaviour
{
    VisualEffect effect;
    public void TriggerVFXProcess()
    {
         effect = GetComponent<VisualEffect>();
        //Recuperation de la durée du fx
        float maxLT = effect.GetFloat("MaxLifetime");
        effect.enabled = true;
        StartCoroutine(PlayAndStopVFX(maxLT));
    }

    private IEnumerator PlayAndStopVFX(float maxLT)
    {
        effect.Reinit();
        effect.Play();
        yield return new WaitForSeconds(maxLT);
        effect.Stop();
        Destroy(gameObject);
    }

}
