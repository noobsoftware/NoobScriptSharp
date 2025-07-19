using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using System.Text.RegularExpressions;
//using Newtonsoft.Json;

using System.Linq;
using Newtonsoft.Json.Linq;
/*using Windows.UI.Input;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;*/
using System.ComponentModel;
using System.Xml.Linq;

/*public class ToJSON {
	
	private List<object> toJSONRecursionDetection;

	public object toJSON(object input) {
		this.toJSONRecursionDetection = new List<object>();
		object res = this.toJSONSub(input);
		this.toJSONRecursionDetection = null;

		return res;
	}

	public object toJSONSub(object input) {
		if(input is PHPScriptFunction) {
			return "undefined";
		} else if(input is PHPScriptObject) {
			PHPSriptObject inputObj = (PHPScriptObject)input;
			if(this.toJSONRecursionDetection.contains(input)) {
				return false;
			}
			this.toJSONRecursionDetection.add(input);
			if(input.isArray) {
				List<object> dictionary = (List<object>)inputObj.getDictionary();
				List<object> returnArray = new List<object>();

				foreach(object subitem in dictionary) {
					returnArray.add(this.toJSONSub(subitem));
				}

				return returnArray;
			} else {
				Dictionary<object, object> dictionary = (Dictionary<object, object>)inputObj.getDictionary();

				Dictionary<object, object> returnDictionary = new Dictionary<object, object>();

				foreach(KeyValuePair<object, object> pair in dictionary) {
					returnDictionary[pair.Key] = this.toJSONSub(pair.value);
				}

				return returnDictionary;
			}
		} else {
			return input;
		}
	}

	public string toJSONString(object input) {
		return JsonConvert.serializeObject(this.toJSON(input));	
	}
}*/

class DecimalJsonConverter : JsonConverter
{
    public DecimalJsonConverter()
    {
    }

    public override bool CanRead
    {
        get
        {
            return false;
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override bool CanConvert(Type objectType)
    {
    	//return true;
        return (objectType == typeof(int) || objectType == typeof(double));
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is double)
        {
            writer.WriteRawValue(JsonConvert.ToString(Convert.ToInt32(value)));
        }
        else
        {
            writer.WriteRawValue(JsonConvert.ToString(value));
        }
    }

}

/*class DecimalJsonConverter : JsonConverter
    {
        public DecimalJsonConverter()
        {
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(float) || objectType == typeof(double));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (DecimalJsonConverter.IsWholeValue(value))
            {
                writer.WriteRawValue(JsonConvert.ToString(Convert.ToInt64(value)));
            }
            else
            {
                writer.WriteRawValue(JsonConvert.ToString(value));
            }
        }

        private static bool IsWholeValue(object value)
        {
            if (value is decimal decimalValue)
            {
                int precision = (Decimal.GetBits(decimalValue)[3] >> 16) & 0x000000FF;
                return precision == 0;
            }
            else if (value is float floatValue)
            {
                return floatValue == Math.Truncate(floatValue);
            }
            else if (value is double doubleValue)
            {
                return doubleValue == Math.Truncate(doubleValue);
            }

            return false;
        }
    }*/

public static class JsonHelper2
{
    public static object Deserialize(string json)
    {
        return ToObject(JToken.Parse(json));
    }

    public static object ToObject(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                return token.Children<JProperty>()
                            .ToDictionary(prop => ToObject(prop.Name),
                                          prop => ToObject(prop.Value));

            case JTokenType.Array:
                return token.Select(ToObject).ToList();
                
            case JTokenType.Integer:

                return ((int)token.Value<int>());
            default:
                return ((JValue)token).Value;
        }
    }
}

public static class JsonHelper
{
    public static object Deserialize(string json)
    {
        return ToObject(JToken.Parse(json));
    }

    public static object ToObject(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                return token.Children<JProperty>()
                            .ToDictionary(prop => prop.Name,
                                          prop => ToObject(prop.Value));

            case JTokenType.Array:
                return token.Select(ToObject).ToList();
            case JTokenType.Integer:

                return ((int)token.Value<int>());
            default:

                return ((JValue)token).Value;
        }
    }
}

class JSParse {
    //string json= JsonConvert.SerializeObject(myObject, _jsonSettings)
    private JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormat = FormatterAssemblyStyle.Full
    };

    public bool containsIdentifier(Dictionary<object, object> parseObject) {
        if(parseObject.ContainsKey("sub_parse_objects")) {
            List<object> subParseObjects = (List<object>)parseObject["sub_parse_objects"];
            foreach(object item in subParseObjects) {
                Dictionary<object, object> subParseObject = (Dictionary<object, object>)item;
                if(subParseObject["label"] is string && (string)subParseObject["label"] == "identifier") {
                    parseObject["containsIdentifier"] = true;
                }
                if(this.containsIdentifier(subParseObject)) {
                    parseObject["containsIdentifier"] = true;
                }
            }
        }
        if(parseObject.ContainsKey("containsIdentifier")) {
            return true;
        }
        return false;
    }

    public bool containsFunctionDefinition(Dictionary<object, object> parseObject) {
        if(parseObject.ContainsKey("sub_parse_objects")) {
            List<object> subParseObjects = (List<object>)parseObject["sub_parse_objects"];
            foreach(object item in subParseObjects) {
                Dictionary<object, object> subParseObject = (Dictionary<object, object>)item;
                if(subParseObject["label"] is string && (string)subParseObject["label"] == "FunctionDefinition") {
                    parseObject["containsFunctionDefinition"] = true;
                }
                if(this.containsFunctionDefinition(subParseObject)) {
                    parseObject["containsFunctionDefinition"] = true;
                }
            }
        }
        if(parseObject.ContainsKey("containsFunctionDefinition")) {
            return true;
        }
        return false;
    }

    public bool containsThis(Dictionary<object, object> parseObject) {
        if(parseObject.ContainsKey("sub_parse_objects")) {
            List<object> subParseObjects = (List<object>)parseObject["sub_parse_objects"];
            foreach(object item in subParseObjects) {
                Dictionary<object, object> subParseObject = (Dictionary<object, object>)item;
                if(subParseObject["label"] is string && (string)subParseObject["label"] == "Identifier" && (string)subParseObject["text_value"] == "this") {
                    parseObject["containsIdentifierThis"] = true;
                }
                if(this.containsFunctionDefinition(subParseObject)) {
                    parseObject["containsIdentifierThis"] = true;
                }
            }
        }
        if(parseObject.ContainsKey("containsIdentifierThis")) {
            return true;
        }
        return false;
    }

    public Dictionary<object, object> preParsedTerminalIndex;

    public string changeAppends(string input) {
    	Regex incrementRegex = new Regex(@"\$.[^\.\)\{=;]+\.\=.[^\{;]+;", RegexOptions.IgnoreCase);

    	int offset = 0;
        MatchCollection increments = incrementRegex.Matches(input);
        foreach(Match m in increments) {
            /*string substringIncrement = m.Value;
            substringIncrement = substringIncrement.Replace(" ", "#");

            input = input.Remove(m.Index, m.Value.Length);
            input = input.Insert(m.Index, substring);*/
            System.Diagnostics.Debug.WriteLine("old string: "+m.Value);
            string substringIncrement = m.Value;
            List<string> splitIncrement = new List<string>(substringIncrement.Split(".="));
            string newIncrement = splitIncrement[0]+"="+splitIncrement[0]+"."+splitIncrement[1]+"";

            System.Diagnostics.Debug.WriteLine("new string: "+newIncrement);

            input = input.Remove(m.Index+offset, m.Value.Length);
            input = input.Insert(m.Index+offset, newIncrement);
            offset += newIncrement.Length -  m.Value.Length;
            //input.Replace(m.Value, substring, StringComparison.CurrentCulture, 
        }

        return input;
    }

    public string changeAppendsAddition(string input) {
    	Regex incrementRegex = new Regex(@"\$.[^\.\)\{=;]+\+\=.[^\{;]+;", RegexOptions.IgnoreCase);

    	int offset = 0;
        MatchCollection increments = incrementRegex.Matches(input);
        foreach(Match m in increments) {
            /*string substringIncrement = m.Value;
            substringIncrement = substringIncrement.Replace(" ", "#");

            input = input.Remove(m.Index, m.Value.Length);
            input = input.Insert(m.Index, substring);*/
            System.Diagnostics.Debug.WriteLine("old string: "+m.Value);
            string substringIncrement = m.Value;
            List<string> splitIncrement = new List<string>(substringIncrement.Split("+="));
            string newIncrement = splitIncrement[0]+"="+splitIncrement[0]+"+"+splitIncrement[1]+"";

            System.Diagnostics.Debug.WriteLine("new string: "+newIncrement);

            input = input.Remove(m.Index+offset, m.Value.Length);
            input = input.Insert(m.Index+offset, newIncrement);
            offset += newIncrement.Length -  m.Value.Length;
            //input.Replace(m.Value, substring, StringComparison.CurrentCulture, 
        }

        return input;
    }

    public string changeAppendsSubtraction(string input) {
    	Regex incrementRegex = new Regex(@"\$.[^\.\)\{=;]+\-\=.[^\{;]+;", RegexOptions.IgnoreCase);

    	int offset = 0;
        MatchCollection increments = incrementRegex.Matches(input);
        foreach(Match m in increments) {
            /*string substringIncrement = m.Value;
            substringIncrement = substringIncrement.Replace(" ", "#");

            input = input.Remove(m.Index, m.Value.Length);
            input = input.Insert(m.Index, substring);*/
            System.Diagnostics.Debug.WriteLine("old string: "+m.Value);
            string substringIncrement = m.Value;
            List<string> splitIncrement = new List<string>(substringIncrement.Split("-="));
            string newIncrement = splitIncrement[0]+"="+splitIncrement[0]+"-("+splitIncrement[1].Split(";")[0]+");";

            System.Diagnostics.Debug.WriteLine("new string: "+newIncrement);

            input = input.Remove(m.Index+offset, m.Value.Length);
            input = input.Insert(m.Index+offset, newIncrement);
            offset += newIncrement.Length -  m.Value.Length;
            //input.Replace(m.Value, substring, StringComparison.CurrentCulture, 
        }

        return input;
    }

    public string preprocess(string input) {
        //
        //NSString* commentsRegex = @"\\/\\*.*?\\*\\/";
        ////System.Diagnostics.Debug.WriteLine("in preprocess");

        input = this.changeAppends(input);
        input = this.changeAppendsAddition(input);
        input = this.changeAppendsSubtraction(input);

    	Regex incrementRegex = new Regex(@"\$.[^\)\{=;]+\+\+\;", RegexOptions.IgnoreCase);

    	int offset = 0;

        MatchCollection increments = incrementRegex.Matches(input);
        foreach(Match m in increments) {
            /*string substringIncrement = m.Value;
            substringIncrement = substringIncrement.Replace(" ", "#");

            input = input.Remove(m.Index, m.Value.Length);
            input = input.Insert(m.Index, substring);*/
            System.Diagnostics.Debug.WriteLine("old string: "+m.Value);
            string substringIncrement = m.Value;
            List<string> splitIncrement = new List<string>(substringIncrement.Split("++;"));
            string newIncrement = splitIncrement[0]+"="+splitIncrement[0]+"+1;";

            System.Diagnostics.Debug.WriteLine("new string: "+newIncrement);

            input = input.Remove(m.Index+offset, m.Value.Length);
            input = input.Insert(m.Index+offset, newIncrement);

            offset += newIncrement.Length - m.Value.Length;
            //input.Replace(m.Value, substring, StringComparison.CurrentCulture, 
        }

        Regex decrementRegex = new Regex(@"\$.[^\)\{=;]+\-\-\;", RegexOptions.IgnoreCase);

        offset = 0;

        MatchCollection decrements = decrementRegex.Matches(input);
        foreach(Match m in decrements) {
            /*string substringIncrement = m.Value;
            substringIncrement = substringIncrement.Replace(" ", "#");

            input = input.Remove(m.Index, m.Value.Length);
            input = input.Insert(m.Index, substring);*/
            System.Diagnostics.Debug.WriteLine("old string: "+m.Value);
            string substringDecrement = m.Value;
            List<string> splitDecrement = new List<string>(substringDecrement.Split("--;"));
            string newDecrement = splitDecrement[0]+"="+splitDecrement[0]+"-1;";

            System.Diagnostics.Debug.WriteLine("new newDecrement: "+newDecrement);
            
            input = input.Remove(m.Index+offset, m.Value.Length);
            input = input.Insert(m.Index+offset, newDecrement);
            offset += newDecrement.Length - m.Value.Length;
            //input.Replace(m.Value, substring, StringComparison.CurrentCulture, 
        }

		//$object->log($this->test);
       	//System.Diagnostics.Debug.WriteLine("in input"+ input);

        Regex newline = new Regex(@"\n");
        input = newline.Replace(input, " ");
        ////System.Diagnostics.Debug.WriteLine("in input"+ input);

        Regex commentsRegex = new Regex(@"\/\*.*?\*\/", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        /*foreach(Match m in commentsRegex.Matches(input)) {
            //System.Diagnostics.Debug.WriteLine(m.Value);
        }*/
        input = commentsRegex.Replace(input, " ");


        

        List<Regex> keywords = new List<Regex>() {
            new Regex(@"!/\*.*?\*!s"),
            new Regex(@"<\?\s"),
            new Regex(@"\s\?>"),
            //@"'.*'",
            //@"#",
            new Regex(@"async\s"),
            new Regex(@"private\s"),
            new Regex(@"public\s"),
            new Regex(@"protected\s"),
            new Regex(@"function\s"),
            new Regex(@"class\s"),
            new Regex(@"return\s"),
            new Regex(@"\sextends\s"),
            new Regex(@"\sas\s"),
            new Regex(@"else\sif"),
            new Regex(@"new\s"),
            new Regex(@"delete\s"),
            new Regex(@"\s+"),
            new Regex(@"#"),
                //@"__HASH__"
        };
        List<string> replace = new List<string>() {
            @"",
            @"",
            @"",
            //@"#",
            @"async#",
            @"private#",
            @"public#",
            @"protected#",
            @"function#",
            @"class#",
            @"return#",
            @"#extends#",
            @"#as#",
            @"else#if",
            @"new#",
            @"delete#",
            @"",
            @" ",
            //@"__HASH__"
        };

        Regex regexStrings = new Regex(@"'[^']*'");
        MatchCollection strings = regexStrings.Matches(input);
        foreach(Match m in strings) {
            string substring = m.Value;
            substring = substring.Replace(" ", "#");

            input = input.Remove(m.Index, m.Value.Length);
            input = input.Insert(m.Index, substring);
            //input.Replace(m.Value, substring, StringComparison.CurrentCulture, 
        }

        int index = 0;
        foreach(Regex reg in keywords) {
            
            input = reg.Replace(input, replace[index]);
            index++;
        }

        //this.local_data_instance = new local_data();
        //this.local_data_instance.init();

        return input;
    }

    private local_data local_data_instance;

    public List<string> storeStates = new List<string>() {
            @"ValueObjectDereferenceWrap", @"ValueDereferenceWrap", @"FollowingObjectFunctionCall", @"ValueDereferenceContinue"
        };
    public Dictionary<object, object> parseObjectItems;
    public int currentStepStart = 0;
    public int lastCurrentStepStart = 0;
    public List<string> skipChars = new List<string>();
    public List<string> endcharDefinitions = new List<string>();

    public Dictionary<string, Regex> regexDefinitions;

    public void start(string codeInput, Dictionary<string, object> setValues=null) {
        //this.storeStates = new List<string>() {
            /*@"ValueObjectDereferenceWrap", @"ValueDereferenceWrap", @"FollowingObjectFunctionCall", @"ValueDereferenceContinue"
        };*/

        this.parseObjectItems = new Dictionary<object, object>();

        this.currentStepStart = 0;
        this.lastCurrentStepStart = 0;
        this.skipChars = new List<string>() { 
            @"}",
            @"]",
            @")",
            @";"
        };
        this.endcharDefinitions = new List<string>();

        this.regexDefinitions = new Dictionary<string, Regex>() {
            { @"VariableDefinition", new Regex(@"\$[_a-z0-9]+(\=((\$)?[a-z0-9(\-\>'\[\])]+))*", RegexOptions.IgnoreCase) },
            { @"ValueDivision", new Regex(@".*\/") },
            { @"ValueModulo", new Regex(@".*%") },
            { @"ValueSubtraction", new Regex(@".*\-") },
            { @"ValueAppend", new Regex(@".*\+") },
            { @"ValueAppendString", new Regex(@".*\.") },
            { @"IdentifierPush", new Regex(@".*\[\]\=") },
            //@"IdentifierPush": @".*\\[\\]\\=",
            { @"NewValue", new Regex(@"new\s[_a-z0-9]+") },
            { @"ReturnStatement", new Regex(@"return\s") },
            //@"IfStatement": @"if(.*){",

            { @"ValueParanthesis", new Regex(@"\(.+\)") },
            { @"ValueObjectDereference", new Regex(@"^(?!(\[|\])$)[_a-z0-9]+\[.+\]$") },
            { @"ValueDereferenceContinueSub", new Regex(@"\[.+\]|\-\>[_a-z0-9]+") },
            { @"FunctionDefinition", new Regex(@"(async\s)?function\([\$_a-z0-9\,]*\)\{") },
            { @"NamedFunctionDefinition", new Regex(@"(async\s)?function\s[_a-z0-9]+\([\$_a-z0-9\,(\=\S)?]*\)\{") },
            { @"WhileLoop", new Regex(@"while\(.+\)\{") },
            { @"ForEach", new Regex(@"foreach\(.+as.+\)\{") },
            { @"ObjectDefinition", new Regex(@"\[.*\]") },
            { @"FunctionCall", new Regex(@"[_a-z0-9]+\(.*\)") },
        };
        string encodedString = @"eyJzdGFydF9zdGF0ZXMiOlsiU2NyaXB0U3RhdGVtZW50Il0sInN0YXRlcyI6eyJTY3JpcHRTdGF0ZW1lbnQiOltbeyJub25fdGVybWluYWwiOiJDbGFzc0RlZmluaXRpb24ifSx7Im5vbl90ZXJtaW5hbCI6IlNjcmlwdFN0YXRlbWVudCIsIm9wdCI6dHJ1ZX1dLFt7Im5vbl90ZXJtaW5hbCI6IlN0YXRlbWVudExpc3QifV1dLCJFeHRlbmRzU3RhdGVtZW50IjpbW3sibm9uX3Rlcm1pbmFsIjoiV2hpdGVTcGFjZSJ9LHsidGVybWluYWwiOiJleHRlbmRzIn0seyJub25fdGVybWluYWwiOiJXaGl0ZVNwYWNlIn0seyJub25fdGVybWluYWwiOiJDbGFzc0lkZW50aWZpZXJSZWZlcmVuY2UifV1dLCJBY2Nlc3NGbGFnIjpbW3sidGVybWluYWwiOiJwcml2YXRlIn1dLFt7InRlcm1pbmFsIjoicHJvdGVjdGVkIn1dLFt7InRlcm1pbmFsIjoicHVibGljIn1dXSwiQ2xhc3NCb2R5IjpbW3sibm9uX3Rlcm1pbmFsIjoiQWNjZXNzRmxhZyJ9LHsibm9uX3Rlcm1pbmFsIjoiV2hpdGVTcGFjZSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVEZWZpbml0aW9uIn0seyJub25fdGVybWluYWwiOiJDbGFzc0JvZHkiLCJvcHQiOnRydWV9XSxbeyJub25fdGVybWluYWwiOiJBY2Nlc3NGbGFnIn0seyJub25fdGVybWluYWwiOiJXaGl0ZVNwYWNlIn0seyJub25fdGVybWluYWwiOiJOYW1lZEZ1bmN0aW9uRGVmaW5pdGlvbiJ9LHsibm9uX3Rlcm1pbmFsIjoiQ2xhc3NCb2R5Iiwib3B0Ijp0cnVlfV1dLCJDbGFzc0RlZmluaXRpb24iOltbeyJ0ZXJtaW5hbCI6ImNsYXNzIn0seyJub25fdGVybWluYWwiOiJXaGl0ZVNwYWNlIn0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn0seyJub25fdGVybWluYWwiOiJFeHRlbmRzU3RhdGVtZW50Iiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoieyJ9LHsibm9uX3Rlcm1pbmFsIjoiQ2xhc3NCb2R5Iiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoifSJ9XV0sIldoaXRlU3BhY2UiOltbeyJyZWdleCI6WyIgIl19XV0sIlNvdXJjZUNoYXJhY3RlciI6W10sIlN0YXRlbWVudCI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhcmlhYmxlRGVmaW5pdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJWYXJpYWJsZUFzc2lnbm1lbnRHcm91cCJ9LHsidGVybWluYWwiOiI7In1dLFt7Im5vbl90ZXJtaW5hbCI6IlJldHVyblN0YXRlbWVudCJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZSJ9LHsidGVybWluYWwiOiI7In1dLFt7Im5vbl90ZXJtaW5hbCI6IldoaWxlTG9vcCJ9XSxbeyJub25fdGVybWluYWwiOiJJZlN0YXRlbWVudCJ9XSxbeyJub25fdGVybWluYWwiOiJGb3JMb29wIn1dLFt7Im5vbl90ZXJtaW5hbCI6IkZvckVhY2gifV1dLCJTdHJvbmdJbmVxdWFsVmFsdWVDb25kaXRpb24iOltbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQ29uZGl0aW9uIn0seyJ0ZXJtaW5hbCI6IiE9PSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIlN0cm9uZ0VxdWFsVmFsdWVDb25kaXRpb24iOltbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQ29uZGl0aW9uIn0seyJ0ZXJtaW5hbCI6Ij09PSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIkluZXF1YWxWYWx1ZUNvbmRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24ifSx7InRlcm1pbmFsIjoiIT0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24ifV1dLCJFcXVhbFZhbHVlQ29uZGl0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9LHsidGVybWluYWwiOiI9PSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIkdyZWF0ZXJWYWx1ZUNvbmRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24ifSx7InRlcm1pbmFsIjoiPiJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIkxlc3NWYWx1ZUNvbmRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24ifSx7InRlcm1pbmFsIjoiPCJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIkdyZWF0ZXJFcXVhbFZhbHVlQ29uZGl0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9LHsidGVybWluYWwiOiI+PSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIkxlc3NFcXVhbFZhbHVlQ29uZGl0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9LHsidGVybWluYWwiOiI8PSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIlZhbHVlQ29uZGl0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiU3Ryb25nRXF1YWxWYWx1ZUNvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJTdHJvbmdJbmVxdWFsVmFsdWVDb25kaXRpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiRXF1YWxWYWx1ZUNvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJJbmVxdWFsVmFsdWVDb25kaXRpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiR3JlYXRlclZhbHVlQ29uZGl0aW9uIn1dLFt7Im5vbl90ZXJtaW5hbCI6Ikxlc3NWYWx1ZUNvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJHcmVhdGVyRXF1YWxWYWx1ZUNvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJMZXNzRXF1YWxWYWx1ZUNvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQ29uZGl0aW9uIn1dXSwiQ29uZGl0aW9uV3JhcCI6W1t7Im5vbl90ZXJtaW5hbCI6IkNvbmRpdGlvbiJ9XV0sIkNvbmRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IkFuZENvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJPckNvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZUNvbmRpdGlvbiJ9XV0sIk9yQ29uZGl0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVDb25kaXRpb24ifSx7InRlcm1pbmFsIjoifHwifSx7Im5vbl90ZXJtaW5hbCI6IkNvbmRpdGlvbiJ9XV0sIkFuZENvbmRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlQ29uZGl0aW9uIn0seyJ0ZXJtaW5hbCI6IiYmIn0seyJub25fdGVybWluYWwiOiJDb25kaXRpb24ifV1dLCJJZlN0YXRlbWVudCI6W1t7InRlcm1pbmFsIjoiaWYifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7InRlcm1pbmFsIjoiKSJ9LHsidGVybWluYWwiOiJ7In0seyJub25fdGVybWluYWwiOiJTdGF0ZW1lbnRMaXN0Iiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoifSJ9LHsibm9uX3Rlcm1pbmFsIjoiRWxzZUlmU3RhdGVtZW50R3JvdXAiLCJvcHQiOnRydWV9XV0sIkVsc2VJZlN0YXRlbWVudEdyb3VwIjpbW3sibm9uX3Rlcm1pbmFsIjoiRWxzZUlmU3RhdGVtZW50In1dLFt7Im5vbl90ZXJtaW5hbCI6IkVsc2VTdGF0ZW1lbnQifV1dLCJFbHNlU3RhdGVtZW50IjpbW3sidGVybWluYWwiOiJlbHNlIn0seyJ0ZXJtaW5hbCI6InsifSx7Im5vbl90ZXJtaW5hbCI6IlN0YXRlbWVudExpc3QiLCJvcHQiOnRydWV9LHsidGVybWluYWwiOiJ9In1dXSwiRWxzZUlmU3RhdGVtZW50IjpbW3sidGVybWluYWwiOiJlbHNlIGlmIn0seyJ0ZXJtaW5hbCI6IigifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn0seyJ0ZXJtaW5hbCI6IikifSx7InRlcm1pbmFsIjoieyJ9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6In0ifSx7Im5vbl90ZXJtaW5hbCI6IkVsc2VJZlN0YXRlbWVudEdyb3VwIiwib3B0Ijp0cnVlfV1dLCJSZXR1cm5TdGF0ZW1lbnQiOltbeyJ0ZXJtaW5hbCI6InJldHVybiJ9LHsibm9uX3Rlcm1pbmFsIjoiV2hpdGVTcGFjZSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7InRlcm1pbmFsIjoiOyJ9XV0sIk5ld1N0YXRlbWVudCI6W1t7InRlcm1pbmFsIjoibmV3In0seyJub25fdGVybWluYWwiOiJXaGl0ZVNwYWNlIn0seyJub25fdGVybWluYWwiOiJDbGFzc0lkZW50aWZpZXJSZWZlcmVuY2UifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVyVmFsdWVzQ29uc3RydWN0b3IiLCJvcHQiOnRydWV9LHsidGVybWluYWwiOiIpIn1dXSwiU3RhdGVtZW50TGlzdCI6W1t7Im5vbl90ZXJtaW5hbCI6IlN0YXRlbWVudCJ9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsIm9wdCI6dHJ1ZX1dXSwiUmVzZXJ2ZWRLZXl3b3JkcyI6WyJjbGFzcyAiLCJwcml2YXRlICIsInB1YmxpYyAiLCJwcm90ZWN0ZWQgIiwidmFyICIsInRydWUiLCJmYWxzZSIsImZ1bmN0aW9uIiwiZnVuY3Rpb24oKSIsImZ1bmN0aW9uKCIsImF3YWl0ICIsImFzeW5jICIsInJldHVybiAiLCJuZXcgIiwiZGVsZXRlICIsImV4dGVuZHMgIl0sIkNsYXNzSWRlbnRpZmllclJlZmVyZW5jZSI6W1t7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXIifV1dLCJJZGVudGlmaWVyUmVmZXJlbmNlIjpbW3sidGVybWluYWwiOiIkIn0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn1dXSwiUHJlZml4ZWRJZGVudGlmaWVyIjpbW3sidGVybWluYWwiOiIkIn0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6Ij0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dLFt7InRlcm1pbmFsIjoiJCJ9LHsibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllciJ9XV0sIklkZW50aWZpZXIiOltbeyJyZWdleCI6WyJhIiwiYiIsImMiLCJkIiwiZSIsImYiLCJnIiwiaCIsImkiLCJqIiwiayIsImwiLCJtIiwibiIsIm8iLCJwIiwicSIsInIiLCJzIiwidCIsInUiLCJ2IiwidyIsIngiLCJ5IiwieiIsIjAiLCIxIiwiMiIsIjMiLCI0IiwiNSIsIjYiLCI3IiwiOCIsIjkiLCJfIl0sInN0b3BfcmVnZXgiOlsiJyIsIlwiIiwiLSIsIiAiLCI9IiwiKyIsIigiLCIpIiwieyIsIn0iLCIuIiwiPCIsIj4iLCImIiwifCIsIlsiLCJdIl0sIm5vdCI6IlJlc2VydmVkS2V5d29yZHMiLCJub3Rfc3RhcnRzd2l0aCI6Ik51bWJlciJ9XV0sIkZpcnN0QXJyYXlJdGVtIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7Im5vbl90ZXJtaW5hbCI6IkFycmF5SXRlbXMiLCJvcHQiOnRydWV9XV0sIkFycmF5SXRlbXMiOltbeyJ0ZXJtaW5hbCI6IiwifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn0seyJub25fdGVybWluYWwiOiJBcnJheUl0ZW1zIiwib3B0Ijp0cnVlfV0sW3sidGVybWluYWwiOiIsIn1dXSwiVmFsdWVOb0NvbmRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6Ik5VTEwifV0sW3sibm9uX3Rlcm1pbmFsIjoiTnVtYmVyIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlN0cmluZyJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZUFwcGVuZCJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZUFwcGVuZFN0cmluZyJ9XSxbeyJub25fdGVybWluYWwiOiJCb29sZWFuIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhcmlhYmxlQXNzaWdubWVudEdyb3VwIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlUGFyYW50aGVzaXMifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOZWdhdGVkIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VXcmFwIn1dLFt7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifV1dLCJWYWx1ZU5vQWRkaXRpb24iOltbeyJub25fdGVybWluYWwiOiJOVUxMIn1dLFt7Im5vbl90ZXJtaW5hbCI6Ik51bWJlciJ9XSxbeyJub25fdGVybWluYWwiOiJTdHJpbmcifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVBc3NpZ25tZW50R3JvdXAifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVQYXJhbnRoZXNpcyJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZURlcmVmZXJlbmNlV3JhcCJ9XSxbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIn1dXSwiVmFsdWVJZGVudGlmaWVyIjpbW3sibm9uX3Rlcm1pbmFsIjoiRnVuY3Rpb25DYWxsUmVmZXJlbmNlIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VXcmFwUmVmZXJlbmNlIn1dLFt7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifV1dLCJWYWx1ZUlkZW50aWZpZXIyIjpbW3sibm9uX3Rlcm1pbmFsIjoiRnVuY3Rpb25DYWxsUmVmZXJlbmNlIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VXcmFwIn1dLFt7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifV1dLCJOVUxMIjpbW3sidGVybWluYWwiOiJOVUxMIn1dXSwiTmV3VmFsdWUiOltbeyJ0ZXJtaW5hbCI6Im4ifSx7InRlcm1pbmFsIjoiZSJ9LHsidGVybWluYWwiOiJ3In0seyJub25fdGVybWluYWwiOiJXaGl0ZVNwYWNlIn0seyJub25fdGVybWluYWwiOiJWYWx1ZUlkZW50aWZpZXJOZXdXcmFwMiJ9XV0sIlZhbHVlSWRlbnRpZmllck5ld1dyYXAyIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyMiJ9XV0sIlZhbHVlSWRlbnRpZmllck5ld1dyYXAiOltbeyJub25fdGVybWluYWwiOiJGdW5jdGlvbkNhbGxSZWZlcmVuY2UifV1dLCJWYWx1ZU5vRGVyZWZlcmVuY2UiOltbeyJub25fdGVybWluYWwiOiJGdW5jdGlvbkRlZmluaXRpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVQYXJhbnRoZXNpcyJ9XSxbeyJub25fdGVybWluYWwiOiJTdHJpbmcifV0sW3sibm9uX3Rlcm1pbmFsIjoiT2JqZWN0RGVmaW5pdGlvbiJ9XV0sIlZhbHVlTXVsdGlwbGljYXRpb24iOltbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQWRkaXRpb24ifSx7InRlcm1pbmFsIjoiKiJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJWYWx1ZSI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTXVsdGlwbGljYXRpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVBcHBlbmQifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVBcHBlbmRTdHJpbmcifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVTdWJ0cmFjdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZURpdmlzaW9uIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTW9kdWxvIn1dLFt7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJQdXNoIn1dLFt7Im5vbl90ZXJtaW5hbCI6IkJvb2xlYW5BbmRDb25kaXRpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiTnVtYmVyIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlN0cmluZyJ9XSxbeyJub25fdGVybWluYWwiOiJOZXdTdGF0ZW1lbnQifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZVdyYXAifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVBc3NpZ25tZW50R3JvdXAifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVQYXJhbnRoZXNpcyJ9XSxbeyJub25fdGVybWluYWwiOiJWYWx1ZU5lZ2F0ZWQifV0sW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSJ9XSxbeyJub25fdGVybWluYWwiOiJEZWxldGVWYWx1ZURlcmVmZXJlbmNlIn1dLFt7Im5vbl90ZXJtaW5hbCI6IkFycmF5U3ByZWFkT3BlcmF0b3IifV0sW3sibm9uX3Rlcm1pbmFsIjoiT2JqZWN0U3ByZWFkT3BlcmF0b3IifV1dLCJJZGVudGlmaWVyUHVzaCI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlSWRlbnRpZmllciJ9LHsidGVybWluYWwiOiJbXT0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiT2JqZWN0U3ByZWFkT3BlcmF0b3IiOltbeyJ0ZXJtaW5hbCI6InsuLi4ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlSWRlbnRpZmllciJ9LHsidGVybWluYWwiOiJ9In1dXSwiQXJyYXlTcHJlYWRPcGVyYXRvciI6W1t7InRlcm1pbmFsIjoiWy4uLiJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6Il0ifV1dLCJEZWxldGVWYWx1ZURlcmVmZXJlbmNlIjpbW3sidGVybWluYWwiOiJkZWxldGUifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlSWRlbnRpZmllciJ9XV0sIlR5cGVvZlZhbHVlIjpbW3sidGVybWluYWwiOiJ0eXBlb2YifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9BZGRpdGlvbiJ9XV0sIlZhbHVlQXBwZW5kU3RyaW5nIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0FkZGl0aW9uIn0seyJ0ZXJtaW5hbCI6Ii4ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiVmFsdWVBcHBlbmQiOltbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQWRkaXRpb24ifSx7InRlcm1pbmFsIjoiKyJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJPYmplY3RDYWxsQXBwZW5kIjpbW3sibm9uX3Rlcm1pbmFsIjoiT2JqZWN0Q2FsbFdyYXAifSx7InRlcm1pbmFsIjoiKyJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJPYmplY3RDYWxsV3JhcCI6W1t7Im5vbl90ZXJtaW5hbCI6IkF3YWl0Iiwib3B0Ijp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6Ik9iamVjdENhbGwifV1dLCJPYmplY3RDYWxsQ29udGludWUiOltbeyJ0ZXJtaW5hbCI6Ii4ifSx7Im5vbl90ZXJtaW5hbCI6Ik9iamVjdENhbGwifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIm9wdCI6dHJ1ZX1dLFt7InRlcm1pbmFsIjoiLiJ9LHsibm9uX3Rlcm1pbmFsIjoiT2JqZWN0RnVuY3Rpb25DYWxsVmFsdWUifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIm9wdCI6dHJ1ZX1dLFt7InRlcm1pbmFsIjoiLiJ9LHsibm9uX3Rlcm1pbmFsIjoiT2JqZWN0SWRlbnRpZmllclJlZmVyZW5jZSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIiwib3B0Ijp0cnVlfV1dLCJPYmplY3RDYWxsIjpbW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSJ9LHsidGVybWluYWwiOiIuIn0seyJub25fdGVybWluYWwiOiJPYmplY3RDYWxsIn1dLFt7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifSx7InRlcm1pbmFsIjoiLiJ9LHsibm9uX3Rlcm1pbmFsIjoiT2JqZWN0RnVuY3Rpb25DYWxsVmFsdWUifV0sW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSJ9LHsidGVybWluYWwiOiIuIn0seyJub25fdGVybWluYWwiOiJPYmplY3RDYWxsVmFsdWUifV1dLCJPYmplY3RDYWxsVmFsdWUiOltbeyJub25fdGVybWluYWwiOiJPYmplY3RJZGVudGlmaWVyUmVmZXJlbmNlIn1dXSwiT2JqZWN0RnVuY3Rpb25DYWxsVmFsdWUiOltbeyJub25fdGVybWluYWwiOiJPYmplY3RGdW5jdGlvbkNhbGwifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVPYmplY3REZXJlZmVyZW5jZVdyYXAifV1dLCJPYmplY3RJZGVudGlmaWVyUmVmZXJlbmNlIjpbW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllciJ9XV0sIkZ1bmN0aW9uQ2FsbFN1YnRyYWN0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiRnVuY3Rpb25DYWxsIn0seyJ0ZXJtaW5hbCI6Ii0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiRnVuY3Rpb25DYWxsRGl2aXNpb24iOltbeyJub25fdGVybWluYWwiOiJGdW5jdGlvbkNhbGwifSx7InRlcm1pbmFsIjoiXC8ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiRnVuY3Rpb25DYWxsQXBwZW5kIjpbW3sibm9uX3Rlcm1pbmFsIjoiRnVuY3Rpb25DYWxsIn0seyJ0ZXJtaW5hbCI6IisifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiQXdhaXQiOltbeyJ0ZXJtaW5hbCI6ImF3YWl0In0seyJub25fdGVybWluYWwiOiJXaGl0ZVNwYWNlIn1dXSwiT2JqZWN0RnVuY3Rpb25DYWxsIjpbW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSJ9LHsidGVybWluYWwiOiIoIn0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZXMiLCJvcHQiOnRydWV9LHsidGVybWluYWwiOiIpIn0seyJub25fdGVybWluYWwiOiJGb2xsb3dpbmdPYmplY3RGdW5jdGlvbkNhbGwiLCJvcHQiOnRydWV9XSxbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIn0seyJ0ZXJtaW5hbCI6IigifSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlclZhbHVlcyIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6IikifSx7Im5vbl90ZXJtaW5hbCI6Ik9iamVjdENhbGxDb250aW51ZSIsIm9wdCI6dHJ1ZX1dXSwiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsUmVmZXJlbmNlIjpbW3sidGVybWluYWwiOiIoIn0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZXMiLCJvcHQiOnRydWV9LHsidGVybWluYWwiOiIpIn1dXSwiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIjpbW3sidGVybWluYWwiOiIoIn0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZXMiLCJvcHQiOnRydWV9LHsidGVybWluYWwiOiIpIn0seyJub25fdGVybWluYWwiOiJGb2xsb3dpbmdPYmplY3RGdW5jdGlvbkNhbGwiLCJvcHQiOnRydWV9XSxbeyJ0ZXJtaW5hbCI6IigifSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlclZhbHVlcyIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6IikifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIm9wdCI6dHJ1ZX1dXSwiSW5saW5lRnVuY3Rpb25EZWZpbml0aW9uQ2FsbCI6W1t7Im5vbl90ZXJtaW5hbCI6IkZ1bmN0aW9uRGVmaW5pdGlvbk5vQXN5bmMifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVyVmFsdWVzIiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoiKSJ9LHsibm9uX3Rlcm1pbmFsIjoiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIiwib3B0Ijp0cnVlfV1dLCJGdW5jdGlvbkNhbGxSZWZlcmVuY2UiOltbeyJub25fdGVybWluYWwiOiJBd2FpdCIsIm9wdCI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIn0seyJ0ZXJtaW5hbCI6IigifSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlclZhbHVlcyIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6IikifSx7Im5vbl90ZXJtaW5hbCI6IkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCIsIm9wdCI6dHJ1ZX1dLFt7Im5vbl90ZXJtaW5hbCI6IkF3YWl0Iiwib3B0Ijp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVyVmFsdWVzIiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoiKSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIiwib3B0Ijp0cnVlfV1dLCJGdW5jdGlvbkNhbGwiOltbeyJub25fdGVybWluYWwiOiJBd2FpdCIsIm9wdCI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIn0seyJ0ZXJtaW5hbCI6IigifSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlclZhbHVlcyIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6IikifSx7Im5vbl90ZXJtaW5hbCI6IkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCIsIm9wdCI6dHJ1ZX1dLFt7Im5vbl90ZXJtaW5hbCI6IkF3YWl0Iiwib3B0Ijp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVyVmFsdWVzIiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoiKSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIiwib3B0Ijp0cnVlfV1dLCJQYXJhbWV0ZXJWYWx1ZXNDb25zdHJ1Y3RvciI6W1t7Im5vbl90ZXJtaW5hbCI6IkZpcnN0UGFyYW1ldGVyVmFsdWUifSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlclZhbHVlIiwib3B0Ijp0cnVlfV1dLCJQYXJhbWV0ZXJWYWx1ZXMiOltbeyJub25fdGVybWluYWwiOiJGaXJzdFBhcmFtZXRlclZhbHVlIn0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZSIsIm9wdCI6dHJ1ZX1dXSwiRmlyc3RQYXJhbWV0ZXJWYWx1ZSI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZSIsIm9wdCI6dHJ1ZX1dXSwiUGFyYW1ldGVyVmFsdWUiOltbeyJ0ZXJtaW5hbCI6IiwifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZSIsIm9wdCI6dHJ1ZX1dXSwiRnVuY3Rpb25MZWZ0IjpbXSwiT2JqZWN0U3RhdGVtZW50TGlzdCI6W1t7Im5vbl90ZXJtaW5hbCI6IkZpcnN0T2JqZWN0U3RhdGVtZW50In0seyJub25fdGVybWluYWwiOiJPYmplY3RTdGF0ZW1lbnQiLCJvcHQiOnRydWV9XV0sIlByb3BlcnR5TmFtZSI6W1t7Im5vbl90ZXJtaW5hbCI6Ik51bWJlciJ9XSxbeyJub25fdGVybWluYWwiOiJTdHJpbmcifV1dLCJGaXJzdE9iamVjdFN0YXRlbWVudCI6W1t7Im5vbl90ZXJtaW5hbCI6IlByb3BlcnR5TmFtZSJ9LHsidGVybWluYWwiOiI9PiJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJPYmplY3RTdGF0ZW1lbnQiOltbeyJ0ZXJtaW5hbCI6IiwifSx7Im5vbl90ZXJtaW5hbCI6IlByb3BlcnR5TmFtZSJ9LHsidGVybWluYWwiOiI9PiJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7Im5vbl90ZXJtaW5hbCI6Ik9iamVjdFN0YXRlbWVudCIsIm9wdCI6dHJ1ZX1dLFt7InRlcm1pbmFsIjoiLCJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7Im5vbl90ZXJtaW5hbCI6Ik9iamVjdFN0YXRlbWVudCIsIm9wdCI6dHJ1ZX1dXSwiT2JqZWN0RGVmaW5pdGlvbiI6W1t7InRlcm1pbmFsIjoiWyJ9LHsibm9uX3Rlcm1pbmFsIjoiT2JqZWN0U3RhdGVtZW50TGlzdCIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6Il0ifV1dLCJBc3luY0Z1bmN0aW9uUHJlZml4IjpbW3sidGVybWluYWwiOiJhc3luYyJ9LHsibm9uX3Rlcm1pbmFsIjoiV2hpdGVTcGFjZSJ9XV0sIkZvckVhY2giOltbeyJ0ZXJtaW5hbCI6ImZvcmVhY2gifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiRm9yZWFjaFNldHRpbmdzIn0seyJ0ZXJtaW5hbCI6IikifSx7InRlcm1pbmFsIjoieyJ9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6In0ifV1dLCJGb3JlYWNoU2V0dGluZ3MiOltbeyJub25fdGVybWluYWwiOiJGb3JlYWNoU2V0dGluZ3MxIn1dLFt7Im5vbl90ZXJtaW5hbCI6IkZvcmVhY2hTZXR0aW5nczIifV1dLCJGb3JlYWNoU2V0dGluZ3MyIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7InRlcm1pbmFsIjoiYXMifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifV1dLCJGb3JlYWNoU2V0dGluZ3MxIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7InRlcm1pbmFsIjoiYXMifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifSx7InRlcm1pbmFsIjoiPT4ifSx7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifV1dLCJLZXlBcnJvdyI6W1t7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifSx7InRlcm1pbmFsIjoiPT4ifV1dLCJGb3JMb29wIjpbW3sidGVybWluYWwiOiJmb3IifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiRm9yU2V0dGluZ3MifSx7InRlcm1pbmFsIjoiKSJ9LHsidGVybWluYWwiOiJ7In0seyJub25fdGVybWluYWwiOiJTdGF0ZW1lbnRMaXN0Iiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoifSJ9XV0sIkZvclNldHRpbmdzIjpbW3sibm9uX3Rlcm1pbmFsIjoiRm9yIn1dXSwiRm9yIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVEZWZpbml0aW9uIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9LHsidGVybWluYWwiOiI7In0seyJub25fdGVybWluYWwiOiJWYXJpYWJsZUFzc2lnbm1lbnRHcm91cCJ9XV0sIkZvck9mIjpbW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSJ9LHsibm9uX3Rlcm1pbmFsIjoiV2hpdGVTcGFjZSJ9LHsidGVybWluYWwiOiJvZiJ9LHsibm9uX3Rlcm1pbmFsIjoiV2hpdGVTcGFjZSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJGb3JJbiI6W1t7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7InRlcm1pbmFsIjoiaW4ifSx7Im5vbl90ZXJtaW5hbCI6IldoaXRlU3BhY2UifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiV2hpbGVMb29wIjpbW3sidGVybWluYWwiOiJ3aGlsZSJ9LHsidGVybWluYWwiOiIoIn0seyJub25fdGVybWluYWwiOiJDb25kaXRpb24ifSx7InRlcm1pbmFsIjoiKSJ9LHsidGVybWluYWwiOiJ7In0seyJub25fdGVybWluYWwiOiJTdGF0ZW1lbnRMaXN0In0seyJ0ZXJtaW5hbCI6In0ifV1dLCJGdW5jdGlvbkRlZmluaXRpb25Ob0FzeW5jIjpbW3sidGVybWluYWwiOiJmdW5jdGlvbiJ9LHsidGVybWluYWwiOiIoIn0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJJbnB1dHMiLCJvcHQiOnRydWV9LHsidGVybWluYWwiOiIpIn0seyJ0ZXJtaW5hbCI6InsifSx7Im5vbl90ZXJtaW5hbCI6IlN0YXRlbWVudExpc3QiLCJvcHQiOnRydWV9LHsidGVybWluYWwiOiJ9In1dXSwiTmFtZWRGdW5jdGlvbkRlZmluaXRpb24iOltbeyJ0ZXJtaW5hbCI6ImZ1bmN0aW9uIn0seyJub25fdGVybWluYWwiOiJXaGl0ZVNwYWNlIn0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6IigifSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlcklucHV0cyIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6IikifSx7InRlcm1pbmFsIjoieyJ9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6In0ifV1dLCJGdW5jdGlvbkRlZmluaXRpb24iOltbeyJub25fdGVybWluYWwiOiJBc3luY0Z1bmN0aW9uUHJlZml4Iiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoiZnVuY3Rpb24ifSx7InRlcm1pbmFsIjoiKCJ9LHsibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVySW5wdXRzIiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoiKSJ9LHsidGVybWluYWwiOiJ7In0seyJub25fdGVybWluYWwiOiJTdGF0ZW1lbnRMaXN0Iiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoifSJ9XV0sIlBhcmFtZXRlcklucHV0cyI6W1t7Im5vbl90ZXJtaW5hbCI6IkZpcnN0UGFyYW1ldGVySW5wdXQifSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlcklucHV0Iiwib3B0Ijp0cnVlfV1dLCJGaXJzdFBhcmFtZXRlcklucHV0IjpbW3sibm9uX3Rlcm1pbmFsIjoiUHJlZml4ZWRJZGVudGlmaWVyIn1dXSwiUGFyYW1ldGVySW5wdXQiOltbeyJ0ZXJtaW5hbCI6IiwifSx7Im5vbl90ZXJtaW5hbCI6IlByZWZpeGVkSWRlbnRpZmllciJ9LHsibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVySW5wdXQiLCJvcHQiOnRydWV9XV0sIkJvb2xlYW5BZGRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IkJvb2xlYW4ifSx7InRlcm1pbmFsIjoiKyJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJCb29sZWFuQW5kQ29uZGl0aW9uV3JhcE5lZ2F0ZWQiOltbeyJ0ZXJtaW5hbCI6IiEifSx7Im5vbl90ZXJtaW5hbCI6IkJvb2xlYW5BbmRDb25kaXRpb24ifV1dLCJCb29sZWFuQW5kQ29uZGl0aW9uV3JhcCI6W1t7Im5vbl90ZXJtaW5hbCI6IkJvb2xlYW5BbmRDb25kaXRpb25XcmFwTmVnYXRlZCJ9XSxbeyJub25fdGVybWluYWwiOiJCb29sZWFuQW5kQ29uZGl0aW9uIn1dXSwiQm9vbGVhbkFuZENvbmRpdGlvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IkNvbmRpdGlvbiJ9XSxbeyJub25fdGVybWluYWwiOiJCb29sZWFuIn1dXSwiQm9vbGVhbiI6W1t7InRlcm1pbmFsIjoidHJ1ZSJ9XSxbeyJ0ZXJtaW5hbCI6ImZhbHNlIn1dXSwiVmFsdWVQYXJhbnRoZXNpc1N1YnRyYWN0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVQYXJhbnRoZXNpcyJ9LHsidGVybWluYWwiOiItIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlZhbHVlUGFyYW50aGVzaXNEaXZpc2lvbiI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlUGFyYW50aGVzaXMifSx7InRlcm1pbmFsIjoiXC8ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiVmFsdWVQYXJhbnRoZXNpc0FkZGl0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVQYXJhbnRoZXNpcyJ9LHsidGVybWluYWwiOiIrIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlZhbHVlTmVnYXRpdmUiOltbeyJ0ZXJtaW5hbCI6Ii0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9BZGRpdGlvbiJ9XV0sIlZhbHVlTmVnYXRlZCI6W1t7InRlcm1pbmFsIjoiISJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiJ9XV0sIlZhbHVlRGVyZWZlcmVuY2VXcmFwUmVmZXJlbmNlIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIiwib3B0Ijp0cnVlfV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZSJ9LHsibm9uX3Rlcm1pbmFsIjoiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIiwib3B0Ijp0cnVlfV1dLCJWYWx1ZURlcmVmZXJlbmNlV3JhcCI6W1t7Im5vbl90ZXJtaW5hbCI6IkF3YWl0Iiwib3B0Ijp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2UifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIm9wdCI6dHJ1ZX1dLFt7Im5vbl90ZXJtaW5hbCI6IkF3YWl0Iiwib3B0Ijp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2UifSx7Im5vbl90ZXJtaW5hbCI6IkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCIsIm9wdCI6dHJ1ZX1dXSwiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlU3ViIjpbW3sidGVybWluYWwiOiJbIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9LHsidGVybWluYWwiOiJdIn1dLFt7InRlcm1pbmFsIjoiLT4ifSx7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXIifV1dLCJWYWx1ZURlcmVmZXJlbmNlQ29udGludWUiOltbeyJub25fdGVybWluYWwiOiJWYWx1ZURlcmVmZXJlbmNlQ29udGludWVTdWIifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIm9wdCI6dHJ1ZX1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZVN1YiJ9LHsibm9uX3Rlcm1pbmFsIjoiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIiwib3B0Ijp0cnVlfV1dLCJWYWx1ZURlcmVmZXJlbmNlIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0RlcmVmZXJlbmNlIn0seyJub25fdGVybWluYWwiOiJWYWx1ZURlcmVmZXJlbmNlQ29udGludWVTdWIiLCJvcHQiOnRydWV9XSxbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIn0seyJub25fdGVybWluYWwiOiJWYWx1ZURlcmVmZXJlbmNlQ29udGludWVTdWIifV0sW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSJ9XV0sIlZhbHVlT2JqZWN0RGVyZWZlcmVuY2VXcmFwIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVPYmplY3REZXJlZmVyZW5jZSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlT2JqZWN0RGVyZWZlcmVuY2UifSx7Im5vbl90ZXJtaW5hbCI6Ik9iamVjdENhbGxDb250aW51ZSIsIm9wdCI6dHJ1ZX1dXSwiVmFsdWVPYmplY3REZXJlZmVyZW5jZSI6W1t7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UifSx7InRlcm1pbmFsIjoiWyJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifSx7InRlcm1pbmFsIjoiXSJ9XV0sIlZhbHVlUGFyYW50aGVzaXMiOltbeyJ0ZXJtaW5hbCI6IigifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn0seyJ0ZXJtaW5hbCI6IikifV1dLCJJZGVudGlmaWVyQXBwZW5kIjpbW3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSJ9LHsidGVybWluYWwiOiIrIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlN0cmluZ0FwcGVuZCI6W1t7Im5vbl90ZXJtaW5hbCI6IlN0cmluZyJ9LHsidGVybWluYWwiOiIrIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlN0cmluZyI6W1t7InRlcm1pbmFsIjoiJyJ9LHsibm9uX3Rlcm1pbmFsIjoiU3RyaW5nQ29udGVudCIsIm9wdCI6dHJ1ZX0seyJ0ZXJtaW5hbCI6IicifV1dLCJTdHJpbmdDb250ZW50IjpbW3sicmVnZXgiOlsiYWxsIl0sInN0b3BfcmVnZXgiOlsiXCIiLCInIl0sImRlbGltaXRfcmVnZXgiOltdfV1dLCJWYWx1ZVN1YnRyYWN0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0FkZGl0aW9uIiwib3B0Ijp0cnVlfSx7InRlcm1pbmFsIjoiLSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJWYWx1ZU1vZHVsbyI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9BZGRpdGlvbiJ9LHsidGVybWluYWwiOiIlIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlZhbHVlRGl2aXNpb24iOltbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQWRkaXRpb24ifSx7InRlcm1pbmFsIjoiXC8ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiTnVtYmVyIjpbW3sicmVnZXgiOlsiMCIsIjEiLCIyIiwiMyIsIjQiLCI1IiwiNiIsIjciLCI4IiwiOSJdfV1dLCJWYXJpYWJsZUFzc2lnbm1lbnRHcm91cCI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhcmlhYmxlQXNzaWdubWVudEluY3JlbWVudCJ9XSxbeyJub25fdGVybWluYWwiOiJWYXJpYWJsZUFzc2lnbm1lbnREZWNyZW1lbnQifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVBc3NpZ25tZW50QWRkaXRpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVBc3NpZ25tZW50U3VidHJhY3Rpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVBc3NpZ25tZW50TXVsdGlwbGljYXRpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVBc3NpZ25tZW50RGl2aXNpb24ifV0sW3sibm9uX3Rlcm1pbmFsIjoiVmFyaWFibGVBc3NpZ25tZW50QXBwZW5kIn1dLFt7Im5vbl90ZXJtaW5hbCI6IlZhcmlhYmxlQXNzaWdubWVudCJ9XV0sIlZhcmlhYmxlQXNzaWdubWVudEluY3JlbWVudCI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlSWRlbnRpZmllciJ9LHsidGVybWluYWwiOiIrKyJ9XV0sIlZhcmlhYmxlQXNzaWdubWVudERlY3JlbWVudCI6W1t7Im5vbl90ZXJtaW5hbCI6IlZhbHVlSWRlbnRpZmllciJ9LHsidGVybWluYWwiOiItLSJ9XV0sIlZhcmlhYmxlQXNzaWdubWVudEFkZGl0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6Iis9In0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlZhcmlhYmxlQXNzaWdubWVudFN1YnRyYWN0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6Ii09In0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlZhcmlhYmxlQXNzaWdubWVudE11bHRpcGxpY2F0aW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6Iio9In0seyJub25fdGVybWluYWwiOiJWYWx1ZSJ9XV0sIlZhcmlhYmxlQXNzaWdubWVudERpdmlzaW9uIjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6IlwvPSJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUifV1dLCJWYXJpYWJsZUFzc2lnbm1lbnRBcHBlbmQiOltbeyJub25fdGVybWluYWwiOiJWYWx1ZUlkZW50aWZpZXIifSx7InRlcm1pbmFsIjoiLj0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiVmFyaWFibGVBc3NpZ25tZW50IjpbW3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6Ij0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn1dXSwiVmFyaWFibGVEZWZpbml0aW9uIjpbW3sidGVybWluYWwiOiIkIn0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6Ij0ifSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIn0seyJ0ZXJtaW5hbCI6IjsifV0sW3sidGVybWluYWwiOiIkIn0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn0seyJ0ZXJtaW5hbCI6IjsifV1dfX0=";
        byte[] data = Convert.FromBase64String(encodedString);
        string decodedString = System.Text.Encoding.UTF8.GetString(data);

        //var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedString, _jsonSettings);
        Dictionary<string, object> dict = (Dictionary<string, object>)JsonHelper.Deserialize(decodedString);//JsonConvert.DeserializeObject<Dictionary<object, object>>(decodedString);
        //////////////////System.Diagnostics.Debug.WriteLine((List<object>)dict["start_states"]);

        List<object> startStates = (List<object>)dict["start_states"];


        this.initArrays();

        string postProcessingString = "{\"ValueDereferenceWrapReference\":{\"last_node\":{\"from\":\"ValueDereferenceContinueSub\",\"to\":\"ValueDereferenceContinueSubReference\"}},\"FunctionCallReference\":{\"last_node\":{\"from\":\"FollowingObjectFunctionCall\",\"to\":\"FollowingObjectFunctionCallReference\"}},\"FunctionCallReference_2\":{\"last_node\":{\"from\":\"ValueDereferenceContinueSub\",\"to\":\"ValueDereferenceContinueSubReference\"}},\"ValueDereferenceWrapReference_2\":{\"last_node\":{\"from\":\"FollowingObjectFunctionCall\",\"to\":\"FollowingObjectFunctionCallReference\"}},\"ValueIdentifierNewWrap2\":{\"all_nodes\":{\"from\":\"ValueDereferenceWrap\",\"to\":\"ValueDereferenceWrapReference\"}},\"ValueDereferenceWrapReference_3\":{\"last_node\":{\"from\":\"IdentifierReference\",\"to\":\"IdentifierReferenceIgnore\"}}}";

        
        Dictionary<string, object> resultsPost = (Dictionary<string, object>)JsonHelper.Deserialize(postProcessingString);

        this.postProcessingDefinition = resultsPost;

        if(setValues != null) {

            /*Dictionary<string, object> jsparseDict = new Dictionary<string, object>() {
            	{ "parse_results", jsparse.parse_results},
            	{"input", jsparse.input},
            	{"preParsedTerminalIndex", jsparse.preParsedTerminalIndex},
            	{"strings", jsparse.strings},
            	{"whiteSpaces", jsparse.whiteSpaces},
            	{"setIdentifiers", jsparse.identifiers},
            };*/

            this.preParsedTerminalIndex = (Dictionary<object, object>)setValues["preParsedTerminalIndex"];
	        this.strings = (Dictionary<object, object>)setValues["strings"];
	        this.whiteSpaces = (Dictionary<object, object>)setValues["whiteSpaces"];
	        this.identifiers = (Dictionary<object, object>)setValues["identifiers"];
	        this.parse_results = (List<object>)setValues["parse_results"];
	        this.mainstring = (string)setValues["input"];
        } else {
	        codeInput = this.preprocess(codeInput);

	        //System.Diagnostics.Debug.WriteLine("preprocessed: "+codeInput);
	        //return;
	        Preparser preparser = new Preparser();

	        this.terminalIndex = preparser.preparseText(codeInput);
	        this.preParsedTerminalIndex = preparser.rangeResults;
	        this.strings = preparser.strings;
	        this.whiteSpaces = preparser.whiteSpaces;
	        this.identifiers = preparser.identifiers;
	        //this.whiteSpaces = preparser.whiteSpaces;

	        
	        //////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(terminalIndex));
	        ////////////System.Diagnostics.Debug.WriteLine("preparsedterminalindex");
	        ////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(preParsedTerminalIndex));
	        
	        ////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(strings));
	        
	        ////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(whiteSpaces));
	        
	        ////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(identifiers));
	        ////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(whiteSpaces));

	        /*this.initArrays();

	        string postProcessingString = "{\"ValueDereferenceWrapReference\":{\"last_node\":{\"from\":\"ValueDereferenceContinueSub\",\"to\":\"ValueDereferenceContinueSubReference\"}},\"FunctionCallReference\":{\"last_node\":{\"from\":\"FollowingObjectFunctionCall\",\"to\":\"FollowingObjectFunctionCallReference\"}},\"FunctionCallReference_2\":{\"last_node\":{\"from\":\"ValueDereferenceContinueSub\",\"to\":\"ValueDereferenceContinueSubReference\"}},\"ValueDereferenceWrapReference_2\":{\"last_node\":{\"from\":\"FollowingObjectFunctionCall\",\"to\":\"FollowingObjectFunctionCallReference\"}},\"ValueIdentifierNewWrap2\":{\"all_nodes\":{\"from\":\"ValueDereferenceWrap\",\"to\":\"ValueDereferenceWrapReference\"}},\"ValueDereferenceWrapReference_3\":{\"last_node\":{\"from\":\"IdentifierReference\",\"to\":\"IdentifierReferenceIgnore\"}}}";

	        
	        Dictionary<string, object> resultsPost = (Dictionary<string, object>)JsonHelper.Deserialize(postProcessingString);

	        this.postProcessingDefinition = resultsPost;*/

	        
	        //System.Diagnostics.Debug.WriteLine("-resultsPost-");
	        //System.Diagnostics.Debug.WriteLine(resultsPost);
	        //System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(resultsPost));

	        List<object> parse_results = this.parse(codeInput, dict);
	        this.parse_results = parse_results;
	    }
        
        //System.Diagnostics.Debug.WriteLine("-debugingresults-");
        //////////System.Diagnostics.Debug.WriteLine(parse_results);
        //System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(parse_results));
        ////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(this.subStateIndex));
    }

    public List<object> parse_results;
    public string mainstring;

    public void run(PHPInterpretation interpret) {
        List<object> parse_results = this.parse_results;
        System.Diagnostics.Debug.WriteLine("parse results: "+parse_results);
        System.Diagnostics.Debug.WriteLine("parse results: "+JsonConvert.SerializeObject(parse_results).Substring(0, 100));
        string input = this.mainstring;

        string encodedString = @"eyJGaXJzdFBhcmFtZXRlcklucHV0Ijp7ImZ1bmN0aW9uIjoicmV0dXJuX3BhcmFtZXRlcl9pbnB1dCIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiUHJlZml4ZWRJZGVudGlmaWVyIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiRmlyc3RQYXJhbWV0ZXJJbnB1dCIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9XX0sIlBhcmFtZXRlcklucHV0Ijp7ImZ1bmN0aW9uIjoiY29sbGVjdF9wYXJhbWV0ZXJzIiwidmFyaWFibGVzIjpbXX0sIlByZWZpeGVkSWRlbnRpZmllciI6eyJmdW5jdGlvbiI6InJldHVybl9wYXJhbWV0ZXJfaW5wdXRfaWRlbnRpZmllcl92YWx1ZSIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllciJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfV19LCJJZGVudGlmaWVyUHVzaCI6eyJmdW5jdGlvbiI6InNjcmlwdF9wdXNoX2FycmF5IiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZUlkZW50aWZpZXIiLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYWx1ZURlcmVmZXJlbmNlV3JhcCI6eyJmdW5jdGlvbiI6InJldHVybl9yZXN1bHRfZGVyZWZlcmVuY2UiLCJwcmVzZXJ2ZV9jb250ZXh0Ijp0cnVlLCJzZXRfY29udGV4dF9zdWIiOnRydWUsIndyYXBfdmFsdWVzIjp0cnVlLCJzd2l0Y2hlcyI6W3sibm9uX3Rlcm1pbmFsIjoiQXdhaXQiLCJzdWJfcGFyc2UiOiJGb2xsb3dpbmdPYmplY3RGdW5jdGlvbkNhbGwifV0sInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZSIsInBhcnNlIjp0cnVlLCJzZXRfY29udGV4dCI6dHJ1ZX0seyJub25fdGVybWluYWwiOlsiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIiwiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIl0sInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9XX0sIlZhbHVlRGVyZWZlcmVuY2VXcmFwUmVmZXJlbmNlIjp7ImZ1bmN0aW9uIjoicmV0dXJuX3Jlc3VsdF9kZXJlZmVyZW5jZSIsInNldF9jb250ZXh0X3N1YiI6dHJ1ZSwid3JhcF92YWx1ZXMiOnRydWUsInN3aXRjaGVzIjpbXSwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOlsiVmFsdWVEZXJlZmVyZW5jZSJdLCJwYXJzZSI6dHJ1ZSwic2V0X2NvbnRleHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjpbIkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCIsIkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbFJlZmVyZW5jZSIsIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSJdLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfV19LCJWYWx1ZVBhcmFudGhlc2lzIjp7ImZ1bmN0aW9uIjoic2V0X3BhcmFudGhlc2lzIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYXJpYWJsZUFzc2lnbm1lbnRJbmNyZW1lbnQiOnsiZnVuY3Rpb24iOiJzZXRfdmFyaWFibGVfaW5jcmVtZW50IiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZUlkZW50aWZpZXIiLCJwYXJzZSI6dHJ1ZX1dfSwiVmFyaWFibGVBc3NpZ25tZW50RGVjcmVtZW50Ijp7ImZ1bmN0aW9uIjoic2V0X3ZhcmlhYmxlX2RlY3JlbWVudCIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVJZGVudGlmaWVyIiwicGFyc2UiOnRydWV9XX0sIlZhcmlhYmxlQXNzaWdubWVudEFkZGl0aW9uIjp7ImZ1bmN0aW9uIjoic2V0X3ZhcmlhYmxlX3ZhbHVlX2FkZGl0aW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOlsiVmFsdWVJZGVudGlmaWVyIiwiVmFsdWVJZGVudGlmaWVyMiJdLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYXJpYWJsZUFzc2lnbm1lbnRTdWJ0cmFjdGlvbiI6eyJmdW5jdGlvbiI6InNldF92YXJpYWJsZV92YWx1ZV9zdWJ0cmFjdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjpbIlZhbHVlSWRlbnRpZmllciIsIlZhbHVlSWRlbnRpZmllcjIiXSwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiVmFyaWFibGVBc3NpZ25tZW50TXVsdGlwbGljYXRpb24iOnsiZnVuY3Rpb24iOiJzZXRfdmFyaWFibGVfdmFsdWVfbXVsdGlwbGljYXRpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6WyJWYWx1ZUlkZW50aWZpZXIiLCJWYWx1ZUlkZW50aWZpZXIyIl0sInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIlZhcmlhYmxlQXNzaWdubWVudERpdmlzaW9uIjp7ImZ1bmN0aW9uIjoic2V0X3ZhcmlhYmxlX3ZhbHVlX2RpdmlzaW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOlsiVmFsdWVJZGVudGlmaWVyIiwiVmFsdWVJZGVudGlmaWVyMiJdLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYXJpYWJsZUFzc2lnbm1lbnRBcHBlbmQiOnsiZnVuY3Rpb24iOiJzZXRfdmFyaWFibGVfdmFsdWVfYXBwZW5kIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOlsiVmFsdWVJZGVudGlmaWVyIiwiVmFsdWVJZGVudGlmaWVyMiJdLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYXJpYWJsZUFzc2lnbm1lbnQiOnsiZnVuY3Rpb24iOiJzZXRfdmFyaWFibGVfdmFsdWUiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6WyJWYWx1ZUlkZW50aWZpZXIiLCJWYWx1ZUlkZW50aWZpZXIyIl0sInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIlZhcmlhYmxlRGVmaW5pdGlvbiI6eyJmdW5jdGlvbiI6InNldF92YXJpYWJsZV92YWx1ZV9pbl9jb250ZXh0IiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9XX0sIk51bWVyaWNTdWJ0cmFjdGlvbiI6eyJmdW5jdGlvbiI6Im51bWVyaWNfc3VidHJhY3Rpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6Ik51bWJlciJ9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiVmFsdWVEaXZpc2lvbiI6eyJmdW5jdGlvbiI6Im51bWVyaWNfZGl2aXNpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9BZGRpdGlvbiIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIlZhbHVlQXBwZW5kU3RyaW5nIjp7ImZ1bmN0aW9uIjoic3RyaW5nX2FkZGl0aW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQWRkaXRpb24iLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYWx1ZU11bHRpcGxpY2F0aW9uIjp7ImZ1bmN0aW9uIjoibnVtZXJpY19tdWx0aXBsaWNhdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0FkZGl0aW9uIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiVmFsdWVBcHBlbmQiOnsiZnVuY3Rpb24iOiJudW1lcmljX2FkZGl0aW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQWRkaXRpb24iLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYWx1ZVN1YnRyYWN0aW9uIjp7ImZ1bmN0aW9uIjoibnVtZXJpY19zdWJ0cmFjdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0FkZGl0aW9uIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiVmFsdWVNb2R1bG8iOnsiZnVuY3Rpb24iOiJudW1lcmljX21vZHVsbyIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0FkZGl0aW9uIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiTnVtZXJpY0FkZGl0aW9uIjp7ImZ1bmN0aW9uIjoibnVtZXJpY19hZGRpdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiTnVtYmVyIn0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJPYmplY3RDYWxsQXBwZW5kIjp7ImZ1bmN0aW9uIjoibnVtZXJpY19hZGRpdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiT2JqZWN0Q2FsbCIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIlN0cmluZ0FwcGVuZCI6eyJmdW5jdGlvbiI6Im51bWVyaWNfYWRkaXRpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlN0cmluZyIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIklkZW50aWZpZXJBcHBlbmQiOnsiZnVuY3Rpb24iOiJudW1lcmljX2FkZGl0aW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiVmFsdWVQYXJhbnRoZXNpc1N1YnRyYWN0aW9uIjp7ImZ1bmN0aW9uIjoibnVtZXJpY19zdWJ0cmFjdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVQYXJhbnRoZXNpcyIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIlZhbHVlUGFyYW50aGVzaXNEaXZpc2lvbiI6eyJmdW5jdGlvbiI6Im51bWVyaWNfZGl2aXNpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlUGFyYW50aGVzaXMiLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYWx1ZVBhcmFudGhlc2lzQWRkaXRpb24iOnsiZnVuY3Rpb24iOiJudW1lcmljX2FkZGl0aW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZVBhcmFudGhlc2lzIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiRnVuY3Rpb25DYWxsU3VidHJhY3Rpb24iOnsiZnVuY3Rpb24iOiJudW1lcmljX3N1YnRyYWN0aW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJGdW5jdGlvbkNhbGwiLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJGdW5jdGlvbkNhbGxEaXZpc2lvbiI6eyJmdW5jdGlvbiI6Im51bWVyaWNfZGl2aXNpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IkZ1bmN0aW9uQ2FsbCIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIkZ1bmN0aW9uQ2FsbEFwcGVuZCI6eyJmdW5jdGlvbiI6Im51bWVyaWNfYWRkaXRpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IkZ1bmN0aW9uQ2FsbCIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIk9iamVjdElkZW50aWZpZXJSZWZlcmVuY2UiOnsiZnVuY3Rpb24iOiJzZXRfcHJvcGVydHlfcmVmZXJlbmNlIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn1dfSwiQ2xhc3NJZGVudGlmaWVyUmVmZXJlbmNlIjp7ImZ1bmN0aW9uIjoic2V0X2NsYXNzX3JlZmVyZW5jZSIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllciJ9XX0sIklkZW50aWZpZXJSZWZlcmVuY2UiOnsiZnVuY3Rpb24iOiJzZXRfdmFyaWFibGVfcmVmZXJlbmNlIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn1dfSwiSWRlbnRpZmllclJlZmVyZW5jZUlnbm9yZSI6eyJmdW5jdGlvbiI6InNldF92YXJpYWJsZV9yZWZlcmVuY2VfaWdub3JlIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIn1dfSwiU3RyaW5nIjp7ImZ1bmN0aW9uIjoic2V0X3N0cmluZyIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiU3RyaW5nQ29udGVudCJ9XX0sIkZvckxvb3AiOnsiZnVuY3Rpb24iOiJjcmVhdGVfZm9yX2xvb3AiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IkZvclNldHRpbmdzIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6ZmFsc2UsIm9wdCI6dHJ1ZX1dfSwiRm9yU2V0dGluZ3MiOnsiZnVuY3Rpb24iOiJzZXRfY29udHJvbF9zd2l0Y2hlcyIsInN3aXRjaGVzIjpbeyJub25fdGVybWluYWwiOiJGb3IifV0sInZhcmlhYmxlcyI6W119LCJGb3IiOnsiZnVuY3Rpb24iOiJzZXRfY29uZGl0aW9uIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYXJpYWJsZURlZmluaXRpb24iLCJwYXJzZSI6dHJ1ZSwic3ViX2NvbnRleHQiOmZhbHNlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWUsInN1Yl9jb250ZXh0IjpmYWxzZX0seyJub25fdGVybWluYWwiOiJWYXJpYWJsZUFzc2lnbm1lbnRHcm91cCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6ZmFsc2V9XX0sIkZvck9mIjp7ImZ1bmN0aW9uIjoic2V0X2NvbmRpdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIkZvckluIjp7ImZ1bmN0aW9uIjoic2V0X2NvbmRpdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9XX0sIldoaWxlTG9vcCI6eyJmdW5jdGlvbiI6ImNyZWF0ZV93aGlsZV9sb29wIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJDb25kaXRpb24iLCJwYXJzZSI6dHJ1ZSwic3ViX2NvbnRleHQiOmZhbHNlfSx7Im5vbl90ZXJtaW5hbCI6IlN0YXRlbWVudExpc3QiLCJwYXJzZSI6dHJ1ZSwic3ViX2NvbnRleHQiOmZhbHNlfV19LCJPckNvbmRpdGlvbiI6eyJmdW5jdGlvbiI6Im9yX2NvbmRpdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVDb25kaXRpb24iLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJDb25kaXRpb24iLCJwYXJzZSI6dHJ1ZSwic3ViX2NvbnRleHQiOnRydWV9XX0sIkFuZENvbmRpdGlvbiI6eyJmdW5jdGlvbiI6ImFuZF9jb25kaXRpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlQ29uZGl0aW9uIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiQ29uZGl0aW9uIiwicGFyc2UiOnRydWUsInN1Yl9jb250ZXh0Ijp0cnVlfV19LCJTdHJvbmdJbmVxdWFsVmFsdWVDb25kaXRpb24iOnsiZnVuY3Rpb24iOiJpbmVxdWFsaXR5X3N0cm9uZyIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24iLCJwYXJzZSI6dHJ1ZSwib2Zmc2V0IjoxfV19LCJTdHJvbmdFcXVhbFZhbHVlQ29uZGl0aW9uIjp7ImZ1bmN0aW9uIjoiZXF1YWxzX3N0cm9uZyIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24iLCJwYXJzZSI6dHJ1ZSwib2Zmc2V0IjoxfV19LCJJbmVxdWFsVmFsdWVDb25kaXRpb24iOnsiZnVuY3Rpb24iOiJpbmVxdWFsaXR5IiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQ29uZGl0aW9uIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiIsInBhcnNlIjp0cnVlLCJvZmZzZXQiOjF9XX0sIkVxdWFsVmFsdWVDb25kaXRpb24iOnsiZnVuY3Rpb24iOiJlcXVhbHMiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24iLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZU5vQ29uZGl0aW9uIiwicGFyc2UiOnRydWUsIm9mZnNldCI6MX1dfSwiR3JlYXRlclZhbHVlQ29uZGl0aW9uIjp7ImZ1bmN0aW9uIjoiZ3JlYXRlciIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24iLCJwYXJzZSI6dHJ1ZSwib2Zmc2V0IjoxfV19LCJMZXNzVmFsdWVDb25kaXRpb24iOnsiZnVuY3Rpb24iOiJsZXNzIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZU5vQ29uZGl0aW9uIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiIsInBhcnNlIjp0cnVlLCJvZmZzZXQiOjF9XX0sIkdyZWF0ZXJFcXVhbFZhbHVlQ29uZGl0aW9uIjp7ImZ1bmN0aW9uIjoiZ3JlYXRlcl9lcXVhbHMiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24iLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZU5vQ29uZGl0aW9uIiwicGFyc2UiOnRydWUsIm9mZnNldCI6MX1dfSwiTGVzc0VxdWFsVmFsdWVDb25kaXRpb24iOnsiZnVuY3Rpb24iOiJsZXNzX2VxdWFscyIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0NvbmRpdGlvbiIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24iLCJwYXJzZSI6dHJ1ZSwib2Zmc2V0IjoxfV19LCJJZlN0YXRlbWVudCI6eyJmdW5jdGlvbiI6ImNyZWF0ZV9pZl9zdGF0ZW1lbnQiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6ZmFsc2V9LHsibm9uX3Rlcm1pbmFsIjoiRWxzZUlmU3RhdGVtZW50R3JvdXAiLCJwYXJzZSI6dHJ1ZX1dfSwiRWxzZVN0YXRlbWVudCI6eyJmdW5jdGlvbiI6InNldF9lbHNlX2NvbmRpdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6ZmFsc2V9XX0sIkVsc2VJZlN0YXRlbWVudCI6eyJmdW5jdGlvbiI6InNldF9jb25kaXRpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6ZmFsc2V9LHsibm9uX3Rlcm1pbmFsIjoiRWxzZUlmU3RhdGVtZW50R3JvdXAiLCJwYXJzZSI6dHJ1ZX1dfSwiRnVuY3Rpb25EZWZpbml0aW9uTm9Bc3luYyI6eyJmdW5jdGlvbiI6ImNyZWF0ZV9zY3JpcHRfZnVuY3Rpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlcklucHV0cyIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6dHJ1ZX1dfSwiTmFtZWRGdW5jdGlvbkRlZmluaXRpb24iOnsiZnVuY3Rpb24iOiJjcmVhdGVfbmFtZWRfc2NyaXB0X2Z1bmN0aW9uIiwic3dpdGNoZXMiOlt7Im5vbl90ZXJtaW5hbCI6IkFzeW5jRnVuY3Rpb25QcmVmaXgifV0sInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllciIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlcklucHV0cyIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6dHJ1ZX1dfSwiRnVuY3Rpb25EZWZpbml0aW9uIjp7ImZ1bmN0aW9uIjoiY3JlYXRlX3NjcmlwdF9mdW5jdGlvbiIsInN3aXRjaGVzIjpbeyJub25fdGVybWluYWwiOiJBc3luY0Z1bmN0aW9uUHJlZml4In1dLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlcklucHV0cyIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6dHJ1ZX1dfSwiRmlyc3RPYmplY3RTdGF0ZW1lbnQiOnsiZnVuY3Rpb24iOiJzZXRfZGljdGlvbmFyeV92YWx1ZSIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiUHJvcGVydHlOYW1lIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiT2JqZWN0U3RhdGVtZW50Ijp7ImZ1bmN0aW9uIjoic2V0X2RpY3Rpb25hcnlfdmFsdWUiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlByb3BlcnR5TmFtZSIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiT2JqZWN0U3RhdGVtZW50IiwicGFyc2UiOnRydWUsIm9wdCI6dHJ1ZX1dfSwiVmFsdWVOZWdhdGl2ZSI6eyJmdW5jdGlvbiI6Im5lZ2F0aXZlX3ZhbHVlIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfV19LCJWYWx1ZU5lZ2F0ZWQiOnsiZnVuY3Rpb24iOiJuZWdhdGVfdmFsdWUiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlTm9Db25kaXRpb24iLCJwYXJzZSI6dHJ1ZX1dfSwiQ2xhc3NCb2R5Ijp7ImZ1bmN0aW9uIjoic2V0X2FjY2Vzc19mbGFnIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJBY2Nlc3NGbGFnIn0seyJub25fdGVybWluYWwiOlsiVmFyaWFibGVEZWZpbml0aW9uIiwiTmFtZWRGdW5jdGlvbkRlZmluaXRpb24iXSwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiQ2xhc3NCb2R5IiwicGFyc2UiOnRydWUsIm9wdCI6dHJ1ZX1dfSwiRXh0ZW5kc1N0YXRlbWVudCI6eyJmdW5jdGlvbiI6InNldF9wcm90b3R5cGUiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IkNsYXNzSWRlbnRpZmllclJlZmVyZW5jZSIsInBhcnNlIjp0cnVlfV19LCJDbGFzc0RlZmluaXRpb24iOnsiZnVuY3Rpb24iOiJjcmVhdGVfc2NyaXB0X29iamVjdCIsInNldF9wb3N0X2Z1bmN0aW9uIjoic2V0X29iamVjdF9pZGVudGlmaWVyIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiRXh0ZW5kc1N0YXRlbWVudCIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiQ2xhc3NCb2R5IiwicGFyc2UiOnRydWV9XX0sIk9iamVjdERlZmluaXRpb24iOnsiZnVuY3Rpb24iOiJjcmVhdGVfc2NyaXB0X29iamVjdCIsInBvc3RfZnVuY3Rpb24iOiJyZXZlcnNlX2FycmF5IiwidmFyaWFibGVzIjpbXX0sIlBhcmFtZXRlcklucHV0cyI6eyJmdW5jdGlvbiI6ImNvbGxlY3RfcGFyYW1ldGVycyIsInZhcmlhYmxlcyI6W119LCJQYXJhbWV0ZXJWYWx1ZXNDb25zdHJ1Y3RvciI6eyJmdW5jdGlvbiI6ImNvbGxlY3RfcGFyYW1ldGVycyIsInZhcmlhYmxlcyI6W119LCJQYXJhbWV0ZXJWYWx1ZXMiOnsicGFyZW50X2NvbnRleHQiOnRydWUsImZ1bmN0aW9uIjoiY29sbGVjdF9wYXJhbWV0ZXJzIiwidmFyaWFibGVzIjpbXX0sIkZpcnN0UGFyYW1ldGVyVmFsdWUiOnsiZnVuY3Rpb24iOiJjb2xsZWN0X3BhcmFtZXRlcnMiLCJ2YXJpYWJsZXMiOltdfSwiUGFyYW1ldGVyVmFsdWUiOnsiZnVuY3Rpb24iOiJjb2xsZWN0X3BhcmFtZXRlcnMiLCJ2YXJpYWJsZXMiOltdfSwiT2JqZWN0U3RhdGVtZW50TGlzdCI6eyJmdW5jdGlvbiI6Im51bGwiLCJ2YXJpYWJsZXMiOltdfSwiU3RhdGVtZW50TGlzdCI6eyJmdW5jdGlvbiI6InJldHVybl9yZXN1bHQiLCJ2YXJpYWJsZXMiOltdfSwiU3RhdGVtZW50Ijp7ImV4Y2VwdGlvbiI6dHJ1ZSwiZnVuY3Rpb24iOiJyZXR1cm5fcmVzdWx0IiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJSZXR1cm5TdGF0ZW1lbnQiLCJwYXJzZSI6dHJ1ZX1dfSwiT2JqZWN0RnVuY3Rpb25DYWxsVmFsdWUiOnsiZnVuY3Rpb24iOiJyZXR1cm5fcHJvcF9yZXN1bHQiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6WyJPYmplY3RGdW5jdGlvbkNhbGwiLCJWYWx1ZU9iamVjdERlcmVmZXJlbmNlV3JhcCJdLCJwYXJzZSI6dHJ1ZX1dfSwiT2JqZWN0Q2FsbFZhbHVlIjp7ImZ1bmN0aW9uIjoicmV0dXJuX3Byb3BfcmVzdWx0IiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJPYmplY3RJZGVudGlmaWVyUmVmZXJlbmNlIiwicGFyc2UiOnRydWV9XX0sIk9iamVjdENhbGxXcmFwIjp7ImZ1bmN0aW9uIjoicmV0dXJuX3ZhbHVlX3Jlc3VsdCIsInN3aXRjaGVzIjpbeyJub25fdGVybWluYWwiOiJBd2FpdCIsInN1Yl9wYXJzZSI6Ik9iamVjdEZ1bmN0aW9uQ2FsbCJ9XSwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJPYmplY3RDYWxsIiwicGFyc2UiOnRydWV9XX0sIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZVN1YiI6eyJmdW5jdGlvbiI6ImdldF9hcnJheV92YWx1ZV9jb250ZXh0Iiwic2V0X2NvbnRleHRfc3ViIjp0cnVlLCJpZ25vcmVfZW1wdHkiOnRydWUsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjpbIlZhbHVlIiwiSWRlbnRpZmllciJdLCJwYXJzZSI6dHJ1ZX1dfSwiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlU3ViUmVmZXJlbmNlIjp7ImZ1bmN0aW9uIjoiZ2V0X2FycmF5X3ZhbHVlX2NvbnRleHRfcmVmZXJlbmNlIiwiaWdub3JlX2VtcHR5Ijp0cnVlLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6WyJWYWx1ZSIsIklkZW50aWZpZXIiXSwicGFyc2UiOnRydWV9XX0sIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSI6eyJmdW5jdGlvbiI6InJldHVybl9yZXN1bHRfZGVyZWZlcmVuY2UiLCJ3cmFwX3ZhbHVlcyI6dHJ1ZSwiaWdub3JlX2VtcHR5Ijp0cnVlLCJzd2l0Y2hlcyI6W10sInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjpbIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZVN1YiIsIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZVN1YlJlZmVyZW5jZSJdLCJwYXJzZSI6dHJ1ZSwic2V0X2NvbnRleHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjpbIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCIsIkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbFJlZmVyZW5jZSJdLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfV19LCJPYmplY3RDYWxsQ29udGludWUiOnsiZnVuY3Rpb24iOiJyZXR1cm5fcHJvcF9yZXN1bHQiLCJ3cmFwX3ZhbHVlcyI6dHJ1ZSwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOlsiT2JqZWN0SWRlbnRpZmllclJlZmVyZW5jZSIsIk9iamVjdENhbGwiLCJPYmplY3RGdW5jdGlvbkNhbGxWYWx1ZSJdLCJwYXJzZSI6dHJ1ZSwic2V0X2NvbnRleHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIiwicGFyc2UiOnRydWUsIm9wdCI6dHJ1ZX1dfSwiT2JqZWN0Q2FsbCI6eyJmdW5jdGlvbiI6InJldHVybl9sYXN0X3Byb3BfcmVzdWx0Iiwid3JhcF92YWx1ZXMiOnRydWUsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSIsInBhcnNlIjp0cnVlLCJzZXRfY29udGV4dCI6dHJ1ZX0seyJub25fdGVybWluYWwiOlsiT2JqZWN0Q2FsbFZhbHVlIiwiT2JqZWN0Q2FsbCIsIk9iamVjdEZ1bmN0aW9uQ2FsbFZhbHVlIl0sInBhcnNlIjp0cnVlfV19LCJOZXdTdGF0ZW1lbnQiOnsiZnVuY3Rpb24iOiJuZXdfaW5zdGFuY2UiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IkNsYXNzSWRlbnRpZmllclJlZmVyZW5jZSIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlclZhbHVlc0NvbnN0cnVjdG9yIiwib3B0Ijp0cnVlLCJwYXJzZSI6dHJ1ZX1dfSwiUmV0dXJuU3RhdGVtZW50Ijp7ImZ1bmN0aW9uIjoicmV0dXJuX3ZhbHVlX3Jlc3VsdCIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiSW5saW5lRnVuY3Rpb25EZWZpbml0aW9uQ2FsbCI6eyJmdW5jdGlvbiI6ImNhbGxfc2NyaXB0X2Z1bmN0aW9uX3N1YiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiRnVuY3Rpb25EZWZpbml0aW9uTm9Bc3luYyIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IlBhcmFtZXRlclZhbHVlcyIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIiwicGFyc2UiOnRydWUsIm9wdCI6dHJ1ZX1dfSwiT2JqZWN0RnVuY3Rpb25DYWxsIjp7ImZ1bmN0aW9uIjoiY2FsbF9zY3JpcHRfZnVuY3Rpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UiLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZXMiLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9XX0sIkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbFJlZmVyZW5jZSI6eyJmdW5jdGlvbiI6ImNhbGxfc2NyaXB0X2Z1bmN0aW9uX3N1Yl9yZWZlcmVuY2UiLCJpZ25vcmVfZW1wdHkiOnRydWUsInNldF9jb250ZXh0X3N1YiI6dHJ1ZSwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZXMiLCJwYXJzZSI6dHJ1ZX1dfSwiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIjp7ImZ1bmN0aW9uIjoiY2FsbF9zY3JpcHRfZnVuY3Rpb25fc3ViIiwiaWdub3JlX2VtcHR5Ijp0cnVlLCJzZXRfY29udGV4dF9zdWIiOnRydWUsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVyVmFsdWVzIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjpbIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCJdLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfV19LCJGdW5jdGlvbkNhbGxSZWZlcmVuY2UiOnsiZnVuY3Rpb24iOiJjYWxsX3NjcmlwdF9mdW5jdGlvbl9yZWZlcmVuY2UiLCJzZXRfY29udGV4dF9zdWIiOnRydWUsInN3aXRjaGVzIjpbeyJub25fdGVybWluYWwiOiJBd2FpdCJ9XSwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiUGFyYW1ldGVyVmFsdWVzIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjpbIlZhbHVlRGVyZWZlcmVuY2VDb250aW51ZSIsIkZvbGxvd2luZ09iamVjdEZ1bmN0aW9uQ2FsbCJdLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfV19LCJGdW5jdGlvbkNhbGwiOnsiZnVuY3Rpb24iOiJjYWxsX3NjcmlwdF9mdW5jdGlvbiIsInNldF9jb250ZXh0X3N1YiI6dHJ1ZSwic3dpdGNoZXMiOlt7Im5vbl90ZXJtaW5hbCI6IkF3YWl0In1dLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UiLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJQYXJhbWV0ZXJWYWx1ZXMiLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOlsiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlIiwiRm9sbG93aW5nT2JqZWN0RnVuY3Rpb25DYWxsIl0sInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9XX0sIlZhbHVlRGVyZWZlcmVuY2UiOnsiZnVuY3Rpb24iOiJyZXR1cm5fcmVzdWx0X2RlcmVmZXJlbmNlIiwid3JhcF92YWx1ZXMiOnRydWUsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjpbIklkZW50aWZpZXJSZWZlcmVuY2UiLCJWYWx1ZU5vRGVyZWZlcmVuY2UiLCJJZGVudGlmaWVyUmVmZXJlbmNlSWdub3JlIl0sInBhcnNlIjp0cnVlLCJzZXRfY29udGV4dCI6dHJ1ZX0seyJub25fdGVybWluYWwiOlsiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlU3ViIiwiVmFsdWVEZXJlZmVyZW5jZUNvbnRpbnVlU3ViUmVmZXJlbmNlIl0sInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9XX0sIlZhbHVlT2JqZWN0RGVyZWZlcmVuY2VXcmFwIjp7ImZ1bmN0aW9uIjoicmV0dXJuX3Jlc3VsdF9kZXJlZmVyZW5jZSIsIndyYXBfdmFsdWVzIjp0cnVlLCJzd2l0Y2hlcyI6W3sibm9uX3Rlcm1pbmFsIjoiT2JqZWN0Q2FsbENvbnRpbnVlIn1dLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlT2JqZWN0RGVyZWZlcmVuY2UiLCJwYXJzZSI6dHJ1ZSwic2V0X2NvbnRleHQiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiT2JqZWN0Q2FsbENvbnRpbnVlIiwicGFyc2UiOnRydWUsIm9wdCI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJWYWx1ZURlcmVmZXJlbmNlQ29udGludWUiLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfV19LCJWYWx1ZU9iamVjdERlcmVmZXJlbmNlIjp7ImZ1bmN0aW9uIjoiZ2V0X2FycmF5X3ZhbHVlIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX1dfSwiQXJyYXkiOnsiZnVuY3Rpb24iOiJjcmVhdGVfc2NyaXB0X29iamVjdCIsInZhcmlhYmxlcyI6W119LCJGaXJzdEFycmF5SXRlbSI6eyJmdW5jdGlvbiI6InNjcmlwdF9hcnJheV9wdXNoIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZSIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IkFycmF5SXRlbXMiLCJwYXJzZSI6dHJ1ZSwib3B0Ijp0cnVlfV19LCJBcnJheUl0ZW1zIjp7ImZ1bmN0aW9uIjoic2NyaXB0X2FycmF5X3B1c2giLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiQXJyYXlJdGVtcyIsInBhcnNlIjp0cnVlLCJvcHQiOnRydWV9XX0sIlR5cGVvZlZhbHVlIjp7ImZ1bmN0aW9uIjoiZ2V0X3R5cGVvZiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWVOb0FkZGl0aW9uIiwicGFyc2UiOnRydWV9XX0sIkRlbGV0ZVZhbHVlRGVyZWZlcmVuY2UiOnsiZnVuY3Rpb24iOiJkZWxldGVfcHJvcGVydHlfcmVmZXJlbmNlIiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZUlkZW50aWZpZXIiLCJwYXJzZSI6dHJ1ZX1dfSwiT2JqZWN0U3ByZWFkT3BlcmF0b3IiOnsiZnVuY3Rpb24iOiJjbG9uZV9vYmplY3QiLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlSWRlbnRpZmllciIsInBhcnNlIjp0cnVlfV19LCJBcnJheVNwcmVhZE9wZXJhdG9yIjp7ImZ1bmN0aW9uIjoiY2xvbmVfb2JqZWN0IiwidmFyaWFibGVzIjpbeyJub25fdGVybWluYWwiOiJWYWx1ZUlkZW50aWZpZXIiLCJwYXJzZSI6dHJ1ZX1dfSwiU2NyaXB0U3RhdGVtZW50Ijp7ImZ1bmN0aW9uIjoibnVsbCIsInZhcmlhYmxlcyI6W119LCJGb3JFYWNoIjp7ImZ1bmN0aW9uIjoiY3JlYXRlX2ZvcmVhY2hfbG9vcCIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiRm9yZWFjaFNldHRpbmdzIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiU3RhdGVtZW50TGlzdCIsInBhcnNlIjp0cnVlLCJzdWJfY29udGV4dCI6ZmFsc2UsIm9wdCI6dHJ1ZX1dfSwiRm9yZWFjaFNldHRpbmdzMSI6eyJmdW5jdGlvbiI6InNldF9jb25kaXRpb24iLCJ2YXJpYWJsZXMiOlt7Im5vbl90ZXJtaW5hbCI6IlZhbHVlIiwicGFyc2UiOnRydWV9LHsibm9uX3Rlcm1pbmFsIjoiSWRlbnRpZmllclJlZmVyZW5jZSIsInBhcnNlIjp0cnVlfSx7Im5vbl90ZXJtaW5hbCI6IklkZW50aWZpZXJSZWZlcmVuY2UiLCJwYXJzZSI6dHJ1ZSwib2Zmc2V0IjoxfV19LCJGb3JlYWNoU2V0dGluZ3MyIjp7ImZ1bmN0aW9uIjoic2V0X2NvbmRpdGlvbiIsInZhcmlhYmxlcyI6W3sibm9uX3Rlcm1pbmFsIjoiVmFsdWUiLCJwYXJzZSI6dHJ1ZX0seyJub25fdGVybWluYWwiOiJJZGVudGlmaWVyUmVmZXJlbmNlIiwicGFyc2UiOnRydWV9XX19";

        byte[] data = Convert.FromBase64String(encodedString);
        string decodedString = System.Text.Encoding.UTF8.GetString(data);

        //var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(decodedString, _jsonSettings);
        Dictionary<object, object> dict = (Dictionary<object, object>)JsonHelper2.Deserialize(decodedString);
        this.resultsDefinition = dict;

        this.assignDictValues(((List<object>)parse_results)[0]);
        this.containsIdentifier((Dictionary<object, object>)parse_results[0]);
        this.containsThis((Dictionary<object, object>)parse_results[0]);
        //this.containsFunctionDefinition((Dictionary<object, object>)parse_results[0]);
        interpret.construct(parse_results, dict, input, this);
        //System.Diagnostics.Debug.WriteLine("-debugingresults-");
        ////////////////System.Diagnostics.Debug.WriteLine(parse_results);
        ////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(parse_results));
        interpret.start(null);
    }

    public void assignDictValues(object parseResults) {
        /*if(parseResults is List<object>) {
            this.assignDictValues(((List<object>)parseResults)[0]);
        } else {*/
            Dictionary<object, object> parseItem = (Dictionary<object, object>)parseResults;
        /*if((string)parseItem["label"] != "ScriptStatement" && !this.resultsDefinition.ContainsKey((string)parseItem["label"])) {
            return;
        }*/
        if((string)parseItem["label"] == "WhiteSpace") {
            return;
        }
        if(((string)parseItem["label"]).Contains("FunctionDefinition")) {
            this.getText((int)parseItem["start_index"], (int)parseItem["stop_index"], true, parseItem);
        }
            List<object> variableReferences = new List<object>();
            if(this.resultsDefinition.ContainsKey((string)parseItem["label"])) {
                variableReferences = (List<object>)((Dictionary<object, object>)this.resultsDefinition[parseItem["label"]])["variables"];
            }
            parseItem["variable_references"] = new List<object>();
            if(parseItem.ContainsKey("sub_parse_objects") && ((List<object>)parseItem["sub_parse_objects"]).Count > 0) {
                
                parseItem["sub_parse_objects_dict"] = new Dictionary<object, object>();
                foreach(object subParseObjectItem in ((List<object>)parseItem["sub_parse_objects"])) {
                    Dictionary<object, object> subParseObjectDict = (Dictionary<object, object>)subParseObjectItem;
                    string label = (string)subParseObjectDict["label"];
                    if(!((Dictionary<object, object>)parseItem["sub_parse_objects_dict"]).ContainsKey(label)) {
                        ((Dictionary<object, object>)parseItem["sub_parse_objects_dict"])[label] = new List<object>();
                    }
                    ((List<object>)((Dictionary<object, object>)parseItem["sub_parse_objects_dict"])[label]).Add(subParseObjectItem);
                    this.assignDictValues(subParseObjectItem);
                }
                ////System.Diagnostics.Debug.WriteLine("set variable references: "+parseItem["label"]);

                foreach(Dictionary<object, object> variableReference in variableReferences) {
                    int offset = 0;
                    if(variableReference.ContainsKey("offset")) {
                        offset = (int)variableReference["offset"];
                    }
                    
                    Dictionary<object, object> subParseObject = this.getSubParseObject(parseItem, variableReference["non_terminal"], offset);
                    if(subParseObject != null) {
                        Dictionary<object, object> varRefCopy = new Dictionary<object, object>(variableReference);
                        varRefCopy["subParseObject"] = subParseObject;
                        ((List<object>)parseItem["variable_references"]).Add(varRefCopy);
                    
                    }
                } 
            } else if(!parseItem.ContainsKey("sub_parse_objects") || ((List<object>)parseItem["sub_parse_objects"]).Count == 0) {
                ////System.Diagnostics.Debug.WriteLine("not-set variable references: "+parseItem["label"]);
                bool isString = false;
                Dictionary<object, object> subParseObject = parseItem;
                if((string)subParseObject["label"] == "StringContent"  || (string)subParseObject["label"] == "Identifier" || (string)subParseObject["label"] == "AccessFlag" || (string)subParseObject["label"] == "ObjectDefinition") {
                    isString = true;
                }
                this.getText((int)subParseObject["start_index"], (int)subParseObject["stop_index"], isString, subParseObject);
            } else {
            
                ////System.Diagnostics.Debug.WriteLine("not-set variable references: "+parseItem["label"]);
            }
        //}
    }

    public object getText(int startIndex, int stopIndex, bool isString, Dictionary<object, object> parseObject) {
        if(parseObject.ContainsKey("text_value")) {
            return parseObject["text_value"];
        }
        ////System.Diagnostics.Debug.WriteLine("get text: "+startIndex+" - "+stopIndex);
        string value = this.sourceText.Substring(startIndex, stopIndex-startIndex);
        ////System.Diagnostics.Debug.WriteLine("get text: "+value+" - "+this.sourceText);

        object set_value = value;
        if(value == "true") {
            isString = true;
            set_value = true;
        } else if(value == "false") {
            isString = true;
            set_value = false;
        } else if(value == "NULL") {
            //isString = true;
            isString = false;
            set_value = 0;
        }
        ////System.Diagnostics.Debug.WriteLine("value is : "+value+" set value: "+set_value+" is string: "+isString);
        if(isString) {
            //set_value = value;
        } else {
            
        	var negative = false;
        	if(set_value is string && set_value.ToString().IndexOf("-") == 0) {
        		negative = true;
        	}
            try {
                set_value = Convert.ToInt32(set_value);
                if(negative) {
                	set_value = -(int)set_value;
                }
            } catch(Exception ex) {
                set_value = value;
            }
        }
        parseObject["text_value"] = set_value;
        return set_value;
    }

    Dictionary<object, object> getSubParseObject(Dictionary<object, object> parseObject, object name, int offset) {
        int setOffset = -1;
        if(parseObject.ContainsKey("sub_parse_objects_dict")) {
            if(name is List<object>) {
                List<object> nt = (List<object>)name;
                foreach(object ntValue in nt) {
                    if(((Dictionary<object, object>)parseObject["sub_parse_objects_dict"]).ContainsKey(ntValue)) {
                        return (Dictionary<object,object>)((List<object>)((Dictionary<object, object>)parseObject["sub_parse_objects_dict"])[ntValue])[offset];
                    }
                }
            } else {
                string ntValue = (string)name;
                if(!((Dictionary<object, object>)parseObject["sub_parse_objects_dict"]).ContainsKey(ntValue)) {
                    return null;
                }
                //if(((List<object>)((Dictionary<object, object>)parseObject["sub_parse_objects_dict"])[ntValue]).Count > offset) {
                    return (Dictionary<object,object>)((List<object>)((Dictionary<object, object>)parseObject["sub_parse_objects_dict"])[ntValue])[offset];
                //}
            }
        }
        return null;
    }

    public List<object> postProcess(List<object> children, Dictionary<string, object> setTransformations, bool parentIsLast) {
    	////System.Diagnostics.Debug.WriteLine("post process : "+JsonConvert.SerializeObject(setTransformations));
        if(setTransformations == null) {
            setTransformations = new Dictionary<string, object>();
        }
        foreach(KeyValuePair<string, object> pair in setTransformations) {
            Dictionary<string, object> transformation = (Dictionary<string, object>)pair.Value;//setTransformations[pair.Key];
            ////System.Diagnostics.Debug.WriteLine("transformation: "+pair.Key+" - "+JsonConvert.SerializeObject(transformation));
            if(transformation.ContainsKey("swap") && children.Count == 2) {
                if((string)((Dictionary<string, object>)children[0])["label"] == (string)((Dictionary<string, object>)transformation["swap"])["from"]) {
                    List<object> newChildren = new List<object>();
                    List<object> revChildren = new List<object>(children);
                    revChildren.Reverse();
                    foreach(object child in revChildren) {
                        newChildren.Add(child);
                    }
                    children = newChildren;
                }
            }

        }
        int key = 0;
        List<object> newChildren2 = new List<object>();
        ////System.Diagnostics.Debug.WriteLine("children count: "+children.Count);
        foreach(Dictionary<object, object> child_immutable in children) {
            Dictionary<object, object> child = new Dictionary<object, object>(child_immutable);
            bool isLast = false;
            if(key == children.Count-1) {
                isLast = true;
            }
            foreach(KeyValuePair<string, object> transformationsKeyPair in setTransformations) {
            	string transformationsKey = transformationsKeyPair.Key;
                Dictionary<string, object> transformations = (Dictionary<string, object>)transformationsKeyPair.Value;//setTransformations[transformationsKey.ToString()];
                if(transformations.ContainsKey("last_node") && isLast && parentIsLast) {
                    if((string)((Dictionary<string, object>)transformations["last_node"])["from"] == (string)child["label"]) {
                    	////System.Diagnostics.Debug.WriteLine("changed child: "+child["label"]+" to "+((Dictionary<string, object>)transformations["last_node"])["to"]);
                        child["label"] = ((Dictionary<string, object>)transformations["last_node"])["to"];
                    }
                }
                if(transformations.ContainsKey("all_nodes")) {
                    if((string)((Dictionary<string, object>)transformations["all_nodes"])["from"] == (string)child["label"]) {
                        
                    	////System.Diagnostics.Debug.WriteLine("changed child: "+child["label"]+" to "+((Dictionary<string, object>)transformations["all_nodes"])["to"]);
                    	
                        child["label"] = ((Dictionary<string, object>)transformations["all_nodes"])["to"];
                    }
                }
            }
            List<object> labels = new List<object>() {
                child["label"],
                child["label"]+"_2",
                child["label"]+"_3",
            };

            foreach(string childLabel in labels) {
                if(this.postProcessingDefinition.ContainsKey(childLabel)) {
            		////System.Diagnostics.Debug.WriteLine("child label: "+childLabel);
                    if(!setTransformations.ContainsKey(childLabel)) {

            			////System.Diagnostics.Debug.WriteLine("setTransformations label");
                        setTransformations[childLabel] = this.postProcessingDefinition[childLabel];
    					////System.Diagnostics.Debug.WriteLine("post process : "+JsonConvert.SerializeObject(setTransformations[childLabel]));
                    }
                }
            }
            child["sub_parse_objects"] = this.postProcess((List<object>)child["sub_parse_objects"], new Dictionary<string, object>(setTransformations), isLast);
            newChildren2.Add(child);
            key++;
        }
        ////System.Diagnostics.Debug.WriteLine("newChildren2 count: "+newChildren2.Count);
        return newChildren2;
    }

    private List<object> delimitRegex = new List<object>() {
        " ",
        ";",
        "(",
        ")",
        ",",
        ":",
    };

    private List<object> subStateRegexIndex;
    private Dictionary<object, object> subStateIndex;
    private Dictionary<object, object> subStateTerminalIndex;

    public void initArrays() {
        this.subStateRegexIndex = new List<object>();
        this.subStateIndex = new Dictionary<object, object>();
        this.subStateTerminalIndex = new Dictionary<object, object>();
    }

    Dictionary<string, object> language;
    List<char> textChar;
    List<string> text;

    public List<object> parse(string input, Dictionary<string, object> language, string margin="") {
        this.language = language;
        this.sourceText = input;
        this.textChar = input.ToCharArray().ToList<char>();
        this.text = new List<string>();
        foreach(char character in textChar) {
            this.text.Add(character.ToString());
        }
        List<object> start_states = (List<object>)language["start_states"];
        int index = 0;
        Dictionary<object, object> set_index = null;
        List<object> parse_objects = new List<object>();
        while(index < this.text.Count) {
            foreach(object state in start_states) {
                //////System.Diagnostics.Debug.WriteLine(margin+"start state subParse: "+state);
                set_index = this.subParse((string)state, index, margin+"    +");
                if(set_index != null) {
                    parse_objects.Add(set_index);
                    index = (int)set_index["stop_index"];
                }

                ////////////////System.Diagnostics.Debug.WriteLine("state : "+state);
                ////////////////System.Diagnostics.Debug.WriteLine(set_index);
            }
            if(set_index == null) {
                return null;
            }
        }
        //return parse_objects;
        List<object> resPost = this.postProcess(parse_objects, null, false);


        ////System.Diagnostics.Debug.WriteLine("-resPost-");
        ////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(resPost));

        return resPost;
    }

    public List<string> return_all_results = new List<string>() {
        @"ValueObjectDereferenceWrap", @"ValueDereferenceWrap", @"FollowingObjectFunctionCall", @"ValueDereferenceContinue"
    };

    public Dictionary<object, object> subParse(string state, int index, string margin) {
        //////System.Diagnostics.Debug.WriteLine(margin+"subParse state: "+state+" index: "+index+" stepstart: "+this.currentStepStart);


        List<object> definition = (List<object>)((Dictionary<string, object>)this.language["states"])[state];
        int start_index = index;
        if(index >= this.text.Count) {
            //////System.Diagnostics.Debug.WriteLine(margin+"return null0");
            return null;
        }
        if(index < this.currentStepStart) {
            //////System.Diagnostics.Debug.WriteLine(margin+"return null1");
            return null;
        }


    	/*string parseString = this.sourceText.Substring(start_index);
    	parseString = parseString.Substring(0, parseString.IndexOf("}"));
    	List<Dictionary<string, object>> resCache = this.local_data_instance.query_("SELECT * FROM parse_cache WHERE input_value = ?", new List<object>() { parseString });
    	if(resCache.Count > 0) {
    		return JsonConvert.DeserializeObject<Dictionary<object, object>>((((Dictionary<string, object>)resCache[0])["output_value"]).ToString());
    	}*/

        List<object> results = new List<object>();
        int set_sub_state_index = 0;
        foreach(object sub_state_item in definition) {
            List<object> sub_state = (List<object>)sub_state_item;
            ////////System.Diagnostics.Debug.WriteLine("subStateIs: "+JsonConvert.SerializeObject(sub_state)+" - "+start_index+" "+state);
            Dictionary<object, object> set_result = this.subState(sub_state, start_index, state, set_sub_state_index, margin+"    |");
            //////////System.Diagnostics.Debug.WriteLine("substateitem: "+JsonConvert.SerializeObject(sub_state_item)+" set_result: "+JsonConvert.SerializeObject(set_result));
            if(return_all_results.IndexOf(state) != -1) {
                //////System.Diagnostics.Debug.WriteLine("return_all_results: "+state);
                if(set_result != null) {
                    results.Add(set_result);
                }
            } else if(set_result != null) {
                //////System.Diagnostics.Debug.WriteLine("not_return_all_results: "+state+"-"+set_result["start_index"]);
                this.currentStepStart = (int)set_result["start_index"];

		        //this.local_data_instance.execute("DELETE FROM parse_cache WHERE input_value = ?", new List<object>() { parseString });
		        //this.local_data_instance.execute("INSERT INTO parse_cache (input_value, output_value) VALUES(?, ?)", new List<object>() { parseString, JsonConvert.SerializeObject(set_result) });

                return set_result;
            }
            set_sub_state_index++;
        }

        ////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(results));

        if(return_all_results.IndexOf(state) == -1) {
            //////System.Diagnostics.Debug.WriteLine("2return_all_results: "+state);
            this.currentStepStart = -1;
            return null;
        }

        int longest = -1;
        Dictionary<object, object> result_return = null;
        foreach(object resultObj in results) {
            Dictionary<object, object> result = (Dictionary<object, object>)resultObj;
            if((int)result["stop_index"] > longest || longest == -1) {
                longest = (int)result["stop_index"];
                result_return = result;
            }
        }
        if(result_return != null) {
            //////System.Diagnostics.Debug.WriteLine("currentStepStart: "+JsonConvert.SerializeObject(result_return));
            this.currentStepStart = (int)result_return["stop_index"];
        }
        //this.local_data_instance.execute("DELETE FROM parse_cache WHERE input_value = ?", new List<object>() { parseString });
        //this.local_data_instance.execute("INSERT INTO parse_cache (input_value, output_value) VALUES(?, ?)", new List<object>() { parseString, JsonConvert.SerializeObject(result_return) });
        return result_return;
    }

    public Dictionary<object, object> subState(List<object> sub_state, int start_index, string state, int set_sub_state_index, string margin) {
        //////System.Diagnostics.Debug.WriteLine(margin+"subState : "+JsonConvert.SerializeObject(sub_state));
        //////////////System.Diagnostics.Debug.WriteLine("state state : "+state+" start_idnex: "+start_index);
        if(this.parseObjectItems.ContainsKey(start_index) && ((Dictionary<object, object>)this.parseObjectItems[start_index]).ContainsKey(state)) {
            //////////////System.Diagnostics.Debug.WriteLine("parseObjectItems - {0} - {1} - {2} - {3}", state, start_index, JsonConvert.SerializeObject(this.parseObjectItems[start_index]));
            this.subStateRegexIndex = new List<object>();
            return (Dictionary<object, object>)((Dictionary<object, object>)this.parseObjectItems[start_index])[state];
        }
        if(this.identifiers.ContainsKey(start_index) && state == "Identifier") {
            //////////////System.Diagnostics.Debug.WriteLine("identifier - {0} - {1} - {2} - {3}", state, start_index, JsonConvert.SerializeObject(this.identifiers[start_index]));
            int stop_index = start_index + (int)((Dictionary<object, object>)this.identifiers[start_index])["length"];
            List<object> subParseObjects = new List<object>();
            Dictionary<object, object> parse_object = new Dictionary<object, object>() {
                { "start_index", start_index },
                { "stop_index", stop_index },
                { "label", "Identifier" },
                { "sub_parse_objects", subParseObjects },
            };
            this.subStateRegexIndex = new List<object>();
            return parse_object;
        }
        ////////////////System.Diagnostics.Debug.WriteLine("in whitespace whitespaces : "+start_index+" - "+state);
        if(this.whiteSpaces.ContainsKey(start_index) && state == "WhiteSpace") {
            ////////////////System.Diagnostics.Debug.WriteLine("in whitespace whitespaces");
            int stop_index = start_index + (int)((Dictionary<object, object>)this.whiteSpaces[start_index])["length"];
            List<object> subParseObjects = new List<object>();
            Dictionary<object, object> parse_object = new Dictionary<object, object>() {
                { "start_index", start_index },
                { "stop_index", stop_index },
                { "label", "WhiteSpace" },
                { "sub_parse_objects", subParseObjects },
            };
            this.subStateRegexIndex = new List<object>();
            return parse_object;
        }
        if(this.strings.ContainsKey(start_index) && state == "String") {
            
            int stop_index = start_index + (int)((Dictionary<object, object>)this.strings[start_index])["length"];
            List<object> subParseObjects = new List<object>();
            if(stop_index == start_index+1) {

            } else {
                int string_content_index = start_index+1;
                int string_content_stop = stop_index-1;
                subParseObjects = new List<object>() {
                    new Dictionary<object, object>() {
                        { "start_index", string_content_index },
                        { "stop_index", string_content_stop },
                        { "label", "StringContent" },
                        { "sub_parse_objects", new List<object>() },
                    }
                };
            }
            Dictionary<object, object> parse_object = new Dictionary<object, object>() {
                { "start_index", start_index },
                { "stop_index", stop_index },
                { "label", "String" },
                { "sub_parse_objects", subParseObjects },
            };
            this.subStateRegexIndex = new List<object>();
            return parse_object;
        }
        int start_index_string = start_index;
        int set_sub_state_index_string = set_sub_state_index;

        if(this.subStateIndex.ContainsKey(start_index_string) && ((Dictionary<object, object>)this.subStateIndex[start_index_string]).ContainsKey(state)  && ((Dictionary<object, object>)((Dictionary<object, object>)this.subStateIndex[start_index_string])[state]).ContainsKey(set_sub_state_index_string)) {
            //////////////System.Diagnostics.Debug.WriteLine("sub state index exists : "+state+" - "+JsonConvert.SerializeObject(this.subStateIndex));
            //////////////System.Diagnostics.Debug.WriteLine("sub state value : "+((Dictionary<object, object>)((Dictionary<object, object>)this.subStateIndex[start_index_string])[state])[set_sub_state_index_string]+" - "+start_index_string+" "+state+" "+set_sub_state_index_string);
            return null;
        }
        //////////////System.Diagnostics.Debug.WriteLine("substate : "+state+" - "+JsonConvert.SerializeObject(this.subStateIndex));
        string firstChar = this.sourceText.Substring(start_index, 1);
        if(this.skipChars.IndexOf(firstChar) != -1) {
            //////////////System.Diagnostics.Debug.WriteLine("skip chars set: "+firstChar+ " - "+start_index_string+" - "+state+" - "+set_sub_state_index_string);
            if(!this.subStateIndex.ContainsKey(start_index_string)) {
                this.subStateIndex[start_index_string] = new Dictionary<object, object>();
            }
            if(!((Dictionary<object, object>)this.subStateIndex[start_index_string]).ContainsKey(state)) {
                ((Dictionary<object, object>)this.subStateIndex[start_index_string])[state] = new Dictionary<object, object>();

            }
            ((Dictionary<object, object>)((Dictionary<object, object>)this.subStateIndex[start_index_string])[state])[set_sub_state_index_string] = true;
            
            return null;
        }

        int startFrom = start_index;
        string subStringFromStart = null;

        if(/*false &&*/ this.regexDefinitions.ContainsKey(state)) { //[state] != null
            Regex stringCapture = this.regexDefinitions[state];
            //////////System.Diagnostics.Debug.WriteLine("start index : "+startFrom);
            subStringFromStart = this.sourceText.Substring(startFrom);
            //////////System.Diagnostics.Debug.WriteLine("subStringFromStart  : "+subStringFromStart);
            int rangeOfSemicolon = subStringFromStart.IndexOf(";");
            ////////System.Diagnostics.Debug.WriteLine("range of semicolon : "+rangeOfSemicolon);
            subStringFromStart = subStringFromStart.Substring(0, rangeOfSemicolon);
            if(true || subStringFromStart != null) {
                MatchCollection matches = stringCapture.Matches(subStringFromStart);
                bool setIndexValue = true;
                /*if(matches.Count > 0 && matches[0].Index == 0 && matches[0].Length == subStringFromStart.Length) {
                    Match m = matches[0];
                } else*/
                ////////System.Diagnostics.Debug.WriteLine("substring : "+subStringFromStart);
                foreach(Match m in matches) {
                    //////////System.Diagnostics.Debug.WriteLine("match : "+m.Index+" - "+m.Value+" "+m.Length);
                    if(m.Index == 0 && m.Length > 0) {
                        setIndexValue = false;
                    }
                }
                if(setIndexValue) {
                    //NSLog(@"match : %@ - %@ - %@ - %@ - %@", subStringFromStart, match, state, start_index_string, set_sub_state_index_string);
                    ////////System.Diagnostics.Debug.WriteLine("match : "+subStringFromStart+" - "+matches.Count+" "+state+" "+start_index_string+" "+set_sub_state_index_string);
                    if(!this.subStateIndex.ContainsKey(start_index_string)) {
                        this.subStateIndex[start_index_string] = new Dictionary<object, object>();
                    }
                    if(!((Dictionary<object, object>)this.subStateIndex[start_index_string]).ContainsKey(state)) {
                        ((Dictionary<object, object>)this.subStateIndex[start_index_string])[state] = new Dictionary<object, object>();

                    }
                    ((Dictionary<object, object>)((Dictionary<object, object>)this.subStateIndex[start_index_string])[state])[set_sub_state_index_string] = true;
                    //////////System.Diagnostics.Debug.WriteLine("regex chars set : "+state+" "+start_index_string+" "+set_sub_state_index_string);
                    return null;
                }
            }
        }
        List<object> sub_parse_objects = new List<object>();
        int index = start_index;
        bool sub_state_valid = true;
        List<object> sub_state_value = sub_state;
        foreach(object elementObj in sub_state_value) {
            Dictionary<string, object> element = (Dictionary<string, object>)elementObj;
            //////////////System.Diagnostics.Debug.WriteLine("substatevalue : "+JsonConvert.SerializeObject(element));
            if(element.ContainsKey("regex")) {
                Dictionary<object, object> has_index = this.hasIndex(start_index, state);
                ////////////////System.Diagnostics.Debug.WriteLine("hasIndex:"+JsonConvert.SerializeObject(has_index)+" - "+state);

                if(has_index != null) {
                    if((int)has_index["results"] == -1) {
                        sub_state_valid = false;
                        //////////////System.Diagnostics.Debug.WriteLine("set sub_state_valid false0 "+JsonConvert.SerializeObject(element));
                    } else {
                        index = (int)has_index["result"];
                    }
                } else {
                    List<object> regex_value = (List<object>)element["regex"];
                    List<object> not = new List<object>();
                    if(element.ContainsKey("not")) {
                        if(((Dictionary<string, object>)this.language["states"]).ContainsKey(element["not"].ToString())) {
                            not = (List<object>)((Dictionary<string, object>)this.language["states"])[element["not"].ToString()];
                            int not_key = 0;
                            List<object> not_replace = new List<object>();
                            foreach(object not_value_obj in not) {
                                string not_value = not_value_obj.ToString();
                                List<char> split = not_value.ToCharArray().ToList<char>();
                                List<string> split_value = new List<string>();
                                foreach(char c in split) {
                                    split_value.Add(c.ToString());
                                }
                                not_replace.Add(split_value);
                                not_key++;
                            }
                            not = not_replace;
                        }
                    }
                    int perform_continue = 1;
                    int set_index = index;
                    List<object> delimit_regex = this.delimitRegex;
                    if(element.ContainsKey("delimit_regex")) {
                        delimit_regex = (List<object>)element["delimit_regex"];
                    }
                    List<object> stop_regex = new List<object>();
                    if(element.ContainsKey("stop_regex")) {
                        stop_regex = (List<object>)element["stop_regex"];
                    }

                    if((string)regex_value[0] == " ") {
                        string regex_value_string = (string)regex_value[0];
                        string character = this.text[set_index];

                        if(character == regex_value_string) {
                            set_index++;
                        } else {
                            perform_continue = -2;
                        }
                    } else {
                        int counter = 0;
                        while(perform_continue == 1 && set_index < this.text.Count) {
                            string character = this.text[set_index];
                            int not_key = 0;
                            Dictionary<object, object> not_store = new Dictionary<object, object>();
                            int set_not_index = 0;
                            foreach(object notItem in not) {
                                not_store[set_not_index] = notItem;
                                set_not_index++;
                            }
                            foreach(object not_value_obj in not) {
                                string not_character = not_store[not_key].ToString();
                                ((List<string>)not_store[not_key]).RemoveAt(0);
                                if(not_character != character) {
                                    not_store.Remove(not_key);
                                }
                                not_key++;
                            }
                            not = (List<object>)not_store.Values.ToList<object>();

                            if(not.Count > 0) {
                                foreach(object not_string_array_obj in not) {
                                    List<object> not_string_array = (List<object>)not_string_array_obj;
                                    if(not_string_array.Count == 0) {
                                        perform_continue = -2;
                                    }
                                }
                            }
                            if(counter == 0 && element.ContainsKey("not_starts_with")) {
                                Regex decimalRegex = new Regex("[0-9]");
                                if(decimalRegex.IsMatch(character)) {
                                    perform_continue = -2;
                                }
                            }
                            if(perform_continue == 1) {
                                if(delimit_regex.IndexOf(character) != -1) {
                                    perform_continue = -1;
                                } else if(stop_regex.IndexOf(character) != -1) {
                                    perform_continue = -1;
                                } else {
                                    if((string)regex_value[0] == "all" || regex_value.IndexOf(character.ToLower()) != -1) {
                                        set_index++;
                                    } else {
                                        perform_continue = -1;
                                    }
                                }
                            }
                            counter++;
                        }
                    }
                    if(perform_continue == -2 || set_index == index) {
                        sub_state_valid = false;
                        //////////////System.Diagnostics.Debug.WriteLine("set sub_state_valid false1 "+JsonConvert.SerializeObject(element));
                        this.subStateRegexIndex.Add(new Dictionary<object, object>() {
                            { "start_index", start_index },
                            { "state", state },
                            { "result", -1 },
                        });
                    } else {
                        index = set_index;
                        this.subStateRegexIndex.Add(new Dictionary<object, object>() {
                            { "start_index", start_index },
                            { "state", state },
                            { "result", set_index },
                        });
                    }
                }
            } else if(element.ContainsKey("non_terminal")) {
                ////////////////System.Diagnostics.Debug.WriteLine("in non terminal");
                List<object> not_2 = new List<object>();
                if(element.ContainsKey("not")) {
                    not_2 = (List<object>)element["not"];
                    if(not_2.IndexOf(this.text[index].ToString()) != -1) {
                        //////////////System.Diagnostics.Debug.WriteLine("set sub_state_valid false2 "+JsonConvert.SerializeObject(element));
                        sub_state_valid = false;
                    }
                }
                List<object> not_nt = new List<object>();
                if(element.ContainsKey("not_nt")) {
                    not_nt = (List<object>)element["not_nt"];
                    foreach(object non_terminal_not_obj in not_nt) {
                        Dictionary<object, object> sub_not_parse = this.subParse((string)non_terminal_not_obj, index, margin+"-");
                        if(sub_not_parse != null) {
                            //////////////System.Diagnostics.Debug.WriteLine("set sub_state_valid false3 "+JsonConvert.SerializeObject(element));
                            sub_state_valid = false;
                        }
                    }
                }
                ////////////////System.Diagnostics.Debug.WriteLine("sub state valid : "+sub_state_valid);
                if(sub_state_valid) {
                    
                    ////////System.Diagnostics.Debug.WriteLine("index : "+index+" - "+state);
                    Dictionary<object, object> sub_parse_valid = this.subParse((string)element["non_terminal"], index, margin+"    ");
                    //////////////System.Diagnostics.Debug.WriteLine("sub parse valid : "+JsonConvert.SerializeObject(element)+" - "+index+" - "+JsonConvert.SerializeObject(JsonConvert.SerializeObject(sub_parse_valid)));
                    ////////////////System.Diagnostics.Debug.WriteLine("sub sub_parse_valid valid : "+(string)element["non_terminal"]);
                    ////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(sub_parse_valid));
                    if(sub_parse_valid != null) {
                        sub_parse_objects.Add(sub_parse_valid);
                        index = (int)sub_parse_valid["stop_index"];
                    } else {
                        if(!element.ContainsKey("opt") || (element.ContainsKey("opt") && (bool)element["opt"] != true)) {
                            //////////////System.Diagnostics.Debug.WriteLine("set sub_state_valid false4 "+JsonConvert.SerializeObject(element));
                            sub_state_valid = false;
                        }
                    }
                }
            } else if(element.ContainsKey("terminal")) {
                int set_index = index;
                bool valid = true;

                ////////////System.Diagnostics.Debug.WriteLine("in terminal : "+element["terminal"]+" preparsedValues: "+set_index+" - ");
                if(this.preParsedTerminalIndex.ContainsKey(set_index) && (string)element["terminal"] == (string)((Dictionary<object, object>)this.preParsedTerminalIndex[set_index])["value"]) {
                    ////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(this.preParsedTerminalIndex[set_index]));
                    set_index += (int)((Dictionary<object, object>)this.preParsedTerminalIndex[set_index])["length"];
                    index = set_index;
                } else {
                    valid = false;
                    //////////////System.Diagnostics.Debug.WriteLine("set sub_state_valid false5 "+JsonConvert.SerializeObject(element));
                    sub_state_valid = false;
                }
                ////////////////System.Diagnostics.Debug.WriteLine("terminal is : "+set_index+" sub state: "+sub_state_valid+" valid "+valid);
            }
            //////////System.Diagnostics.Debug.WriteLine("sub_state_valid: "+sub_state_valid+" "+state+" el: "+JsonConvert.SerializeObject(element));
            if(!sub_state_valid) {
                //////////////System.Diagnostics.Debug.WriteLine("perform break");
                break;
            }
        }
        //
        //NSLog(@"continue after break : %@ - %@", state, @(sub_state_valid));
        //////////////System.Diagnostics.Debug.WriteLine("continue after break: "+state+" - "+sub_state_valid);
        if(sub_state_valid) {
            //////System.Diagnostics.Debug.WriteLine("sub_state_valid: "+sub_state_valid+" "+state+" index: "+index);
            this.currentStepStop = index;
            Dictionary<object, object> parse_object = new Dictionary<object, object>() {
                {
                    "start_index",
                     start_index
                },{
                    "stop_index",
                     index
                },{
                    "label",
                     state
                },{
                    "sub_parse_objects",
                     sub_parse_objects
                }
            };
            if(this.storeStates.IndexOf(state) == -1) {
                if(!this.parseObjectItems.ContainsKey(start_index)) {
                    this.parseObjectItems[start_index] = new Dictionary<object, object>();
                }
                ((Dictionary<object, object>)this.parseObjectItems[start_index])[state] = parse_object;
            }
            this.subStateRegexIndex = new List<object>();
            return parse_object;
        }
        this.currentStepStop = start_index;

        if(!this.subStateIndex.ContainsKey(start_index_string)) {
            ((Dictionary<object, object>)this.subStateIndex)[start_index_string] = new Dictionary<object, object>();
        }
        if(!((Dictionary<object, object>)this.subStateIndex[start_index_string]).ContainsKey(state)) {
            
            ((Dictionary<object, object>)this.subStateIndex[start_index_string])[state] = new Dictionary<object, object>();
        }
        ((Dictionary<object, object>)((Dictionary<object, object>)this.subStateIndex[start_index_string])[state])[set_sub_state_index_string] = true;
        return null;

    }

    public Dictionary<object, object> hasIndex(int start_index, string state) {
        foreach(object sub_state_regex_item_obj in this.subStateRegexIndex) {
            Dictionary<object, object> sub_state_regex_item = (Dictionary<object, object>)sub_state_regex_item_obj;

            if((int)sub_state_regex_item["start_index"] == start_index && (string)sub_state_regex_item["state"] == state) {
                return sub_state_regex_item;
            }
        }
        return null;
    }

    public int currentStepStop;

    public string sourceText;

    Dictionary<object, object> resultsDefinition;

    Dictionary<string, object> postProcessingDefinition;

    public Dictionary<object, object> terminalIndex;
    public Dictionary<object, object> strings;
    public Dictionary<object, object> whiteSpaces;
    public Dictionary<object, object> identifiers;
}

class Preparser {

    public List<object> tokens;
    public string sourceText;
    public Dictionary<object, object> rangeResults = new Dictionary<object, object>();
    public Dictionary<object, object> strings  = new Dictionary<object, object>();
    public Dictionary<object, object> whiteSpaces = new Dictionary<object, object>();
    public Dictionary<object, object> identifiers = new Dictionary<object, object>();
    public Dictionary<object, object> numbers  = new Dictionary<object, object>();

    public Dictionary<object, object> preparseText(string sourceText) {
        Dictionary<object, object> results = new Dictionary<object, object>();
        Dictionary<object, object> resultsRange = new Dictionary<object, object>();

        /*List<Regex> terminalStrings = new List<Regex>() {
            new Regex(@"\$"),
            new Regex(@"\("),
            new Regex(@"\)"),
            new Regex(@"public\s"),
            new Regex(@"private\s"),
            new Regex(@"function"),
            new Regex(@"extends\s"),
            new Regex(@"protected\s"),
            new Regex(@"\[\.\.\."),
            new Regex(@"delete\s"),
            new Regex(@"foreach"),
            new Regex(@"while"),
            new Regex(@"for"),
            new Regex(@"\-\>"),
            new Regex(@"%"),
            new Regex(@"\{"),
            new Regex(@"\}"),
            new Regex(@","),
            new Regex(@"NULL"),
            new Regex(@"\[\]\="),
            new Regex(@";"),
            new Regex(@"async\s"),
            new Regex(@"class\s"),
            new Regex(@"\sas\s"),
            new Regex(@"await\s"),
            new Regex(@"return\s"),
            new Regex(@"\["),
            new Regex(@"\]"),
            new Regex(@"\!\=\="),
            new Regex(@"\=\=\="),
            new Regex(@"\!\="),
            new Regex(@"\=\="),
            new Regex(@"new\s"),
            new Regex(@"if"),
            new Regex(@"else\sif"),
            new Regex(@"else"),
            new Regex(@"\|\|"),
            new Regex(@"&&"),
            new Regex(@"\+\="),
            new Regex(@"\-\="),
            new Regex(@"\*\="),
            new Regex(@"\/\\="),
            new Regex(@"\.\="),
            new Regex(@"\."),
            new Regex(@"\=\>"),
            new Regex(@"\<\="),
            new Regex(@"\>\="),
            new Regex(@"\<"),
            new Regex(@"\>"),
            new Regex(@"\="),
            new Regex(@"\!"),
            //@"'",
            //@"+=",
            //@"-=",
            new Regex(@"\="),
            new Regex(@"\+\+"),
            new Regex(@"\-\-"),
            new Regex(@"\+"),
            new Regex(@"\-"),
            new Regex(@"\/"),
            new Regex(@"\*"),
            new Regex(@"false"),
            new Regex(@"true"),
            //@"\\s",
        };*/

        List<string> terminalStrings = new List<string>() {
            @"\$",
        @"\(",
        @"\)",
        @"public\s",
        @"private\s",
        @"function",
        @"extends\s",
        @"protected\s",
        @"\[\.\.\.",
        @"delete\s",
        @"foreach",
        @"while",
        @"for",
        @"\-\>",
        @"%",
        @"\{",
        @"\}",
        @",",
        @"NULL",
        @"\[\]\=",
        @";",
        @"async\s",
        @"class\s",
        @"\sas\s",
        @"await\s",
        @"return\s",
        @"\[",
        @"\]",
        @"\!\=\=",
        @"\=\=\=",
        @"\!\=",
        @"\=\=",
        @"new\s",
        @"if",
        @"else\sif",
        @"else",
        @"\|\|",
        @"&&",
        @"\+\=",
        @"\-\=",
        @"\*\=",
        @"\/\=",
        @"\.\=",
        @"\.",
        @"\=\>",
        @"\<\=",
        @"\>\=",
        @"\<",
        @"\>",
        @"\=",
        @"\!",
        //@"'",
        //@"+=",
        //@"-=",
        //@"\=",
        @"\+\+",
        @"\-\-",
        @"\+",
        @"\-",
        @"\/",
        @"\*",
        @"false",
        @"true",
        };

        Regex stringCapture = new Regex(@"(['])((?:\\\1|(?:(?!\1)).)*)(\1)");
        MatchCollection stringsArray = stringCapture.Matches(sourceText);
        foreach(Match m in stringsArray) {
            this.strings[m.Index] = new Dictionary<object, object>() {
                {"length", m.Length },
                {"value", m.Value }
            };
        }

        this.findIdentifiers(sourceText);

        //Regex regexSuffix = new Regex(@"(?=([^']*'[^']*')*[^']*$)");
        string regexSuffix = @"(?=([^']*'[^']*')*[^']*$)";

        foreach(string terminalString in terminalStrings) {
            string regex_string = terminalString+regexSuffix;
            Regex regexValue = new Regex(regex_string);
            MatchCollection stringsArrayItem = regexValue.Matches(sourceText);
            foreach(Match m in stringsArrayItem) {
                int location = m.Index;
                int length = m.Length;
                string value = m.Value;
                string startChar = value.Substring(0, 1);
                string endChar = value.Substring(value.Length-1);
                int perform_split = -1;
                if(startChar == " ") {
                    location++;
                    length--;
                    perform_split = 1;
                }
                if(endChar == " ") {
                    length--;
                    perform_split++;
                }
                if(perform_split >= 0) {
                    List<string> split = value.Split(" ").ToList<string>();
                    if(perform_split == 2) {
                        value = split[1];
                    } else {
                        value = split[0];
                    }
                }

                if(!resultsRange.ContainsKey(location) && !this.identifiers.ContainsKey(location)) {
                    Dictionary<object, object> dictResult = new Dictionary<object, object>() {
                        { "length", value.Length },
                        { "value", value }
                    };
                    resultsRange[location] = dictResult;
                }
            }
        }
        this.rangeResults = resultsRange;
        this.findWhiteSpaces(sourceText);

        return results;
    }

    public void findIdentifiers(string input) {
        Regex stringcapture = new Regex(@"(\$|\-\>|function\s)[_a-z0-9]+", RegexOptions.IgnoreCase);
        MatchCollection collection = stringcapture.Matches(input);
        foreach(Match m in collection) {
            string value = m.Value;
            int location = m.Index;
            int length = m.Length;
            if(value.Contains("$")) {
                location++;
                length--;
                value = value.Substring(1, value.Length-1);
            } else if(value.Contains("->")) {
                location += 2;
                length -= 2;
                value = value.Substring(2, value.Length-2);
            } else if(value.Contains("function ")) {
                location += 9;
                length -= 9;
                value = value.Substring(9, value.Length-9);
            }
            if(!this.identifiers.ContainsKey(location)) {
                this.identifiers[location] = new Dictionary<object, object>() {
                    { "length", length },
                    { "value", value }
                };
            }
        }
    }

    public void findWhiteSpaces(string input) {
        Regex stringCapture = new Regex(@"\s");
        MatchCollection matches = stringCapture.Matches(input);
        foreach(Match m in matches) {
            if(!this.whiteSpaces.ContainsKey(m.Index)) {
                this.whiteSpaces[m.Index] = new Dictionary<object, object>() {
                    {"length", m.Length },
                    {"value", m.Value },
                };
            }
        }
    }

    /*public void findNumbers() {

    }*/
}