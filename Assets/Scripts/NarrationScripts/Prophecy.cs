using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

//Prophecy est la classe instantiée pour chaque prophétie en kit
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
        //{ throw new Exception("La  prophétie suivante a été créée avec un nombre incorrect de story entities, validators ou updators : "+sentence); }

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

        Debug.Log("nombre d'entités à trouver pour cette prophétie = "+ProphecyStoryEntityTypes.Length);
        for (int i=0; i< ProphecyStoryEntityTypes.Length; i++) {
            currentStoryEntity = NarrationManager.Instance.GetFittingEntity(ProphecyStoryEntityTypes[i], ProphecyValidators[i]);
            //Debug.Log("entite trouvee pour le critere n°"+i+" est "+currentStoryEntity.Name);
            if (currentStoryEntity == null)
            {
                throw new Exception("Aucune entite n'a été trouvée pour le critère "+i+" de la prophétie : "+SentenceToFill);
            }
            else
            {
                ProphecyStoryEntities.Add(currentStoryEntity);
                ProphecyStoryEntitiesNames.Add(currentStoryEntity.Name);
                
            }
        }
        Debug.Log("array de toutes les entites trouvées : "+ string.Join(", ",ProphecyStoryEntitiesNames.ToArray()));
        string fullProchecy = string.Format(SentenceToFill, ProphecyStoryEntitiesNames.ToArray());
        return new LegoProphecy(fullProchecy, ProphecyStoryEntities.ToArray());  
    }

}
//LegoProphecy est une struct-conteneur, dont le seul but est de rassembler en un seul objet la phrase à afficher au joueur complétée et les storyEntities qui devront être affectées par la prophétie 
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
