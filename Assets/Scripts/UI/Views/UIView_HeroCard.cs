using UnityEngine;
using UnityEngine.UI;

public class UIView_HeroCard : MonoBehaviour
{
    public HeroInstance Instance { get; private set; }
    public Image Portrait;
    public UIView_Bar MovementBar;
    public UIView_Bar ManaBar;

    public void Init(HeroInstance instance)
    {
        Instance = instance;
        Portrait.sprite = Instance.Model.Portrait;
        Portrait.gameObject.SetActive(true);
    }

    public void Disable()
    {
        Instance = null;
        Portrait.sprite = null;
        Portrait.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Instance == null) return;

        MovementBar.Slider.value = (float)Instance.RemainingMovementPoints / GameConfig.Configuration.MovementBarMaxMovementPoints;
    }
}
