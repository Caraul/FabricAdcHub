using System;

namespace FabricAdcHub.Core.Commands
{
    public struct NamedFlag<TValue>
    {
        public NamedFlag(TValue value)
            : this()
        {
            Value = value;
            IsDefined = true;
        }

        public TValue Value { get; }

        public bool IsDefined { get; }

        public bool IsDropped { get; private set; }

        public bool IsUndefined => !IsDefined && !IsDropped;

        public NamedFlag<TOutValue> ChangeType<TOutValue>(Func<TValue, TOutValue> transform)
        {
            if (IsUndefined)
            {
                return NamedFlag<TOutValue>.Undefined;
            }

            if (IsDropped)
            {
                return NamedFlag<TOutValue>.Dropped;
            }

            return new NamedFlag<TOutValue>(transform(Value));
        }

        public static readonly NamedFlag<TValue> Undefined = default(NamedFlag<TValue>);

        public static readonly NamedFlag<TValue> Dropped = new NamedFlag<TValue> { IsDropped = true };
    }
}
