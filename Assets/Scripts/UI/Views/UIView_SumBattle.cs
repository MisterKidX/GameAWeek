using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_SumBattle : MonoBehaviour
{
    [SerializeField]
    Image _attackerView;
    [SerializeField]
    Image _defenderView;
    [SerializeField]
    TMP_Text _attackerName;
    [SerializeField]
    TMP_Text _defenderName;
    [SerializeField]
    TMP_Text _attackerOutcome;
    [SerializeField]
    TMP_Text _defenderOutcome;

    [SerializeField]
    Image _outcome;
    [SerializeField]
    TMP_Text _description;
    [SerializeField]
    UIView_CasualtiesStrip _attackerCasualties;
    [SerializeField]
    UIView_CasualtiesStrip _defenderCasualties;

    [SerializeField]
    Button _done;

    public void Init(bool attackerWon, Action done, Sprite attackerView, Sprite defenderView, string attackerName, string defenderName,
        (UnitModel, int)[] attackerCasualties, (UnitModel, int)[] defenderCasualties)
    {
        _attackerView.sprite = attackerView;
        _defenderView.sprite = defenderView;

        _attackerName.text = attackerName;
        _defenderName.text = defenderName;

        _attackerOutcome.text = attackerWon ? "Victorious" : "Defeated";
        _defenderOutcome.text = attackerWon ? "Defeated" : "Victorious";

        _description.text = attackerWon ? "A glorious victory!" : "A shameful defeat";

        _attackerCasualties.Init(attackerCasualties);
        _defenderCasualties.Init(defenderCasualties);

        _done.onClick.RemoveAllListeners();
        _done.onClick.AddListener(() => Destroy(gameObject));
        _done.onClick.AddListener(() => done?.Invoke());
    }
}
