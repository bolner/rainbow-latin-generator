namespace RainbowLatinReader;


interface IScheduler<INPUT_TYPE, RESULT_TYPE> {
    public void AddTask(ISchedulerTask<RESULT_TYPE> task);
    public void Run();
    public List<RESULT_TYPE> GetResults();
}
