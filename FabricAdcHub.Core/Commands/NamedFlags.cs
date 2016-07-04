using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
                _parameters[name].Add(value);
            }
        }

        public NamedFlag<int> GetNamedInt(string name)
        {
            IList<string> value;
            if (!_parameters.TryGetValue(name, out value))
            {
                return NamedFlag<int>.Undefined;
            }

            var valueStr = value.Single();
            return string.IsNullOrEmpty(valueStr)
                ? NamedFlag<int>.Dropped
                : new NamedFlag<int>(int.Parse(valueStr));
        }

        public NamedFlag<string> GetNamedString(string name)
        {
            IList<string> value;
            if (!_parameters.TryGetValue(name, out value))
            {
                return NamedFlag<string>.Undefined;
            }

            var valueStr = value.Single();
            return string.IsNullOrEmpty(valueStr)
                ? NamedFlag<string>.Dropped
                : new NamedFlag<string>(valueStr);
        }

        public NamedFlag<IList<string>> GetNamedStrings(string name)
        {
            IList<string> value;
            if (!_parameters.TryGetValue(name, out value))
            {
                return NamedFlag<IList<string>>.Undefined;
            }

            var valueStr = value.First();
            return string.IsNullOrEmpty(valueStr)
                ? NamedFlag<IList<string>>.Dropped
                : new NamedFlag<IList<string>>(value);
        }

        public void SetNamedInt(string name, NamedFlag<int> value)
        {
            if (!value.IsUndefined)
            {
                CreateParameter(name);
                _parameters[name].Add(value.IsDropped ? string.Empty : value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void SetNamedString(string name, NamedFlag<string> value)
        {
            if (!value.IsUndefined)
            {
                CreateParameter(name);
                _parameters[name].Add(value.IsDropped ? string.Empty : value.Value);
            }
        }

        public void SetNamedStrings(string name, NamedFlag<IList<string>> value)
        {
            if (!value.IsUndefined && value.Value.Any())
            {
                CreateParameter(name);
                _parameters[name] = value.IsDropped ? new[] { string.Empty }.ToList() : value.Value;
            }
        }

        public bool? GetBool(string name)
        {
            return GetValue(name, value => value == "1");
        }

        public int? GetInt(string name)
        {
            return GetValue(name, int.Parse);
        }

        public string GetString(string name)
        {
            return GetValue(name, value => value);
        }

        public IList<string> GetStrings(string name)
        {
            return GetValues(name, value => value);
        }

        public TValue GetValue<TValue>(string name, Func<string, TValue> transform)
        {
            var namedParameter = GetNamedString(name);
            return namedParameter.IsDefined ? transform(namedParameter.Value) : default(TValue);
        }

        public TValue GetValues<TValue>(string name, Func<IList<string>, TValue> transform)
        {
            var namedParameter = GetNamedStrings(name);
            return namedParameter.IsDefined ? transform(namedParameter.Value) : default(TValue);
        }

        public void SetBool(string name, bool? value)
        {
            SetValue(name, value, boolValue => boolValue ? "1" : "0");
        }

        public void SetInt(string name, int? value)
        {
            SetValue(name, value, intValue => intValue.ToString(CultureInfo.InvariantCulture));
        }

        public void SetString(string name, string value)
        {
            SetValue(name, value, stringValue => stringValue);
        }

        public void SetStrings(string name, IList<string> value)
        {
            SetValue(name, value, stringValue => stringValue);
        }

        public void SetValue<TOutValue>(string name, TOutValue? value, Func<TOutValue, string> transform)
            where TOutValue : struct
        {
            var namedParameter = value == null ? NamedFlag<string>.Undefined : new NamedFlag<string>(transform(value.Value));
            SetNamedString(name, namedParameter);
        }

        public void SetValue<TOutValue>(string name, TOutValue value, Func<TOutValue, string> transform)
            where TOutValue : class
        {
            var namedParameter = value == null ? NamedFlag<string>.Undefined : new NamedFlag<string>(transform(value));
            SetNamedString(name, namedParameter);
        }

        public void SetValue<TOutValue>(string name, IList<TOutValue> value, Func<IList<TOutValue>, IList<string>> transform)
            where TOutValue : class
        {
            var namedParameter = value == null || !value.Any() ? NamedFlag<IList<string>>.Undefined : new NamedFlag<IList<string>>(transform(value));
            SetNamedStrings(name, namedParameter);
        }

        public string ToText()
        {
            return Command.BuildString(_parameters.Where(pair => pair.Value.Any()).SelectMany(pair => pair.Value.Select(value => pair.Key + value)));
        }

        private void CreateParameter(string name)
        {
            if (!_parameters.ContainsKey(name))
            {
                _parameters[name] = new List<string>();
            }
        }

        private readonly Dictionary<string, IList<string>> _parameters = new Dictionary<string, IList<string>>();
    }
}
