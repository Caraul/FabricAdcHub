using System;

namespace FabricAdcHub.Core.Messages
{
    public struct NamedParameter<TValue>
    {
        public NamedParameter(TValue value)
            : this()
        {
            Value = value;
            IsDefined = true;
        }

        public TValue Value { get; }

        public bool IsDefined { get; }

        public bool IsDropped { get; private set; }

        public bool IsUndefined => !IsDefined && !IsDropped;

        public NamedParameter<TOutValue> ChangeType<TOutValue>(Func<TValue, TOutValue> transform)
        {
            if (IsUndefined)
            {
                return NamedParameter<TOutValue>.Undefined;
            }

            if (IsDropped)
            {
                return NamedParameter<TOutValue>.Dropped;
            }

            return new NamedParameter<TOutValue>(transform(Value));
        }

        public static readonly NamedParameter<TValue> Undefined = default(NamedParameter<TValue>);

        public static readonly NamedParameter<TValue> Dropped = new NamedParameter<TValue> { IsDropped = true };
    }
}
