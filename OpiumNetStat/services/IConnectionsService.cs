namespace OpiumNetStat.services
{
    public interface IConnectionsService
    {
        void StartWork();
        void Get24HourDataAsync();
    }
}