using Gameplay.Shared.Item.Pick;
using Shared.Logging;
using Unity.Netcode;

namespace Gameplay.Matches.LostSoul.Item.Pick.LostSoul
{
    /// <summary><see cref="ItemPickerS{T}"/> subclass that can pick <see cref="LostSoulPickableItem"/> items.</summary>
    public class LostSoulPickerS : ItemPickerS<LostSoulPickableItem>, IPickableItemCourier<LostSoulPickableItem>
    {
        private readonly NetworkVariable<bool> _soulPicked = new NetworkVariable<bool>();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _soulPicked.OnValueChanged += OnSoulPickedDropped;
            MatchEventsS.EvPlayerDied  += TryDrop;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            MatchEventsS.EvPlayerDied -= TryDrop;
        }
        
        private void TryDrop(ulong playerId, ulong? _)
        {
            if (OwnerClientId == playerId)
            {
                Drop();
            }
        }

        public override void Pick(LostSoulPickableItem pickableItem)
        {
            base.Pick(pickableItem);
            _soulPicked.Value = true;
        }

        public override void Drop()
        {
            if (_soulPicked.Value)
            {
                base.Drop();
                _soulPicked.Value = false;
            }
        }

        private void OnSoulPickedDropped(bool oldVal, bool newVal)
        {
            if (newVal)
            {
                Log.Inf(nameof(LostSoulPickerS), "Lost soul picked");
            }
            else
            {
                Log.Inf(nameof(LostSoulPickerS), "Lost soul dropped");
            }
        }

        public bool CanDeliverToClient(PickableItemDeliveryTargetS<LostSoulPickableItem> targetS)
        {
            return _soulPicked.Value;
        }

        public void DeliverToClient(PickableItemDeliveryTargetS<LostSoulPickableItem> targetS)
        {
            _soulPicked.Value = false;
        }
    }
}