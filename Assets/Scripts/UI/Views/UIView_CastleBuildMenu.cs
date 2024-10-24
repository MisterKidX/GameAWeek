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
    UIView_BuildingBuildConfirmationModal p_buildingModal;
    [SerializeField]
    Button _exit;

    public CastleInstance _castleInstance;

    Action _exitAction;

    public void Init(CastleInstance instance, Action exit)
    {
        _castleInstance = instance;
        _exitAction = exit;

        _exit.onClick.RemoveAllListeners();
        _exit.onClick.AddListener(Exit);

        Draw();
    }

    private void Exit()
    {
        Destroy(gameObject);
        _exitAction?.Invoke();
    }

    private void Draw()
    {
        EmptyRows();

        foreach (var kvp in _castleInstance.BuiltBuildings)
        {
            BuildingModel model = kvp.Key;
            BuildingInstance instance = kvp.Value;

            bool isBuilt = instance != null;
            bool hasUpgrades = model.Upgrade != null;

            var buildingCard = Instantiate(p_buildingCard);
            ParentCardToRow(buildingCard, model.Order);

            if (_castleInstance.BuiltThisTurn)
            {
                if (!isBuilt)
                {
                    buildingCard.SetState(BuildingStateView.CantBuild);
                    buildingCard.Init(model, () => ShowBuildingModal(model, false));
                }
                else if (isBuilt && hasUpgrades)
                {
                    buildingCard.SetState(BuildingStateView.CantBuild);
                    buildingCard.Init(model, () => ShowBuildingModal(model, false));
                }
                else
                {
                    buildingCard.SetState(BuildingStateView.Built);
                    buildingCard.Init(model, () => ShowBuildingModal(model, false));
                }

                continue;
            }

            if (isBuilt && !hasUpgrades)
            {
                buildingCard.SetState(BuildingStateView.Built);
                buildingCard.Init(model, () => ShowBuildingModal(model, false));
            }
            else if (isBuilt && hasUpgrades && _castleInstance.HasEnoughResourcesForBuilding(model.Upgrade))
            {
                buildingCard.SetState(BuildingStateView.CanBuild);
                buildingCard.Init(model.Upgrade, () => ShowBuildingModal(model, true));
            }
            else if (!isBuilt && _castleInstance.HasEnoughResourcesForBuilding(model))
            {
                buildingCard.SetState(BuildingStateView.CanBuild);
                buildingCard.Init(model, () => ShowBuildingModal(model, true));
            }
            else
            {
                buildingCard.SetState(BuildingStateView.NotEnoughResources);
                buildingCard.Init(model, () => ShowBuildingModal(model, false));
            }
        }
    }

    private void ShowBuildingModal(BuildingModel model, bool canBebuilt)
    {
        var modal = Instantiate(p_buildingModal);
        modal.Init(model, canBebuilt, ConfirmBuild);
    }

    private void ConfirmBuild(BuildingModel model)
    {
        _castleInstance.BuildOrUpgrade(model);
        Exit();
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
