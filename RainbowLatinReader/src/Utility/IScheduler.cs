namespace RainbowLatinReader;


interface IScheduler<PAYLOAD_TYPE> where PAYLOAD_TYPE: IProcessable {
    public void AddTask(PAYLOAD_TYPE task);
    public void Run();
    public List<PAYLOAD_TYPE> GetResults();
}
