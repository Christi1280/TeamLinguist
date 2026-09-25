using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; //save parent
        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; //semi - transparent during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; //follow mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f; //no longer transparent

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //Slot where items dropped
        if(dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null )
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot != null)
        {
            if(dropSlot.currentItem != null)
            {
                //Swap items if slot already has one
                dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                originalSlot.currentItem = dropSlot.currentItem;
                dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            } else
            {
                originalSlot.currentItem = null;
            }
            //Move item into dropslot
            transform.SetParent(dropSlot.transform);
            dropSlot.currentItem = gameObject;
        } else
        {
            //No slot
            transform.SetParent(originalParent);
        }
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
  
}
