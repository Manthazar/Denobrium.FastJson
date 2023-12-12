using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Denobrium.Json.Serialization
{
    /// <summary>
    /// Represents a token within a json string.
    /// </summary>
    internal enum Token
    {
        None = -1,           // Used to denote no Lookahead available
        Curly_Open,
        Curly_Close,
        Squared_Open,
        Squared_Close,
        Colon,
        Comma,
        String,
        Number,
        True,
        False,
        Null
    }
}
