using Serilog;

namespace SeriLogWebAPI
{
    public class LogContextEnrichment
    {
        private readonly RequestDelegate next;

        public LogContextEnrichment(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context,IDiagnosticContext diagnosticContext) // UserContext userContext, RequestContext requestContext, 
        {
            //diagnosticContext.Set(Constants.IdentityClaims.UserId, userContext.UserId);
            //diagnosticContext.Set("Enviroment", GeneralFunctions.RequestEnvironment.GetEnumStringValue());
            diagnosticContext.Set("ApplicationName", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            diagnosticContext.Set("ApplicationVersion", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);            

            //diagnosticContext.Set(Constants.HeaderTypes.Channel, userContext.Channel);
            //diagnosticContext.Set(Constants.HeaderTypes.UserAgent, requestContext.UserAgent);
            //diagnosticContext.Set(Constants.HeaderTypes.CorrelationId, requestContext.CorrelationId);
            await next(context);

        }
    }
}
