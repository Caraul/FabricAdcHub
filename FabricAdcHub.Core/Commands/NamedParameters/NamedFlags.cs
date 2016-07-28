using System.Collections.Generic;
using System.Linq;
using FabricAdcHub.Core.Utilites;

namespace FabricAdcHub.Core.Commands.NamedParameters
{
    public class NamedFlags
    {
        public NamedFlags()
        {
        }

        public NamedFlags(IEnumerable<string> parameters)
        {
            foreach (var parameter in parameters)
            {
                var name = parameter.Substring(0, 2);
                var value = parameter.Substring(2);
                if (!Flags.ContainsKey(name))
                {
                    Flags[name] = new HashSet<string>();
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    Flags[name].Add(value);
                }
            }
        }

        public NamedFlagState GetFlagState(string name)
        {
            HashSet<string> value;
            if (!Flags.TryGetValue(name, out value))
            {
                return NamedFlagState.Undefined;
            }

            return !value.Any() ? NamedFlagState.Dropped : NamedFlagState.Defined;
        }

        public HashSet<string> GetFlagValue(string name)
        {
            HashSet<string> value;
            return Flags.TryGetValue(name, out value) ? value : null;
        }

        public void SetFlagValue(string name, HashSet<string> value)
        {
            Flags[name] = value;
        }

        public void Union(NamedFlags other)
        {
            foreach (var pair in other.Flags)
            {
                Flags[pair.Key] = pair.Value;
            }
        }

        public void Summarize()
        {
            var dropped = Flags.Where(pair => !pair.Value.Any()).Select(pair => pair.Key);
            foreach (var key in dropped)
            {
                Flags.Remove(key);
            }
        }

        public string ToText()
        {
            var defined = Flags.Where(pair => pair.Value.Any()).SelectMany(pair => pair.Value.Select(value => (pair.Key + value).Escape()));
            var dropped = Flags.Where(pair => !pair.Value.Any()).Select(pair => pair.Key);
            return MessageSerializer.BuildText(defined.Union(dropped));
        }

        protected Dictionary<string, HashSet<string>> Flags { get; } = new Dictionary<string, HashSet<string>>();
    }
}
