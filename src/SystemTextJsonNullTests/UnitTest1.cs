using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Newtonsoft.Json.Linq;

namespace SystemTextJsonNullTests;

public class UnitTest1
{
    public static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { InterceptNullSetter, }
        }
    };

    public static void InterceptNullSetter(JsonTypeInfo typeInfo)
    {
        foreach (var (propertyInfo, setProperty) in from propertyInfo in typeInfo.Properties
                                                    let setProperty = propertyInfo.Set
                                                    where setProperty is not null
                                                    select (propertyInfo, setProperty))
        {
            propertyInfo.Set = (obj, value) =>
            {
                if (value is null)
                {
                    if (propertyInfo.IsRequired)
                    {
                        throw new JsonException($"Null value not allowed for '{propertyInfo.Name}'");
                    }

                    if (NullabilityInfo(propertyInfo)?.WriteState is NullabilityState.Nullable)
                    {
                        setProperty(obj, value);
                    }
                }
                else
                {
                    setProperty(obj, value);
                }
            };
        }

        static NullabilityInfo? NullabilityInfo(JsonPropertyInfo value)
        {
            NullabilityInfoContext context = new();

            return value.AttributeProvider switch
            {
                FieldInfo fieldInfo => context.Create(fieldInfo),
                PropertyInfo propertyInfo => context.Create(propertyInfo),
                _ => null
            };
        }
    }

    [Fact]
    public void RequireValueNull()
    {
        var json = """
        {
            "RequireValue": null
        }
        """;

        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions));

        Assert.Equal("Null value not allowed for 'requireValue'", exception.Message);
    }

    [Fact]
    public void RequireValueNull2()
    {
        var json = "{}";

        var exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions));

        Assert.Equal("JSON deserialization for type 'SystemTextJsonNullTests.Dto' was missing required properties, including the following: requireValue", exception.Message);
    }

    [Fact]
    public void NotNullDefaultValueNull()
    {
        var json = """
        {
            "RequireValue": "",
            "NotNullDefaultValue": null
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal(sut.NotNullDefaultValue, "");
    }

    [Fact]
    public void NotNullDefaultValueNull2()
    {
        var json = """
        {
            "RequireValue": ""
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal(sut.NotNullDefaultValue, "");
    }

    [Fact]
    public void NullableDefaultValueExpectDefault()
    {
        var json = """
        {
            "RequireValue": ""
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal("", sut.NullableDefaultValue);
    }

    [Fact]
    public void NullableDefaultValueExpectNull()
    {
        var json = """
        {
            "RequireValue": "",
            "NullableDefaultValue": null
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal(sut.NullableDefaultValue, null);
    }

    [Fact]
    public void NullableDefaultValueExpectAAA()
    {
        var json = """
        {
            "RequireValue": "",
            "NullableDefaultValue": "AAA"
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal("", sut.RequireValue);
        Assert.Equal("AAA", sut.NullableDefaultValue);
    }

    [Fact]
    public void NullableValueExpectDefault()
    {
        var json = """
        {
            "RequireValue": ""
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal(sut.NullableValue, null);
    }

    [Fact]
    public void NullableValueExpectNull()
    {
        var json = """
        {
            "RequireValue": "",
            "NullableValue": null
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal(sut.NullableValue, null);
    }

    [Fact]
    public void NullableValueExpectCCC()
    {
        var json = """
        {
            "RequireValue": "",
            "NullableValue": "CCC"
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal("", sut.RequireValue);
        Assert.Equal(sut.NullableValue, "CCC");
    }


    [Fact]
    public void StringValuesExpectDefault()
    {
        var json = """
        {
            "RequireValue": ""
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal(sut.StringValues, []);
    }

    [Fact]
    public void StringValuesExpectNull()
    {
        var json = """
        {
            "RequireValue": "",
            "StringValues": null
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal(sut.RequireValue, "");
        Assert.Equal(sut.StringValues, []);
    }

    [Fact]
    public void StringValuesExpectCCC()
    {
        var json = """
        {
            "RequireValue": "",
            "StringValues": ["CCC"]
        }
        """;

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);

        Assert.Equal("", sut.RequireValue);
        Assert.Equal(sut.StringValues, ["CCC"]);
    }
}


public record Dto
{
    public required string RequireValue { get; init; }
    public string NotNullDefaultValue { get; init; } = "";
    public string? NullableDefaultValue { get; init; } = "";
    public string? NullableValue { get; init; }
    public IEnumerable<string> StringValues { get; init; } = [];
}
