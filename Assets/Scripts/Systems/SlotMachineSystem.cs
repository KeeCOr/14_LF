namespace SlotDefense
{
    public class SlotMachineSystem
    {
        private readonly float _xpPerSpin;
        private float _xpBuffer;
        private int _spinCharges;

        public int SpinCharges => _spinCharges;
        public float XPBuffer => _xpBuffer;

        public SlotMachineSystem(float xpPerSpin = 100f) => _xpPerSpin = xpPerSpin;

        public void AddXP(float amount)
        {
            _xpBuffer += amount;
            while (_xpBuffer >= _xpPerSpin)
            {
                _xpBuffer -= _xpPerSpin;
                _spinCharges++;
            }
        }

        public bool TrySpin()
        {
            if (_spinCharges <= 0) return false;
            _spinCharges--;
            return true;
        }
    }
}
