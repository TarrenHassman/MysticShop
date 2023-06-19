namespace JSONExporter
{
    public class JESaleInfo: JEComponent
    {
        public SaleInfo saleInfo;
        override public void Preprocess()
        {
            saleInfo = unityComponent as SaleInfo;
        }

        override public void QueryResources()
        {
        }

        new public static void Reset()
        {

        }

        public override JSONComponent ToJSON()
        {
            var json = new JSONSaleInfo();
            json.type = "SaleInfo";
            json.seller = saleInfo.seller;
            return json;
        }
        
    }
}