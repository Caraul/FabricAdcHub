using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace FabricAdcHub.Core.Messages
{
    public class NamedParameters
    {
        public NamedParameters()
        {
        }

        public NamedParameters(IEnumerable<string> parameters)
        {
            foreach (var parameter in parameters)
            {
                var name = parameter.Substring(0, 2);
                var value = parameter.Substring(2);
                CreateParameter(name);
                _parameters[name].Add(value);
            }
        }

        public static TValue? GetValueOrDefault<TValue>(NamedParameter<TValue> value, TValue? defaultValue)
            where TValue : struct
        {
            if (value.IsUndefined)
            {
                return defaultValue;
            }

            return value.IsDropped ? (TValue?)null : value.Value;
        }

        public static TValue GetValueOrDefault<TValue>(NamedParameter<TValue> value, TValue defaultValue)
            where TValue : class
        {
            if (value.IsUndefined)
            {
                return defaultValue;
            }

            return value.IsDropped ? null : value.Value;
        }

        public NamedParameter<int> GetNamedInt(string name)
        {
            IList<string> value;
            if (!_parameters.TryGetValue(name, out value))
            {
                return NamedParameter<int>.Undefined;
            }

            var valueStr = value.Single();
            return string.IsNullOrEmpty(valueStr)
                ? NamedParameter<int>.Dropped
                : new NamedParameter<int>(int.Parse(valueStr));
        }

        public NamedParameter<string> GetNamedString(string name)
        {
            IList<string> value;
            if (!_parameters.TryGetValue(name, out value))
            {
                return NamedParameter<string>.Undefined;
            }

            var valueStr = value.Single();
            return string.IsNullOrEmpty(valueStr)
                ? NamedParameter<string>.Dropped
                : new NamedParameter<string>(valueStr);
        }

        public NamedParameter<IList<string>> GetNamedStrings(string name)
        {
            IList<string> value;
            if (!_parameters.TryGetValue(name, out value))
            {
                return NamedParameter<IList<string>>.Undefined;
            }

            var valueStr = value.First();
            return string.IsNullOrEmpty(valueStr)
                ? NamedParameter<IList<string>>.Dropped
                : new NamedParameter<IList<string>>(value);
        }

        public void SetNamedInt(string name, NamedParameter<int> value)
        {
            if (!value.IsUndefined)
            {
                CreateParameter(name);
                _parameters[name].Add(value.IsDropped ? string.Empty : value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void SetNamedString(string name, NamedParameter<string> value)
        {
            if (!value.IsUndefined)
            {
                CreateParameter(name);
                _parameters[name].Add(value.IsDropped ? string.Empty : value.Value);
            }
        }

        public void SetNamedStrings(string name, NamedParameter<IList<string>> value)
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
            var namedParameter = value == null ? NamedParameter<string>.Undefined : new NamedParameter<string>(transform(value.Value));
            SetNamedString(name, namedParameter);
        }

        public void SetValue<TOutValue>(string name, TOutValue value, Func<TOutValue, string> transform)
            where TOutValue : class
        {
            var namedParameter = value == null ? NamedParameter<string>.Undefined : new NamedParameter<string>(transform(value));
            SetNamedString(name, namedParameter);
        }

        public void SetValue<TOutValue>(string name, IList<TOutValue> value, Func<IList<TOutValue>, IList<string>> transform)
            where TOutValue : class
        {
            var namedParameter = value == null || !value.Any() ? NamedParameter<IList<string>>.Undefined : new NamedParameter<IList<string>>(transform(value));
            SetNamedStrings(name, namedParameter);
        }

        public string ToText()
        {
            return Message.BuildString(_parameters.Where(pair => pair.Value.Any()).SelectMany(pair => pair.Value.Select(value => pair.Key + value)));
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
