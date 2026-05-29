using UnityEngine;

namespace SlotDefense
{
    public enum LowPolyWindProfile
    {
        Tree,
        Grass,
        Banner,
        Torch,
        Portal,
        AmbientProp
    }

    public class LowPolyWindAnimator : MonoBehaviour
    {
        [SerializeField] private float strength = 1f;
        [SerializeField] private float speed = 1.2f;
        [SerializeField] private float positionAmplitude = 0.03f;
        [SerializeField] private float rotationAmplitude = 2.5f;
        [SerializeField] private float scaleAmplitude = 0.012f;
        [SerializeField] private float phase;
        [SerializeField] private Vector3 positionAxis = Vector3.right;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        private Vector3 _baseLocalPosition;
        private Quaternion _baseLocalRotation;
        private Vector3 _baseLocalScale;
        private bool _baseCaptured;

        public float Strength => strength;
        public float Speed => speed;
        public float PositionAmplitude => positionAmplitude;
        public float RotationAmplitude => rotationAmplitude;
        public float ScaleAmplitude => scaleAmplitude;

        public static LowPolyWindAnimator Attach(GameObject target, LowPolyWindProfile profile, float phaseOffset = 0f)
        {
            if (target == null) return null;
            var wind = target.GetComponent<LowPolyWindAnimator>();
            if (wind == null) wind = target.AddComponent<LowPolyWindAnimator>();
            wind.Configure(profile, phaseOffset);
            wind.CaptureBaseTransform();
            return wind;
        }

        public void Configure(LowPolyWindProfile profile, float phaseOffset = 0f)
        {
            phase = phaseOffset;
            switch (profile)
            {
                case LowPolyWindProfile.Tree:
                    strength = 1f;
                    speed = 0.72f;
                    positionAmplitude = 0.025f;
                    rotationAmplitude = 2.8f;
                    scaleAmplitude = 0.010f;
                    positionAxis = Vector3.right;
                    rotationAxis = Vector3.forward;
                    break;
                case LowPolyWindProfile.Grass:
                    strength = 1f;
                    speed = 1.45f;
                    positionAmplitude = 0.018f;
                    rotationAmplitude = 5.2f;
                    scaleAmplitude = 0.018f;
                    positionAxis = Vector3.right;
                    rotationAxis = Vector3.forward;
                    break;
                case LowPolyWindProfile.Banner:
                    strength = 1f;
                    speed = 1.05f;
                    positionAmplitude = 0.012f;
                    rotationAmplitude = 4.4f;
                    scaleAmplitude = 0.014f;
                    positionAxis = Vector3.up;
                    rotationAxis = Vector3.forward;
                    break;
                case LowPolyWindProfile.Torch:
                    strength = 0.65f;
                    speed = 2.1f;
                    positionAmplitude = 0.010f;
                    rotationAmplitude = 3.5f;
                    scaleAmplitude = 0.026f;
                    positionAxis = Vector3.up;
                    rotationAxis = Vector3.forward;
                    break;
                case LowPolyWindProfile.Portal:
                    strength = 0.8f;
                    speed = 0.95f;
                    positionAmplitude = 0.018f;
                    rotationAmplitude = 1.6f;
                    scaleAmplitude = 0.018f;
                    positionAxis = Vector3.up;
                    rotationAxis = Vector3.forward;
                    break;
                default:
                    strength = 0.55f;
                    speed = 0.85f;
                    positionAmplitude = 0.014f;
                    rotationAmplitude = 1.7f;
                    scaleAmplitude = 0.008f;
                    positionAxis = Vector3.right;
                    rotationAxis = Vector3.forward;
                    break;
            }
        }

        public void CaptureBaseTransform()
        {
            _baseLocalPosition = transform.localPosition;
            _baseLocalRotation = transform.localRotation;
            _baseLocalScale = transform.localScale;
            _baseCaptured = true;
        }

        public Vector3 SamplePositionOffset(float time)
        {
            float wave = Mathf.Sin(time * speed + phase);
            float gust = Mathf.Sin(time * speed * 0.37f + phase * 1.7f) * 0.35f;
            return positionAxis.normalized * ((wave + gust) * positionAmplitude * strength);
        }

        public Quaternion SampleRotation(float time)
        {
            float wave = Mathf.Sin(time * speed + phase);
            return Quaternion.AngleAxis(wave * rotationAmplitude * strength, rotationAxis.normalized);
        }

        public Vector3 SampleScale(float time)
        {
            float wave = Mathf.Sin(time * speed * 1.31f + phase);
            float scale = 1f + wave * scaleAmplitude * strength;
            return new Vector3(scale, Mathf.Max(0.01f, 1f - (scale - 1f) * 0.45f), scale);
        }

        private void Awake() => CaptureBaseTransform();

        private void OnEnable()
        {
            if (!_baseCaptured) CaptureBaseTransform();
        }

        private void LateUpdate()
        {
            if (!_baseCaptured) CaptureBaseTransform();
            float t = Time.time;
            transform.localPosition = _baseLocalPosition + SamplePositionOffset(t);
            transform.localRotation = _baseLocalRotation * SampleRotation(t);
            transform.localScale = Vector3.Scale(_baseLocalScale, SampleScale(t));
        }
    }
}
