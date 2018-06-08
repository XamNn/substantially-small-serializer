# substantially-small-serializer
Very lightweight, efficient, and small data serializer and deserializer for c# supporting JSON and possibly other languages in the future.

HINT: add this to the top of your source file
```cs
using SSSerializer;
using SSSerializer.Json;
```

### Data Structures (Nodes)

1. StringNode.
A simple string value.
```cs
var MyString = "hello";
var myValueNode = (ValueNode)MyString;
```

2. ObjectNode.
A dictionary of string keys and Nodes.
```
var myObjectNode = new ObjectNode();
myObjectNode.Items.Add("key", (ValueNode)"value");
```

3. ArrayNode.
A list of INodes.
```cs
var myArrayNode = new ArrayNode();
myArrayNode.Items.Add((ValueNode)"value");
myArrayNode.Items.Add(new ObjectNode {{ "key", (ValueNode)"value" }});
```

### Node to JSON
Serialize an INode into a string containing json:
```cs
var myJson = new JSONWriter().Write(MyNode, prettyPrint: true);
```

### JSON to Node
Deserialize a string containing json into an INode:
```cs
var myINode = new JSONReader().Read(myJson);
```
