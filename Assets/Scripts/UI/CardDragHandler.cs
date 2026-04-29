using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlotDefense
{
    public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int         slotIndex;
        public ArenaSystem arenaSystem;
        public Font        font;

        private GameObject _ghost;
        private Canvas     _canvas;

        private void Start() => _canvas = GetComponentInParent<Canvas>();

        public void OnBeginDrag(PointerEventData e)
        {
            var card = GameManager.Instance?.Hand.GetSlot(slotIndex);
            if (card == null || card.cardType != CardType.Unit) return;

            _ghost = new GameObject("DragGhost", typeof(RectTransform));
            _ghost.transform.SetParent(_canvas.transform, false);
            _ghost.transform.SetAsLastSibling();

            var img = _ghost.AddComponent<Image>();
            img.color = new Color(0.25f, 0.6f, 1f, 0.82f);
            img.raycastTarget = false;
            ((RectTransform)_ghost.transform).sizeDelta = new Vector2(220f, 95f);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(_ghost.transform, false);
            ((RectTransform)labelGo.transform).sizeDelta = new Vector2(210f, 85f);
            var txt = labelGo.AddComponent<Text>();
            txt.font      = font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.text      = $"{card.cardName}\n[{card.placementCost}스핀]";
            txt.fontSize  = 20;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color     = Color.white;
            txt.raycastTarget = false;
        }

        public void OnDrag(PointerEventData e)
        {
            if (_ghost == null) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_canvas.transform, e.position, e.pressEventCamera, out var pos);
            ((RectTransform)_ghost.transform).anchoredPosition = pos;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (_ghost != null) { Destroy(_ghost); _ghost = null; }

            var card = GameManager.Instance?.Hand.GetSlot(slotIndex);
            if (card == null || card.cardType != CardType.Unit) return;

            var world = Camera.main.ScreenToWorldPoint(e.position);
            world.z = 0f;
            if (world.x > -0.5f) return;

            if (!GameManager.Instance.SlotMachine.TryConsume(card.placementCost)) return;
            GameManager.Instance.Hand.Use(slotIndex);
            world.x = Mathf.Min(world.x, -0.5f);
            var go = Instantiate(arenaSystem.unitPrefab, world, Quaternion.identity);
            go.GetComponent<UnitController>().Init(card.unitStats, isPlayerUnit: true, portal: arenaSystem.portal);
            go.SetActive(true);
        }
    }
}
