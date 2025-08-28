using System.Collections.Generic;

[System.Serializable]
public class EnviromentData
{
    public List<string> pickedUpItems;

    public EnviromentData(List<string> _pickedUpItems)
    {
        pickedUpItems = _pickedUpItems;
    }
}