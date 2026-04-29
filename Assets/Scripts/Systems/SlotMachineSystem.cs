namespace SlotDefense
{
    public class SlotMachineSystem
    {
        private readonly float _chargeInterval;
        private float _chargeTimer;
        private int _spinCharges;
        private const int MaxCharges = 10;

        public int   SpinCharges   => _spinCharges;
        public float SecondsToNext => _spinCharges >= MaxCharges ? 0f : _chargeInterval - _chargeTimer;

        public SlotMachineSystem(float chargeInterval = 12f, int initialCharges = 0)
        {
            _chargeInterval = chargeInterval;
            _spinCharges    = initialCharges;
        }

        public void Tick(float deltaTime)
        {
            if (_spinCharges >= MaxCharges) return;
            _chargeTimer += deltaTime;
            while (_chargeTimer >= _chargeInterval && _spinCharges < MaxCharges)
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
    }
}
