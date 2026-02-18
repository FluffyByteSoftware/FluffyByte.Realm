/*
 * (DiskManagerEvents.cs)
 *------------------------------------------------------------
 * Created - Monday, February 9, 2026@7:02:15 AM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Tools.Broadcasting.Events;

public class RequestFileReadEvent : EventArgs
{
    public string FilePath { get; set; } = string.Empty;
    private byte[]? _data;

    public void SetData(byte[]? data) => _data = data;
    public byte[]? GetData() => _data;
}

public class RequestFileReadTextEvent : EventArgs
{
    public string FilePath { get; set; } = string.Empty;
    private string? _text;

    public void SetText(string? text) => _text = text;
    public string? GetText() => _text;
}

public class RequestFileWriteByteEvent : EventArgs
{
    public byte[] Data { get; set; } = [];
    public string FilePath { get; set; } = string.Empty;
}

public class RequestFileWriteTextEvent : EventArgs
{
    public string Text { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}

public class LogWriteEvent : EventArgs
{
    public string Message { get; set; } = string.Empty;
}

/*
 *------------------------------------------------------------
 * (DiskManagerEvents.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */