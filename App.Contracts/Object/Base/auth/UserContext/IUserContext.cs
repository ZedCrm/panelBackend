using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Contracts.Object.Base.auth.UserContext
{
    public interface IUserContext
    {
        int? GetUserId();
        bool IsAuthenticated();
        // می‌تونی متدهای دیگه‌ای مثل GetUserRoles یا GetClaim اضافه کنی
    }
}