namespace CSharpDiscovery.Quest03 {
    public class HistoricalMonument : PointOfInterest {
        public int BuildYear {
            get;set;
        }
        public HistoricalMonument(){}
        public HistoricalMonument(string Name, double Latitude, double Longitude, int buildyear) : base (Name, Latitude, Longitude) {
            BuildYear = buildyear;
        }
        public override string ToString() {
            string result = $"{this.Name} is a historical monument built in {BuildYear}";
            return result;
        }
    }
}