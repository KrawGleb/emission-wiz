using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Exceptions;
using EmissionWiz.Models.Interfaces.Builders;

namespace EmissionWiz.Logic.Builders;

internal class GeoKeyDirectoryBuilder : IGeoKeyDirectoryBuilder
{
    class KeyEntry
    {
        /// <summary>
        /// Key-ID value of the Key
        /// </summary>
        public ushort KeyId { get; set; }

        /// <summary>
        /// Which TIFF tag contains the values(s) of the Key (0, TiffTag.GEOTIFF_GEODOUBLEPARAMSTAG, TiffTag.GEOTIFF_GEOASCIIPARAMSTAG) 
        /// </summary>
        public ushort TIFFTagLocation { get; set; }

        /// <summary>
        /// Number of vaalues in this key
        /// </summary>
        public ushort Count { get; set; }

        /// <summary>
        /// Actual value of the Key (if TIFFTagLocation == 0) otherwise index
        /// </summary>
        public ushort ValueOffset { get; set; }
    }

    /// <summary>
    /// Current version of Key implementation
    /// </summary>
    private readonly short _keyDirectoryVersion = 1;

    /// <summary>
    /// Revision of Key-Sets are used
    /// </summary>
    private readonly short _keyRevision = 1;

    /// <summary>
    /// Set of Key-codes are used
    /// </summary>
    private readonly short _minorRevision = 1;

    /// <summary>
    /// How many Keys are defined by the rest of this Tag
    /// </summary>
    private int _numberOfKeys => Keys.Count;

    private List<KeyEntry> Keys { get; set; } = new();
    private List<double> GeoDoubleParamsTag { get; set; } = new();
    private List<string> GeoAsciiParamsTag { get; set; } = new();

    public IGeoKeyDirectoryBuilder AddKey<T>(GeoKey<T> key)
    {
        var isShortValue = typeof(T) == typeof(ushort);

        var entry = new KeyEntry
        {
            Count = (ushort)key.Count,
            KeyId = key.KeyId,
            TIFFTagLocation = key.TIFFTagLocation,
            ValueOffset = (ushort)(isShortValue ? key.Values.Cast<ushort>().First() : 0)
        };

        if (!isShortValue)
        {
            int offset;
            if (typeof(T) == typeof(double))
            {
                offset = GeoDoubleParamsTag.Count;
                GeoDoubleParamsTag.AddRange(key.Values.Cast<double>());
            }
            else if (typeof(T) == typeof(string))
            {
                offset = GeoAsciiParamsTag.Count;
                GeoAsciiParamsTag.AddRange(key.Values.Cast<string>());
            }
            else
                throw new AppException("Unsupported value type");

            entry.ValueOffset = (ushort)offset;
        }

        Keys.Add(entry);

        return this;
    }

    public GeoKeyDirectoryResult Build()
    {
        IEnumerable<int> geoKeyDirectoryTag = [_keyDirectoryVersion, _keyRevision, _minorRevision, _numberOfKeys];
        foreach (var key in Keys)
            geoKeyDirectoryTag = geoKeyDirectoryTag.Concat([key.KeyId, key.TIFFTagLocation, key.Count, key.ValueOffset]);

        var result = new GeoKeyDirectoryResult()
        {
            GeoKeyDirectoryTag = geoKeyDirectoryTag.ToArray(),
            GeoDoubleParamsTag = [GeoDoubleParamsTag.Count, .. GeoDoubleParamsTag.Cast<object>()],
            GeoAsciiParamsTag = [GeoAsciiParamsTag.Count, .. GeoAsciiParamsTag.Cast<object>()],
        };

        GeoAsciiParamsTag.Clear();
        GeoDoubleParamsTag.Clear();

        return result;
    }
}
