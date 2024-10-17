using UnityEngine;
using UnityEngine.UI;

public class UIView_HeroCard : MonoBehaviour
{
    public HeroInstance Instance { get; private set; }
    public Image Portrait;
    public UIVIew_Bar MovementBar;
    public UIVIew_Bar ManaBar;

    public void Init(HeroInstance instance)
    {
        Instance = instance;
        Portrait.sprite = instance.Model.Portrait;
        MovementBar.Slider.value = (float)instance.RemainingMovementPoints / GameConfig.MovementBarMaxMovementPoints;
        Portrait.gameObject.SetActive(true);
    }
}
