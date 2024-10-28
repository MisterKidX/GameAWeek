using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIView_UnitBox : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField]
    Image _portrait;
    [SerializeField]
    TMP_Text _amount;

    Action<UIView_UnitBox, UIView_UnitBox> _endDrag;
    GameObject _copy = null;
    int _amountInternal;

    public bool Active { get; private set; }

    public UIView_UnitStrip Manager { get; private set; }

    public void Init(Sprite portrait, int amount, UIView_UnitStrip manager, Action<UIView_UnitBox, UIView_UnitBox> endDrag)
    {
        Manager = manager;

        Init(portrait, amount, endDrag);
    }

    private void Init(Sprite portrait, int amount, Action<UIView_UnitBox, UIView_UnitBox> endDrag)
    {
        _amountInternal = amount;
        _portrait.sprite = portrait;
        _endDrag = endDrag;

        if (amount > 0)
        {
            _amount.text = amount.ToString();
            _amount.gameObject.SetActive(true);
        }
        else
            _amount.gameObject.SetActive(false);

        Active = true;
    }

    public void Deactivate()
    {
        _portrait.sprite = null;
        _amount.gameObject.SetActive(false);
        Active = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _copy = Instantiate(this.gameObject, transform.parent);
        var le = _copy.AddComponent<LayoutElement>();
        le.ignoreLayout = true;
        var canvasGroup = _copy.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0.3f;
        canvasGroup.blocksRaycasts = false;
        var canvas = _copy.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1000;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _copy.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Destroy(_copy);
        var first = eventData.hovered.FirstOrDefault(go => go.TryGetComponent(out UIView_UnitBox _));
        if (first == null) return;
        var box = first.GetComponent<UIView_UnitBox>();
        _endDrag?.Invoke(this, box);
    }
}