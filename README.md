# substantially-small-serializer
Very lightweight, efficient, and small data serializer and deserializer for c# supporting JSON and possibly other languages in the future.

HINT: add this to the top of your source file
```cs
using SSSerializer;
using SSSerializer.Json;
```

### Data Structures (INodes)

1. ValueNode.
A simple string value.
```cs
var MyString = "hello";
var myValueNode = (ValueNode)MyString;
```

2. ObjectNode.
A dictionary of string keys and INodes.
Works like a Dictionary<string, INode>.
```
var myObjectNode = new ObjectNode();
myObjectNode.Add("key", (ValueNode)"value");
```

3. ArrayNode.
A list of INodes.
Works like a List\<INode>.
```cs
var myArrayNode = new ArrayNode();
myArrayNode.Add((ValueNode)"value");
myArrayNode.Add(new ObjectNode {{ "key", (ValueNode)"value" }});
```

### INode to JSON
Serialize an INode into a string containing json:
```cs
var myJson = new JSONWriter().Write(MyINode, prettyPrint: true);
```

### JSON to INode
Deserialize a string containing json into an INode:
```cs
var myINode = new JSONReader().Read(myJson);
```
