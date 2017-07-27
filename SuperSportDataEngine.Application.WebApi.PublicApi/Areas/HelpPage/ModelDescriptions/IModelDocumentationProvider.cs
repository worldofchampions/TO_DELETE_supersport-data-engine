using System;
using System.Reflection;

namespace SuperSportDataEngine.Application.WebApi.PublicApi.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}