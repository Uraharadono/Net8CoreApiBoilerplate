using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Net7CoreApiBoilerplate.Api.Utility.Extensions
{
    public static class ModelStateExtensions
    {
        public static string GetErrorMessage(ModelStateDictionary state)
        {
            string valueToLog = "Server side validation fail on: " + DateTime.Now + ".  \n";
            foreach (var modelState in state.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    valueToLog += error.ErrorMessage + "\n";
                }
            }
            return valueToLog;
        }
    }
}
