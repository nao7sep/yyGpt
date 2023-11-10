﻿namespace yyGptLib
{
    // Could be an enum, but I want to make sure the value is returned as a lowercase string.

    public partial class yyGptChatMessageRole: IEquatable <yyGptChatMessageRole>
    {
        public string Value { get; private set; }

        public yyGptChatMessageRole (string value) => Value = value;

        public bool Equals (yyGptChatMessageRole? role) => Value.Equals (role?.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals (object? obj) => Equals (obj as yyGptChatMessageRole);

        public override int GetHashCode () => Value.GetHashCode ();

        public override string ToString () => Value;
    }
}
