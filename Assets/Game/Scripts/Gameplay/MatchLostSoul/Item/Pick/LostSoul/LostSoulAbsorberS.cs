using Game.Scripts.Gameplay.Shared.Item.Pick;

namespace Game.Scripts.Gameplay.MatchLostSoul.Item.Pick.LostSoul
{
    public class LostSoulAbsorberS : PickableItemAbsorberS<LostSoulPickableItem>
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            EvItemAbsorbed += InvokeLostSoulAbsorbedEvent;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            EvItemAbsorbed -= InvokeLostSoulAbsorbedEvent;
        }

        private void InvokeLostSoulAbsorbedEvent(ulong clientId)
        {
            MatchEventsS.SendEvLostSoulAbsorbed(clientId);
        }
    }
}