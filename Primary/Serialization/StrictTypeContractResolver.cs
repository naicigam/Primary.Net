using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Primary.Serialization
{
    internal class StrictTypeContractResolver : DefaultContractResolver
    {
        private readonly Type _targetType;

        public StrictTypeContractResolver(Type targetType) => _targetType = targetType;

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            => base.CreateProperties
            (
                _targetType.IsAssignableFrom(type) ? _targetType : type,
                memberSerialization
            );
    }
}
