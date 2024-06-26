﻿using System.Text;

namespace EmissionWiz.API.Code.Helpers;

/// <summary>
/// Captures the completed-line notifications from a <see cref="EventedStreamReader"/>,
/// combining the data into a single <see cref="string"/>.
/// </summary>
internal sealed class EventedStreamStringReader : IDisposable
{
    private readonly EventedStreamReader _eventedStreamReader;
    private readonly StringBuilder _stringBuilder = new();
    private bool _isDisposed;

    public EventedStreamStringReader(EventedStreamReader eventedStreamReader)
    {
        _eventedStreamReader = eventedStreamReader
                               ?? throw new ArgumentNullException(nameof(eventedStreamReader));
        _eventedStreamReader.OnReceivedLine += OnReceivedLine;
    }

    public string ReadAsString() => _stringBuilder.ToString();

    private void OnReceivedLine(string line) => _stringBuilder.AppendLine(line);

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _eventedStreamReader.OnReceivedLine -= OnReceivedLine;
            _isDisposed = true;
        }
    }
}