using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Attributes;
using Application.Common.Contracts.Services;
using Application.Extensions;
using Application.Models.Common;
using Application.Models.User;
using Bogus;

namespace Application.Services {
    internal class ErrorGenerationService : IErrorGenerationService {

        public void GenerateErrors(GenerateErrorsArgs args) {
            var rng = new Random(args.Seed);
            var errorAmount = _GetErrorNumber(rng, args.ErrorProbability);

            foreach(var user in args.Users) {
                _GenerateErrors(user, rng, errorAmount, args.Locale);
            }
        }

        private void _GenerateErrors(UserDto user, Random rng, int errorAmount, string locale) {
            var props = user.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<SkipErrorGenerationAttribute>() == null)
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
            _GenerateErrorForProperty(user, rng, locale, errorProp, propValue);
        }

        private void _GenerateErrorForProperty(UserDto user, Random rng, string locale, PropertyInfo errorProp, string propValue) {
            if(string.IsNullOrEmpty(propValue))
                return;

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
            var insertIndex = rng.Next(propValue.Length);
            var isLetter = rng.Next(2) == 1;

            Randomizer.Seed = rng;
            var faker = new Faker(locale.GetFakerLocale());

            return propValue.Insert(insertIndex, isLetter ? faker.Lorem.Letter() : faker.Random.Digits(1)[0].ToString());
        }

        private string _SwapRandom(string propValue, Random rng) {
            if (propValue.Length == 1) {
                return propValue;
            }

            var chars = propValue.ToCharArray();

            var firstIndex = rng.Next(chars.Length - 1);
            (chars[firstIndex], chars[firstIndex + 1]) = (chars[firstIndex + 1], chars[firstIndex]);
            return new string(chars);
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
