using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Contracts.Contexts;
using Application.Common.Contracts.Services;
using Application.Extensions;
using Application.Models.Common;
using Application.Models.User;
using Bogus;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;
using static Bogus.DataSets.Name;

namespace Application.Services {

    internal class UserGenerationService : IUserGenerationService {
        private readonly IApplicationDbContext _context;

        public UserGenerationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserDto>> GetUsers(GetUsersArgs args, CancellationToken cancellationToken = default) {
            var rng = new Random(args.Seed + args.Page);

            var generated = await _GenerateUsersAsync(args, rng, cancellationToken);
            await _SetMiddleNamesAsync(args, rng, generated, cancellationToken);

            return generated;
        }

        private Task<List<UserDto>> _GenerateUsersAsync(GetUsersArgs args, Random rng, CancellationToken cancellationToken) {
            Randomizer.Seed = rng;

            var faker = new Faker<UserDto>(args.Locale.GetFakerLocale())
                            .RuleFor(src => src.Id, _ =>  _BuildId(rng))
                            .RuleFor(src => src.FirstName, faker => faker.Name.FirstName())
                            .RuleFor(src => src.LastName, faker => faker.Name.LastName())
                            .RuleFor(src => src.Gender, faker => faker.Person.Gender)
                            .RuleFor(src => src.Address, this._BuildAddress)
                            .RuleFor(src => src.Phone, faker => this._BuildPhone(faker, args));

            return Task.Run(() => faker.Generate(args.PageSize), cancellationToken);
        }

        private async Task _SetMiddleNamesAsync(GetUsersArgs args, Random rng, List<UserDto> generated, CancellationToken cancellationToken = default) {
            var middleNamesByGender = new Dictionary<Gender, List<string>> {
                { Gender.Male, await _GetLocalMiddleNamesByGenderAsync(args, rng, Gender.Male, cancellationToken) },
                { Gender.Female, await _GetLocalMiddleNamesByGenderAsync(args, rng, Gender.Female, cancellationToken) }
            };

            if(middleNamesByGender[0].Count > 0) {
                foreach(var user in generated) {
                    var index = rng.Next(middleNamesByGender[0].Count / 2);
                    user.MiddleName = middleNamesByGender[user.Gender][index];
                }
                return;
            }

            await _SetGeneratedMiddleNamesAsync(args, rng, generated, cancellationToken);
        }

        private async Task<List<string>> _GetLocalMiddleNamesByGenderAsync(GetUsersArgs args, Random rng, Gender gender, CancellationToken cancellationToken = default) {
            var locals = _context.MiddleNames
                .Where(x => x.Locale == args.Locale)
                .Where(x => string.IsNullOrEmpty(x.Gender) || x.Gender.ToUpper() == gender.GetGenderPrefix());

            var localsCount = await locals.CountAsync(cancellationToken);
            if (localsCount == 0) {
                return Enumerable.Empty<string>().ToList();
            }

            return await locals
                .Skip(rng.Next(localsCount - args.PageSize))
                .Take(args.PageSize)
                .Select(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        private Task _SetGeneratedMiddleNamesAsync(GetUsersArgs args, Random rng, List<UserDto> generated, CancellationToken cancellationToken = default) {
            Randomizer.Seed = rng;

            var faker = new Faker(args.Locale.GetFakerLocale());
            var existenceProbability = (float)rng.NextDouble();
            
            return Task.Run(() => {
                foreach(var user in generated) {
                    faker.Person.Gender = user.Gender;
                    user.MiddleName = faker.Name.FirstName().OrNull(faker, existenceProbability);
                }
            }, cancellationToken);
        }
        

        private string _BuildId(Random rng) {
            var bytes = new byte[16];
            rng.NextBytes(bytes);
            return new Guid(bytes).ToString();
        }

        private string _BuildAddress(Faker faker) {
            return $"{faker.Address.StreetAddress()}, "
                + $"{faker.Address.SecondaryAddress().OrNull(faker)}, "
                + faker.Address.City();
        }

        private string _BuildPhone(Faker faker, GetUsersArgs args) {
            var phoneUtil = PhoneNumberUtil.GetInstance();
            var phone = phoneUtil.Parse(faker.Phone.PhoneNumber(), new RegionInfo(new CultureInfo(args.Locale).LCID).TwoLetterISORegionName);
            return phoneUtil.Format(phone, PhoneNumberFormat.INTERNATIONAL);
        }
    }
}
