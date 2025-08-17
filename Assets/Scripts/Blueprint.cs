using UnityEngine;

public class Blueprint
{
    public string itemName;
    public string req1;
    public string req2;

    public int req1Amount;
    public int req2Amount;

    public int numOfRequirements;

    public int numberOfItemsToProduce;

    public Blueprint(string name, int producedItems, int amountOfItemsRequired, string requirement_1, int requirement_1_Amount, string requirement_2, int requirement_2_Amount)
    {
        itemName = name;
        numOfRequirements = amountOfItemsRequired;

        numberOfItemsToProduce = producedItems;

        req1 = requirement_1;
        req2 = requirement_2;
        req1Amount = requirement_1_Amount;
        req2Amount = requirement_2_Amount;
    }
}
