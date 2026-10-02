namespace SlotDefense
{
    public interface IOpponent
    {
        void OnUpdate(float deltaTime);
        void ReceiveTransferredMonster(MonsterConfig monster);
    }
}
