using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MyFrameWork.AppTool;

namespace App.utility
{
    public static class ValidationUtility
{

     private static readonly OPT opt = new OPT();


    public static OPT ValidateNotEmpty(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            return opt.Failed($"{fieldName} نمی‌تواند خالی باشد.");
        return opt.Succeeded();
    }

    public static OPT ValidateLength(string value, string fieldName, int minLength, int maxLength)
    {
        if (value.Length < minLength || value.Length > maxLength)
            return opt.Failed($"{fieldName} باید بین {minLength} تا {maxLength} کاراکتر باشد.");
        return opt.Succeeded();
    }

    public static OPT ValidateRange(decimal value, string fieldName, decimal minValue, decimal maxValue)
    {
        if (value < minValue || value > maxValue)
            return opt.Failed($"{fieldName} باید بین {minValue} تا {maxValue} باشد.");
        return opt.Succeeded();
    }

   public static async Task<OPT> ValidateUniqueAsync<T, TKey>(
    IBaseRep<T, TKey> repository, 
    Expression<Func<T, bool>> predicate, 
    string errorMessage)
    where T : Domain.Objects.BaseDomain
{
    var exists = await repository.ExistAsync(predicate);
    if (exists)
        return opt.Failed(errorMessage);
    return opt.Succeeded();
}
}
}