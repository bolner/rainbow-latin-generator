namespace RainbowLatinReader;

class SchedulerTask<RESULT_TYPE> : ISchedulerTask<RESULT_TYPE> {
    private readonly Func<RESULT_TYPE> lambda;
    private RESULT_TYPE? result;

    public SchedulerTask(Func<RESULT_TYPE> lambda) {
        this.lambda = lambda;
    }

    public void Run() {
        result = lambda();
    }

    public RESULT_TYPE? GetResult() {
        return result;
    }
}
