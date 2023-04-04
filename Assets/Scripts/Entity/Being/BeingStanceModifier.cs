using JsonKnownTypes;
using Newtonsoft.Json;
using Entity.Being.Stances.Modifiers;

namespace Entity.Being.Stances
{
    [JsonConverter(typeof(JsonKnownTypesConverter<BeingStanceModifier>))]
    [JsonDiscriminator(Name = "Type")]
    [JsonKnownType(typeof(LerpModdableValueModifier), nameof(LerpModdableValueModifier))]
    public abstract class BeingStanceModifier
    {
        protected BeingActor self;
        protected BeingStance modifiedStance;

        public virtual void Initialize(BeingActor self, BeingStance modifiedStance)
        {
            this.self = self;
            this.modifiedStance = modifiedStance;
        }
    }
}
