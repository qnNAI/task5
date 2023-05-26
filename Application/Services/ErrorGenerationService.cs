using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Attributes;
using Application.Models.User;

namespace Application.Services {
    internal class ErrorGenerationService {

        public void GenerateErrors(IEnumerable<UserDto> users, Random rng, double errorProbability, string locale) {
            var errorAmount = _GetErrorNumber(rng, errorProbability);

            foreach(var user in users) {
                _GenerateErrors(user, rng, errorAmount, locale);
            }

        }

        private void _GenerateErrors(UserDto user, Random rng, int errorAmount, string locale) {
            var props = user.GetType()
                .GetProperties(BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SkipErrorGenerationAttribute>() != null)
                .Where(x => x.PropertyType == typeof(string))
                .ToList();

            for(int i = 0; i < errorAmount; i++) {
                _GenerateErrorForUser(user, rng, props, locale);
            }
        }

        private void _GenerateErrorForUser(UserDto user, Random rng, List<PropertyInfo> props, string locale) {
            var errorPropIndex = rng.Next(props.Count);
            var errorProp = props[errorPropIndex];

            var propValue = errorProp.GetValue(user)?.ToString() ?? string.Empty;
            var errorNumber = Enum.GetValues(typeof(Errors)).Length;
            var error = (Errors)rng.Next(errorNumber);

            switch(error) {
                case Errors.Delete:
                    errorProp.SetValue(user, _DeleteRandom(propValue, rng));
                    break;
                case Errors.Insert:
                    errorProp.SetValue(user, _InsertRandom(propValue, rng, locale));
                    break;
                case Errors.Swap:
                    errorProp.SetValue(user, _SwapRandom(propValue, rng));
                    break;
            }
        }

        private string _DeleteRandom(string propValue, Random rng) {
            var removeIndex = rng.Next(propValue.Length);
            return propValue.Remove(removeIndex, 1);
        }

        private string _InsertRandom(string propValue, Random rng, string locale) {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            //cultureInfo.TextInfo.
            return null;
        }

        private string _SwapRandom(string propValue, Random rng) {
            throw new NotImplementedException();
        }

        private static int _GetErrorNumber(Random rng, double errorProbability) {
            var errorNumber = (int)errorProbability;
            var additionalErrorProb = errorProbability - errorNumber;
            if(rng.NextDouble() <= additionalErrorProb) {
                errorNumber++;
            }

            return errorNumber;
        }

        private enum Errors {
            Delete,
            Insert,
            Swap
        }
    }
}
