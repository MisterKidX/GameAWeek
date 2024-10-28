using System;
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
        transform.position = _instance.CombatWorldPosition;
        if (flip)
            RootView.transform.localScale = new Vector3(-1, 1, 1);

        _instance.OnAttack += _instance_OnAttack;
        _instance.OnDefend += _instance_OnDefend;
        _instance.OnDie += _instance_OnDie;
    }

    private void _instance_OnDie(UnitInstance instance)
    {
        _animator.SetTrigger("4_Death");
        Selection.gameObject.SetActive(false);
        this.enabled = false;
    }

    private void _instance_OnDefend()
    {
        _animator.SetTrigger("3_Damaged");
    }

    private void _instance_OnAttack()
    {
        _animator.SetTrigger("2_Attack");
    }

    private void Update()
    {
        MovePosition();

        if (_instance.Selected)
            Selection.gameObject.SetActive(true);
        else
            Selection.gameObject.SetActive(false);

        _amount.text = _instance.Amount.ToString();
    }

    Vector3 _vel;
    private void MovePosition()
    {
        var magnitude = (transform.position - _instance.CombatWorldPosition).magnitude;
        if (magnitude > 0.05f)
        {
            _animator.SetBool("1_Move", true);
            //transform.position = Vector3.Slerp(
            //    transform.position, _instance.CombatWorldPosition, _instance.Model.HexAnimationSpeed);
            transform.position = Vector3.SmoothDamp(transform.position, _instance.CombatWorldPosition,
                ref _vel, _instance.Model.HexAnimationSpeed - 0.02f);
        }
        else
        {
            _animator.SetBool("1_Move", false);
            transform.position = _instance.CombatWorldPosition;
        }
    }
}
