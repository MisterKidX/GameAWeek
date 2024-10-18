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
        Portrait.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Instance == null) return;

        Portrait.sprite = Instance.Model.Portrait;
        MovementBar.Slider.value = (float)Instance.RemainingMovementPoints / GameConfig.MovementBarMaxMovementPoints;
    }
}
