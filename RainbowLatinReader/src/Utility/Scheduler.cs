/*
Copyright 2024 Tamas Bolner

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
namespace RainbowLatinReader;

class Scheduler<PAYLOAD_TYPE> : IScheduler<PAYLOAD_TYPE> where PAYLOAD_TYPE: IProcessable {
    private readonly int threadCount;
    private int taskCount = 0;
    private readonly List<List<PAYLOAD_TYPE>> taskBuckets = new();
    private readonly List<PAYLOAD_TYPE> results = new();

    public Scheduler(int threadCount) {
        this.threadCount = threadCount;

        for(int i = 0; i < threadCount; i++) {
            taskBuckets.Add([]);
        }
    }

    public void AddTask(PAYLOAD_TYPE task) {
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
                if (task != null) {
                    results.Add(task);
                }
            }
        }
    }

    private static void Worker(object? param)
    {
        if (param == null) {
            return;
        }

        List<PAYLOAD_TYPE> tasks = (List<PAYLOAD_TYPE>)param;

        foreach(var task in tasks) {
            task.Process();
        }
    }

    public List<PAYLOAD_TYPE> GetResults() {
        return results;
    }
}
