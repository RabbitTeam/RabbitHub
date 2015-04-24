using System.Web.Razor.Generator;
using System.Web.WebPages.Razor;

namespace Rabbit.Web.Mvc.Mvc.ViewEngines.Razor
{
    internal interface IRazorCompilationEvents
    {
        void CodeGenerationStarted(RazorBuildProvider provider);

        void CodeGenerationCompleted(RazorBuildProvider provider, CodeGenerationCompleteEventArgs e);
    }
}