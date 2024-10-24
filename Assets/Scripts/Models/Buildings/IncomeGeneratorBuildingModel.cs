using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Income Generator", menuName = "Models/Buildings/Income Generator")]
public class IncomeGeneratorBuildingModel : BuildingModel
{
    public ResourceModel Resource;
    public int Income;
}