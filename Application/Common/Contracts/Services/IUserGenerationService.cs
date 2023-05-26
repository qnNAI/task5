using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Common;
using Application.Models.User;

namespace Application.Common.Contracts.Services {

    public interface IUserGenerationService {
        Task<List<UserDto>> GetUsers(GetUsersArgs args, CancellationToken cancellationToken = default);
    }
}
