using R3;
using TankPlayground.Config;
using TankPlayground.Configs;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace TankPlayground.Gameplay
{
    public class NetworkStatsController : NetworkBehaviour
    {
        [SerializeField] private TankConfig _tankConfig;

        private IStatCollection _stats;
        private IStatusEffectService _statusEffectService;
        
        private StatusEffectsDatabase _effectsDatabase;
        private NetworkDictionary<int, NetworkStatValue> _networkStats;
        
        public NetworkList<NetworkStatusEffect> ActiveEffects { get; private set; }

        private readonly Subject<(StatType type, NetworkStatValue stat)> _statsChangedSubject = new();
        public Observable<(StatType type, NetworkStatValue stat)> StatsChangedObservable => _statsChangedSubject;

        [Inject]
        public void Construct(StatusEffectsDatabase effectsDatabase) => _effectsDatabase = effectsDatabase;

        private void Awake()
        {
            ActiveEffects = new();
            _networkStats = new();
            _stats = new StatCollection();

            foreach (StatData statData in _tankConfig.ResourceStats)
                _stats.RegisterResource(new(statData.Type, statData.Value, statData.SetToMax));

            foreach (StatData statData in _tankConfig.AttributesStats)
                _stats.Register(new AttributeStat(statData.Type, statData.Value));
        }

        public override void OnNetworkSpawn()
        {
            _networkStats.OnDictionaryChanged += OnStatsChanged;

            if (!IsServer)
                return;

            _statusEffectService = new StatusEffectService(_stats);
            _statusEffectService.EffectsObservable
                .Subscribe(effects =>
                {
                    ActiveEffects.Clear();

                    foreach (StatusEffect effect in effects)
                    {
                        ActiveEffects.Add(new NetworkStatusEffect(
                            effect.Config.Name,
                            effect.RemainingDuration,
                            effect.Stacks));
                    }
                })
                .AddTo(this);

            InitializeNetworkValues();
        }

        public override void OnNetworkDespawn() => _networkStats.OnDictionaryChanged -= OnStatsChanged;

        private void Update()
        {
            if (!IsServer)
                return;

            _statusEffectService.Tick(Time.deltaTime);
        }
        
        public float GetStatValue(StatType type) => _networkStats[(int)type].Current;
        
        public NetworkStatValue GetNetworkStatValue(StatType type) => _networkStats[(int)type];
        
        public bool TryGetServerResource(StatType type, out ResourceStat resource)
        {
            AssertServer();
            return _stats.TryGetResource(type, out resource);
        }

        public void ApplyEffect(StatusEffectConfig effectConfig)
        {
            if (IsServer)
                ApplyEffectInternal(effectConfig);
            else
                ApplyEffectServerRpc(effectConfig.Name);
        }

        private void InitializeNetworkValues()
        {
            foreach (IStat attributeStat in _stats.Attributes)
            {
                _networkStats.Add((int)attributeStat.Type, new(attributeStat.Value, attributeStat.Value));

                attributeStat.ChangedObservable
                    .Subscribe(value => _networkStats[(int)attributeStat.Type] = new(attributeStat.Value, value))
                    .AddTo(this);
            }

            foreach (ResourceStat resourceStat in _stats.Resources)
            {
                _networkStats.Add((int)resourceStat.Type, new(resourceStat.Max, resourceStat.Current.CurrentValue));

                resourceStat.ChangedObservable
                    .Subscribe(value => _networkStats[(int)resourceStat.Type] = new(resourceStat.Max, value))
                    .AddTo(this);
            }
        }

        private void OnStatsChanged(NetworkDictionaryEvent<int, NetworkStatValue> changeEvent) =>
            _statsChangedSubject.OnNext(((StatType)changeEvent.Key, changeEvent.Value));

        [ServerRpc]
        private void ApplyEffectServerRpc(string effectName)
        {
            StatusEffectConfig config = _effectsDatabase.GetStatusEffectConfig(effectName);

            if (config == null)
                return;

            ApplyEffectInternal(config);
        }

        private void ApplyEffectInternal(StatusEffectConfig config)
        {
            StatusEffect effect = new(config);
            _statusEffectService.AddEffect(effect);
        }

        private void AssertServer()
        {
            if (!IsServer)
                Debug.LogError($"[{nameof(NetworkStatsController)}] Server-only method called on client!");
        }
    }
}