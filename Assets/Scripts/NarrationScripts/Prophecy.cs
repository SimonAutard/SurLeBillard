using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

//Prophecy est la classe instanti�e pour chaque proph�tie en kit
public class Prophecy
{
    // Attributs
    public string SentenceToFill { get; private set; }
    public Type[] ProphecyStoryEntityTypes {get; private set;}
    public List<StoryEntity> ProphecyStoryEntities {  get; private set;}
    public List<(string, object)[]> ProphecyValidators { get; private set; }
    public List<(string, object)[]> ProphecyUpdators { get; private set; }

    // Constructeur obligatoire
    public Prophecy(string sentence, Type[] storyEntityTypes , List<(string, object)[]> validators, List<(string, object)[]> storyStateUpdators)

    {
        SentenceToFill = sentence;
        ProphecyStoryEntityTypes = storyEntityTypes;
        ProphecyValidators = validators;
        ProphecyUpdators = storyStateUpdators;
        //if (ProphecyStoryEntityTypes.Length != ProphecyValidators.Count || ProphecyStoryEntityTypes.Length != ProphecyUpdators.Count) 
        //{ throw new Exception("La  proph�tie suivante a �t� cr��e avec un nombre incorrect de story entities, validators ou updators : "+sentence); }

    } 
    /// <summary>
    /// Renvoie le legoProphecy une fois que la prophetie a ete completee
    /// </summary>
    /// <returns></returns>
    public LegoProphecy GetCompletedProphecy()
    {
        List<StoryEntity> ProphecyStoryEntities = new List<StoryEntity>();
        List<string> ProphecyStoryEntitiesNames = new List<string>();
        StoryEntity currentStoryEntity;

        Debug.Log("nombre d'entit�s � trouver pour cette proph�tie = "+ProphecyStoryEntityTypes.Length);
        for (int i=0; i< ProphecyStoryEntityTypes.Length; i++) {
            currentStoryEntity = NarrationManager.Instance.GetFittingEntity(ProphecyStoryEntityTypes[i], ProphecyValidators[i]);
            //Debug.Log("entite trouvee pour le critere n�"+i+" est "+currentStoryEntity.Name);
            if (currentStoryEntity == null)
            {
                throw new Exception("Aucune entite n'a �t� trouv�e pour le crit�re "+i+" de la proph�tie : "+SentenceToFill);
            }
            else
            {
                ProphecyStoryEntities.Add(currentStoryEntity);
                ProphecyStoryEntitiesNames.Add(currentStoryEntity.Name);
                
            }
        }
        Debug.Log("array de toutes les entites trouv�es : "+ string.Join(", ",ProphecyStoryEntitiesNames.ToArray()));
        string fullProchecy = string.Format(SentenceToFill, ProphecyStoryEntitiesNames.ToArray());
        return new LegoProphecy(fullProchecy, ProphecyStoryEntities.ToArray());  
    }

}
//LegoProphecy est une struct-conteneur, dont le seul but est de rassembler en un seul objet la phrase � afficher au joueur compl�t�e et les storyEntities qui devront �tre affect�es par la proph�tie 
public struct LegoProphecy
{
    public string Sentence;
    public StoryEntity[] StoryEntities;

    public LegoProphecy(string sentence, StoryEntity[] storyEntities)
    {
        Sentence = sentence;
        StoryEntities = storyEntities;
    }
}
