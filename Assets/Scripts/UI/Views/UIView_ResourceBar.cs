using System;
using System.Collections.Generic;
using UnityEngine;

public class UIView_ResourceBar : MonoBehaviour
{
    public RectTransform Container;
    public UIView_ResourceBox p_ResourceBox;

    internal void Init(List<ResourceInstance> resources)
    {
        ClearContainer();

        foreach (ResourceInstance resource in resources)
        {
            var box = Instantiate(p_ResourceBox, Container);
            box.Init(resource);
        }
    }

    private void ClearContainer()
    {
        foreach (Transform child in Container.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
