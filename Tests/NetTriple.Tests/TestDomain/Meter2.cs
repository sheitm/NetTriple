namespace NetTriple.Tests.TestDomain
{
    public class Meter2
    {
        public string ManufacturerSerialNumber { get; set; }
        public string NetOwnerAssetId { get; set; }
        public string Label { get; set; }
        public string AssetModelName { get; set; }
        public string AssetState { get; set; }
        public string AssetLocation { get; set; }
        public int ManufacturingYear { get; set; }
        public string Firmware { get; set; }
        public int MeterInterval { get; set; }

        public int ServiceIntervalEntity { get; set; }
        public int ServiceIntervalValue { get; set; }
    }
}
