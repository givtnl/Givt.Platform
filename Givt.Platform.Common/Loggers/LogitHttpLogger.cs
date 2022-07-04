using Serilog;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.Logger;

namespace Givt.Platform.Common.Loggers
{
    public class LogitHttpLogger : CallerMemberLogger
    {
        public LogitHttpLogger(string tag, string apiKey)
        {
            string logitUri = "https://api.logit.io/v2";

            _logger = new LoggerConfiguration().Enrich.WithProperty("tag", tag)
                    .WriteTo.HttpSink(logitUri, apiKey).CreateLogger();
        }
    }
}
