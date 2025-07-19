using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.Design;

class toJSON {

	private bool treatNumbersAsStrings = false;

	public toJSON(bool treatNumbersAsStrings=false) {
		this.treatNumbersAsStrings = treatNumbersAsStrings;
	}

    private List<object> recursionDetection;//Dictionary<object, object> recursionDetection;

    public object convert(object input) {
    	//System.Diagnostics.Debug.WriteLine("perform convert: "+input);
        this.recursionDetection = new List<object>();//new Dictionary<object, object>();
        object res = this.toJSONSub(input);
        this.recursionDetection = null;
        return res;
    }

    public string toJSONString(object input) {
    	JsonSerializerSettings set = new JsonSerializerSettings();
        set.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //set.MaxDepth = 512;
        return JsonConvert.SerializeObject(this.convert(input), set);
    }

    public object toJSONSub(object input) {
    	//System.Diagnostics.Debug.WriteLine("input: "+input);
        if(this.treatNumbersAsStrings) {
        	if(input == null || input == "" || (input is bool && (bool)input == false) || (input is int && (int)input == -1)) {
   		 		//System.Diagnostics.Debug.WriteLine("input is null: "+input);
        		return "0"; //input.ToString();
        	} else if(input is bool && (bool)input == true) {
        		return "1";
        	}
        }
        if(input is PHPScriptFunction) {
            return "undefined";
        }
        if(input is PHPScriptEvaluationReference) {
        	return "undefined";
        }
        if(input is PHPVariableReference) {
            return this.toJSONSub(((PHPVariableReference)input).get(null));
        }
        if(input is PHPUndefined) {
            return "undefined";
        }
        if(input is PHPScriptObject) {
            if(this.recursionDetection.Contains(input)) {
                return false;
            }
            this.recursionDetection.Add(input);
            PHPScriptObject scriptObj = (PHPScriptObject)input;
            if(scriptObj.isArray) {
                List<object> returnArr = new List<object>();
                List<object> dictValues = (List<object>)scriptObj.getDictionary();
                foreach(object value in dictValues) {
                	//System.Diagnostics.Debug.WriteLine("arr value: "+value);
                    returnArr.Add(this.toJSONSub(value));
                }
                return returnArr;
            } else {
            	if(this.treatNumbersAsStrings) {
            		List<object> returnDict = new List<object>();
            		List<object> keys = new List<object>(scriptObj.getKeys());
            		List<object> values = new List<object>();
	                Dictionary<object, object> dictValues = (Dictionary<object, object>)scriptObj.getDictionary();
	                foreach(object key in keys) {
	                	values.Add(dictValues[key]);
	                }
	                returnDict.Add(keys);
	                returnDict.Add(values);
	                return returnDict;
            	} else {
	                Dictionary<object, object> returnDict = new Dictionary<object, object>();
	                Dictionary<object, object> dictValues = (Dictionary<object, object>)scriptObj.getDictionary();
	                foreach(KeyValuePair<object, object> pair in dictValues) {
	                	//System.Diagnostics.Debug.WriteLine("dict value: "+pair.Value);
	                    returnDict[pair.Key] = this.toJSONSub(pair.Value);
	                }
	                return returnDict;
	            }
            } 
        } else if(input is List<object>) {
        	List<object> resList = new List<object>();
        	foreach(object resItem in ((List<object>)input)) {
        		resList.Add(this.toJSONSub(resItem));
        	}
        	return resList;
            //return new List<object>((IEnumerable<object>)input);
        } else if(input is Dictionary<object, object>) {
        	Dictionary<object, object> resList = new Dictionary<object, object>();
        	foreach(KeyValuePair<object, object> resItem in ((Dictionary<object, object>)input)) {
        		resList[resItem.Key] = this.toJSONSub(resItem.Value);
        	}
        	return resList;
            //return new Dictionary<object, object>((Dictionary<object, object>)input);
        } else if(input is int || input is string || input is double || input is float) {
        	if(this.treatNumbersAsStrings) {
        		return input.ToString();
        	}
        	return input;
        }
        //return "undefined";
        return input;
    }
}