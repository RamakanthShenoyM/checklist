Yes, you can serialize a polymorphic object graph to JSON in C# using either **System.Text.Json** (the native JSON library) or **Newtonsoft.Json**. However, they handle polymorphism differently.

## **Using System.Text.Json (Native C# JSON)**
By default, `System.Text.Json` does not support polymorphic serialization out-of-the-box. You must enable it explicitly using the `[JsonDerivedType]` attribute or a custom `JsonConverter`.

### **Option 1: Using `JsonDerivedType` (C# 7.0 and later)**
```csharp
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class Root
{
    public string Name { get; set; }
    public List<BaseType> Items { get; set; } = new();
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DerivedTypeA), typeDiscriminator: "A")]
[JsonDerivedType(typeof(DerivedTypeB), typeDiscriminator: "B")]
public abstract class BaseType
{
    public string CommonProperty { get; set; }
}

public class DerivedTypeA : BaseType
{
    public int SpecificToA { get; set; }
}

public class DerivedTypeB : BaseType
{
    public bool SpecificToB { get; set; }
}

class Program
{
    static void Main()
    {
        var root = new Root
        {
            Name = "Example",
            Items = new List<BaseType>
            {
                new DerivedTypeA { CommonProperty = "A1", SpecificToA = 42 },
                new DerivedTypeB { CommonProperty = "B1", SpecificToB = true }
            }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

        string json = JsonSerializer.Serialize(root, options);
        Console.WriteLine(json);
    }
}
```
### **Output**
```json
{
  "Name": "Example",
  "Items": [
    {
      "$type": "A",
      "CommonProperty": "A1",
      "SpecificToA": 42
    },
    {
      "$type": "B",
      "CommonProperty": "B1",
      "SpecificToB": true
    }
  ]
}
```
### **Deserialization**
This setup will also allow correct deserialization:
```csharp
var deserializedRoot = JsonSerializer.Deserialize<Root>(json, options);
```
---
### **Option 2: Using Custom `JsonConverter<BaseType>`**
If you're using an older version of C# or want more control, you can use a custom converter.

---

## **Using Newtonsoft.Json**
Newtonsoft.Json provides built-in support for polymorphism using the `[JsonConverter(typeof(StringEnumConverter))]` and `$type` metadata.

```csharp
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Root
{
    public string Name { get; set; }
    public List<BaseType> Items { get; set; } = new();
}

[JsonConverter(typeof(Newtonsoft.Json.Converters.JsonSubtypes), "Type")]
[JsonSubtypes.KnownSubType(typeof(DerivedTypeA), "A")]
[JsonSubtypes.KnownSubType(typeof(DerivedTypeB), "B")]
public abstract class BaseType
{
    public string CommonProperty { get; set; }
    public abstract string Type { get; }
}

public class DerivedTypeA : BaseType
{
    public override string Type => "A";
    public int SpecificToA { get; set; }
}

public class DerivedTypeB : BaseType
{
    public override string Type => "B";
    public bool SpecificToB { get; set; }
}

class Program
{
    static void Main()
    {
        var root = new Root
        {
            Name = "Example",
            Items = new List<BaseType>
            {
                new DerivedTypeA { CommonProperty = "A1", SpecificToA = 42 },
                new DerivedTypeB { CommonProperty = "B1", SpecificToB = true }
            }
        };

        string json = JsonConvert.SerializeObject(root, Formatting.Indented);
        Console.WriteLine(json);

        var deserializedRoot = JsonConvert.DeserializeObject<Root>(json);
    }
}
```
### **Output**
```json
{
  "Name": "Example",
  "Items": [
    {
      "Type": "A",
      "CommonProperty": "A1",
      "SpecificToA": 42
    },
    {
      "Type": "B",
      "CommonProperty": "B1",
      "SpecificToB": true
    }
  ]
}
```
---
## **Which One Should You Use?**
- If you're using **.NET 7+**, `System.Text.Json` with `[JsonDerivedType]` is the recommended way.
- If you're using **older .NET versions** or require more flexibility, **Newtonsoft.Json** provides more mature support.

Would you like help tweaking the implementation for a specific use case?