namespace EmissionWiz.Models.Dto;

public class GeoKey<T>
{
    /// <summary>
    /// Key-ID value of the Key
    /// </summary>
    public ushort KeyId { get; set; }

    /// <summary>
    /// Which TIFF tag contains the values(s) of the Key
    /// </summary>
    public ushort TIFFTagLocation { get; set; }

    /// <summary>
    /// Number of vaalues in this key
    /// </summary>
    public int Count => Values.Count;

    public List<T> Values { get; set; } = new();
}
