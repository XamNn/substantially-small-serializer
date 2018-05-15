using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSerializer.Json
{
    public class JSONReader
    {
        public INode Read(string content)
        {
            TokenStream = StreamTokensFrom(content);
            return NextValue(true);
        }

        [Serializable]
        public class JSONParsingException : Exception
        {
            public JSONParsingException() { }
            public JSONParsingException(string message) : base(message) { }
            public JSONParsingException(string message, Exception inner) : base(message, inner) { }
            protected JSONParsingException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        IEnumerator<Token> TokenStream;

        int line = 1, column = 1;
        string GetLocationString()
        {
            return line.ToString() + " : " + column.ToString();
        }

        INode NextValue(bool move)
        {
            if (!move || TokenStream.MoveNext())
            {
                if (TokenStream.Current.TokenType == TokenType.String)
                {
                    return (ValueNode)TokenStream.Current.Match;
                }
                if (TokenStream.Current.TokenType == TokenType.ObjectStart)
                {
                    var obj = new ObjectNode();
                    for(; ; )
                    {
                        if (TokenStream.MoveNext())
                        {
                            if (TokenStream.Current.TokenType == TokenType.ObjectEnd) return obj;
                            var (key, node) = NextKeyNodePair(false);
                            obj.Add(key, node);
                            if (TokenStream.MoveNext())
                            {
                                if (TokenStream.Current.TokenType == TokenType.Comma) continue;
                                if (TokenStream.Current.TokenType == TokenType.ObjectEnd) return obj;
                            }
                            throw new JSONParsingException("Expected comma ',' or end of object '}' at " + GetLocationString());
                        }
                        throw new JSONParsingException("Expected key or end of object '}' at " + GetLocationString());
                    }
                }
                if (TokenStream.Current.TokenType == TokenType.ArrayStart)
                {
                    var arr = new ArrayNode();
                    for(; ; )
                    {
                        if (TokenStream.MoveNext())
                        {
                            if (TokenStream.Current.TokenType == TokenType.ArrayEnd) return arr;
                            arr.Add(NextValue(false));
                            if (TokenStream.MoveNext())
                            {
                                if (TokenStream.Current.TokenType == TokenType.Comma) continue;
                                if (TokenStream.Current.TokenType == TokenType.ArrayEnd) return arr;
                            }
                            throw new JSONParsingException("Expected comma ',' or end of array ']' at " + GetLocationString());
                        }
                        throw new JSONParsingException("Expected value, object '{', array '[', or end of array ']' at " + GetLocationString());
                    }
                }
            }
            throw new JSONParsingException("Expected value, object '{', or array '[' at" + GetLocationString());
        }
        (string, INode) NextKeyNodePair(bool move)
        {
            if ((!move || TokenStream.MoveNext()) && TokenStream.Current.TokenType == TokenType.String)
            {
                var key = TokenStream.Current.Match;
                if (TokenStream.MoveNext() && TokenStream.Current.TokenType == TokenType.Colon)
                {
                    return (key, NextValue(true));
                }
                throw new JSONParsingException("Colon ':' expected at " + GetLocationString());
            }
            throw new JSONParsingException("Key expected at " + GetLocationString());
        }

        enum TokenType
        {
            String,
            Comma,
            Colon,
            ObjectStart,
            ObjectEnd,
            ArrayStart,
            ArrayEnd,
        }
        struct Token
        {
            public TokenType TokenType;
            public string Match;
        }
        IEnumerator<Token> StreamTokensFrom(string source)
        {
            void LineInc()
            {
                line++;
                column = 0;
            }

            for (int i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (c == '\n') LineInc();
                else if (char.IsWhiteSpace(c)) ;
                else if (c == '"')
                {
                    int starti = i + 1;
                    for (; ; )
                    {
                        i++;
                        column++;
                        if (source.Length == i) throw new JSONParsingException("Unexpected end of input at " + GetLocationString());
                        if (source[i] == '"') break;
                    }
                    yield return new Token { Match = source.Substring(starti, i - starti), TokenType = TokenType.String };
                }
                else if (c == ',') yield return new Token { Match = ",", TokenType = TokenType.Comma };
                else if (c == ':') yield return new Token { Match = ":", TokenType = TokenType.Colon };
                else if (c == '{') yield return new Token { Match = "{", TokenType = TokenType.ObjectStart };
                else if (c == '}') yield return new Token { Match = "}", TokenType = TokenType.ObjectEnd };
                else if (c == '[') yield return new Token { Match = "[", TokenType = TokenType.ArrayStart };
                else if (c == ']') yield return new Token { Match = "]", TokenType = TokenType.ArrayEnd };
                else throw new JSONParsingException("Unexpected character at " + GetLocationString());
                column++;
            }
        }
    }
}
