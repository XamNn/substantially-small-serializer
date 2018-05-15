using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSerializer
{
    public interface INode
    {
    }
    public class ValueNode : INode
    {
        public string Value { get; set; }

        public static implicit operator ValueNode(string value)
        {
            return new ValueNode { Value = value };
        }
        public static implicit operator string(ValueNode value)
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