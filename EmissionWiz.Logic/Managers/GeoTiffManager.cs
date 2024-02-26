namespace EmissionWiz.Logic.Managers;

internal class GeoTiffManager : BaseManager
{
    public void BuildTiffFromFlatArray(decimal[] source)
    {

    }

    private byte[] BuildByteArray(decimal[] source)
    {
        var halfOfSize = source.Length;
        var size = halfOfSize * 2;
        var result = new byte[size, size];

        var max = source.Max();
        var unifiedArr = source.Select(x => (byte)(x / max * 255));



        return new byte[0];
    }

}
