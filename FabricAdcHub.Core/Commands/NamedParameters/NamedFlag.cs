using System.Collections.Generic;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public abstract class NamedFlag<TValue>
    {
        protected NamedFlag(string name, NamedFlags namedFlags)
        {
            Name = name;
            _namedFlags = namedFlags;
        }

        public string Name { get; }

        public TValue Value
        {
            get
            {
                return State != NamedFlagState.Defined ? default(TValue) : FromStrings(_namedFlags.GetFlagValue(Name));
            }

            set
            {
                _namedFlags.SetFlagValue(Name, ToStrings(value));
            }
        }

        public NamedFlagState State => _namedFlags.GetFlagState(Name);

        public bool IsUndefined => State == NamedFlagState.Undefined;

        public bool IsDropped => State == NamedFlagState.Dropped;

        public bool IsDefined => State == NamedFlagState.Defined;

        public abstract HashSet<string> ToStrings(TValue value);

        public abstract TValue FromStrings(HashSet<string> value);

        private readonly NamedFlags _namedFlags;
    }
}
