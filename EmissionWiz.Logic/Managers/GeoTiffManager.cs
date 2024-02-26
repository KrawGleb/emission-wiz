using BitMiracle.LibTiff.Classic;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers;

internal class GeoTiffManager : BaseManager, IGeoTiffManager
{
    public void BuildTiffFromFlatArray(double[] source)
    {
        var size = source.Length * 2;

        using var output = Tiff.Open("test.tif", "w");
        output.SetField(TiffTag.IMAGEWIDTH, size);
        output.SetField(TiffTag.IMAGELENGTH, size);
        output.SetField(TiffTag.SAMPLESPERPIXEL, 1);
        output.SetField(TiffTag.BITSPERSAMPLE, 8);
        output.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
        output.SetField(TiffTag.ROWSPERSTRIP, size);
        output.SetField(TiffTag.XRESOLUTION, 88.0);
        output.SetField(TiffTag.YRESOLUTION, 88.0);
        output.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);
        output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
        output.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
        output.SetField(TiffTag.COMPRESSION, Compression.NONE);
        output.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);

        var byteArray = BuildByteArray(source);
        for (var i = 0; i < byteArray.Length; i++)
        {
            output.WriteScanline(byteArray[i], i);
        }

        output.WriteDirectory();
    }

    private byte[][] BuildByteArray(double[] source)
    {
        var halfOfSize = source.Length;
        var size = halfOfSize * 2;
        var result = new byte[size][];

        var max = source.Max();
        var unifiedArr = source.Select(x => (byte)(x / max * 255)).ToArray();

        for (var i = 0; i < size; i++)
        {
            result[i] = new byte[size];

            for (var j = 0; j < size; j++)
            {
                var distance = Math.Sqrt(Math.Pow(Math.Abs(i - halfOfSize), 2) + Math.Pow(Math.Abs(j - halfOfSize), 2));
                distance = distance > halfOfSize - 1 ? halfOfSize - 1 : distance;

                result[i][j] = unifiedArr[(int)Math.Floor(distance)];
            }
        }

        return result;
    }

}
