using UnityEngine;
namespace SlotDefense
{
    // 월드 공간에서 오브젝트 위에 표시되는 HP 바.
    // AddComponent 직후 Setup() 호출 필요.
    public class HpBar : MonoBehaviour
    {
        private GameObject _root;
        private SpriteRenderer _fill;
        private Transform _fillT;
        private float _fw;

        public void Setup(float yOffset = 0.45f, float width = 0.7f)
        {
            _fw = width;

            _root = new GameObject("HpBar");
            _root.transform.SetParent(transform, false);
            _root.transform.localPosition = new Vector3(0f, yOffset, 0f);
            _root.SetActive(false);

            Bar("Bg",   new Color(0.15f, 0.15f, 0.15f, 0.9f), z: 0f,     w: width + 0.06f, h: 0.13f, order: 9);
            _fill  = Bar("Fill", Color.green,                   z: -0.01f, w: width,          h: 0.11f, order: 10);
            _fillT = _fill.transform;
        }

        public void SetRatio(float ratio)
        {
            if (_root == null) return;
            ratio = Mathf.Clamp01(ratio);
            bool show = ratio < 0.9999f;
            _root.SetActive(show);
            if (!show) return;

            _fillT.localScale    = new Vector3(_fw * ratio, 0.11f, 1f);
            _fillT.localPosition = new Vector3(-_fw * 0.5f * (1f - ratio), 0f, -0.01f);
            _fill.color          = Color.Lerp(Color.red, Color.green, ratio);
        }

        private SpriteRenderer Bar(string n, Color c, float z, float w, float h, int order)
        {
            var go = new GameObject(n);
            go.transform.SetParent(_root.transform, false);
            go.transform.localPosition = new Vector3(0f, 0f, z);
            go.transform.localScale    = new Vector3(w, h, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = Pixel(c);
            sr.sortingOrder = order;
            return sr;
        }

        private static Sprite Pixel(Color c)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, c);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }
    }
}
