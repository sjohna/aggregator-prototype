using LiteDB;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace aggregator_server
{
    public static class LiteDBFunctions
    {
        public static void DoLiteDBGlobalSetUp()
        {
            BsonMapper.Global.RegisterType<Instant>
            (
                serialize: (instant) => NodaTime.Text.InstantPattern.ExtendedIso.Format(instant),
                deserialize: (bson) => NodaTime.Text.InstantPattern.ExtendedIso.Parse(bson.AsString).Value  // dangerous: what if this fails?
            );
        }
    }
}
