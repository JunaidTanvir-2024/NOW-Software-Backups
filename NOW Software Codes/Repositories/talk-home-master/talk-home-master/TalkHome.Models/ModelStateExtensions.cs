using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TalkHome.Models
{
    public static class ModelStateExtensions
    {
        public static List<string> RetrieveAllModelErrors(this ModelStateDictionary modelState, bool includeExceptions = false)
        {
            var list = modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();

            if (includeExceptions)
                list.AddRange(RetrieveAllModelExceptions(modelState));

            return list;
        }

        public static List<string> RetrieveAllModelExceptions(this ModelStateDictionary modelState)
        {
            return modelState.Keys.SelectMany(key => modelState[key].Errors)
                 .Select(x => x.Exception.ToString())
                 .ToList();
        }

    }
}
