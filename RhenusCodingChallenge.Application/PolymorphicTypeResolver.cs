using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using RhenusCodingChallenge.Domain;
using RhenusCodingChallenge.Domain.Player.Events;

namespace RhenusCodingChallenge.Application
{
    public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            Type basePointType = typeof(PlayerEvent);
            if (jsonTypeInfo.Type == basePointType || jsonTypeInfo.Type.IsAssignableTo(type))
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    //TypeDiscriminatorPropertyName = "$point-type",
                    //IgnoreUnrecognizedTypeDiscriminators = true,
                    UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor,
                    DerivedTypes =
                {
                    new JsonDerivedType(typeof(PlayerAccountCreatedEvent), "PlayerAccountCreatedEvent"),
                    new JsonDerivedType(typeof(PlayerAddsNewFundsEvent), "PlayerAddsNewFundsEvent"),
                    new JsonDerivedType(typeof(PlayerLosesEvent), "PlayerLosesEvent"),
                    new JsonDerivedType(typeof(PlayerWinsEvent), "PlayerWinsEvent"),
                    new JsonDerivedType(typeof(PlayerWithdrawsFundsEvent), "PlayerWithdrawsFundsEvent")
                }
                };
            }

            return jsonTypeInfo;
        }

        public static Action<JsonTypeInfo> AddDerivedTypes(Type baseType, params Type[] derivedTypes)
        {
            return typeInfo =>
            {
                if (typeInfo.Kind != JsonTypeInfoKind.Object)
                {
                    return;
                }

                if (baseType.IsAssignableFrom(typeInfo.Type))
                {
                    var applicableDerivedTypes = derivedTypes
                        // Check to make sure the current derived type is actually assignable to typeInfo.Type (which might also be a derived type of baseType)
                        .Where(t => typeInfo.Type.IsAssignableFrom(t))
                        .ToArray();

                    if (applicableDerivedTypes.Length > 0)
                    {
                        typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                        {
                            //IgnoreUnrecognizedTypeDiscriminators = true,
                            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                        };
                    }

                    foreach (var type in applicableDerivedTypes)
                    {
                        typeInfo.PolymorphismOptions!.DerivedTypes.Add(new JsonDerivedType(type, type.Name));
                    }
                }
            };
        }
    }
}
