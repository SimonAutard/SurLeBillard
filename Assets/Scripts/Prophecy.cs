using UnityEngine;

public class LegoProphecy
{
    public string sentence = "full prophecy";
    public GameEntity[] gameEntityArray;
}
public class Prophecy
{
    // Attributs
    public int EntitiesNumber { get; private set; }
    public string sentence { get; private set; }

    // Constructeur obligatoire
    public Prophecy(int entityNumber)
    {
        EntitiesNumber = entityNumber;
    }
    public LegoProphecy GetCompletedProphecy()
    {
        return new LegoProphecy();  
    }

    private void GetFittingEntities()
    {

    }
    public void ChangeGameWorld(GameEntity[] gameEntityList)
    {
        //Contient des refs aux entités utilisées, des refs à leurs attributs 
    }
}
