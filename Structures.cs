using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSSerializer
{
    public interface INode
    {
    }
    public class StringNode : INode
    {
        public string Value { get; set; }

        public static implicit operator StringNode(string value)
        {
            return new StringNode { Value = value };
        }
        public static implicit operator string(StringNode value)
        {
            return value.Value;
        }
    }
    public class ObjectNode : Dictionary<string, INode>, INode
    {
        public new INode this[string key]
        {
            get
            {
                //we get rid of the usual exception and simply return null if key was not present
                if (!TryGetValue(key, out var node)) return null;
                return node;
            }
            set
            {
                base[key] = value;
            }
        }
    }
    public class ArrayNode : List<INode>, INode { }
}