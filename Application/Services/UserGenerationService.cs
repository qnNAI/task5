using System;
using System.Collections.Generic;
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

namespace Application.Services {

    internal class UserGenerationService : IUserGenerationService {
        private readonly IApplicationDbContext _context;
        private readonly Dictionary<string, Func<GetUsersArgs, Random, List<UserDto>, CancellationToken, Task>> _middleNameProviders;

        private const string MALE_PREFIX = "M";
        private const string FEMALE_PREFIX = "F";

        public UserGenerationService(IApplicationDbContext context)
        {
            _context = context;
            _middleNameProviders = new() {
                { "ru-RU", _SetStoredMiddleNamesAsync },
                { "pl-PL", _SetGeneratedMiddleNamesAsync },
                { "en-US", _SetGeneratedMiddleNamesAsync }
            };
        }

        public async Task<List<UserDto>> GetUsers(GetUsersArgs args, CancellationToken cancellationToken = default) {
            var rng = new Random(args.Seed + args.Page);

            var generated = await _GenerateUsersAsync(args, rng, cancellationToken);
            await _middleNameProviders[args.Locale](args, rng, generated, cancellationToken);

            return generated;
        }

        private Task<List<UserDto>> _GenerateUsersAsync(GetUsersArgs args, Random rng, CancellationToken cancellationToken) {
            Randomizer.Seed = rng;

            var faker = new Faker<UserDto>(args.Locale.GetFakerLocale())
                            .RuleFor(src => src.Id, _ => {
                                var bytes = new byte[16];
                                rng.NextBytes(bytes);
                                return new Guid(bytes).ToString();
                            })
                            .RuleFor(src => src.FirstName, faker => faker.Name.FirstName())
                            .RuleFor(src => src.LastName, faker => faker.Name.LastName())
                            .RuleFor(src => src.Gender, faker => faker.Person.Gender == Bogus.DataSets.Name.Gender.Male ? MALE_PREFIX : FEMALE_PREFIX)
                            .RuleFor(src => src.Address, faker =>
                                $"{faker.Address.StreetAddress()}, "
                                + $"{faker.Address.SecondaryAddress().OrNull(faker)}, "
                                + faker.Address.City())
                            .RuleFor(src => src.Phone, faker => faker.Phone.PhoneNumber());

            return Task.Run(() => faker.Generate(args.PageSize), cancellationToken);
        }

        private async Task _SetStoredMiddleNamesAsync(GetUsersArgs args, Random rng, List<UserDto> generated, CancellationToken cancellationToken = default) {
            var middleNamesByGender = new Dictionary<string, List<string>> {
                { MALE_PREFIX, await _GetLocalMiddleNamesByGenderAsync(args, rng, MALE_PREFIX, cancellationToken) },
                { FEMALE_PREFIX, await _GetLocalMiddleNamesByGenderAsync(args, rng, FEMALE_PREFIX, cancellationToken) }
            };

            for(int i = 0; i < generated.Count; i++) {
                generated[i].MiddleName = middleNamesByGender[generated[i].Gender][i];
            }
        }

        private async Task<List<string>> _GetLocalMiddleNamesByGenderAsync(GetUsersArgs args, Random rng, string gender, CancellationToken cancellationToken = default) {
            var locals = _context.MiddleNames
                .Where(x => x.Locale == args.Locale)
                .Where(x => string.IsNullOrEmpty(x.Gender) || x.Gender.ToUpper() == gender.ToUpper());

            var localsCount = await locals.CountAsync(cancellationToken);

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
                    user.MiddleName = faker.Name.FirstName().OrNull(faker, existenceProbability);
                }
            }, cancellationToken);
        }
    }
}
