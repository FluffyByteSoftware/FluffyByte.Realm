/*
 * (CachedFile.cs)
 *------------------------------------------------------------
 * Created - Sunday, February 8, 2026@8:20:23 PM
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

namespace FluffyByte.Realm.Tools.Disk;

public class CachedFile
{
    public string FilePath { get; set; } = string.Empty;
    public byte[] Data { get; set; } = [];
    public bool IsDirty { get; set; }
    public DateTime LastAccessed { get; set; }
}

/*
 *------------------------------------------------------------
 * (CachedFile.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */