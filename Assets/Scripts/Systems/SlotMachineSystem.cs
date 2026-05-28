using UnityEngine;
namespace SlotDefense
{
    public class SlotMachineSystem
    {
        private readonly float _chargeInterval;
        private readonly int _maxCharges;
        private float _chargeTimer;
        private int _spinCharges;
        public const int DefaultMaxCharges = 6;

        public int   SpinCharges    => _spinCharges;
        public int   MaxSpinCharges => _maxCharges;
        public float ChargeRatio    => _maxCharges <= 0 ? 0f : (float)_spinCharges / _maxCharges;
        public float SecondsToNext  => _spinCharges >= _maxCharges ? 0f : _chargeInterval - _chargeTimer;

        public SlotMachineSystem(float chargeInterval = 12f, int initialCharges = 0, int maxCharges = DefaultMaxCharges)
        {
            _chargeInterval = chargeInterval;
            _maxCharges     = Mathf.Max(1, maxCharges);
            _spinCharges    = Mathf.Clamp(initialCharges, 0, _maxCharges);
        }

        public void Tick(float deltaTime)
        {
            if (_spinCharges >= _maxCharges) return;
            _chargeTimer += deltaTime;
            while (_chargeTimer >= _chargeInterval && _spinCharges < _maxCharges)
            {
                _chargeTimer -= _chargeInterval;
                _spinCharges++;
            }
        }

        public bool TrySpin()
        {
            if (_spinCharges <= 0) return false;
            _spinCharges--;
            return true;
        }

        public bool TryConsume(int charges)
        {
            if (_spinCharges < charges) return false;
            _spinCharges -= charges;
            return true;
        }

        public void AddCharge(int amount)
        {
            _spinCharges = Mathf.Clamp(_spinCharges + amount, 0, _maxCharges);
        }
    }
}
