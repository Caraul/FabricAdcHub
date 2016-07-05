using System;

namespace FabricAdcHub.Core.Commands
{
    public class NamedFlag<TValue>
    {
        public NamedFlag(string name)
            : this(name, NamedFlagState.Undefined)
        {
        }

        public NamedFlag(string name, TValue value)
            : this(name, NamedFlagState.Defined)
        {
            _value = value;
        }

        public string Name { get; }

        public TValue Value
        {
            get
            {
                return State != NamedFlagState.Defined ? default(TValue) : _value;
            }

            set
            {
                _value = value;
                State = NamedFlagState.Defined;
            }
        }

        public NamedFlagState State { get; set; }

        public bool IsUndefined => State == NamedFlagState.Undefined;

        public bool IsDropped => State == NamedFlagState.Dropped;

        public bool IsDefined => State == NamedFlagState.Defined;

        public static NamedFlag<TValue> Undefined(string name)
        {
            return new NamedFlag<TValue>(name, NamedFlagState.Undefined);
        }

        public static NamedFlag<TValue> Dropped(string name)
        {
            return new NamedFlag<TValue>(name, NamedFlagState.Dropped);
        }

        public void FromNullable<TOtherValue>(TOtherValue value) where TOtherValue : class
        {
            if (value == null)
            {
                State = NamedFlagState.Undefined;
            }
            else
            {
                Value = (TValue)Convert.ChangeType(value, typeof(TValue));
            }
        }

        public void FromNullable<TOtherValue>(TOtherValue? value) where TOtherValue : struct
        {
            if (value == null)
            {
                State = NamedFlagState.Undefined;
            }
            else
            {
                Value = (TValue)Convert.ChangeType(value, typeof(TValue));
            }
        }

        private NamedFlag(string name, NamedFlagState state)
        {
            Name = name;
            State = state;
        }

        private TValue _value;
    }
}
