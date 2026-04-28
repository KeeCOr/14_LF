using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public class ScreenFlash : MonoBehaviour
    {
        public static ScreenFlash Instance { get; private set; }

        private Image _overlay;

        private void Awake()
        {
            Instance = this;
            _overlay = GetComponent<Image>();
            if (_overlay == null) { Debug.LogError("[ScreenFlash] Image component missing"); return; }
            _overlay.raycastTarget = false;
            SetAlpha(0f);
        }

        public void Play(Color color, float maxAlpha, float fadeIn, float fadeOut)
        {
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine(color, maxAlpha, fadeIn, fadeOut));
        }

        private IEnumerator FlashCoroutine(Color color, float maxAlpha, float fadeIn, float fadeOut)
        {
            color.a = 0f;
            _overlay.color = color;
            float t = 0f;
            while (t < fadeIn)
            {
                t += Time.deltaTime;
                SetAlpha(Mathf.Lerp(0f, maxAlpha, t / fadeIn));
                yield return null;
            }
            SetAlpha(maxAlpha);
            t = 0f;
            while (t < fadeOut)
            {
                t += Time.deltaTime;
                SetAlpha(Mathf.Lerp(maxAlpha, 0f, t / fadeOut));
                yield return null;
            }
            SetAlpha(0f);
        }

        private void SetAlpha(float a)
        {
            var c = _overlay.color;
            c.a = a;
            _overlay.color = c;
        }
    }
}
