using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApplicationMVC.Areas.Template.Configurations;

namespace WebApplicationMVC.Areas.Template.Attributes
{
    public class AllowLoginAttribute : Attribute, IPageFilter
    {
        void IPageFilter.OnPageHandlerExecuted(PageHandlerExecutedContext context) { }

        void IPageFilter.OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (!ApplicationConfiguration.AllowLogin)
            {
                context.Result = new RedirectResult("/Home/Error?statusCode=404");
            }
        }

        void IPageFilter.OnPageHandlerSelected(PageHandlerSelectedContext context) { }
    }
}
