namespace TaskScheduler.Models
{
    public class SchedulerConfiguration
    {
        public int TaskExecutionDelayDays { get; set; }
        public int DataExpirationDays { get; set; }
        
        public int TaskExecutionDelayInMilliseconds
        {
            get
            {
                return TaskExecutionDelayDays * 24 * 60 * 60 * 1000;
            }
        }

        public string RequestUrl { get; set; }
    }
}
