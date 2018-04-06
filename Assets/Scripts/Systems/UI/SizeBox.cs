using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("UI/SizeBox", 30)]
public class SizeBox : UIBehaviour
{
    private RectTransform rect;
    private Vector2 rectSize;

    public Vector2 objectSize;
    public Vector4 padding;

    private Vector2 gridSize;

    protected override void Start()
    {
        base.Start();
        rect = GetComponent<RectTransform>();
        rect.pivot = Vector2.one * 0.5f;
        rectSize = (rect.parent as RectTransform).sizeDelta;
        rect.sizeDelta = rectSize;
        Vector2 slotSize = new Vector2(objectSize.x + padding.x + padding.y, objectSize.y + padding.z + padding.w);
        gridSize = new Vector2
            (
                Mathf.FloorToInt(rectSize.x / (slotSize.x)),
                Mathf.FloorToInt(rectSize.y / (slotSize.y))
            );

        for (int i = 0; i < rect.childCount; i++)
        {
            RectTransform child = rect.GetChild(i) as RectTransform;
            child.pivot = new Vector2(0, 1);
            child.sizeDelta = objectSize;
            Vector2 slotPosition = new Vector2
                (
                    slotSize.x * (i % gridSize.x),
                    -(slotSize.x * Mathf.Floor((i / gridSize.x)))
                );
            Vector2 newPosition = slotPosition + new Vector2(padding.x, -padding.y);
            newPosition += new Vector2(-rectSize.x / 2, rectSize.y / 2);
            child.localPosition = newPosition;
            Debug.Log(newPosition, child);
        }
    }
}
