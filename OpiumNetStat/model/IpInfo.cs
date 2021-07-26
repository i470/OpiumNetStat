using Prism.Mvvm;

namespace OpiumNetStat.model
{
    public class IpInfo:BindableBase
    {
        private string status;
        private string country;
        private string countryCode;
        private string region;
        private string city;
        private string org;
        private string ip;
        private string isp;
        private string query;

       

        public string Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        public string Country
        {
            get => country;
            set => SetProperty(ref country, value);
        }
        public string CountryCode
        {
            get => countryCode;
            set => SetProperty(ref countryCode, value);
        }
        public string Region
        {
            get => region;
            set => SetProperty(ref region, value);
        }
        public string City
        {
            get => city;
            set => SetProperty(ref city, value);
        }
        public string Org
        {
            get => org;
            set => SetProperty(ref org, value);
        }
        public string Ip
        {
            get => ip;
            set => SetProperty(ref ip, value);
        }
        public string Isp
        {
            get => isp;
            set => SetProperty(ref isp, value);
        }
        public string Query
        {
            get => query;
            set => SetProperty(ref query, value);
        }
        public string V { get; }
    }

   
}
