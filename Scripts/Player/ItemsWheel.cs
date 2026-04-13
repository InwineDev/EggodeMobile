using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GridBrushBase;

public class ItemsWheel : NetworkBehaviour
{
    [SerializeField] private userSettings us;
    [SerializeField] private List<Image> items = new List<Image>();
    private int smesh;
    [SerializeField] private Transform wheel;
    private bool isAnimating = false;
    private int rotationDirection = 1;
    private Tween animationStraha;
    private int archiveItemId;
    private void OnEnable()
    {
        if(us)
            us.OnChangeItem += ChangeItem;
    }

    private void OnDisable()
    {
        if (us)
            us.OnChangeItem -= ChangeItem;
    }

    void ChangeItem(int itemId)
    {
        if (isAnimating) return;
        isAnimating = true;
        animationStraha.Kill(true);
        StopCoroutine(ChangeItemCoroutine());
        if (archiveItemId > itemId)
        {
            rotationDirection = -1;
        } else
        {
            rotationDirection = 1;
        }
        archiveItemId = itemId;
        smesh = (smesh + rotationDirection + 4) % 4;

        int itemCount = us.idannie.Count;

        List<TipikalPredmet> localItems = new List<TipikalPredmet>();

        foreach (var item1 in us.idannie)
        {
            if (item1.itemdat.amount > 0) localItems.Add(item1);
        }

        items[(0 + smesh) % 4].sprite = us.idannie[(itemId - 2 + itemCount) % itemCount].texture;
        items[(1 + smesh) % 4].sprite = us.idannie[(itemId - 1 + itemCount) % itemCount].texture;
        items[(2 + smesh) % 4].sprite = us.idannie[itemId % itemCount].texture;
        items[(3 + smesh) % 4].sprite = us.idannie[(itemId + 1) % itemCount].texture;

        wheel.DOMoveX(10, 0.2f)
            .From(0)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                animationStraha = wheel.DORotate(
                        new Vector3(0, 0, wheel.eulerAngles.z + 90 * rotationDirection),
                        0.15f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        isAnimating = false;
                        StartCoroutine(ChangeItemCoroutine());
                    });
            });
    }

    IEnumerator ChangeItemCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isAnimating)
        {
            wheel.DOMoveX(-1000, 0.4f)
                .SetEase(Ease.InOutBack);
        }
    }
}
