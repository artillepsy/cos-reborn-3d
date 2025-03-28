using Gameplay.Shared.Item.Pick;

namespace Gameplay.Matches.LostSoul.Item.Pick.LostSoul
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