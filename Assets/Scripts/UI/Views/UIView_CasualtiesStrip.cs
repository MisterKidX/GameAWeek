using UnityEngine;

public class UIView_CasualtiesStrip : MonoBehaviour
{
    [SerializeField]
    UIView_ImageText p_unitBox;
    [SerializeField]
    RectTransform _container;

    public void Init((UnitModel, int)[] casulaties)
    {
        foreach (var unit in casulaties)
        {
            if (unit.Item1 == null)
                continue;
            if (unit.Item2 == 0)
                continue;

            var inst = Instantiate(p_unitBox, _container);
            inst.Init(unit.Item1.Portrait, unit.Item2.ToString());
        }
    }
}
