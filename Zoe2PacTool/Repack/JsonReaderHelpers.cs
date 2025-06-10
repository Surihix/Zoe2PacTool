using System.Text.Json;

/// <summary>
/// Provides methods for JsonReader with token and value types checks.
/// </summary>
public class JsonReaderHelpers
{
    /// <summary>
    /// Parses the current JSON token value from the source as a <see cref="byte"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The value of the UTF-8 encoded token.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static byte GetByte(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetByte();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as a <see cref="sbyte"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to an sbyte.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static sbyte GetSByte(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetSByte();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as a <see cref="short"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to an short.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static int GetInt16(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetInt16();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as an <see cref="ushort"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to a ushort.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static int GetUInt16(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetUInt16();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as an <see cref="int"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to an int.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static int GetInt32(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetInt32();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as an <see cref="uint"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to a uint.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static uint GetUInt32(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetUInt32();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as a <see cref="long"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to an long.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static long GetInt64(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetInt64();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as an <see cref="ulong"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to a ulong.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static ulong GetUInt64(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetUInt64();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as a <see cref="float"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to a float.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static float GetFloat(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetSingle();
    }


    /// <summary>
    /// Parses the current JSON token value from the source as a <see cref="double"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The UTF-8 encoded token value parsed to a double.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static double GetDouble(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        NumberCheck(ref jsonReader, propertyName);

        return jsonReader.GetDouble();
    }


    /// <summary>
    /// Parses the current JSON token value from the source, unescaped, and transcoded as a <see cref="string"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// The token value parsed to a string, or <see langword="null" /> if <see cref="Utf8JsonReader.TokenType"/> is <see cref="JsonTokenType.Null"/>.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static string GetString(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);

        _ = jsonReader.Read();
        if (jsonReader.TokenType != JsonTokenType.String)
        {
            throw new Exception($"TokenType exception. '{propertyName}' value is not a string.");
        }

        string? value = jsonReader.GetString();

        return value == null ? string.Empty : value;
    }


    /// <summary>
    /// Parses the current JSON token value from the source as a <see cref="bool"/>
    /// and advances to the next line.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if <see cref="Utf8JsonReader.TokenType"/> is <see cref="JsonTokenType.True"/>; <see langword="false"/> if <see cref="Utf8JsonReader.TokenType"/> is <see cref="JsonTokenType.False"/>.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static bool GetBoolean(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);

        _ = jsonReader.Read();
        if (jsonReader.TokenType != JsonTokenType.True)
        {
            if (jsonReader.TokenType != JsonTokenType.False)
            {
                throw new Exception($"TokenType exception. '{propertyName}' value is not a boolean.");
            }
        }

        return jsonReader.GetBoolean();
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="byte"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="byte"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<byte> GetByteValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var bytesList = new List<byte>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            bytesList.Add(jsonReader.GetByte());
        }

        return bytesList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="sbyte"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="sbyte"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<sbyte> GetSByteValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var bytesList = new List<sbyte>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            bytesList.Add(jsonReader.GetSByte());
        }

        return bytesList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="short"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="short"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<short> GetInt16ValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<short>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetInt16());
        }

        return numbersList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="ushort"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="ushort"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<ushort> GetUInt16ValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<ushort>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetUInt16());
        }

        return numbersList;
    }

    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="int"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="int"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<int> GetInt32ValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<int>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetInt32());
        }

        return numbersList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="uint"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="uint"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<uint> GetUInt32ValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<uint>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetUInt32());
        }

        return numbersList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="long"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="long"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<long> GetInt64ValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<long>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetInt64());
        }

        return numbersList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="ulong"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="ulong"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<ulong> GetUInt64ValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<ulong>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetUInt64());
        }

        return numbersList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="float"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="float"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<float> GetFloatValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<float>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetSingle());
        }

        return numbersList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="double"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="double"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<double> GetDoubleValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var numbersList = new List<double>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            ArrayNumberCheck(jsonReader, propertyName);

            numbersList.Add(jsonReader.GetDouble());
        }

        return numbersList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="string"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="string"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<string> GetStringValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var stringList = new List<string>();
        string? value;

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (jsonReader.TokenType != JsonTokenType.String)
            {
                throw new Exception($"TokenType exception in array. detected value that is not a string in '{propertyName}' array.");
            }

            value = jsonReader.GetString();

            if (value == null)
            {
                stringList.Add(string.Empty);
            }
            else
            {
                stringList.Add(value);
            }
        }

        return stringList;
    }


    /// <summary>
    /// Parses the current JSON array type token's values from the source, as <see cref="bool"/> values
    /// and advances to the line where the array ends.
    /// </summary>
    /// <returns>
    /// A list of <see cref="bool"/> values.
    /// </returns>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static List<bool> GetBoolValuesInArray(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);

        var boolList = new List<bool>();

        while (true)
        {
            _ = jsonReader.Read();

            if (jsonReader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (jsonReader.TokenType != JsonTokenType.True)
            {
                if (jsonReader.TokenType != JsonTokenType.False)
                {
                    throw new Exception($"TokenType exception in array. detected value that is not a boolean in '{propertyName}' array.");
                }
            }

            boolList.Add(jsonReader.GetBoolean());
        }

        return boolList;
    }


    /// <summary>
    /// Advances to the current JSON array type token's starting line from the source.
    /// </summary>
    /// <param name="jsonReader">The Utf8JsonReader variable that has to be passed with a ref keyword.</param>
    /// <param name="propertyName">The name of the current json property</param>
    public static void AdvanceToJsonArrayStart(ref Utf8JsonReader jsonReader, string propertyName)
    {
        PropertyCheck(ref jsonReader, propertyName);
        ArrayStartCheck(ref jsonReader, propertyName);
    }


    private static void PropertyCheck(ref Utf8JsonReader jsonReader, string propertyName)
    {
        _ = jsonReader.Read();

        if (jsonReader.TokenType != JsonTokenType.PropertyName)
        {
            throw new Exception($"TokenType exception. '{propertyName}' is not a 'PropertyName'.");
        }

        if (jsonReader.GetString() != propertyName)
        {
            throw new Exception($"PropertyName mismatch exception. expected '{propertyName}' as name.");
        }
    }


    private static void NumberCheck(ref Utf8JsonReader jsonReader, string propertyName)
    {
        _ = jsonReader.Read();

        if (jsonReader.TokenType != JsonTokenType.Number)
        {
            throw new Exception($"TokenType exception. '{propertyName}' value is not a number.");
        }
    }


    private static void ArrayStartCheck(ref Utf8JsonReader jsonReader, string propertyName)
    {
        _ = jsonReader.Read();

        if (jsonReader.TokenType != JsonTokenType.StartArray)
        {
            throw new Exception($"TokenType exception. '{propertyName}' value is not an array start.");
        }
    }


    private static void ArrayNumberCheck(Utf8JsonReader jsonReader, string propertyName)
    {
        if (jsonReader.TokenType != JsonTokenType.Number)
        {
            throw new Exception($"TokenType exception in array. detected value that is not a number in '{propertyName}' array.");
        }
    }
}