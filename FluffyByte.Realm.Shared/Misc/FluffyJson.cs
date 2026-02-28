/*
 * (FluffyJson.cs)
 *------------------------------------------------------------
 * Created - 2/28/2026 7:41:51 AM
 * Created by - Seliris
 *-------------------------------------------------------------
 */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluffyByte.Realm.Shared.Misc
{
    public static class FluffyJson
    {
        public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }



    /*
     *------------------------------------------------------------
     * (FluffyJson.cs)
     * See License.txt for licensing information.
     *-----------------------------------------------------------
     */
}