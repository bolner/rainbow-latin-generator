namespace RainbowLatinReader;

class SchedulerTask<RESULT_TYPE> : ISchedulerTask<RESULT_TYPE> {
    private readonly Func<RESULT_TYPE> resultFactory;
    private RESULT_TYPE? result;

    public SchedulerTask(Func<RESULT_TYPE> resultFactory) {
        this.resultFactory = resultFactory;
    }

    public void Run() {
        result = resultFactory();
    }

    public RESULT_TYPE? GetResult() {
        return result;
    }
}
