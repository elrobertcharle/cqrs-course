using System;

namespace Post.Cmd.Api.Producer.Options
{
    public class OutboxPollingWorkerOptions
    {
        public int TotalWorkers { get; set; }
    }
}
