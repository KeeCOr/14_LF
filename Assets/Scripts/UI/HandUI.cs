using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace SlotDefense
{
    public class HandUI : MonoBehaviour
    {
        public Button[]    cardButtons;
        public Image[]     cardIcons;
        public Text[]      cardNames;
        public ArenaSystem arenaSystem;
        public Image       deployZoneOverlay;

        private void OnEnable() => GameEvents.OnCardObtained += RefreshHand;
        private void OnDisable() => GameEvents.OnCardObtained -= RefreshHand;

        private void Start()
        {
            for (int i = 0; i < cardButtons.Length; i++)
            {
                int index = i;
                cardButtons[i].onClick.AddListener(() => SelectSlot(index));
            }
        }

        private void Update() => RefreshDisplay();

        private static readonly Color ColorNormal   = new Color(0.2f, 0.25f, 0.45f, 0.9f);
        private static readonly Color ColorSelected = new Color(0.2f, 0.65f, 0.25f, 0.9f);
        private static readonly Color ColorBuff     = new Color(0.5f, 0.38f, 0.05f, 0.9f);
        private static readonly Color ColorSkill    = new Color(0.4f, 0.1f, 0.55f, 0.9f);
        private static readonly Color ColorEnhanced = new Color(1f, 0.85f, 0.1f, 0.9f);

        private void RefreshDisplay()
        {
            if (GameManager.Instance == null) return;
            int selected = arenaSystem != null ? arenaSystem.SelectedSlot : -1;
            for (int i = 0; i < cardButtons.Length; i++)
            {
                var card = GameManager.Instance.Hand.GetSlot(i);
                bool isSelected = selected == i && card != null;
                cardButtons[i].interactable = card != null;
                cardIcons[i].sprite = card?.icon;
                cardIcons[i].gameObject.SetActive(card != null);

                Color bgColor;
                if (card == null)
                {
                    cardNames[i].text = "---";
                    bgColor = ColorNormal;
                }
                else if (card.cardType == CardType.Buff)
                {
                    cardNames[i].text = $"{card.cardName}\n[클릭 → 즉시 발동]";
                    bgColor = ColorBuff;
                }
                else if (card.cardType == CardType.Skill)
                {
                    bool skillSelected = arenaSystem != null && arenaSystem.SelectedSkillSlot == i;
                    cardNames[i].text = skillSelected
                        ? $"{card.cardName}\n> 지점 클릭"
                        : $"{card.cardName}\n[클릭 → 지점 선택]";
                    bgColor = skillSelected ? new Color(0.7f, 0.3f, 0.9f, 0.95f) : ColorSkill;
                }
                else
                {
                    string costStr = $"[{card.ElementalCost.Total}에너지]";
                    cardNames[i].text = isSelected
                        ? $"{card.cardName} {costStr}\n> 배치 클릭"
                        : $"{card.cardName} {costStr}";
                    bgColor = isSelected ? ColorSelected : ColorNormal;
                }

                if (cardButtons[i].targetGraphic is Image bg)
                    bg.color = bgColor;
            }

            if (deployZoneOverlay != null)
            {
                int sel = arenaSystem != null ? arenaSystem.SelectedSlot : -1;
                bool unitPending = sel >= 0 && GameManager.Instance?.Hand.GetSlot(sel)?.cardType == CardType.Unit;
                float a = unitPending ? 0.35f : 0.10f;
                deployZoneOverlay.color = new Color(0.3f, 0.75f, 1f, a);
            }
        }

        private void SelectSlot(int index)
        {
            if (GameManager.Instance == null) return;
            var card = GameManager.Instance.Hand.GetSlot(index);
            if (card == null) return;

            if (card.cardType == CardType.Buff)
            {
                GameManager.Instance.Hand.Use(index);
                GameEvents.GlobalBuffApplied(card.buffEffect);
                return;
            }

            if (card.cardType == CardType.Skill)
            {
                arenaSystem.SelectSkillSlot(index);
                return;
            }

            arenaSystem.SelectHandSlot(index);
        }

        private void RefreshHand(CardData card, CardTier tier)
        {
            RefreshDisplay();
            for (int i = 0; i < cardButtons.Length; i++)
            {
                if (GameManager.Instance?.Hand.GetSlot(i) == card)
                {
                    StartCoroutine(PopInSlot(i));
                    if (tier == CardTier.Enhanced) StartCoroutine(BlinkSlot(i));
                    break;
                }
            }
        }

        private IEnumerator PopInSlot(int index)
        {
            var t = cardButtons[index].transform;
            float elapsed = 0f;
            while (elapsed < 0.15f)
            {
                elapsed += Time.deltaTime;
                float s = Mathf.Lerp(0f, 1.2f, elapsed / 0.15f);
                t.localScale = new Vector3(s, s, 1f);
                yield return null;
            }
            elapsed = 0f;
            while (elapsed < 0.07f)
            {
                elapsed += Time.deltaTime;
                float s = Mathf.Lerp(1.2f, 1f, elapsed / 0.07f);
                t.localScale = new Vector3(s, s, 1f);
                yield return null;
            }
            t.localScale = Vector3.one;
        }

        private IEnumerator BlinkSlot(int index)
        {
            for (int i = 0; i < 3; i++)
            {
                if (cardButtons[index].targetGraphic is Image bg)
                    bg.color = ColorEnhanced;
                yield return new WaitForSeconds(0.15f);
                RefreshDisplay();
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}
