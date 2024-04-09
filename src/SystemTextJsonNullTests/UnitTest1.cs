using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
    public void RequireValueNullShouldBeException()
    {
        var json = """
        {
            "RequireValue": null
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Null(bcl.RequireValue);

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions));
    }

    [Fact]
    public void RequireValueNullShouldBeException2()
    {
        var json = "{}";

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Dto>(json));

        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions));
    }

    [Fact]
    public void NullableValueShouldBeNull()
    {
        var json = """
        {
            "RequireValue": "...",
            "NullableValue": null
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Null(bcl.NullableValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Null(aums.NullableValue);
    }

    [Fact]
    public void NullableValueShouldBeNull2()
    {
        var json = """
        {
            "RequireValue": "..."
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Null(bcl.NullableValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Null(aums.NullableValue);
    }

    [Fact]
    public void NullableValueExpectCCC()
    {
        var json = """
        {
            "RequireValue": "...",
            "NullableValue": "CCC"
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Equal("CCC", bcl.NullableValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal("CCC", aums.NullableValue);
    }

    [Fact]
    public void NotNullDefaultValueShouldBeDefault()
    {
        var json = """
        {
            "RequireValue": "...",
            "NotNullDefaultValue": null
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Null(bcl.NotNullDefaultValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal("initialized", aums.NotNullDefaultValue);
    }

    [Fact]
    public void NotNullDefaultValueShouldBeDefault2()
    {
        var json = """
        {
            "RequireValue": "..."
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Equal("initialized", bcl.NotNullDefaultValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal("initialized", aums.NotNullDefaultValue);
    }

    [Fact]
    public void NotNullDefaultValueShouldBeKKK()
    {
        var json = """
        {
            "RequireValue": "...",
            "NotNullDefaultValue": "KKK"
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Equal("KKK", bcl.NotNullDefaultValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal("KKK", aums.NotNullDefaultValue);
    }

    [Fact]
    public void NullableDefaultValueShouldBeNull()
    {
        var json = """
        {
            "RequireValue": "...",
            "NullableDefaultValue": null
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Equal("initialized", bcl.NotNullDefaultValue);

        var sut = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Null(sut.NullableDefaultValue);
    }

    [Fact]
    public void NullableDefaultValueShouldBeDefault()
    {
        var json = """
        {
            "RequireValue": "..."
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Equal("initialized", bcl.NotNullDefaultValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal("initialized", aums.NullableDefaultValue);
    }

    [Fact]
    public void NullableDefaultValueShouldBeAAA()
    {
        var json = """
        {
            "RequireValue": "...",
            "NullableDefaultValue": "AAA"
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal("AAA", bcl.NullableDefaultValue);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal("AAA", aums.NullableDefaultValue);
    }

    [Fact]
    public void StringValuesShouldBeEmpty()
    {
        var json = """
        {
            "RequireValue": "..."
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Equal([], bcl.StringValues);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal([], aums.StringValues);
    }

    [Fact]
    public void StringValuesShouldBeEmpty2()
    {
        var json = """
        {
            "RequireValue": "...",
            "StringValues": null
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Null(bcl.StringValues);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal([], aums.StringValues);
    }

    [Fact]
    public void StringValuesShouldBeCCC()
    {
        var json = """
        {
            "RequireValue": "...",
            "StringValues": ["CCC"]
        }
        """;

        var bcl = JsonSerializer.Deserialize<Dto>(json);
        Assert.Equal(["CCC"], bcl.StringValues);

        var aums = JsonSerializer.Deserialize<Dto>(json, JsonSerializerOptions);
        Assert.Equal(["CCC"], aums.StringValues);
    }
}


public record Dto
{
    public required string RequireValue { get; init; }
    public string? NullableValue { get; init; }
    public string NotNullDefaultValue { get; init; } = "initialized";
    public string? NullableDefaultValue { get; init; } = "initialized";
    public IEnumerable<string> StringValues { get; init; } = [];
}
