namespace RainbowLatinReader;

interface ISchedulerTask<RESULT_TYPE> {
    public void Run();
    public RESULT_TYPE? GetResult();
}
