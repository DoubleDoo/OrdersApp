


namespace testkontur.WebsitesClasses
{
    public abstract class OrdersWebSiteDataElements
    {
        public abstract string GetTypeData(string link);
        public abstract string GenerateSearchLink(string keyWord);
        public abstract string GetOrdererData(string link);
        public abstract string GetFederalData(string link);
        public abstract string GetCityData(string link);
        public abstract string GetDateData(string link);
        public abstract string GetInfoData(string link);
        public abstract string GetPriceData(string link);
    }
}
