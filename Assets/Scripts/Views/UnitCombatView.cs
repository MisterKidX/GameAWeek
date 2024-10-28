using TMPro;
using UnityEngine;

public class UnitCombatView : MonoBehaviour
{
    [SerializeField]
    Animator _animator;
    [SerializeField]
    TMP_Text _amount;
    [SerializeField]
    GameObject RootView;
    [SerializeField]
    GameObject Selection;

    UnitInstance _instance;
    public void Init(UnitInstance unitInstance, bool flip)
    {
        _instance = unitInstance;
        if (flip)
            RootView.transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Update()
    {
        transform.position = _instance.CombatWorldPosition;

        if (_instance.Selected)
            Selection.gameObject.SetActive(true);
        else
            Selection.gameObject.SetActive(false);

        _amount.text = _instance.Amount.ToString();
    }
}
