using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GridBox : UIBehaviour
{
    private RectTransform rect;
    private Vector2 rectSize;

    public bool vertical;
    public int sections;
    private int rows;
    public Vector4 padding;

    private Vector2 slotSize;
    private Vector2 objectSize;

    protected override void Start()
    {
        base.Start();
        rect = GetComponent<RectTransform>();
        rect.pivot = Vector2.one * 0.5f;
    }

    private void Update()
    {
        rectSize = (rect.parent as RectTransform).sizeDelta + rect.sizeDelta;
        if (vertical) slotSize = new Vector2((rectSize.x / sections), (rectSize.x / sections));
        else slotSize = new Vector2((rectSize.y / sections), (rectSize.y / sections));
        objectSize = new Vector2(slotSize.x - padding.x - padding.z, slotSize.y - padding.y - padding.w);

        rows = rect.childCount / sections;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < sections; c++)
            {
                Vector2 slotPosition;
                if (vertical) slotPosition = new Vector2(slotSize.x * c, -(slotSize.y * r));
                else slotPosition = new Vector2(slotSize.x * r, -(slotSize.y * c));
                Vector2 newPosition = slotPosition + new Vector2(padding.x, -padding.y);
                newPosition += new Vector2(-rectSize.x / 2, rectSize.y / 2);

                RectTransform child = rect.GetChild(c + (sections * r)) as RectTransform;
                child.pivot = new Vector2(0, 1);
                child.sizeDelta = objectSize;
                child.localPosition = newPosition;
            }
        }
    }

    protected override void OnValidate()
    {
        sections = Mathf.Max(sections, 1);
    }
}
