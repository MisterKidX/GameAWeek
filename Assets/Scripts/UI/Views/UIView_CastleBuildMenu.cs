using System;
using UnityEngine;
using UnityEngine.UI;

public class UIView_CastleBuildMenu : MonoBehaviour
{
    [SerializeField]
    RectTransform[] _contentRows;
    [SerializeField]
    UIView_BuildingCard p_buildingCard;
    [SerializeField]
    Button _exit;

    public CastleInstance _castleInstance;

    public void Init(CastleInstance instance)
    {
        _castleInstance = instance;
        _exit.onClick.RemoveAllListeners();
        _exit.onClick.AddListener(() => Destroy(gameObject));

        Show();
    }

    private void Show()
    {
        EmptyRows();

        foreach (var kvp in _castleInstance.BuiltBuildings)
        {
            BuildingModel model = kvp.Key;
            BuildingInstance instance = kvp.Value;

            bool isBuilt = instance != null;
            bool hasUpgrades = model.Upgrade != null;

            var buildingCard = Instantiate(p_buildingCard);
            buildingCard.Init(model);
            ParentCardToRow(buildingCard, model.Order);

            if (!isBuilt && _castleInstance.BuiltThisTurn)
                buildingCard.SetState(BuildingStateView.CantBuild);
            else if (isBuilt && !hasUpgrades)
                buildingCard.SetState(BuildingStateView.Built);
            else if (!isBuilt && _castleInstance.HasEnoughResourcesForBuilding(model))
                buildingCard.SetState(BuildingStateView.CanBuild);
            else
                buildingCard.SetState(BuildingStateView.NotEnoughResources);
        }
    }

    private void ParentCardToRow(UIView_BuildingCard buildingCard, int order)
    {
        int index = (int)Mathf.Floor(order / 10f);
        buildingCard.transform.SetParent(_contentRows[index]);
    }

    private void EmptyRows()
    {
        foreach (var row in _contentRows)
        {
            foreach (RectTransform transform in row)
            {
                Destroy(transform.gameObject);
            }
        }
    }
}
