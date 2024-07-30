using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int score;
    public List<int> cardStates;
    public int rows;
    public int cols;
    public List<int> cardIDs;
}
