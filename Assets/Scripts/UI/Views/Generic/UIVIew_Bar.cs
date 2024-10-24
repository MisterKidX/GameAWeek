using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UIView_Bar : MonoBehaviour
{
    public Slider Slider;

    private void Awake()
    {
        Slider = GetComponent<Slider>();
    }
}
