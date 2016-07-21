using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FabricAdcHub.Core.Utilites;

namespace FabricAdcHub.Core.Commands
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
                CreateParameter(name);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _parameters[name].Add(value);
                }
            }
        }

        public NamedFlags Get(NamedFlag<bool> flag)
        {
            Get(flag, _ => true);
            return this;
        }

        public NamedFlags Get(NamedFlag<int> flag)
        {
            Get(flag, int.Parse);
            return this;
        }

        public NamedFlags Get(NamedFlag<string> flag)
        {
            Get(flag, _ => _);
            return this;
        }

        public NamedFlags Get(NamedFlag<Uri> flag)
        {
            Get(flag, value => new Uri(value));
            return this;
        }

        public NamedFlags Get(NamedFlag<HashSet<string>> flag)
        {
            return InternalGetNamedFlag(flag, _ => _);
        }

        public NamedFlags Get<TValue>(NamedFlag<TValue> flag, Func<string, TValue> converter)
        {
            return InternalGetNamedFlag(flag, values => converter(values.Single()));
        }

        public NamedFlags Get<TValue>(NamedFlag<TValue> flag, Func<TValue> undefinedConverter, Func<TValue> droppedConverter, Func<string, TValue> valueConverter)
        {
            HashSet<string> value;
            if (!_parameters.TryGetValue(flag.Name, out value))
            {
                flag.Value = undefinedConverter();
            }
            else
            {
                flag.Value = !value.Any() ? droppedConverter() : valueConverter(value.Single());
            }

            return this;
        }

        public NamedFlags Set(NamedFlag<bool> flag)
        {
            Set(flag, _ => "1");
            return this;
        }

        public NamedFlags Set(NamedFlag<int> flag)
        {
            Set(flag, value => value.ToString(CultureInfo.InvariantCulture));
            return this;
        }

        public NamedFlags Set(NamedFlag<string> flag)
        {
            Set(flag, _ => _);
            return this;
        }

        public NamedFlags Set(NamedFlag<Uri> flag)
        {
            Set(flag, value => value.ToString());
            return this;
        }

        public NamedFlags Set(NamedFlag<HashSet<string>> flag)
        {
            return InternalSetNamedFlag(flag, _ => _);
        }

        public NamedFlags Set<TValue>(NamedFlag<TValue> flag, Func<TValue, string> converter)
        {
            return InternalSetNamedFlag(flag, _ => new HashSet<string> { converter(flag.Value) });
        }

        public string ToText()
        {
            return Command.BuildString(_parameters.Where(pair => pair.Value.Any()).SelectMany(pair => pair.Value.Select(value => (pair.Key + value).Escape())));
        }

        private void CreateParameter(string name)
        {
            if (!_parameters.ContainsKey(name))
            {
                _parameters[name] = new HashSet<string>();
            }
        }

        private NamedFlags InternalGetNamedFlag<TValue>(NamedFlag<TValue> flag, Func<HashSet<string>, TValue> converter)
        {
            HashSet<string> value;
            if (!_parameters.TryGetValue(flag.Name, out value))
            {
                flag.State = NamedFlagState.Undefined;
            }
            else
            {
                if (!value.Any())
                {
                    flag.State = NamedFlagState.Dropped;
                }
                else
                {
                    flag.Value = converter(value);
                }
            }

            return this;
        }

        private NamedFlags InternalSetNamedFlag<TValue>(NamedFlag<TValue> flag, Func<TValue, HashSet<string>> converter)
        {
            if (flag.State != NamedFlagState.Undefined)
            {
                CreateParameter(flag.Name);
                if (flag.State != NamedFlagState.Dropped)
                {
                    _parameters[flag.Name] = converter(flag.Value);
                }
            }

            return this;
        }

        private readonly Dictionary<string, HashSet<string>> _parameters = new Dictionary<string, HashSet<string>>();
    }
}
