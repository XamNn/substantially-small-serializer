using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSSerializer.Json
{
    public class JSONWriter
    {
        public string Write(INode root, bool prettyPrint)
        {
            this.prettyPrint = prettyPrint;
            WriteNode(root);
            return builder.ToString();
        }

        StringBuilder builder = new StringBuilder();
        bool prettyPrint;
        bool nextline;
        int indent = 0;

        void WriteString(string s)
        {
            if (prettyPrint && nextline)
            {
                builder.Append('\n');
                builder.Append('\t', indent);
                nextline = false;
            }
            builder.Append(s);
        }
        void WriteNode(INode node)
        {
            if (node is StringNode val)
            {
                WriteString("\"");
                WriteString(val);
                WriteString("\"");
            }
            else if (node is ObjectNode obj)
            {
                if (obj.Count == 0)
                {
                    WriteString("{}");
                    return;
                }
                WriteString("{");
                indent++;
                nextline = true;
                bool isnotfirst = false;
                foreach (var x in obj)
                {
                    if (isnotfirst)
                    {
                        WriteString(",");
                        nextline = true;
                    }
                    else isnotfirst = true;
                    WriteString("\"");
                    WriteString(x.Key);
                    WriteString("\"");
                    if (prettyPrint) WriteString(" : ");
                    else WriteString(":");
                    WriteNode(x.Value);
                }
                indent--;
                nextline = true;
                WriteString("}");
            }
            else if (node is ArrayNode arr)
            {
                if (arr.Count == 0)
                {
                    WriteString("[]");
                    return;
                }
                WriteString("[");
                indent++;
                nextline = true;
                bool isnotfirst = false;
                foreach (var x in arr)
                {
                    if (isnotfirst)
                    {
                        WriteString(",");
                        nextline = true;
                    }
                    else isnotfirst = true;
                    WriteNode(x);
                }
                indent--;
                nextline = true;
                WriteString("]");
            }
        }
    }
}
