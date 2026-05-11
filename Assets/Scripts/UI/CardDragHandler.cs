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

        private void Awake()  => _canvas = GetComponentInParent<Canvas>();
        private void Start()  { if (_canvas == null) _canvas = FindObjectOfType<Canvas>(); }

        public void OnBeginDrag(PointerEventData e)
        {
            if (_canvas == null) return;
            var card = GameManager.Instance?.Hand.GetSlot(slotIndex);
            if (card == null || card.cardType == CardType.Buff) return;

            bool isSkill = card.cardType == CardType.Skill;

            _ghost = new GameObject("DragGhost", typeof(RectTransform));
            _ghost.transform.SetParent(_canvas.transform, false);
            _ghost.transform.SetAsLastSibling();

            var img = _ghost.AddComponent<Image>();
            img.color = isSkill ? new Color(0.55f, 0.1f, 0.9f, 0.85f) : new Color(0.2f, 0.55f, 1f, 0.82f);
            img.raycastTarget = false;
            ((RectTransform)_ghost.transform).sizeDelta = new Vector2(200f, 90f);

            var labelGo = new GameObject("Label", typeof(RectTransform));
            labelGo.transform.SetParent(_ghost.transform, false);
            ((RectTransform)labelGo.transform).sizeDelta = new Vector2(190f, 82f);
            var txt = labelGo.AddComponent<Text>();
            txt.font      = font != null ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.text      = isSkill
                ? $"{card.cardName}\n드래그 → 발동 위치"
                : $"{card.cardName}\n[{card.ElementalCost.Total}에너지]";
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

        private static ElementType? DominantElement(CardData card)
        {
            if (card.fireCost == 0 && card.ironCost == 0 && card.lifeCost == 0) return null;
            if (card.fireCost >= card.ironCost && card.fireCost >= card.lifeCost) return ElementType.Fire;
            if (card.ironCost >= card.lifeCost) return ElementType.Iron;
            return ElementType.Life;
        }

        public void OnEndDrag(PointerEventData e)
        {
            if (_ghost != null) { Destroy(_ghost); _ghost = null; }

            var card = GameManager.Instance?.Hand.GetSlot(slotIndex);
            if (card == null) return;

            var world = Camera.main.ScreenToWorldPoint(e.position);
            world.z = 0f;

            if (card.cardType == CardType.Skill)
            {
                if (!GameManager.Instance.ElementalEnergy.TryConsume(card.ElementalCost)) return;
                GameManager.Instance.Hand.Use(slotIndex);
                GameManager.Instance.UseSkill(card.skillEffect, world);
                return;
            }

            if (card.cardType != CardType.Unit) return;
            if (world.x > -0.5f) return;

            if (!GameManager.Instance.ElementalEnergy.TryConsume(card.ElementalCost)) return;
            GameManager.Instance.Hand.Use(slotIndex);
            world.x = Mathf.Min(world.x, -0.5f);
            var prefab = card.unitPrefab != null ? card.unitPrefab : arenaSystem.unitPrefab;
            var go = Instantiate(prefab, world, Quaternion.identity);
            go.GetComponent<UnitController>().Init(card.unitStats, isPlayerUnit: true, portal: arenaSystem.portal, element: DominantElement(card));
            go.SetActive(true);
        }
    }
}
