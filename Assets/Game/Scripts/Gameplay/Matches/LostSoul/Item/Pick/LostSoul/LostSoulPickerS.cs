using Gameplay.Matches.LostSoul.Context;
using Gameplay.Shared.Init;
using Gameplay.Shared.Item.Pick;
using Shared.Logging;
using Unity.Netcode;

namespace Gameplay.Matches.LostSoul.Item.Pick.LostSoul
{
    /// <summary><see cref="ItemPickerS{T}"/> subclass that can pick <see cref="LostSoulPickableItem"/> items.</summary>
    public class LostSoulPickerS : ItemPickerS<LostSoulPickableItem>, IPickableItemCourier<LostSoulPickableItem>, IMatchInitServer
    {
        private MatchContext _context;
        
        private readonly NetworkVariable<bool> _soulPicked = new NetworkVariable<bool>();

        private bool IsPlayerAlive => _context.PlayersDataS[OwnerClientId].IsAlive;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _soulPicked.OnValueChanged += OnSoulPickedDropped;
                MatchEventsS.EvPlayerDied  += TryDrop;    
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                base.OnNetworkDespawn();
                MatchEventsS.EvPlayerDied -= TryDrop;    
            }
        }

        public void InitServer(MatchContext context)
        {
            _context = context;
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
            if (!IsPlayerAlive)
            {
                return;
            }
            
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
                Log.Inf(nameof(LostSoulPickerS), $"Lost soul picked, player: {OwnerClientId}");
            }
            else
            {
                Log.Inf(nameof(LostSoulPickerS), $"Lost soul dropped, player: {OwnerClientId}");
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