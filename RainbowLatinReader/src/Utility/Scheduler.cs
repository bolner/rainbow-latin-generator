namespace RainbowLatinReader;

class Scheduler<INPUT_TYPE, RESULT_TYPE> : IScheduler<INPUT_TYPE, RESULT_TYPE> {
    private readonly int threadCount;
    private int taskCount = 0;
    private readonly List<List<ISchedulerTask<RESULT_TYPE>>> taskBuckets = new();
    private readonly List<RESULT_TYPE> results = new();

    public Scheduler(int threadCount) {
        this.threadCount = threadCount;

        for(int i = 0; i < threadCount; i++) {
            taskBuckets.Add(new List<ISchedulerTask<RESULT_TYPE>>());
        }
    }

    public void AddTask(ISchedulerTask<RESULT_TYPE> task) {
        taskBuckets[taskCount % threadCount].Add(task);
        taskCount++;
    }

    public void Run() {
        var threads = new List<Thread>();

        foreach(var bucket in taskBuckets) {
            if (bucket.Count < 1) {
                continue;
            }

            var thread = new Thread(new ParameterizedThreadStart(Worker));
            threads.Add(thread);
            thread.Start(bucket);
        }

        foreach(var thread in threads) {
            thread.Join();
        }

        foreach(var bucket in taskBuckets) {
            if (bucket.Count < 1) {
                continue;
            }

            foreach(var task in bucket) {
                RESULT_TYPE? output = task.GetResult();

                if (output != null) {
                    results.Add(output);
                }
            }
        }
    }

    private static void Worker(object? param)
    {
        if (param == null) {
            return;
        }

        List<ISchedulerTask<RESULT_TYPE>> tasks = (List<ISchedulerTask<RESULT_TYPE>>)param;

        foreach(var task in tasks) {
            task.Run();
        }
    }

    public List<RESULT_TYPE> GetResults() {
        return results;
    }
}
