using UnityEngine;

namespace SlotDefense
{
    [CreateAssetMenu(fileName = "BackgroundConfig", menuName = "SlotDefense/BackgroundConfig")]
    public class BackgroundConfig : ScriptableObject
    {
        [Header("카메라 배경색")]
        public Color battleSkyColor    = new Color(0.48f, 0.72f, 0.88f);
        public Color survivalSkyColor  = new Color(0.10f, 0.08f, 0.18f);

        [Header("카메라 설정")]
        public float orthographicSize  = 5f;
        public float cameraY           = -0.5f;

        [Header("지면 색상")]
        public Color battleGroundColor   = new Color(0.25f, 0.55f, 0.20f);
        public Color survivalGroundColor = new Color(0.15f, 0.20f, 0.15f);
    }
}
