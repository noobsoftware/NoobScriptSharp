using Newtonsoft.Json;
//using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
//using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
/*using Windows.ApplicationModel.Activation;
using Windows.Devices.Bluetooth.Background;
using Windows.Foundation.Diagnostics;
using Windows.Networking.Vpn;
using Windows.Storage.Pickers;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Input.Preview.Injection;
using System.Threading;
using Windows.UI.Xaml;*/
using System.Threading;

class NSObject : object {
    public bool isEqualTo(NSObject compare) {
        return false;
    }
}

class PHPControl : NSObject {
    public PHPScriptObject context;
    public void construct(PHPScriptObject context) {
        this.context = context;
    }

    public object run(PHPScriptObject context, object lastValidCondition) {
        return false;
    }

}

class PHPReturnResult : NSObject {
    public object result;

    public void construct(object value) {
        if(value is string) {
            //if(((NSString)value).isEqualToString(new NSString("[]"))) {
            if(value == "[]") {
                value = new PHPScriptObject();
                ((PHPScriptObject)value).isArray = true;
            }
        }
        if(value is PHPReturnResult) {
            PHPReturnResult returnResultValue = new PHPReturnResult();
            this.result = returnResultValue.result;
        } else {
            this.result = value;
        }
    }

    public object get() {
        return this.result;
    }
}

class PHPScriptEvaluationReference : NSObject {
    public PHPScriptObject contextValue;
    public PHPInterpretation interpretation;
    public Dictionary<object, object> subObjectDict;
    public PHPScriptFunction lastSetValidContext;
    public PHPScriptFunction lastCurrentFunctionContext;
    public object preventSetValidContext;
    public object preserveContext;
    public object inParentContextSetting;
    public PHPScriptFunction lastFunctionContextValue;
    public bool isAsync;

    public object callFun(PHPScriptObject context) {
    	//PHPScriptObject context;

        //////////////////////////System.Diagnostics.Debug.WriteLine("in call fun : "+this.contextValue+" - "+contextInput);
        if(context == null) {
            context = this.contextValue;
        } /*else {
        	context = contextInput;
        }*/
        ////////////////////////System.Diagnostics.Debug.WriteLine("in call fun");
        lock(context) {
            Dictionary<object, object> subObjectDict = this.subObjectDict;

            ////////////////System.Diagnostics.Debug.WriteLine("callfun-value: "+context);
            ////////////////////////System.Diagnostics.Debug.WriteLine("callfun-value: "+this.lastSetValidContext);
            //////////////////////////System.Diagnostics.Debug.WriteLine("callfun-value: "+this.preventSetValidContext);
            ////////////////////////System.Diagnostics.Debug.WriteLine("callfun-value: "+this.preserveContext);
            ////////////////////////System.Diagnostics.Debug.WriteLine("callfun-value: "+this.lastCurrentFunctionContext);

            object variableValue = this.interpretation.execute(subObjectDict, context, null, null, null, null, this.lastSetValidContext, (bool)this.preventSetValidContext, (PHPScriptFunction)this.preserveContext, (bool)this.inParentContextSetting, this.lastCurrentFunctionContext, this.isAsync);

       		////////////////////////System.Diagnostics.Debug.WriteLine("in-after call fun: "+variableValue);
            if(context is PHPScriptFunction && ((PHPScriptFunction)context).isReturnValueItem) {
                ((PHPScriptFunction)context).returnResultValue = variableValue;
            }
            return variableValue;
        }
        return null;
    }


}

class PHPValuesOperator  {
    public string operatorValue;
}

class PHPLoopControl : PHPControl {
    public PHPScriptEvaluationReference condition;
    public PHPScriptEvaluationReference subRoutine;
    
    public void setCondition(PHPScriptEvaluationReference condition, PHPScriptEvaluationReference subRoutine) {
        this.condition = condition;
        this.subRoutine = subRoutine;
    }

    public new object run(PHPScriptObject context, object lastValidCondition) {
        ////////////////////////System.Diagnostics.Debug.WriteLine("loop result: "+callFunctionResult);
        //////////////////////System.Diagnostics.Debug.WriteLine("PHPLoopControl run");
        //return false;
        object callFunctionResult = this.condition.callFun(null);
        //////////////////////System.Diagnostics.Debug.WriteLine("loop result: "+callFunctionResult);
        callFunctionResult = context.parseInputVariable(callFunctionResult);
        //////////////////////System.Diagnostics.Debug.WriteLine("loop result: "+callFunctionResult);
        //NSNumber boolresult = (NSNumber)callFunctionResult;
        //bool bool_value = boolresult.boolValue();
        bool bool_value = Convert.ToBoolean(callFunctionResult);
        //////////////////////System.Diagnostics.Debug.WriteLine("loop result: "+bool_value);
        while(bool_value) {
            object possible_return_result = this.subRoutine.callFun(null);
            //////////////////////////////System.Diagnostics.Debug.WriteLine("possible return result: "+possible_return_result);
            if(possible_return_result is PHPReturnResult) {
                return possible_return_result;
            }
            callFunctionResult = this.condition.callFun(null);
            callFunctionResult = context.parseInputVariable(callFunctionResult);
            /*boolresult = (NSNumber)callFunctionResult;
            bool_value = boolresult.boolValue();*/
            //////////////////////System.Diagnostics.Debug.WriteLine("loop result: "+callFunctionResult);
            bool_value = Convert.ToBoolean(callFunctionResult);
            //bool_value = (bool)callFunctionResult;
            //////////////////////////////System.Diagnostics.Debug.WriteLine("possible return callFunctionResult: "+bool_value);
        }

        /*NSNumber falseResult = new NSNumber();
        falseResult.setBoolValue(false);
        return falseResult;*/
        //////////////////////System.Diagnostics.Debug.WriteLine("loop return result: false");
        return false;
    }
}

class PHPForLoopControl : PHPControl {
    public string type;
    public bool forEach2;
    public PHPScriptEvaluationReference iterator;
    public PHPScriptEvaluationReference condition;
    public PHPScriptEvaluationReference variableDefinition;
    public PHPScriptEvaluationReference subRoutine;
    public object fromVariable;
    public PHPVariableReference keyVariable;
    public PHPVariableReference iterationVariable;
    public object referenceValue;
    public object referenceKeys;

    public void setSwitches(Dictionary<object, object> switches) {
        if(switches.Keys.Count > 0) {
            this.type = (string)(switches.Keys.ToList<object>()[0]);
        }
    }

    public void setForeach(bool flag) {
        this.type = "foreach";//new NSString("foreach");
    }

    public void setCondition(object a, object b, object c=null) {
        if(c == null) {

        }
        //////////////////////////////System.Diagnostics.Debug.WriteLine("setCondition a: "+a+" b:"+b+" c:"+c);
        if(this.type == "for") {
            //////////////////////////////System.Diagnostics.Debug.WriteLine("setCondition1"); 
            this.variableDefinition = (PHPScriptEvaluationReference)a;
            this.condition = (PHPScriptEvaluationReference)b;
            this.iterator = (PHPScriptEvaluationReference)c;
        } else {
            if(c != null) {
                //////////////////////////////System.Diagnostics.Debug.WriteLine("setCondition2");
                this.fromVariable = a;
                this.keyVariable = (PHPVariableReference)b;
                this.iterationVariable = (PHPVariableReference)c;
            } else {
                //////////////////////////////System.Diagnostics.Debug.WriteLine("setCondition2");
                this.forEach2 = true;
                this.fromVariable = a;
                this.iterationVariable = (PHPVariableReference)b;
            }
        }
    }

    public new object run(PHPScriptObject context, object lastValidCondition) {
        if(this.type == "foreach") {
            object values = null;
            List<object> keys = null;

            bool fromCache = false;

            if(this.referenceValue == null) {
                values = context.parseInputVariable(this.fromVariable);
            } else {
                values = (object)this.referenceValue;
                keys = (List<object>)this.referenceKeys;
                fromCache = true;
            }

            PHPScriptObject reference;

            if(!fromCache) {
                if(values is PHPScriptObject) {
                    reference = (PHPScriptObject)values;
                    if(!reference.isArray) {
                        keys = reference.getKeys();
                    }
                    values = reference.getDictionary();
                    
                } else {
                    reference = (PHPScriptObject)((PHPVariableReference)this.fromVariable).get(null);
                    values = reference.getDictionary();

                    if(!reference.isArray) {
                        keys = reference.getKeys();
                    }
                }
            }
            if(values is List<object>) {
                
                if(((List<object>)values).Count == 0) {
                    return null;
                }
            } else {
                if(((Dictionary<object, object>)values).Count == 0) {
                    return null;
                }
            }

            if(!fromCache) {
                if(keys != null) {
                    this.referenceKeys = keys;
                }
                if(values != null) {
                    this.referenceValue = true;
                }
            }

            if(this.forEach2) {
                if(values is List<object>) {
                    List<object> dictionaryValues = new List<object>((List<object>)values);
                    foreach(object value in dictionaryValues) {
                        this.iterationVariable.set(value, null);
                        object result = this.subRoutine.callFun(null);
                        //////////////////////////////System.Diagnostics.Debug.WriteLine("foreach result callfun: "+result);
                        if(result is PHPReturnResult) {
                            return result;
                        }
                    }
                } else {
                    Dictionary<object, object> dictionaryValues = new Dictionary<object, object>((Dictionary<object, object>)values);
                    foreach(object key in keys) {
                        object value = dictionaryValues[key];
                        this.iterationVariable.set(value, null);
                        object result = this.subRoutine.callFun(null);
                        if(result is PHPReturnResult) {
                            return result;
                        }
                    }
                }
            } else {
                if(values is List<object>) {
                    List<object> dictionaryValues = new List<object>((List<object>)values);
                    int key_iterator = 0;
                    foreach(object value in dictionaryValues) {
                        this.keyVariable.set(key_iterator, null);
                        this.iterationVariable.set(value, null);

                        object result = this.subRoutine.callFun(null);
                        if(result is PHPReturnResult) {
                            return result;
                        }

                        key_iterator++;
                    }
                } else {
                    Dictionary<object, object> dictionaryValues = new Dictionary<object, object>((Dictionary<object, object>)values);
                    foreach(object key in keys) {
                        object value = dictionaryValues[key];

                        this.keyVariable.set(key, null);
                        this.iterationVariable.set(value, null);

                        object result = this.subRoutine.callFun(null);
                        if(result is PHPReturnResult) {
                            return result;
                        }
                    }
                }
            }
        } else {
            /*this.variableDefinition.callFun(null);
            bool conditionResult = this.condition.callFun(null);

            //bool boolValue = ((NSNumber)conditionResult).boolValue();
            while(boolValue == true) {
                object result = this.subRoutine.callFun(null);
                this.iterator.callFun(null);
                if(result is PHPReturnResult) {
                    return result;
                }
            }*/
        }
        return false;
    }
}

class PHPMultiConditionalControl : PHPControl {
    public List<object> subRoutines;

    public void constructMultiConditionalControl() {
        this.subRoutines = new List<object>();
    }

    public void setCondition(object condition, PHPScriptEvaluationReference subRoutine) {
        if(subRoutine == null) {

        }
        this.subRoutines.Add(new Dictionary<object, object>() {
            { "sub_routine", subRoutine },
            { "condition", condition }
        });
    }

    public void setElseCondition(PHPScriptEvaluationReference subRoutine) {
        this.subRoutines.Add(new Dictionary<object, object>() {
            { "sub_routine", subRoutine },
            //{ "condition", null }
        });
    }

    public new object run(PHPScriptObject context, object lastValidCondition) {
        int key = 0;
        ////////////////////System.Diagnostics.Debug.WriteLine("run multiconditional");
        List<object> reverseObjectEnumerator = new List<object>(this.subRoutines);
        reverseObjectEnumerator.Reverse();
        bool performBreak = false;
        foreach(Dictionary<object, object> value in reverseObjectEnumerator) {
            object valueCondition = null;
            if(value.ContainsKey("condition")) {
	            valueCondition = value["condition"];
	            ////////////////////System.Diagnostics.Debug.WriteLine("run valueCondition: "+valueCondition+" key: "+key);
	            if(valueCondition is List<object>) {
	                valueCondition = ((List<object>)valueCondition)[0];
	            }
	            valueCondition = context.resolveValueReferenceVariableArray(valueCondition);
	            if(valueCondition.ToString() == "true") {
	            	valueCondition = true;
	            } else if(valueCondition.ToString() == "false") {
	            	valueCondition = false;
	            }
	            if(valueCondition != null) {
		            valueCondition = Convert.ToBoolean(valueCondition);
		        }
	           /* //////////////System.Diagnostics.Debug.WriteLine("run valueCondition: "+valueCondition+" "+key);

	            if((bool)valueCondition) {
	                //////////////System.Diagnostics.Debug.WriteLine("is true--");
	            }
	            if(valueCondition == null) {
	                //////////////System.Diagnostics.Debug.WriteLine("is null--");
	            }*/
	        }

            if(valueCondition == null && key == this.subRoutines.Count - 1) {
                //////////////////////System.Diagnostics.Debug.WriteLine("in multi 1");
                object result = ((PHPScriptEvaluationReference)value["sub_routine"]).callFun(null);
                //////////////////////////////System.Diagnostics.Debug.WriteLine("conditional result1: "+result);
                if(result is PHPReturnResult) {
                    return (object)result;
                }
                performBreak = true;
            } else if(valueCondition != null && ((bool)valueCondition)) {
                //////////////////////System.Diagnostics.Debug.WriteLine("in multi 2");
                object result = ((PHPScriptEvaluationReference)value["sub_routine"]).callFun(null);
                //////////////////////////////System.Diagnostics.Debug.WriteLine("conditional result2: "+result);
                if(result is PHPReturnResult) {
                    return (object)result;
                }
                performBreak = true;
            }
            if(performBreak) {
                return false;
            }
            key++;
        }
        return false;
        //NSNumber falseResult = new NSNumber(false);
        //return falseResult;
    }
}

class PHPVariableReference : NSObject {
    public object identifier;
    public PHPScriptObject context;
    public bool isProperty;
    public bool defineInContext;
    public bool ignoreSetContext;
    public PHPScriptFunction currentContext;
    public object itemValue;
    public object itemContainer;

    public void construct(object identifier, PHPScriptObject context, bool isProperty=false, bool defineInContext=false, bool ignoreSetContext=false) {
        this.identifier = identifier;
        /*if(this.identifier is NSString) {
            this.identifier = ((NSString)this.identifier).string_value;
        }*/
        this.context = context;
        this.isProperty = isProperty;
        this.defineInContext = defineInContext;
        this.ignoreSetContext = ignoreSetContext;
        //////////////////////////System.Diagnostics.Debug.WriteLine("constructed PHPVariableReference");
    }

    public void resetContext(PHPScriptFunction context) {
        this.context = context;
    }

    public object get(PHPScriptObject context) {
        object value = this.getSub(context);
        return value;
    }

    public object getSub(PHPScriptObject context) {
        /*if(this.itemContainer != null) {
            if(this.itemContainer is Dictionary<object, object>) {
                return ((Dictionary<object, object>)this.itemContainer)[this.identifier];
            } else {
                return ((List<object>)this.itemContainer)[(int)this.identifier];
            }
        }*/
        //////////////////System.Diagnostics.Debug.WriteLine("get: "+context+" prop: "+this.isProperty+" cont: "+this.context+" id: "+this.identifier);
        //////////////////System.Diagnostics.Debug.WriteLine("parentContext is : "+this.context.parentContext);
        if(this.isProperty) {
            if(this.context.isArray) {
                //this.itemContainer = this.context.getDictionary();
                object intermediateResult = this.context.getVariableValue(this.identifier);
                //////////////////System.Diagnostics.Debug.WriteLine("get value0 : "+intermediateResult);
                return intermediateResult;
            } else {
                //this.itemContainer = this.context.getDictionary();
                object intermediateResult = this.context.getDictionaryValue(this.identifier, false, true, context);
                //////////////////System.Diagnostics.Debug.WriteLine("get value1 : "+intermediateResult);
                return intermediateResult;
            }
        } else {
            if(this.context is PHPScriptObject && !(this.context is PHPScriptFunction)) {
                if(this.currentContext != null) {
                    object intermediateResult2 =  this.currentContext.getVariableValue(this.identifier);
            	    //////////////////System.Diagnostics.Debug.WriteLine("get value2 : "+intermediateResult2);
                	return intermediateResult2;
                }

                if(this.context.parentContext != null) {
                    //this.itemContainer = this.context.parentContext;
                    object intermediateResult2 = this.context.parentContext.getVariableValue(this.identifier);
            	    //////////////////System.Diagnostics.Debug.WriteLine("get value3 : "+intermediateResult2);
                    return intermediateResult2;
                }
            }

            //this.itemContainer = ((PHPScriptFunction)this.context).getVariableContainer(this.identifier);
            //////////////////////////////System.Diagnostics.Debug.WriteLine("get last case: "+this.identifier);
            //return ((Dictionary<object, object>)this.itemContainer)[this.identifier];
            object intermediateResult = ((PHPScriptFunction)this.context).getVariableValue(this.identifier);
           	//////////////////System.Diagnostics.Debug.WriteLine("get value4 : "+intermediateResult);
            return intermediateResult;
        }
    }

    public void set(object value, PHPScriptFunction context) {
        /*if(this.itemContainer != null) {
            if(this.itemContainer is Dictionary<object, object>) {
                ((Dictionary<object, object>)this.itemContainer)[this.identifier] = value;
            } else {
                List<object> arrContainer = (List<object>)this.itemContainer;
                
                ((List<object>)this.itemContainer)[(int)this.identifier] = value;
            }
            return;
        }*/
        //////System.Diagnostics.Debug.WriteLine("set: "+value+" identifier: "+this.identifier);
        if(this.isProperty) {
        	//////System.Diagnostics.Debug.WriteLine("set as prop");
            this.context.setDictionaryValue(this.identifier, value, context);
        } else {
            if(this.context is PHPScriptObject && !(this.context is PHPScriptFunction) && this.context.parentContext != null) {
                this.context.parentContext.setVariableValue(this.identifier, value);
            } else {
                ((PHPScriptFunction)this.context).setVariableValue(this.identifier, value, this.defineInContext);
                
            }
        }
    }
}

class PHPInterpretation {
    public object parsedTree;
    public object definition;
    public string source;

    public object getText(int startIndex, int stopIndex, bool isString, Dictionary<object, object> parseObject) {
        return this.parseInstance.getText(startIndex, stopIndex, isString, parseObject);

        if(parseObject.ContainsKey("text_value")) {
            return parseObject["text_value"];
        }
        ////////////////////////System.Diagnostics.Debug.WriteLine("get text: "+startIndex+" - "+stopIndex);
        string value = this.source.Substring(startIndex, stopIndex-startIndex);
        ////////////////////////System.Diagnostics.Debug.WriteLine("get text: "+value+" - "+this.sourceText);

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
        ////////////////////////System.Diagnostics.Debug.WriteLine("value is : "+value+" set value: "+set_value+" is string: "+isString);
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

    public JSParse parseInstance;

    public void construct(List<object> parsedTree, Dictionary<object, object> definition, string source, JSParse parseInstance) {
        this.parsedTree = parsedTree;
        this.definition = definition;
        this.source = source;
        this.parseInstance = parseInstance;

        List<object> setParsedTree = new List<object>();
        foreach(Dictionary<object, object> node in parsedTree) {
            setParsedTree.Add(this.removeWhiteSpace(node));
        }
        this.parsedTree = setParsedTree;
    }

    public object makeIntoObjects(object input) {
    	//////////////System.Diagnostics.Debug.WriteLine("make into objects: "+input);
        if(input == null || input.ToString() == "null") {
        	return 0;
        }
        if(input is string || input is bool || input is int || input is double) {
            return input;
        }
        if(input is List<object>) {
            PHPScriptObject result = new PHPScriptObject();
            result.initArrays();
            result.isArray = true;
            result.interpretation = this;
            int index = 0;
            foreach(object arrayItem in (List<object>)input) {
                object insertItem = this.makeIntoObjects(arrayItem);
                result.scriptArrayPush(insertItem);
                index++;
            }

            //result.parentContext = this.globalContext;
            return result;
        } else if(input is List<Dictionary<string, object>>) {
            PHPScriptObject result = new PHPScriptObject();
            result.initArrays();
            result.isArray = true;
            result.interpretation = this;
            int index = 0;
            foreach(Dictionary<string, object> arrayItem in (List<Dictionary<string, object>>)input) {
                object insertItem = this.makeIntoObjects(arrayItem);
                result.scriptArrayPush(insertItem);
                index++;
            }

            //result.parentContext = this.globalContext;
            return result;
        } else if(input is Dictionary<object, object>) {
            
            PHPScriptObject result = new PHPScriptObject();
            result.initArrays();
            //result.isArray = true;
            result.interpretation = this;
            //int index = 0;
            Dictionary<object, object> dictInput = (Dictionary<object, object>)input;
            foreach(KeyValuePair<object, object> pair in dictInput) {
                object insertItem = this.makeIntoObjects(pair.Value);
                result.setDictionaryValue(pair.Key, insertItem, null);
                //result.scriptArrayPush(insertItem);
                //index++;
            }
            
            return result;
        } else if(input is Dictionary<string, object>) {
            
            PHPScriptObject result = new PHPScriptObject();
            result.initArrays();
            //result.isArray = true;
            result.interpretation = this;
            //int index = 0;
            Dictionary<string, object> dictInput = (Dictionary<string, object>)input;
            foreach(KeyValuePair<string, object> pair in dictInput) {
                object insertItem = this.makeIntoObjects(pair.Value);
                result.setDictionaryValue(pair.Key, insertItem, null);
                //result.scriptArrayPush(insertItem);
                //index++;
            }
            
            return result;
        } else if(input is Dictionary<string, string>) {
            
            PHPScriptObject result = new PHPScriptObject();
            result.initArrays();
            //result.isArray = true;
            result.interpretation = this;
            //int index = 0;
            Dictionary<string, string> dictInput = (Dictionary<string, string>)input;
            foreach(KeyValuePair<string, string> pair in dictInput) {
                object insertItem = this.makeIntoObjects(pair.Value);
                result.setDictionaryValue(pair.Key, insertItem, null);
                //result.scriptArrayPush(insertItem);
                //index++;
            }
            
            return result;
        }
        return input;
    }

    public PHPScriptFunction currentContext;

    public string receiveData(object input, Func<string, bool> callbackMain=null) {
        //object PHPData = this.makeIntoObjects(input);
        PHPScriptObject baseInstance = (PHPScriptObject)this.currentContext.getVariableValue("base_instance");

        //////////////System.Diagnostics.Debug.WriteLine("base instance is : "+baseInstance);

        PHPScriptFunction receiveData = (PHPScriptFunction)baseInstance.getDictionaryValue("receive_request");

        //receiveData.interpretation = this;

        //////////////System.Diagnostics.Debug.WriteLine("receive_request instance is : "+receiveData);

        //return null;

        /*PHPScriptFunction objectKeys = new PHPScriptFunction();
        objectKeys.initArrays();
        //this.setDictionaryValue("keys", objectKeys);
        //objectKeys.prototype = this;
        objectKeys.setClosure((values, self_instance) => {
            //object value = this.parseInputVariable(((List<object>)values[0])[0]);
            //////////////System.Diagnostics.Debug.WriteLine("in callback1 : "+((List<object>)values[0])[0]);
            object result = ((List<object>)values[0])[0];


            //////////////System.Diagnostics.Debug.WriteLine("in callback1 : "+result);

	        callbackMain(result.ToString());
	        return null;
        }, "main");*/

        return ((PHPReturnResult)(receiveData.callCallback(new List<object>{ new Dictionary<object, object>() {
	        	{
	        		"message", input
	        	}, 
	        	/*{
	        		"callback",
	        		objectKeys
	        	}*/
			}
        }))).result.ToString();
        ////////////////System.Diagnostics.Debug.WriteLine("receive request res: "+result);

        /*result = baseInstance.parseInputVariable(result);


   		toJSON toJSONInstance = new toJSON();
   		string result_string_value = toJSONInstance.toJSONString(result);

        return result_string_value.ToString();*/
        //return null;
    }

    public Dictionary<object, object> removeWhiteSpace(Dictionary<object, object> node) {
        List<object> subParseObjects = new List<object>();
        foreach(Dictionary<object, object> value in (List<object>)node["sub_parse_objects"]) {
            string label = value["label"].ToString();
            if(label == "WhiteSpace" || label == "comment") {

            } else {
                subParseObjects.Add(this.removeWhiteSpace(value));
            }
        }
        node["sub_parse_objects"] = subParseObjects;
        return node;
    }

    public PHPScriptFunction start(PHPScriptFunction globalContext) {
        this.execute((Dictionary<object, object>)((List<object>)this.parsedTree)[0], this.currentContext, null, null, null, null, null, false, null, false, null, false);
        return globalContext;
    }

    public PHPScriptFunction globalContext;
    //public PHPScriptFunction currentContext;

    public void initGlobalContext(string basePath) {
        PHPScriptFunction globalContext = new PHPScriptFunction();
        globalContext.initArrays();
        globalContext.interpretation = this;
        globalContext.identifier = "GLOBAL";

        this.globalContext = globalContext;
        this.currentContext = globalContext;
        PHPIncludedObjects includedObjects = new PHPIncludedObjects();
        /*[self setMainObjectItem:includedObjects];
    [includedObjects setInterpretation:self];
    [includedObjects init:globalContext];
    [globalContext setVariableValue:@"object" value:includedObjects defineInContext:@true inputParameter:@false overrideThis:@false];
*/
        includedObjects.interpretation = this;
        includedObjects.init(globalContext);
        globalContext.setVariableValue("object", includedObjects, true, false, false);

        PHPDataWrap dataWrap = new PHPDataWrap();

        dataWrap.interpretation = this;
        dataWrap.init(globalContext, basePath);
        globalContext.setVariableValue("data", dataWrap, true, false, false);

        PHPDates dates = new PHPDates();
        dates.interpretation = this;
        dates.init(globalContext);
        globalContext.setVariableValue("date", dates, true, false, false);
    }

    public void setPHPItem(PHPScriptObject phpItem, string name) {
        phpItem.interpretation = this;
        this.currentContext.setVariableValue(name, phpItem, true, false, false);
    }

    public object execute(Dictionary<object, object> subParseObject, PHPScriptObject context, string lastParentContextParseLabel, PHPScriptObject lastParentContext, PHPControl controlInput, Dictionary<object, object> subSwitches, PHPScriptFunction lastSetValidContext, bool preventSetValidContext, PHPScriptObject preserveContext, bool inParentContextSetting, PHPScriptFunction lastCurrentFunctionContext, bool containedAsync) {
        PHPControl control = null;
        //Console.WriteLine("subparseobject : "+subParseObject["label"]);
        if(controlInput != null) {
            control = controlInput;
        }

        if(context is PHPScriptFunction) {
            lastCurrentFunctionContext = (PHPScriptFunction)context;
        }

        if(subParseObject == null) {
            return new List<object>();
        }

        bool setSubContextSub = false;
        List<object> subContextCallback = new List<object>();
        Dictionary<object, object> dictTemp = new Dictionary<object, object>();
        if(((Dictionary<object, object>)this.definition).ContainsKey(subParseObject["label"])) {
            dictTemp = (Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]];
        }
        //if(this.definition[subParseObject["label"]]["set_context_sub"] != null) {
        if(dictTemp.ContainsKey("set_context_sub")) {
            setSubContextSub = true;
        }

        if(dictTemp.ContainsKey("parent_context")) {
            context = preserveContext;
            inParentContextSetting = true;
        }/* else {
            inParentContextSetting = false;
        }*/
        if(dictTemp.ContainsKey("preserve_context")) {
            if(context is PHPScriptFunction) {
                preserveContext = context;
            }
        }
        string postFunction = null;
        bool disableFunctionCall = false;
        List<object> empt_array_parse_objects = new List<object>() { "FollowingObjectFunctionCall" };
        List<object> subParseObjectsList = (List<object>)subParseObject["sub_parse_objects"];
        Dictionary<object, object> curSubParseObject = dictTemp;//null;
        //curSubParseObject = ((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]]);
        
        if(subParseObjectsList.Count == 0 && !curSubParseObject.ContainsKey("ignore_empty")) {
            if(empt_array_parse_objects.IndexOf(subParseObject["label"]) != -1) {
                return new List<object>();
            } else {
                return subParseObject["text_value"];
            }
        } else if(((Dictionary<object, object>)this.definition).ContainsKey(subParseObject["label"])) {
            //create script function
            List<object> switches = new List<object>();
            if(((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]]).ContainsKey("switches")) {
                switches = new List<object>(((List<object>)((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]])["switches"]));
                //////////////////////////////System.Diagnostics.Debug.WriteLine("switches : "+switches.Count);
            }
            Dictionary<object, object> switchesSet = new Dictionary<object, object>();
            if(switches.Count > 0) {
                if((string)((Dictionary<object, object>)switches[0])["non_terminal"] == "AsyncFunctionPrefix") {
                    //if(((Dictionary<object, object>)((List<object>)((Dictionary<object, object>)subParseObject)["sub_parse_objects"])[0])["label"] == ((Dictionary<object, object>)((List<object>)switches)[0])["non_terminal"]) {
                    //if([subParseObject[@"sub_parse_objects"][0][@"label"] isEqualToString:switches[0][@"non_terminal"]]) {
                    //////////////////////////////System.Diagnostics.Debug.WriteLine("--label: "+(string)((Dictionary<object, object>)((List<object>)subParseObject["sub_parse_objects"])[0])["label"]);
                    //////////////////////////////System.Diagnostics.Debug.WriteLine("--nonterminal: "+(string)((Dictionary<object, object>)switches[0])["non_terminal"]);
                    if((string)((Dictionary<object, object>)((List<object>)subParseObject["sub_parse_objects"])[0])["label"] == (string)((Dictionary<object, object>)switches[0])["non_terminal"]) {
                        switchesSet[((Dictionary<object, object>)switches[0])["non_terminal"]] = true;
                        //////////////////////////////System.Diagnostics.Debug.WriteLine("switches : "+JsonConvert.SerializeObject(switchesSet));
                    }
                }
            }

            string function = null;

            if(((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]]).ContainsKey("function")) {
                function = (string)((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]])["function"];
            }

            object callFunctionResult = null;
            string setPostFunction = null;
            if(((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]]).ContainsKey("set_post_function")) {
                setPostFunction = (string)((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]])["set_post_function"];
            }

            bool createdControl = false;
            bool controlFromNode = false;

            if(function != null) {
                ////////////////////////////System.Diagnostics.Debug.WriteLine("function : "+function);
                if(function == "create_named_script_function") {
                    context = context.createNamedScriptFunction(context);
                    function = "set_parameters_named";
                    ((PHPScriptFunction)context).debugText = (string)((Dictionary<object, object>)subParseObject)["text_value"];
                    //[(PHPScriptFunction*)context setDebugText:(NSString*)[self getText:[(NSNumber*)subParseObject[@"start_index"] intValue] stop:[(NSNumber*)subParseObject[@"stop_index"] intValue] isString:true parseObject:subParseObject]];
                } else if(function == "create_script_function") {
                    //////////////////////////////System.Diagnostics.Debug.WriteLine("create script function switches: "+JsonConvert.SerializeObject(switchesSet));
                    //bool AsyncFunctionPrefix = false;
                    if(switchesSet.Count == 0) {
                    	if(context is PHPScriptFunction) {
	                        context = (PHPScriptObject)((PHPScriptFunction)context).createScriptFunctionAlt(false);
	                        //////////////////System.Diagnostics.Debug.WriteLine("context res is : "+context);
                    	} else {
	                        context = context.createScriptFunction(false);
	                    }
                    } else {
                    	if(context is PHPScriptFunction) {
	                        context = (PHPScriptObject)((PHPScriptFunction)context).createScriptFunctionAlt((bool)switchesSet.Values.ToList<object>()[0]);
	                        //////////////////System.Diagnostics.Debug.WriteLine("context res is : "+context);
                    	} else {
	                        context = context.createScriptFunction((bool)switchesSet.Values.ToList<object>()[0]);
	                    }
                        containedAsync = true;
                    }
                    ((PHPScriptFunction)context).debugText = (string)((Dictionary<object, object>)subParseObject)["text_value"];
                    function = "set_parameters";
                    //lastCurrentFunctionContext = (PHPScriptFunction)context;
                } else if(function == "create_script_object") {
                	////////////////System.Diagnostics.Debug.WriteLine("create script object");
                    if(setPostFunction != null) {
                        if(context == this.currentContext) {

                        }

                        PHPScriptObject objectNew = ((PHPScriptFunction)context).createScriptObjectFunc(false);
                        if(!(context is PHPScriptFunction)) {
                            objectNew.currentFunctionContext = context.currentObjectFunctionContext();
                        } else {
                            objectNew.currentFunctionContext = (PHPScriptFunction)context;
                        }
                        context = objectNew;
                    } else {
                        if(context is PHPScriptFunction) {
                            context = ((PHPScriptFunction)context).createScriptObject(false);
                        } else {
                            context = context.createScriptObject(false);
                        }
                    }
                    function = null;
                    if(setPostFunction != null) {
                        function = setPostFunction;
                    }

                    if(((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]]).ContainsKey("post_function")) {
                        postFunction = (string)((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]])["post_function"];
                        //////////System.Diagnostics.Debug.WriteLine("POST-function: "+postFunction);
                    }
                    List<object> subparsedebug = (List<object>)subParseObject["sub_parse_objects"];
                    if(subparsedebug.Count == 3) {
	                    ////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(subparsedebug[1]));
	                    ////////////////System.Diagnostics.Debug.WriteLine(subParseObject["sub_parse_objects"]);
	                }


                    callFunctionResult = context;

                } else if(function == "create_if_statement") {
                    control = new PHPMultiConditionalControl();
                    control.construct(context);
                    ((PHPMultiConditionalControl)control).constructMultiConditionalControl();
                    function = "set_condition";
                    createdControl = true;
                } else if(function == "create_while_loop") {
                    
                    control = new PHPLoopControl();
                    control.construct(context);
                    //((PHPMultiConditionalControl)control).constructMultiConditionalControl();
                    function = "set_condition";
                    createdControl = true;
                } else if(function == "create_for_loop") {
                    
                    control = new PHPForLoopControl();
                    control.construct(context);
                    //((PHPMultiConditionalControl)control).constructMultiConditionalControl();
                    function = "set_sub_routine";
                    createdControl = true;
                } else if(function == "create_foreach_loop") {
                    
                    control = new PHPForLoopControl();
                    control.construct(context);
                    ((PHPForLoopControl)control).setForeach(true);
                    //((PHPMultiConditionalControl)control).constructMultiConditionalControl();
                    function = "set_sub_routine";
                    createdControl = true;
                } else if(function == "set_control_switches") {
                    function = null;
                }
                if(controlFromNode) {
                    if(createdControl && control != null) {
                        object controlResult = null;
                        if(control is PHPMultiConditionalControl) {
                            ((PHPMultiConditionalControl)control).run(context, lastSetValidContext);
                        } else if(control is PHPLoopControl) {
                            ((PHPLoopControl)control).run(context, lastSetValidContext);
                        } else if(control is PHPForLoopControl) {
                            ((PHPForLoopControl)control).run(context, lastSetValidContext);
                        }
                        if((controlResult is bool && (bool)controlResult != false) || controlResult is PHPReturnResult) {
                            return controlResult;
                        }
                    }
                    return null;
                }
            }

            
        	////////////////////////System.Diagnostics.Debug.WriteLine("after-stuff-continue-subparseobject : "+subParseObject["label"]);

            List<object> values = new List<object>();
            //if(subParseObject.ContainsKey("variable_references")) {
            ////////////////////////////////System.Diagnostics.Debug.WriteLine("parse item : "+subParseObject["label"]);
            ////////////////////////System.Diagnostics.Debug.WriteLine("varrefdict len: "+((List<object>)subParseObject["variable_references"]).Count);
            foreach(object varRefDict in (List<object>)subParseObject["variable_references"]) {
                Dictionary<object, object> variableReference = (Dictionary<object, object>)varRefDict;
                ////////////////////////System.Diagnostics.Debug.WriteLine("varrefdict iteration");
                /*string debugValue = "varRefDict iteration : "+((Dictionary<object, object>)(variableReference["subParseObject"]))["label"];
            	////////////////////////System.Diagnostics.Debug.WriteLine(debugValue);
            	////////////////////////System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(variableReference.Keys));*/
            	/*if(variableReference.ContainsKey("non_terminal")) {
	            	////////////////////////System.Diagnostics.Debug.WriteLine(((Dictionary<object, object>)(variableReference["subParseObject"]))["text_value"]);
	            }*/
                int offset = 0;
                if(variableReference.ContainsKey("offset")) {
                    offset = (int)variableReference["offset"];
                }
                ////////////////////////System.Diagnostics.Debug.WriteLine("offset: "+offset);
                if(variableReference.ContainsKey("sub_context") || (setSubContextSub && variableReference.ContainsKey("opt"))) {
                    ////////////////////////System.Diagnostics.Debug.WriteLine("opt or sub_context: "+variableReference);
                    object subObject;
                    subObject = variableReference["subParseObject"];
                    if(subObject != null) {
                        PHPScriptEvaluationReference scriptReference;
                        //Dictionary<object, object> subObjectDict = new Dictionary<object, object>((Dictionary<object, object>)subObject);
                        //if(!subObjectDict.ContainsKey("scriptReference")) {
                        /*if(!(context is PHPScriptFunction)) {
                        	////////////////////////System.Diagnostics.Debug.WriteLine("IS NOT FUNC context");
                        }*/
                            scriptReference = new PHPScriptEvaluationReference();
                            scriptReference.interpretation = this;
                            scriptReference.contextValue = context;
                            scriptReference.subObjectDict = (Dictionary<object, object>)subObject;
                            scriptReference.lastCurrentFunctionContext = lastCurrentFunctionContext;
                            scriptReference.lastSetValidContext = lastSetValidContext;
                            scriptReference.preventSetValidContext = preventSetValidContext;
                            scriptReference.preserveContext = preserveContext;
                            scriptReference.inParentContextSetting = inParentContextSetting;
                            scriptReference.isAsync = containedAsync;
                        /*} else {
                            scriptReference = null;
                        }*/
                        //	////////////////////////System.Diagnostics.Debug.WriteLine("IS NOT FUNC context after");
                        if(variableReference.ContainsKey("sub_context")) {
                            values.Add(scriptReference);
                        } else {
                            subContextCallback.Add(scriptReference);
                        }
                    }
                } else {
                    object variableValue = null;
                    bool setValueCallback = false;
                    if(variableReference.ContainsKey("variableValue")) {
                        variableValue = variableReference["variableValue"];
                    } else if(variableReference.ContainsKey("parse") && (bool)(variableReference["parse"])) {
                        ////////////////////////System.Diagnostics.Debug.WriteLine("non parse: "+variableReference);
                        object subObject = variableReference["subParseObject"];
                        //////////////////////System.Diagnostics.Debug.WriteLine("execute subObject : "+((Dictionary<object, object>)variableReference["subParseObject"])["text_value"]);
                        if(subObject != null) {
                        	////////////////////System.Diagnostics.Debug.WriteLine("sub object is : "+subObject);
	                        Dictionary<object, object> parseItemDebug = (Dictionary<object, object>)variableReference["subParseObject"];
	                        if(parseItemDebug.ContainsKey("start_index")) {
		                        string text = this.getText((int)parseItemDebug["start_index"], (int)parseItemDebug["stop_index"], true, parseItemDebug).ToString();
	                        	////////////////////System.Diagnostics.Debug.WriteLine("parse: "+text);
	                        }
                        	//////////////////////System.Diagnostics.Debug.WriteLine("execute call 2");
                            variableValue = this.execute((Dictionary<object, object>)subObject, context, lastParentContextParseLabel, lastParentContext, control, null, lastSetValidContext, preventSetValidContext, preserveContext, inParentContextSetting, lastCurrentFunctionContext, containedAsync);
                          //  ////////////////////System.Diagnostics.Debug.WriteLine("variableValue : "+variableValue);
                            //Debug
                            /*if(variableValue is List<object>) {
                                foreach(object debugItem in ((List<object>)variableValue)) {
                                    ////////////////////System.Diagnostics.Debug.WriteLine(debugItem); //.Join<object>("-");
                                }
                            }*/
                            if(((Dictionary<object, object>)subObject).ContainsKey("containsIdentifier")) {
                                setValueCallback = true;
                            }
                        }
                    } else if(variableReference.ContainsKey("non_terminal")) {
                        
                        variableValue = ((Dictionary<object, object>)variableReference["subParseObject"])["text_value"];
                        ////////////////////////System.Diagnostics.Debug.WriteLine("non terminal: "+variableValue);
                    } else {
                        variableValue = variableReference["value"];
                    }
                    if(variableValue != null) {
                        /*bool perform_next = true;
                        if(variableValue is PHPVariableReference) {
                            perform_next = false;
                            if(!((PHPVariableReference)variableValue).ignoreSetContext) {
                                perform_next = true;
                            }
                            //////////////////////////////System.Diagnostics.Debug.WriteLine("before : "+((PHPVariableReference)variableValue).ignoreSetContext);
                            //////////////////////////////System.Diagnostics.Debug.WriteLine("beforeContainskey : "+variableReference.ContainsKey("set_context"));
                            
                            if(variableReference.ContainsKey("set_context")) {
                                //////////////////////////////System.Diagnostics.Debug.WriteLine("beforeContainskey : "+variableReference["set_context"]);
                            }
                        }*/
                        if(variableReference.ContainsKey("set_context") && ((bool)variableReference["set_context"]) && !((variableValue is PHPVariableReference) && ((PHPVariableReference)variableValue).ignoreSetContext)/*perform_next*/) {
                            object variableValueReference = variableValue;
                            ////////////////////////System.Diagnostics.Debug.WriteLine("before1 set context value as : "+variableValue);
                            if(variableValue is PHPVariableReference) {
                                variableValue = ((PHPVariableReference)variableValue).get(null);
                                //////////////////////////////System.Diagnostics.Debug.WriteLine("before2 set context value as : "+variableValue);
                            }
                            if(lastParentContext == null) {
                                lastParentContext = context;
                                lastParentContextParseLabel = "Value";
                            }
                            if(variableValue != null && (variableValue is PHPScriptObject || variableValue is PHPScriptFunction)) {
                                if(variableValue is PHPScriptObject) {
                                    ((PHPScriptObject)variableValue).interpretation = this;
                                }
                                PHPScriptObject scriptObjectVariableValue = (PHPScriptObject)variableValue;

                                context = scriptObjectVariableValue;
                                ////////////////////////System.Diagnostics.Debug.WriteLine("set context value as : "+context);
                            }
                        }
                        if(variableValue != null) {
                            values.Add(variableValue);
                            if(setValueCallback) {

                            } else {
                                subParseObject["variableValue"] = variableValue;
                            }
                        }
                    }

           		 	//////////////////////////System.Diagnostics.Debug.WriteLine("end-"+debugValue);
                }

            	//////////////////////////System.Diagnostics.Debug.WriteLine("varRefDict iteration-end : "+((Dictionary<object, object>)(variableReference["subParseObject"]))["label"]);
            }
            //stuff


        	////////////////////////System.Diagnostics.Debug.WriteLine("after-stuff-continue-subparseobject : "+subParseObject["label"]);

            ////////////////////////System.Diagnostics.Debug.WriteLine("continue : "+context);

            object returnValueResultStopExecution = null;
            //Dictionary<object, object>
            if(((List<object>)((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]])["variables"]).Count == 0 || (values.Count == 0 && ((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]]).ContainsKey("exception"))) {
                foreach(object subObject in ((List<object>)subParseObject["sub_parse_objects"])) {
                    bool storeVariable = false;
                    Dictionary<object, object> subObjectDictvalue = (Dictionary<object, object>)subObject;
                    if(subObjectDictvalue.ContainsKey("containsIdentifier")) {
                        storeVariable = true;
                    }
                    if(returnValueResultStopExecution == null) {
                        object subValueResultItem;
                        if(subObjectDictvalue.ContainsKey("variableValueAlt")) {
                            subValueResultItem = subObjectDictvalue["variableValueAlt"];
                        } else {
                        	////////////////////////System.Diagnostics.Debug.WriteLine("execute call 3");
                            subValueResultItem = this.execute(subObjectDictvalue, context, lastParentContextParseLabel, lastParentContext, control, null, lastSetValidContext, preventSetValidContext, preserveContext, inParentContextSetting, lastCurrentFunctionContext, containedAsync);
                            if(storeVariable) {
                                subObjectDictvalue["variableValueAlt"] = subValueResultItem;
                            }
                        }
                        if(subValueResultItem is PHPReturnResult) {
                            returnValueResultStopExecution = subValueResultItem;
                        } else {
                            if(subValueResultItem != null) {
                                values.Add(subValueResultItem);
                            }
                        }
                    }
                }
                if(returnValueResultStopExecution == null) {
                    List<object> values_store1 = values;
                    values = new List<object>();
                    values.Add(values_store1);
                }
                if(returnValueResultStopExecution != null) {
                    return returnValueResultStopExecution;
                }
            }
            if(function != null && function == "set_parameters" && values.Count == 1) {
                List<object> setValuesAddition = new List<object>() {
                    new List<object>()
                };
                setValuesAddition.AddRange(values);
                values = setValuesAddition;
            }
            //if(function != null && disableFunctionCall == "set_parameters")

            ////////////////////////System.Diagnostics.Debug.WriteLine("continue : "+context);
            //object test = new object();
            lock(context) {
                /*JsonSerializerSettings set = new JsonSerializerSettings();
                set.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                set.MaxDepth = 4;
                try {
	                //////System.Diagnostics.Debug.WriteLine("continue : func : "+function+" - "+subParseObject["label"]+" - "+JsonConvert.SerializeObject(values, set));
	            } catch(Exception e) {*/
	            	toJSON toJSONDebug = new toJSON();

	                //////System.Diagnostics.Debug.WriteLine("continue : func : "+function+" - "+subParseObject["label"]+" - "+toJSONDebug.toJSONString(values));
	            //}
                //NSLog(@"continue : func : %@ - %@ - %@", function, subParseObject[@"label"], values);
          		////////System.Diagnostics.Debug.WriteLine("continue : func : "+context+" - "+function+" - "+values.Count+" "+subParseObject["label"]);
      		    foreach(object valueitem in values) {
                    ////////System.Diagnostics.Debug.WriteLine("valueitem : "+valueitem);
                    if(valueitem is List<object>) {
                        foreach(object o in ((List<object>)valueitem)) {
                           ////////System.Diagnostics.Debug.WriteLine("valueitemSub : "+o);
                        }
                    }
                }
                if(function != null) {
                    if(returnValueResultStopExecution != null) {
                    	//////////////////////////System.Diagnostics.Debug.WriteLine("stop execution value : "+returnValueResultStopExecution);
                        return returnValueResultStopExecution;
                    }
                    if(function == "call_script_function_sub") {
                        values.Add(new List<object>());
                    }

                    if(((Dictionary<object, object>)((Dictionary<object, object>)this.definition)[subParseObject["label"]]).ContainsKey("wrap_values")) {
                        /*setPostFunction = (string)((Dictionary<object, object>)((Dictionary<object, object>)this.definition)["label"])["set_post_function"];*/
                        values = new List<object>() { values };
                    }

                    if(disableFunctionCall) {
                        return values;
                    }

                    object setContext = context;
                    //lock(setContext) {
                    if(function == "set_condition" || function == "set_else_condition" || function == "set_sub_routine") {
                        setContext = control;
                    }

                    List<object> callUserFuncArrayValues = new List<object>();

                    while(setContext is PHPReturnResult) {
                        setContext = ((PHPReturnResult)setContext).get();
                    }

                    if(function != null) {
                        
                        //////////////////////////////System.Diagnostics.Debug.WriteLine("setcontext is : "+setContext);
                        
                        //////////////////////////////System.Diagnostics.Debug.WriteLine("function : "+function); //+ " - "+JsonConvert.SerializeObject(values, Formatting.None, set));*/
                        ////////////////////////System.Diagnostics.Debug.WriteLine("function : "+function);
                    

                        switch(function) {
                            case "numeric_multiplication":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).numericMultiplication(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                } else {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).numericMultiplication(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                }
                                break;
                            case "numeric_subtraction":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).numericSubtraction(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                } else {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).numericSubtraction(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                }
                                break;
                            case "numeric_division":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).numericDivision(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                } else {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).numericDivision(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                }
                                break;
                            case "string_addition":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).stringAddition(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                } else {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).stringAddition(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                }
                                break;
                            case "numeric_addition":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).numericAddition(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                } else {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).numericAddition(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                }
                                break;
                            case "set_sub_routine":
                                if(setContext is PHPLoopControl) {
                                    ((PHPLoopControl)setContext).subRoutine = (PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 0);
                                } else if(setContext is PHPForLoopControl) {
                                    ((PHPForLoopControl)setContext).subRoutine = (PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 0);
                                }
                                //////////////////////////////System.Diagnostics.Debug.WriteLine("after set sub routine");
                                break;
                            case "set_parameters":
                                if(values.Count == 1) {
                                    values.Insert(0, new List<object>());
                                }
                                callFunctionResult = ((PHPScriptFunction)setContext).setParameters((List<object>)this.getMutableArrayValueNil(values, 0), (PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 1));
                                break;
                            case "set_parameters_named":
                                
                                if(values.Count == 1) {
                                    values.Insert(0, new List<object>());
                                }
                                callFunctionResult = ((PHPScriptFunction)setContext).setParametersNamed(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1), this.getMutableArrayValueNil(values, 2));
                                break;
                            case "set_object_identifier":
                                ((PHPScriptObject)setContext).setObjectIdentifier(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "script_push_array":
                                if(setContext is PHPScriptFunction) {
                                    ((PHPScriptFunction)setContext).scriptPushArray(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                } else if(setContext is PHPScriptObject) {
                                    ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).scriptPushArray(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                }
                                break;
                            case "return_result_dereference":
	                            ////////////////////////System.Diagnostics.Debug.WriteLine("in return_result_dereference");
                                if(!(setContext is PHPScriptObject)) {
                                    setContext = this.globalContext.parseInputVariable(setContext);
                                }
                                callFunctionResult = ((PHPScriptObject)setContext).returnResultDereference(this.getMutableArrayValueNil(values, 0));
                               	////////////////////////System.Diagnostics.Debug.WriteLine("returndereference callfunctionresult: "+callFunctionResult+" - "+subParseObject["label"]);
                               	/*if(callFunctionResult is PHPScriptObject) {
                               		toJSON toJSONInstance = new toJSON();
                               		string toJSONDebug = toJSONInstance.toJSONString(callFunctionResult);
                               		////////////////////////System.Diagnostics.Debug.WriteLine(toJSONDebug);
                               	}*/
                                break;
                            case "set_paranthesis":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).setParanthesis(this.getMutableArrayValueNil(values, 0));
                                } else if(setContext is PHPScriptObject) {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).setParanthesis(this.getMutableArrayValueNil(values, 0));
                                }
                                break;
                            /*case "set_variable_increment":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).setVariableIncrement(
                                } else if(setContext is PHPScriptObject) {

                                }
                                break;
                            case "set_variable_decrement":
                                break;
                            case "set_variable_value_append":
                                break;*/

                            case "set_variable_value":
                            	bool set_var_value_defineInContext = false;
                            	bool set_var_value_inputParameter = false;
                            	bool set_var_value_overrideThis = false;

                            	if(this.getMutableArrayValueNil(values, 2) != null) {
                            		set_var_value_defineInContext = (bool)this.getMutableArrayValueNil(values, 2);
                            	}
                            	if(this.getMutableArrayValueNil(values, 3) != null) {
                            		set_var_value_inputParameter = (bool)this.getMutableArrayValueNil(values, 3);
                            	}
                            	if(this.getMutableArrayValueNil(values, 4) != null) {
                            		set_var_value_overrideThis = (bool)this.getMutableArrayValueNil(values, 4);
                            	}
	                            //object name, object value, bool defineInContext=false, bool inputParameter=false, bool overrideThis=false)
                                if(setContext is PHPScriptFunction) {

                                	//////////////////////////System.Diagnostics.Debug.WriteLine("set_variable_value values funct count: "+values.Count);
                                    foreach(object valueObj in values) {
                                    	//////////////////////////System.Diagnostics.Debug.WriteLine("valueobj : "+valueObj);
                                    }
                                    ////////////////////////////System.Diagnostics.Debug.WriteLine("in set " + JsonConvert.SerializeObject(values, Formatting.None, set));

                                    callFunctionResult = ((PHPScriptFunction)setContext).setVariableValue(this.getMutableArrayValueNil(values, 0),this.getMutableArrayValueNil(values, 1),(bool)set_var_value_defineInContext,(bool)set_var_value_inputParameter,(bool)set_var_value_overrideThis);
                                } else if(setContext is PHPScriptObject) {
                                    

                                	//////////////////////////System.Diagnostics.Debug.WriteLine("set_variable_value values obj count: "+values.Count);
                                    foreach(object valueObj in values) {
                                    	//////////////////////////System.Diagnostics.Debug.WriteLine("valueobj : "+valueObj);
                                    }
                                    ////////////////////////////System.Diagnostics.Debug.WriteLine("in set " + JsonConvert.SerializeObject(values, Formatting.None, set));

                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).setVariableValue(this.getMutableArrayValueNil(values, 0),this.getMutableArrayValueNil(values, 1),(bool)set_var_value_defineInContext,(bool)set_var_value_inputParameter,(bool)set_var_value_overrideThis);
                                }
                                break;

                            case "set_variable_value_in_context":
                                bool defineInContextSetVarInContext = true;
                                bool inputParameterSetVarInContext = false;
                                bool overrideThisSetVarInContext = false;

                                if(this.getMutableArrayValueNil(values, 4) != null) {
                                    overrideThisSetVarInContext = (bool)this.getMutableArrayValueNil(values, 4);
                                }


                                if(setContext is PHPScriptFunction) {
                                    ////////////////////////System.Diagnostics.Debug.WriteLine("funct");


                                    
	                                if(this.getMutableArrayValueNil(values, 3) != null) {
	                                    inputParameterSetVarInContext = (bool)this.getMutableArrayValueNil(values, 3);
	                                } else {
	                                	defineInContextSetVarInContext = false;
	                                }

                                    //defineInContextSetVarInContext = true;
                                    if(this.getMutableArrayValueNil(values, 2) != null) {
                                        defineInContextSetVarInContext = (bool)this.getMutableArrayValueNil(values, 2);
                                    } else {
                                    	defineInContextSetVarInContext = true;
                                    }


                                    ////////////////////////System.Diagnostics.Debug.WriteLine("values count: "+values.Count);
                                    /*foreach(object valueObj in values) {
                                    	////////////////////////System.Diagnostics.Debug.WriteLine("valueobj : "+valueObj);
                                    	if(valueObj is List<object>) {
                                    		foreach(object valueObj2 in (List<object>)valueObj) {
                                    			////////////////////////System.Diagnostics.Debug.WriteLine("valueobj2 : "+valueObj2);
                                    		}
                                    	}
                                    }*/
                                    //////////////////////////System.Diagnostics.Debug.WriteLine("in set " + JsonConvert.SerializeObject(values, Formatting.None, set) + " - " + defineInContextSetVarInContext + " - " + inputParameterSetVarInContext + " - " + overrideThisSetVarInContext);

                                    callFunctionResult = ((PHPScriptFunction)setContext).setVariableValueInContext(this.getMutableArrayValueNil(values, 0),this.getMutableArrayValueNil(values, 1),(bool)defineInContextSetVarInContext,inputParameterSetVarInContext, overrideThisSetVarInContext, (PHPScriptObject)lastCurrentFunctionContext); //,(bool)this.getMutableArrayValueNil(values, 3),(bool)this.getMutableArrayValueNil(values, 4));
                                } else if(setContext is PHPScriptObject) {
                                    //////////////////////////System.Diagnostics.Debug.WriteLine("obj");

                                    if(this.getMutableArrayValueNil(values, 2) != null) {
                                        defineInContextSetVarInContext = (bool)this.getMutableArrayValueNil(values, 2);
                                    }
                                    if(this.getMutableArrayValueNil(values, 3) != null) {
	                                    inputParameterSetVarInContext = (bool)this.getMutableArrayValueNil(values, 3);
	                                }

                                    ////////////////////////////System.Diagnostics.Debug.WriteLine("in set " + JsonConvert.SerializeObject(values, Formatting.None, set) + " - " + defineInContextSetVarInContext + " - " + inputParameterSetVarInContext + " - " + overrideThisSetVarInContext);

                                    callFunctionResult = ((PHPScriptObject)setContext).setVariableValueInContext(this.getMutableArrayValueNil(values, 0),this.getMutableArrayValueNil(values, 1), defineInContextSetVarInContext,inputParameterSetVarInContext, overrideThisSetVarInContext, lastCurrentFunctionContext);
                                        //(bool)this.getMutableArrayValueNil(values, 2),(bool)this.getMutableArrayValueNil(values, 3),(bool)this.getMutableArrayValueNil(values, 4));
                                }
                                //////////////////////////System.Diagnostics.Debug.WriteLine("call function result : "+callFunctionResult);
                                break;
                            case "set_property_reference":
                                callFunctionResult = ((PHPScriptObject)setContext).setPropertyReference(this.getMutableArrayValueNil(values, 0));
                                setContext = false;
                                break;
                            case "set_class_reference":
                            	if(setContext is PHPScriptFunction) {
	                                callFunctionResult = ((PHPScriptFunction)setContext).setClassReference(this.getMutableArrayValueNil(values, 0).ToString());
	                            } else {
	                                callFunctionResult = ((PHPScriptObject)setContext).setClassReference(this.getMutableArrayValueNil(values, 0).ToString());
	                            }
                                setContext = false;
                                ////////////////////////////System.Diagnostics.Debug.WriteLine("set class ref callfunctionresult: "+callFunctionResult);
                                break;
                            case "set_variable_reference":
                                
                                //////////////////System.Diagnostics.Debug.WriteLine("set variable value in context: "+setContext+" - "+((PHPScriptObject)setContext).parentContext);
                                foreach(object debugItem in values) {
                                    //////////////////System.Diagnostics.Debug.WriteLine(debugItem);
                                }

                                bool a = false;
                                bool b = false;
                                if(this.getMutableArrayValueNil(values, 1) != null) {
                                    a = (bool)this.getMutableArrayValueNil(values, 1);
                                }
                                if(this.getMutableArrayValueNil(values, 2) != null) {
                                    b = (bool)this.getMutableArrayValueNil(values, 2);
                                }
                                //if(setContext is PHPScriptObject && !(setContext is PHPScriptFunction)) {
	                                callFunctionResult = ((PHPScriptObject)setContext).setVariableReference(this.getMutableArrayValueNil(values, 0), a, b);
	                            /*} else {
	                                callFunctionResult = ((PHPScriptFunction)setContext).setVariableReference(this.getMutableArrayValueNil(values, 0), a, b);
	                            }*/ //, (bool)this.getMutableArrayValueNil(values, 1), (bool)this.getMutableArrayValueNil(values, 2));
                                ((PHPVariableReference)callFunctionResult).currentContext = lastCurrentFunctionContext;
                                //////////////////System.Diagnostics.Debug.WriteLine("variable reference : "+callFunctionResult);
                                
                                //////////////////System.Diagnostics.Debug.WriteLine("variable reference : "+((PHPVariableReference)callFunctionResult).get(null));
                                break;
                            case "set_variable_reference_ignore":
                                ////////////////////////System.Diagnostics.Debug.WriteLine("set_variable_reference_ignore Count: "+values.Count);
                                bool setvarrrefignore_ignoreValue = true;
                                if(this.getMutableArrayValueNil(values, 1) != null) {
	                                setvarrrefignore_ignoreValue = (bool)this.getMutableArrayValueNil(values, 1);
	                            }
                                callFunctionResult = ((PHPScriptObject)setContext).setVariableReferenceIgnore(this.getMutableArrayValueNil(values, 0), setvarrrefignore_ignoreValue);
                                ////////////////////////System.Diagnostics.Debug.WriteLine("set_variable_reference_ignore res: "+callFunctionResult);
                                ((PHPVariableReference)callFunctionResult).currentContext = lastCurrentFunctionContext;
                                break;
                            case "set_string":
                                callFunctionResult = this.getMutableArrayValueNil(values, 0).ToString();
                                //callFunctionResult = ((PHPScriptObject)setContext).setString(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "set_condition":
                                if(setContext is PHPMultiConditionalControl) {
                                	foreach(object debugItem in values) {
	                               		////////////////////System.Diagnostics.Debug.WriteLine(debugItem);
	                                }
                                    ((PHPMultiConditionalControl)setContext).setCondition(this.getMutableArrayValueNil(values, 0), (PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 1));
                                } else if(setContext is PHPLoopControl) {
                                    ((PHPLoopControl)setContext).setCondition((PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 0), (PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 1));
                                } else if(setContext is PHPForLoopControl) {
                                    ((PHPForLoopControl)setContext).setCondition(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1), this.getMutableArrayValueNil(values, 2));
                                }
                                break;
                            case "or_condition":
                                callFunctionResult = ((PHPScriptFunction)setContext).orCondition(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "and_condition":
                                callFunctionResult = ((PHPScriptFunction)setContext).andCondition(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "inequality":
                               	////System.Diagnostics.Debug.WriteLine("inequality-interpr");
                                callFunctionResult = ((PHPScriptFunction)setContext).inequality(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "equals":
                                callFunctionResult = ((PHPScriptFunction)setContext).equals(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "inequality_strong":
                                callFunctionResult = ((PHPScriptFunction)setContext).inequalityStrong(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "equals_strong":
                                callFunctionResult = ((PHPScriptFunction)setContext).equalsStrong(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "greater":
                                callFunctionResult = ((PHPScriptFunction)setContext).greater(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "less":
                                callFunctionResult = ((PHPScriptFunction)setContext).less(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "greater_equals":
                                callFunctionResult = ((PHPScriptFunction)setContext).greaterEquals(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "less_equals":
                                callFunctionResult = ((PHPScriptFunction)setContext).lessEquals(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "set_else_condition":
                                ((PHPMultiConditionalControl)setContext).setElseCondition((PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 0));
                                break;
                            /*case "set_condition":
                                ((PHPMultiConditionalControl)setContext).setCondition((PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 0),(PHPScriptEvaluationReference)this.getMutableArrayValueNil(values, 1));
                                break;*/
                            case "set_dictionary_value":
                            	//////////System.Diagnostics.Debug.WriteLine("in interpret set: "+this.getMutableArrayValueNil(values, 0)+" "+this.getMutableArrayValueNil(values, 1));
                            	if(setContext is PHPScriptFunction) {
	                                ((PHPScriptFunction)setContext).setDictionaryValue(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1), lastCurrentFunctionContext);
                            	} else {
	                                ((PHPScriptObject)setContext).setDictionaryValue(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1), lastCurrentFunctionContext);
	                            }
                                break;
                            case "negative_value":
                                callFunctionResult = ((PHPScriptObject)setContext).negativeValue(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "negate_value":
                                callFunctionResult = ((PHPScriptObject)setContext).negateValue(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "set_access_flag":
                                ((PHPScriptObject)setContext).setAccessFlag(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "set_prototype":
                            	////////////////System.Diagnostics.Debug.WriteLine("set prototype: "+setContext+" "+this.getMutableArrayValueNil(values, 0));
                                ((PHPScriptObject)setContext).prototype = (PHPScriptObject)this.getMutableArrayValueNil(values, 0);
                                break;
                            case "return_parameter_input":
                                callFunctionResult = ((PHPScriptObject)setContext).returnParameterInput(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "return_parameter_input_identifier_value":
                                callFunctionResult = ((PHPScriptObject)setContext).returnParameterInputIdentifierValue(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "collect_parameters":
                                //////////////////////////////System.Diagnostics.Debug.WriteLine("collect parameters values: ");
                                foreach(object value in values) {
                                    //////////////////////////////System.Diagnostics.Debug.WriteLine("value : "+value);
                                }
                                callFunctionResult = ((PHPScriptObject)setContext).collectParameters((List<object>)this.getMutableArrayValueNil(values, 0));
                                break;
                            case "return_result":
                                callFunctionResult = ((PHPScriptObject)setContext).returnResult(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                
                                //////////////////////////////System.Diagnostics.Debug.WriteLine("return_result callfunctionResult is : "+callFunctionResult);
                                break;
                            case "return_prop_result":
                                callFunctionResult = ((PHPScriptObject)setContext).returnPropResult(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "return_value_result":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext).returnValueResult(this.getMutableArrayValueNil(values, 0));
                                } else if(setContext is PHPScriptObject) {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext).returnValueResult(this.getMutableArrayValueNil(values, 0));
                                }
                                break;
                            case "get_array_value_context":
                                
                                //////////////////////////////System.Diagnostics.Debug.WriteLine("get_array_value_context: "+setContext+" - "+((PHPScriptObject)setContext).parentContext);
                              

                                if(!(setContext is PHPScriptObject)) {
                                    setContext = this.globalContext.parseInputVariable(setContext);
                                }
                                bool returnReference = false;
                                if(this.getMutableArrayValueNil(values, 1) != null) {
                                    returnReference = (bool)this.getMutableArrayValueNil(values, 1);
                                }
                                //bool returnReference = (bool)this.getMutableArrayValueNil(values, 1);
                                ////////////////////////////System.Diagnostics.Debug.WriteLine("return reference: "+returnReference);
                                callFunctionResult = ((PHPScriptObject)setContext).getArrayValueContext(this.getMutableArrayValueNil(values, 0), (bool)returnReference, lastCurrentFunctionContext);
                                ////////////////////////System.Diagnostics.Debug.WriteLine("array value context: "+callFunctionResult);
                                foreach(object debugItem in values) {
                                    ////////////////////////System.Diagnostics.Debug.WriteLine(debugItem);
                                }
                                //foreach(object valueItemDebug in values
                                if(callFunctionResult is PHPScriptFunction) {
                                    //////////////////////////////System.Diagnostics.Debug.WriteLine("identifier : "+((PHPScriptFunction)callFunctionResult).identifier);
                                    //////////////////////////////System.Diagnostics.Debug.WriteLine("identifier : "+((PHPScriptFunction)callFunctionResult).debugText);
                                }
                                break;
                            case "get_array_value_context_reference":
                                if(!(setContext is PHPScriptObject)) {
                                    setContext = this.globalContext.parseInputVariable(setContext);
                                }
                                bool returnReferenceValue = true;
                                if(this.getMutableArrayValueNil(values, 1) != null) {
                                    returnReferenceValue = (bool)this.getMutableArrayValueNil(values, 1);
                                }
                                callFunctionResult = ((PHPScriptObject)setContext).getArrayValueContextReference(this.getMutableArrayValueNil(values, 0), (bool)returnReferenceValue);
                                ////////////////////////////System.Diagnostics.Debug.WriteLine("return get_array_value_context_reference: "+callFunctionResult);
                                break;
                            case "return_last_prop_result":
                                callFunctionResult = ((PHPScriptObject)setContext).returnLastPropResult(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "new_instance":
                                if(setContext is PHPScriptFunction) {
                                    callFunctionResult = ((PHPScriptFunction)setContext)._newInstance((PHPScriptObject)this.getMutableArrayValueNil(values, 0), (List<object>)this.getMutableArrayValueNil(values, 1));
                                } else {
                                    callFunctionResult = ((PHPScriptFunction)((PHPScriptObject)setContext).parentContext)._newInstance((PHPScriptObject)this.getMutableArrayValueNil(values, 0), (List<object>)this.getMutableArrayValueNil(values, 1));
                                }
                                break;
                            case "call_script_function_sub":
                                bool returnObject = false;
                                if(this.getMutableArrayValueNil(values, 3) != null) {
                                    returnObject = (bool)this.getMutableArrayValueNil(values, 3);
                                }
                                callFunctionResult = ((PHPScriptObject)setContext).callScriptFunctionSub(this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1), this.getMutableArrayValueNil(values, 2), returnObject, (PHPScriptFunction)preserveContext);
                                break;
                            case "call_script_function_reference":
                                callFunctionResult = ((PHPScriptObject)setContext).callScriptFunctionReference(this.getMutableArrayValueNil(values, 0), (List<object>)this.getMutableArrayValueNil(values, 1), (bool)this.getMutableArrayValueNil(values, 2), (bool)this.getMutableArrayValueNil(values, 3));
                                break;
                            case "call_script_function":
                                callFunctionResult = ((PHPScriptFunction)setContext).callScriptFunction((PHPVariableReference)this.getMutableArrayValueNil(values, 0), (List<object>)this.getMutableArrayValueNil(values, 1), (bool)this.getMutableArrayValueNil(values, 2));
                                break;
                            case "get_array_value":
                                if(!(setContext is PHPScriptObject)) {
                                    setContext = this.globalContext.parseInputVariable(setContext);
                                }
                                callFunctionResult = ((PHPScriptObject)setContext).getArrayValue((PHPVariableReference)this.getMutableArrayValueNil(values, 0), this.getMutableArrayValueNil(values, 1));
                                break;
                            case "script_array_push":
                            	//////////////////System.Diagnostics.Debug.WriteLine("set context is : "+setContext);
                                if(!(setContext is PHPScriptObject)) {
                                    setContext = this.globalContext.parseInputVariable(setContext);
                                }
                            	//////////////////System.Diagnostics.Debug.WriteLine("set context is : "+setContext);
                                ((PHPScriptObject)setContext).scriptArrayPush(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "delete_property_reference":
                            	//////////////System.Diagnostics.Debug.WriteLine("delete property ref: "+setContext+" "+this.getMutableArrayValueNil(values, 0));
                                ((PHPScriptObject)setContext).deletePropertyReference(this.getMutableArrayValueNil(values, 0));
                                break;
                            case "clone_object":
                                callFunctionResult = ((PHPScriptObject)setContext).cloneObject(this.getMutableArrayValueNil(values, 0));
                                break;
                        }

                    }
                    if(createdControl && control != null) {
                        object controlResult = null; //control.run((PHPScriptFunction)context, lastSetValidContext);
                       	//////////////////////System.Diagnostics.Debug.WriteLine("in control call");
                        if(control is PHPMultiConditionalControl) {
                            controlResult = ((PHPMultiConditionalControl)control).run(context, lastSetValidContext);
                        } else if(control is PHPLoopControl) {
                            controlResult = ((PHPLoopControl)control).run(context, lastSetValidContext);
                        } else if(control is PHPForLoopControl) {
                            controlResult = ((PHPForLoopControl)control).run(context, lastSetValidContext);
                        }
                        //////////////////////System.Diagnostics.Debug.WriteLine("in control call : "+controlResult);
                        if((controlResult is bool && (bool)controlResult != false) || controlResult is PHPReturnResult) {
                            return controlResult;
                        }
                    }
                	//}
                    //if(this.definition
                }
                if(postFunction != null) {
                    //////////////////////////////System.Diagnostics.Debug.WriteLine("in postFunction");
                    if(postFunction == "reverse_array") {
                        context.reverseArray();
                    }
                }
                if(callFunctionResult != null) {
                    List<object> results = new List<object>();
                    List<object> subContextCallbacks = subContextCallback;
                    foreach(object subContextCallbackItem in subContextCallbacks) {
                        ////////////////////////System.Diagnostics.Debug.WriteLine("callfunc result: "+callFunctionResult);
                        if(!(callFunctionResult is PHPScriptObject)) {
                            callFunctionResult = this.globalContext.parseInputVariable(callFunctionResult);
                        }
                        if(callFunctionResult is PHPUndefined) {

                       		////////////////System.Diagnostics.Debug.WriteLine("callfunc result after: "+callFunctionResult);
                			////////////////System.Diagnostics.Debug.WriteLine("callfunctionResult is : "+callFunctionResult+" - "+function+" "+((PHPScriptFunction)context).debugText);
                        }
                        object subCallItemResultValue = ((PHPScriptEvaluationReference)subContextCallbackItem).callFun((PHPScriptObject)callFunctionResult);
                        ////////////////////////System.Diagnostics.Debug.WriteLine("sub call item result value : "+subCallItemResultValue);
                        if(subCallItemResultValue != null) {
                            results.Add(subCallItemResultValue);
                        }
                    }
                    if(results.Count == 0) {
                        //////////////////////////////System.Diagnostics.Debug.WriteLine("return direct: "+callFunctionResult);
                        return callFunctionResult;
                    }
                    return results;
                }
            }
        } else if(((List<object>)subParseObject["sub_parse_objects"]).Count == 1) {
            ////////////////////////System.Diagnostics.Debug.WriteLine("execute call 4");
            return this.execute((Dictionary<object, object>)((List<object>)subParseObject["sub_parse_objects"])[0], context, lastParentContextParseLabel, lastParentContext, controlInput, null, lastSetValidContext, preventSetValidContext, preserveContext, inParentContextSetting, lastCurrentFunctionContext, containedAsync);
        }
        return null;
    }

    public object getMutableArrayValueNil(List<object> list, int index) {
        if(list.Count > index) {
            return list[index];
        }
        return null;
    }
}

class PHPScriptFunction : PHPScriptObject {

    public PHPScriptFunction() {

    }

    public string functionName;
    public Dictionary<object, object> closures;
    public bool isAsync;
    public bool allowReturn;
    public string accessFlag;
    public PHPScriptFunction lastCaller;

    public bool invalidateExecutionCompletion;
    //public Dictionary<object, object> 
    public bool? isCallback = null;
    public bool containsCallbacks;

    public bool completedExecution = false;
    public bool toUnset;
    public Dictionary<object, object> inputParameters;
    public Dictionary<object, object> variables;
    public Dictionary<object, object>  classes;
    public List<object> inputParameterKeys;

    public PHPScriptObject classValueClass;
    public PHPScriptEvaluationReference scriptIndexReference;

    public bool hasRunOnce;
    public string debugText;
    public bool hasNoReturn;
    public bool isReturnValueItem = false;
    public object returnResultValue;
    public object nsValue;
    public int retainCountValue;
    public int callCount = -1;

    public object callCallback(List<object> array) {
        List<object> arrayValue = new List<object>();
        if(array != null) {
            foreach(object arrayItem in array) {
            	//////////////System.Diagnostics.Debug.WriteLine("in callcallback: "+this.getInterpretation());
            	object itemValue = this.getInterpretation().makeIntoObjects(arrayItem);
            	//////////////System.Diagnostics.Debug.WriteLine("item value is: "+itemValue);
                arrayValue.Add(itemValue);
            }
        }

        List<object> arr = new List<object>() { arrayValue };

        //return this.callScriptFunctionSub(this, arr, false, false, null);
        return this.callScriptFunctionSub(arr);
    }

    //public void unsetScriptFunction
    public void canUnset() {

    }

    public void unset(PHPScriptFunction context) {

    }

    public PHPScriptFunction copyScriptFunction() {
        PHPScriptFunction result = new PHPScriptFunction();
        result.initArrays();
        result.debugText = this.debugText;
        result.isCallback = this.isCallback;
        result.isAsync = this.isAsync;
        result.parentContext = this.parentContext;
        result.parentContextStrong = this.parentContextStrong;
        result.isArray = this.isArray;
        result.accessFlags = this.accessFlags;
        result.accessFlag = this.accessFlag;
        result.prototype = this.prototype;
        result.identifier = this.identifier;
        result.interpretation = this.getInterpretation();
        result.closures = this.closures;
        result.inputParameters = new Dictionary<object, object>(this.inputParameters);
        result.inputParameterKeys = new List<object>(this.inputParameterKeys);
        result.variables = new Dictionary<object, object>();
        result.allowReturn = this.allowReturn;
        result.classValueClass = this.classValueClass;
        result.scriptIndexReference = this.scriptIndexReference;
        result.classes = this.classes;

        return result;
    }

    public void initArrays() {
        this.isAsync = false;
        this.dictionary = new Dictionary<object, object>();
        this.dictionaryArray = new List<object>();
        this.dictionaryAux = new List<object>();
        this.accessFlags = new Dictionary<object, object>();
        this.closures = new Dictionary<object, object>();
        this.inputParameters = new Dictionary<object, object>();
        this.inputParameterKeys = new List<object>();
        this.variables = new Dictionary<object, object>();
    }

    public void scriptPushArray(object array, object item) {
        array = this.parseInputVariable(array);
        ////////////////////////System.Diagnostics.Debug.WriteLine("script push arrray : "+array);
        PHPScriptObject arrayObject = (PHPScriptObject)array;
        arrayObject.scriptArrayPush(item);
    }

    public void construct2(PHPScriptFunction parentContext, PHPInterpretation evaluation) {
        if(parentContext != null) {
            this.parentContext = parentContext;
            this.parentContextStrong = parentContextStrong;
            this.interpretation = parentContext.interpretation;
        } else {
            this.interpretation = evaluation;
            this.allowReturn = false;
        }
    }

    public void resetThis(PHPScriptObject context=null) {
        if(context == null) {
            context = this.prototype;
        }

        if(context == null) {
            return;
        }
        PHPScriptObject originalContext = context;
        while(context.instanceItem != null) {
            context = context.instanceItem;
        }
        this.setVariableValue("this", context, true, false, true);
        this.setVariableValue("self_instance", originalContext, true, false, true);
    }

    

    public PHPScriptObject getObject() {
        if(this.prototype != null) {
            return this.prototype;
        }
        return null;
    }

    public PHPScriptObject _newInstance(PHPScriptObject obj, List<object> parameterValues) {
        PHPScriptObject _newInstance = obj.getNewInstance(this);
        _newInstance.callClassConstructor(parameterValues);
        return _newInstance;
    }

    public void setInputVariables(List<object> inputs) {
        int index = 0;
        Dictionary<object, object> selfInputParameters = new Dictionary<object, object>(this.inputParameters);
        List<object> selfInputKeys = new List<object>(this.inputParameterKeys);
        this.inputParameterKeys = new List<object>();
        foreach(string inputName in selfInputKeys) {
            object inputDefaultValue = selfInputParameters[inputName];
            if(inputDefaultValue != null) {
                this.setVariableValueInContext(inputName, inputDefaultValue, true, true);
            }

            if(inputs.Count > index && inputs[index] != null) {
                this.setVariableValueInContext(inputName, inputs[index], true, true);
            } else if(inputDefaultValue == null) {
                this.setVariableValueInContext(inputName, null, true, true);
            }
            index++;
        }
    }

    public object getVariableContainer(object name) {
        lock(this.variables) {
            if(this.variables.ContainsKey(name)) {
                object val = this.variables[name];
                if(val != null && !(val is PHPUndefined)) {
                    return this.variables;
                }
            }

            if(this.parentContext != null && this.parentContext != this) {
                object val = this.parentContext.getVariableContainer(name);

                if(val != null && !(val is PHPUndefined)) {
                    return val;
                }
            }
            return null;
        }
    }
    
    public new object getVariableValue(object name) {
        ////System.Diagnostics.Debug.WriteLine("get variable value: "+name+" json: "+JsonConvert.SerializeObject(this.variables.Keys));
        lock(this.variables) {
            if(this.variables.ContainsKey(name)) {
                object val = this.variables[name];
        		////////////////
                if(val != null && !(val is PHPUndefined)) {
                	////System.Diagnostics.Debug.WriteLine("return variable val: "+name);
                    return val;
                }
            }

            if(this.parentContext != null && this.parentContext != this) {
        		//////////////////System.Diagnostics.Debug.WriteLine("get parent variable value: "+name);
                object val = this.parentContext.getVariableValue(name);

                if(val != null && !(val is PHPUndefined)) {
                    return val;
                }
            }
            return null;
        }
    }

    public string getClassIdentifier(PHPScriptObject classValue) {
        foreach(KeyValuePair<object, object> pair in this.classes) {
            if(pair.Value == classValue) {
                return (string)pair.Key;
            }
        }
        return null;
    }

    public PHPScriptObject setClassReference(string identifier) {
    	//////////////////////////////System.Diagnostics.Debug.WriteLine("set ref class: "+identifier);
        if(this.classes != null && this.classes.ContainsKey(identifier)) {
        	//////////////////////////////System.Diagnostics.Debug.WriteLine("return class: "+this.classes[identifier]);
            return (PHPScriptObject)this.classes[identifier];
        }
        if(this.parentContext != null) {
            return this.parentContext.setClassReference(identifier);
        }
        return null;
    }

    public void setClassValue(string identifier, PHPScriptObject scriptObject) {
        if(this.classes == null) {
            this.classes = new Dictionary<object, object>();
        }

        scriptObject.classIdentifierValue = identifier;
        this.classes[identifier] = scriptObject;
    }

    /*Dictionary<object, object> copyClasses(PHPInterpretation interpretation) {

    }*/

    public Dictionary<object, object> getClassesAsValue() {
        return this.classes;
    }

    //public object setVariableValueAddition

    public object setVariableValue(object name, object value, bool defineInContext=false, bool inputParameter=false, bool overrideThis=false) {
        lock(this.variables) {

        	//////System.Diagnostics.Debug.WriteLine("in set variable value: "+name+" value : "+value);
            object stringName = name;
            //string stringName = name.ToString();
            if(!overrideThis && stringName == "this" || (name is PHPVariableReference && ((PHPVariableReference)name).identifier == "this" && !((PHPVariableReference)name).isProperty)) {
                return false;
            }
            string stringValue = null;
            if(value is string) {
                stringValue = (string)value;
                if(stringValue == "[]") {
                    value = new PHPScriptObject();
                    ((PHPScriptObject)value).initArrays();
                    ((PHPScriptObject)value).isArray = true;


	               /* value = new PHPScriptObject();
	                PHPScriptObject valueObj = (PHPScriptObject)value;
	                valueObj.initArrays();
	                valueObj.isArray = true;

	                valueObj.parentContext = (PHPScriptFunction)this;*/
                    //((PHPScriptObject)value).parentContext = true;
                } else if(stringValue == "''") {
                    value = "";
                }
            }
            bool isProperty = false;
            bool valueIsProperty = false;
            if(value is PHPVariableReference) {
                PHPVariableReference variableReferenceValue = (PHPVariableReference)value;
                valueIsProperty = variableReferenceValue.isProperty;
                value = variableReferenceValue.get(null);
            }

            /*if([value isKindOfClass:[NSMutableArray class]] || [value isKindOfClass:[NSArray class]]) {
	            value = [self resolveValueArray:value];
	            ////////////////////////////////NSLog/@"ins resolve value array: %@", value);
	        }*/
	        if(value is List<object>) {
	        	value = this.resolveValueArray(value);
	        }
        	value = this.resolveValueReferenceVariableArray(value);
            //value = this.parseInputVariable(value);

            if(inputParameter) {
                if(name is Dictionary<object, object>) {
                    Dictionary<object, object> nameDictionaryValue = (Dictionary<object, object>)name;
                    this.inputParameterKeys.Add(nameDictionaryValue["identifier"]);
                    this.inputParameters[nameDictionaryValue["identifier"]] = nameDictionaryValue["value"];
                } else if(!(name is PHPVariableReference)) {
                    this.inputParameterKeys.Add(name);
                    this.inputParameters[name] = null;
                } else {
                    this.inputParameterKeys.Add(((PHPVariableReference)name).identifier);
                    this.inputParameters[((PHPVariableReference)name).identifier] = null;
                }
            }
            if(name is PHPVariableReference) {
                isProperty = ((PHPVariableReference)name).isProperty;
            }
            if(value is PHPReturnResult) {
                value = ((PHPReturnResult)value).result;
            }

	        /*if(([name isKindOfClass:[NSMutableArray class]])) {
	            //[self variables][()]
	            if([name isKindOfClass:[NSMutableArray class]]) {
	                name = ((NSMutableArray*)name)[0];
	            }
	        }*/
	        if(name is List<object>) {
	        	name = ((List<object>)name)[0];
	        }

	        /*if([name isKindOfClass:[PHPVariableReference class]]) {
	            PHPVariableReference* nameVariableReference = (PHPVariableReference*)name;
	            [nameVariableReference set:value context:self]; //[self currentObjectFunctionContext] //self
	        } else {
	            NSString* nameString = (NSString*)name;
	            //VIP-BREYTING
	            NSMutableDictionary* variableContainer = (NSMutableDictionary*)[self getVariableContainer:nameString];
	            if(variableContainer == NULL) {
	                [self variables][nameString] = value;
	            } else if([self variables][nameString] != nil || defineInContextBool || ([self parentContext] == nil || [self parentContext] == NULL)) {
	                [self variables][nameString] = value;
	            } else if([self parentContext] != nil || [self parentContext] != NULL) {
	                if([self prototype] == nil) {
	                    [[self parentContext] setVariableValue:name value:value defineInContext:nil inputParameter:nil overrideThis:nil];
	                } else {
	                    [self variables][nameString] = value;
	                }
	            }
	        }
       		return value;*/

            //value = this.parseInputVariable(value);

            if(name is PHPVariableReference) {
            	//////System.Diagnostics.Debug.WriteLine("set var-ref: "+name);
                PHPVariableReference nameVariableReference = (PHPVariableReference)name;
                nameVariableReference.set(value, this);
            } else {
                //string nameString = name.ToString();
                object variableContainer = this.getVariableContainer(name);
                if(variableContainer == null) {
                    this.variables[name] = value;
                } else if(this.variables.ContainsKey(name) || defineInContext || (this.parentContext == null)) {
                    this.variables[name] = value;
                } else if(this.parentContext != null) {
                    if(this.prototype == null) {
                        this.parentContext.setVariableValue(name, value);
                    } else {
                        this.variables[name] = value;
                    }
                }
            }
            return value;
        }
    }


    public object setVariableValueInContext(object name, object value, bool defineInContext=true, bool inputParameter=false, bool overrideThis=false, PHPScriptObject context=null) {
        
        ////////////////////////System.Diagnostics.Debug.WriteLine("set variable in context1: "+name+" - "+value);

        return this.setVariableValue(name, value, defineInContext, false, overrideThis);
    }

    public object prepareSetValue(object value) {
        if(value is string) {
            string valueString = value.ToString();
            if(valueString == "[]") {
                value = new PHPScriptObject();
                ((PHPScriptObject)value).initArrays();
            } else if(valueString == "''") {
                value = "";
            }
        }
        value = this.parseInputVariable(value);
        return value;
    }

    public object getDictionaryValue(object name, bool returnReference=false, bool createIfNotExists=true, PHPScriptObject context=null) {
    	////////////////System.Diagnostics.Debug.WriteLine("get dict value: "+name);
        if(this.dictionary.ContainsKey(name)) {
            if(returnReference) {
                PHPVariableReference reference = new PHPVariableReference();
                reference.construct(name, this, true);
                return reference;
            }
            return this.dictionary[name];
        }
        if(createIfNotExists) {
            this.dictionary[name] = new PHPUndefined();
            if(returnReference) {
                PHPVariableReference reference = new PHPVariableReference();
                reference.construct(name, this, true);
                return reference;
            }
            return this.dictionary[name];
        }
        return null;
    }

    public void setDictionaryValue(object name, object value, PHPScriptFunction context) {
    	//////////System.Diagnostics.Debug.WriteLine("set func dictionary value: "+name+" "+value);
        if(value == null) {
            value = name;
            value = this.parseInputVariable(value);
            this.dictionaryAux.Add(value);
            return;
        }
        value = this.parseInputVariable(value);
        value = this.prepareSetValue(value);
        this.dictionary[name] = value;
    }

    public void setFunctionIdentifier(string identifier) {
        this.identifier = identifier;
        this.classValueClass.setDictionaryValue(identifier, this, null);
    }

    public PHPScriptObject createScriptObject(object values) {
        PHPScriptObject script_object = new PHPScriptObject();
        script_object.initArrays();
        script_object.construct(this);
        return script_object;
    }

    public PHPScriptObject createScriptObjectClass(object values) {
        PHPScriptObject result = new PHPScriptObject();
        result.initArrays();
        return result;
    }

    public /*new*/ PHPScriptFunction createScriptFunctionAlt(bool isAsync=false) {
        PHPScriptFunction scriptFunction = new PHPScriptFunction();
        scriptFunction.initArrays();
        scriptFunction.isAsync = isAsync;

        scriptFunction.construct((PHPScriptFunction)this);
        //////////////////System.Diagnostics.Debug.WriteLine("create script function is async: "+isAsync+" this : "+this);
        return scriptFunction;
    }

    public PHPScriptObject createScriptObjectFunc(object values) {
        PHPScriptObject scriptObject = new PHPScriptObject();
        scriptObject.initArrays();
        scriptObject.construct(this);
        return scriptObject;
    }

    public PHPScriptFunction setParametersNamed(object identifier, object parameters, object scriptIndexReference) {
        //////////////////////////////System.Diagnostics.Debug.WriteLine("setParametersNamed : "+identifier+" "+parameters+" "+scriptIndexReference);
        if(parameters is PHPScriptEvaluationReference) {
            this.scriptIndexReference = (PHPScriptEvaluationReference)parameters;
            parameters = new List<object>();
        } else {
            this.scriptIndexReference = (PHPScriptEvaluationReference)scriptIndexReference;
        }
        this.setFunctionIdentifier((string)identifier);
        this.functionName = (string)identifier;


        List<object> parametersArray = (List<object>)parameters;
        foreach(object parameter in parametersArray) {
            this.setVariableValue(parameter, null, true, true);
        }
        return this;
    }

    public PHPScriptFunction setParameters(List<object> parameters, PHPScriptEvaluationReference scriptIndexReference) {
        this.scriptIndexReference = scriptIndexReference;
        foreach(object parameter in parameters) {
            this.setVariableValue(parameter, null, true, true);
        }
        return this;
    }

    public List<object> numericDivision(object valueA, object valueB) {
        
        PHPValuesOperator valueOperator = new PHPValuesOperator();
        valueOperator.operatorValue = "/";
        if(!(valueB is List<object>)) {
            return new List<object>() { 
                valueA, 
                valueOperator,
                valueB
            };
        } else {
            List<object> result = new List<object>() { 
                valueA, 
                valueOperator
            };
            result.AddRange((IEnumerable<object>)valueB);
            return result;
        }
        return null;
    }
    public List<object> numericSubtraction(object valueA, object valueB) {
        
        PHPValuesOperator valueOperator = new PHPValuesOperator();
        valueOperator.operatorValue = "-";
        if(!(valueB is List<object>)) {
            return new List<object>() { 
                valueA, 
                valueOperator,
                valueB
            };
        } else {
            List<object> result = new List<object>() { 
                valueA, 
                valueOperator
            };
            result.AddRange((IEnumerable<object>)valueB);
            return result;
        }
        return null;
    }
    public List<object> numericMultiplication(object valueA, object valueB) {
        
        PHPValuesOperator valueOperator = new PHPValuesOperator();
        valueOperator.operatorValue = "*";
        if(!(valueB is List<object>)) {
            return new List<object>() { 
                valueA, 
                valueOperator,
                valueB
            };
        } else {
            List<object> result = new List<object>() { 
                valueA, 
                valueOperator
            };
            result.AddRange((IEnumerable<object>)valueB);
            return result;
        }
        return null;
    }
    public List<object> numericModulo(object valueA, object valueB) {
        
        PHPValuesOperator valueOperator = new PHPValuesOperator();
        valueOperator.operatorValue = "%";
        if(!(valueB is List<object>)) {
            return new List<object>() { 
                valueA, 
                valueOperator,
                valueB
            };
        } else {
            List<object> result = new List<object>() { 
                valueA, 
                valueOperator
            };
            result.AddRange((IEnumerable<object>)valueB);
            return result;
        }
        return null;
    }
    public List<object> numericAddition(object valueA, object valueB) {
        PHPValuesOperator valueOperator = new PHPValuesOperator();
        valueOperator.operatorValue = "+";
        if(!(valueB is List<object>)) {
            return new List<object>() { 
                valueA, 
                valueOperator,
                valueB
            };
        } else {
            List<object> result = new List<object>() { 
                valueA, 
                valueOperator
            };
            result.AddRange((IEnumerable<object>)valueB);
            return result;
        }
        return null;
    }
    public List<object> stringAddition(object valueA, object valueB) {
        //////////////////////////////System.Diagnostics.Debug.WriteLine("in stringaddition: "+valueA+" - "+valueB);
        PHPValuesOperator valueOperator = new PHPValuesOperator();
        valueOperator.operatorValue = ".";
        if(!(valueB is List<object>)) {
            //////////////////////////////System.Diagnostics.Debug.WriteLine("return arr: ");
            return new List<object>() { 
                valueA, 
                valueOperator,
                valueB
            };
        } else {
            List<object> result = new List<object>() { 
                valueA, 
                valueOperator
            };
            result.AddRange((IEnumerable<object>)valueB);
            return result;
        }
        return null;
    }

    public object setParanthesis(object value) {
        if(value is PHPScriptObject || value is PHPScriptFunction || value is PHPScriptEvaluationReference) {
            return value;
        }
        return new List<object>() { value };
    }

    public object returnFollowingResult(object value) {
        return value;
    }

    public object returnValueResult(object value) {
        if(value is PHPVariableReference) {
            value = ((PHPVariableReference)value).get(null);
        }
        //if(value is List<object>) {
            value = this.parseInputVariable(value);
        //}
        //////////////////////////////System.Diagnostics.Debug.WriteLine("set returnValueResult: "+value);
        PHPReturnResult returnResult = new PHPReturnResult();
        returnResult.construct(value);
        return returnResult;
    }

    public void setClosure(Func<List<object>, PHPScriptFunction, object> func, string name) {
        this.closures[name] = func;
    }

    public object callScriptFunction(PHPVariableReference identifierReference, List<object> parameterValues, bool awaited=false, bool returnObject=false) {
        object identifierName = identifierReference.identifier;
        PHPScriptFunction scriptFunction = (PHPScriptFunction)this.getVariableValue(identifierName);
        return this.callScriptFunctionSub(scriptFunction, parameterValues, awaited, returnObject, null);
    }
    //public object callClosure

    /*public void setClassReference(string identifier) {
        this.classReference = identifier;
    }*/
}

class PHPScriptObject : NSObject {
    public string classIdentifierValue;
    public bool reverseIteratorSet;
    public int nextKey;
    public PHPScriptFunction parentContext;
    public PHPScriptFunction parentContextStrong;
    public bool isArray = false;
    public List<object> dictionaryKeys;
    public Dictionary<object, object> dictionary;
    public List<object> dictionaryArray;
    public List<object> dictionaryAux;
    public PHPScriptObject prototype;
    public Dictionary<object, object> accessFlags;
    public PHPScriptObject originalClass;
    public string identifier;
    public PHPInterpretation interpretation;
    public PHPScriptObject parentClass;
    public PHPInterpretation interpretationForOjbect;
    public PHPScriptFunction currentFunctionContext;
    public bool globalObject;
    public PHPScriptObject instanceItem;

    public void sort(/*Func<object, object, int>*/ PHPScriptFunction callback) {
        //System.Diagnostics.Debug.WriteLine("sort :"+callback+" parent "+callback.parentContext);
        List<object> items = new List<object>(this.dictionaryArray);
        //toJSON jsonInstance = new toJONS();
        //List<object> items = jsonInstance.toJSON(this);
        items.Sort((a, b) => {
            /*int callback_result = (int)callback.callCallback(new List<object>() { a, b });
            ////System.Diagnostics.Debug.WriteLine("sort res: "+callback_result);
            return callback_result;*/
            ////System.Diagnostics.Debug.WriteLine("in sort outer: "+callback);
            a = this.parseInputVariable(a);
            b = this.parseInputVariable(b);
            List<object> parameterValuesValue = new List<object>();
            List<object> parameterValuesValueSub = new List<object>();
            parameterValuesValueSub.Add(a);
            parameterValuesValueSub.Add(b);
            parameterValuesValue.Add(parameterValuesValueSub);
            PHPReturnResult res = (PHPReturnResult)this.callScriptFunctionSub(callback, parameterValuesValue);
            //System.Diagnostics.Debug.WriteLine("call return res: "+res.get());
            return Convert.ToInt32(res.get());
            /* NSMutableArray* parameterValuesValue = [[NSMutableArray alloc] init];
	        NSMutableArray* parameterValuesValueSub = [[NSMutableArray alloc] init];
	        [parameterValuesValueSub addObject:a];
	        [parameterValuesValueSub addObject:b];
	        [parameterValuesValue addObject:parameterValuesValueSub];
	        //(NSMutableArray*)@[@[a, b]]
	        NSObject* result = [self callScriptFunctionSub:callback parameterValues:parameterValuesValue awaited:nil returnObject:nil interpretation:nil];
	        result = [self parseInputVariable:result];*/
            //return callback(a,b);
        });
        items.Reverse();
        this.dictionaryArray = items;
    }

    public void removeItem(object item) {
        object dict = this.getDictionary();
        lock(dict) {
            if(dict is List<object>) {
                List<object> arr = (List<object>)dict;
                arr.Remove(item);
            } else {
                Dictionary<object, object> dictArr = (Dictionary<object, object>)dict;
                List<object> keys = new List<object>();
                foreach(KeyValuePair<object, object> pair in dictArr) {
                    if(pair.Value == item) {
                        keys.Add(pair.Key);
                    }
                }
                foreach(object key in keys) {
                    dictArr.Remove(key);
                }
                //dictArr.all
            }
        }
    }

    public void initArrays() {
        this.reverseIteratorSet = false;
        this.nextKey = 0;
        this.dictionary = new Dictionary<object, object>();
        this.dictionaryKeys = new List<object>();
        this.dictionaryArray = new List<object>();
        this.dictionaryAux = new List<object>();
        this.accessFlags = new Dictionary<object, object>();
        
    }

    public PHPScriptFunction currentObjectFunctionContext() {
        if(this.currentFunctionContext != null) {
            return this.currentFunctionContext;
        }
        return this.parentContext;
    }

    public PHPInterpretation getInterpretation() {
    	if(this.interpretation != null) {
	        return this.interpretation;
	    }
	    if(this.parentContext != null) {
	    	return this.parentContext.getInterpretation();
	    }
	    return null;
    }

    public void setAccessFlag(object accessflag, object property) {
        ////////////////////////////System.Diagnostics.Debug.WriteLine("accessflag : "+accessflag+" - "+property);
        if(property is PHPVariableReference) {
            //NSString numberValue = (NSString)accessflag;
            string numberValue = (string)accessflag;
            PHPVariableReference referenceValue = (PHPVariableReference)property;
            this.accessFlags[referenceValue.identifier] = numberValue;
            ////////////////////////////System.Diagnostics.Debug.WriteLine("property value : "+((PHPVariableReference)property).get(null));
        } else {
            PHPScriptFunction objectValue = (PHPScriptFunction)property;
            objectValue.accessFlag = (string)accessflag;
        }
    }

    public void callClassConstructor(List<object> parameterValues) {
        if(parameterValues == null) {
            parameterValues = new List<object>();
        }
        ////////////////System.Diagnostics.Debug.WriteLine("get constructor is : ");
        object constructor = this.getDictionaryValue("__construct", false, false, null);
        ////////////////System.Diagnostics.Debug.WriteLine("constructor is : "+constructor);
        bool cancel = false;
        if(constructor is PHPVariableReference) {
            if(((PHPVariableReference)constructor).get(null) is PHPUndefined) {
                cancel = true;
            }
        }
        if(constructor != null && !cancel) {
            this.callScriptFunctionSub(constructor, parameterValues);
        }
    }

    public string getClassIdentifierFromClass() {
        return this.originalClass.classIdentifierValue;
    }

    public bool instanceOf(string className) {
        string stringValue = className;
        if(this.getClassIdentifierFromClass() == stringValue) {
            return true;
        }
        if(this.prototype != null) {
            return this.prototype.instanceOf(className);
        }
        return false;
    }

    public PHPScriptObject getNewInstance(PHPScriptFunction context) {
        List<object> functions = new List<object>();
        PHPScriptObject prototypeClone = null;
        if(this.prototype != null) {
            prototypeClone = this.prototype.getNewInstance(context);
            ////////////////System.Diagnostics.Debug.WriteLine("prototypeclone : "+prototypeClone+" "+context.debugText);
        }
        PHPScriptObject selfClone = this.copyScriptObject();
        selfClone.parentContextStrong = context;
        selfClone.interpretation = context.getInterpretation();
        if(prototypeClone != null) {
            selfClone.prototype = prototypeClone;
            prototypeClone.instanceItem = selfClone;
        } else {
            selfClone.prototype = null;
        }

        //////////////////////////////System.Diagnostics.Debug.WriteLine("in new instance");

        Dictionary<object, object> newDictionary = new Dictionary<object, object>();
        foreach(KeyValuePair<object, object> keyPair in selfClone.dictionary) {
            object key = keyPair.Key;
            object dictionaryValue = keyPair.Value;//selfClone.dictionary[key];
            //////////////////////////////System.Diagnostics.Debug.WriteLine("pair : "+key+" - "+dictionaryValue);
            if(dictionaryValue is PHPScriptFunction) {
                dictionaryValue = ((PHPScriptFunction)dictionaryValue).copyScriptFunction();
                ((PHPScriptFunction)dictionaryValue).prototype = selfClone;
                //////////////////////////////System.Diagnostics.Debug.WriteLine("set prototype: "+selfClone+ " key: "+key);
                ((PHPScriptFunction)dictionaryValue).parentContext = context;
                ((PHPScriptFunction)dictionaryValue).parentContextStrong = context;
            } else if(dictionaryValue is PHPScriptObject) {
                dictionaryValue = ((PHPScriptObject)dictionaryValue).copyScriptObject();
            }/* else if(dictionaryValue is string) {
                //dictionaryValue = new NSString(((NSString)dictionaryValue).string_value);
            } else if(dictionaryValue is NSNumber) {
                dictionaryValue = new NSNumber((NSNumber)dictionaryValue);
            }*/

            newDictionary[key] = dictionaryValue;
        }
        selfClone.dictionary = newDictionary;
        selfClone.originalClass = this;
        return selfClone;
    }

    public PHPScriptObject copyScriptObject() {
        PHPScriptObject result = new PHPScriptObject();
        result.parentContext = this.parentContext;
        result.isArray = this.isArray;
        result.dictionaryKeys = new List<object>(this.dictionaryKeys);
        result.dictionary = new Dictionary<object, object>(this.dictionary);
        result.dictionaryArray = new List<object>(this.dictionaryArray);
        result.dictionaryAux = new List<object>(this.dictionaryAux);

        result.accessFlags = new Dictionary<object, object>(this.accessFlags);
        result.identifier = this.identifier;
        result.interpretation = this.interpretation;

        return result;
    }

    public PHPScriptObject setClassReference(string identifier) {
        if(this.parentContext != null) {
            return this.parentContext.setClassReference(identifier);
        }
        return null;
    }

    public PHPScriptFunction createNamedScriptFunction(PHPScriptObject classValue) {
        PHPScriptFunction scriptFunction = new PHPScriptFunction();
        scriptFunction.initArrays();
        scriptFunction.parentContext = this.parentContext;
        scriptFunction.parentContextStrong = this.parentContext;
        scriptFunction.classValueClass = this;
        return scriptFunction;
    }

    public object setVariableValueInContext(object name, object value, bool defineInContext=true, bool inputParameter=false, bool overrideThis=false, PHPScriptFunction context=null) {
        if(value == null) {
            value = new PHPUndefined();
        }
       	////////////System.Diagnostics.Debug.WriteLine("set variable in context dict : "+name+" - "+value);
        this.setDictionaryValue(name, value, context);
        return this.setVariableReference(name); // null, null
    }

    /*public NSNumber return_boolean_true() {
        NSNumber returnValue = new NSNumber(true);
        return returnValue;
    }

    public NSNumber return_boolean_false() {
        NSNumber returnValue = new NSNumber(false);
        return returnValue;
    }*/

    public void setObjectIdentifier(object identifier) {
        this.parentContext.setClassValue(Convert.ToString(identifier), this);
    }

    public void construct(PHPScriptFunction parentContext) {
        if(parentContext == null) {
            parentContext = this.parentContext;
        }

        if(this is PHPScriptFunction) {
            this.parentContextStrong = parentContext;
        }
        this.parentContext = parentContext;
    }

    public void setClassExtends(PHPScriptObject classValue) {
        this.parentClass = classValue;
    }

    public PHPVariableReference getArrayValueContextReference(object index, bool returnReference=true) {
        /*if(returnReference == null) {
            returnReference = new NSNumber(true);
        }*/
        //////System.Diagnostics.Debug.WriteLine("getArrayValueContextReference: "+index+" returnRef: "+returnReference);
        index = this.parseInputVariable(index);

        PHPVariableReference result = (PHPVariableReference)this.getArrayValueContext(index, returnReference, null);
        result.ignoreSetContext = true;
        //////////////////////////System.Diagnostics.Debug.WriteLine("getArrayValueContextReference res: "+result);
        //////////////////////////System.Diagnostics.Debug.WriteLine("getArrayValueContextReference resvalue: "+result.get(null));
        return result;
    }

    public object getArrayValueContext(object index, bool returnReference, PHPScriptObject context) {
      	if(index is PHPVariableReference) {
            index = ((PHPVariableReference)index).get(context);
        }
        //////////////////////////System.Diagnostics.Debug.WriteLine("getArrayValueContext: "+index);
        if(index is List<object>) {
            index = this.resolveValueArray(index);
        }

        index = this.parseInputVariable(index);
        //////////////////////////System.Diagnostics.Debug.WriteLine("getArrayValueContext: "+index);
        object value = this.getDictionaryValue(index, returnReference, true, context);
        //////////////////////////System.Diagnostics.Debug.WriteLine("getArrayValueContextva: "+value);
        /*if(value is PHPScriptFunction) {
            ((PHPScriptFunction)value).resetThis(this);
        }*/
        return value;
    }

    public string getTypeOf(object value) {
        value = this.parseInputVariable(value);
        if(value is PHPUndefined) {
            return "undefined";
        }
        string stringValue = this.makeIntoString(value);
        if(value == null) {
            return "undefined";
        }
        if(value is PHPScriptObject) {
            if(value is PHPScriptFunction) {
                return "function";
            } else {
                if(!((PHPScriptObject)value).isArray) {
                    return "array";
                }
                return "object";
            }
        }
        if(value is PHPNull) {
            return "NULL";
        }
        if(value is int || value is double) {
            return "number";
        }
        if(value is string) {
            return "string";
        }
        return "undefined";
    }

    public void setArray(bool isArray) {
        this.isArray = isArray;
    }

    public void scriptArrayUnshift(object item) {
        item = this.parseInputVariable(item);
        this.dictionaryArray.Insert(0, item);
    }

    public object scriptArrayShift() {
        int index = this.dictionaryArray.Count - 1;
        if(index >= 0) {
            object returnItem = this.dictionaryArray[index];
            this.dictionaryArray.RemoveAt(index);
            return returnItem;
        }
        return new PHPUndefined();
    }

    public PHPScriptObject getParentContext() {
        if(this is PHPScriptFunction) {
            return this;
        }
        return this.parentContext;
    }

    public void scriptArrayPush(object item) {
    	////////////////////////System.Diagnostics.Debug.WriteLine("scriptArrayPush: "+this.dictionaryArray+" - "+item);
        lock(this.dictionaryArray) {
            item = this.parseInputVariable(item);
            this.dictionaryArray.Add(item);
            this.isArray = true;
        }
    }

    public object resolveValueReferenceVariableArray(object item) {
        if(item is PHPReturnResult) {
            item = ((PHPReturnResult)item).get();
        }
        if(item is PHPVariableReference) {
            item = ((PHPVariableReference)item).get(null);
        }
        if(item is List<object>) {
            item = this.resolveValueArray(item);
        }
        return item;
    }

    public object scriptArrayPop() {
        int index = 0;
        object returnItem = this.dictionaryArray[index];
        this.dictionaryArray.RemoveAt(index);
        return returnItem;
    }

    public List<object> reverseArray() {
        //////////////////////////////System.Diagnostics.Debug.WriteLine("reverseArray called");
        List<object> dictionaryAuxSetValues = new List<object>();
        foreach(object item in this.dictionaryAux) {
            object itemItem = item;
            if(itemItem is string && ((string)item) == "[]") {
                itemItem = new PHPScriptObject();
                PHPScriptObject itemObj = (PHPScriptObject)itemItem;
                itemObj.initArrays();
                itemObj.parentContext = this.parentContext;
                itemObj.isArray = true;
            }
            dictionaryAuxSetValues.Add(itemItem);
        }
        this.dictionaryAux = dictionaryAuxSetValues;

        if(this.dictionaryAux.Count > 0) {
            object firstValue = this.dictionaryAux[0];
            this.dictionaryAux.RemoveAt(0);
            List<object> reverseKeys = new List<object>();
            List<object> reverseDictionary = new List<object>();
            List<object> revDict = this.dictionaryAux.ToList<object>();
            revDict.Reverse();
            foreach(object item in revDict) {
                object itemItem = item;
                reverseDictionary.Add(itemItem);
            }

            List<object> setDictionaryArrayValue = new List<object>();
            setDictionaryArrayValue.Add(firstValue);
            this.dictionaryArray = setDictionaryArrayValue;
            this.dictionaryArray.AddRange(reverseDictionary);
            this.isArray = true;
        } else if(this.dictionaryArray.Count > 0) {
            List<object> keys = new List<object>();
            int index = 0;
            foreach(object item in this.dictionaryArray) {
                keys.Add(index);
                index++;
            }

            if(keys.Count > 0) {
                int firstKey = (int)keys[0];
                object firstValue = this.dictionaryArray[0];
                this.dictionaryArray.RemoveAt(0);
                keys.RemoveAt(0);
                List<object> keysReversed = new List<object>();
                List<object> valuesReversed = new List<object>();
                List<object> keysRev = new List<object>(keys);
                keysRev.Reverse();
                foreach(object reverseItem in keysRev) {
                    keysReversed.Add(reverseItem);
                }
                List<object> dictRev = new List<object>(this.dictionaryArray);
                dictRev.Reverse();
                foreach(object reverseItem in dictRev) {
                    object itemItem = reverseItem;
                    valuesReversed.Add(itemItem);
                }

                this.dictionaryArray = new List<object>();
                this.dictionaryArray.Add(firstValue);
                foreach(object value in valuesReversed) {
                    this.dictionaryArray.Add(value);
                }
            }
        } else if(this.dictionaryKeys.Count > 0) {
            List<object> keys = this.dictionaryKeys;
            object firstKey = keys[0];
            keys.RemoveAt(0);

            List<object> returnKeys = new List<object>();
            returnKeys.Add(firstKey);

            List<object> keysRev = new List<object>(keys);
            keysRev.Reverse();

            foreach(object key in keysRev) {
                returnKeys.Add(key);
            }
            this.dictionaryKeys = returnKeys;
        }
        return this.dictionaryArray;
    }

    public List<object> getKeys() {
        if(this.isArray) {
            return null;
        }
        return this.dictionaryKeys;
    }

    public bool isUndefined(object value) {
        if(value is PHPUndefined) {
            return true;
        } else if(value == null) {
            return true;
        }
        return false;
    }

    public List<object> getDictionaryValues() {
        if(this.isArray) {
            return this.dictionaryArray;
        }
        List<object> keys = this.getKeys();
        List<object> result = new List<object>();
        foreach(object key in keys) {
            if(this.dictionary.ContainsKey(key) && this.dictionary[key] != null) {
                result.Add(this.dictionary[key]);
            }
        }
        return result;
    }

    public object getArrayVariableValue(PHPVariableReference identifier, object index) {
        index = this.parseInputVariable(index);
        object identifierName = identifier.identifier;
        PHPScriptObject array = (PHPScriptObject)this.getVariableValue(identifierName);
        object value = array.getDictionaryValue(index);
        return value;
    }

    public object getArrayValue(PHPVariableReference identifier, object index) {
        index = this.parseInputVariable(index);
        object identifierName = identifier.identifier;
        PHPScriptObject array = (PHPScriptObject)this.getDictionaryValue(identifierName);
        object value = array.getDictionaryValue(index);
        return value;
    }

    public object getVariableValue(object name) {
        if(this.isArray) {
            if(name is int) {
                return this.dictionaryArray[((int)name)];
            }
        }
        if(this.dictionary.ContainsKey(name)) {
            return this.dictionary[name];
        }

        return null;
    }

    public bool isPrototypeOf(PHPScriptObject prototypeCheck) {
        PHPScriptObject prototype = this;
        while(prototype != null) {
            if(prototype == prototypeCheck) {
                return true;
            }
            prototype = prototype.prototype;
        }
        prototype = this;
        while(prototypeCheck != null) {
            if(prototype == prototypeCheck) {
                return true;
            }
            prototypeCheck = prototypeCheck.prototype;
        }
        return false;
    }

    public object getDictionaryValue(object name, bool returnReference=false, bool createIfNotExists=true, PHPScriptObject context=null) {
        lock(this) {
            PHPScriptObject caller = this;
            if(context != null) {
                caller = context;
            }

            if(caller is PHPScriptFunction) {
                caller = caller.prototype;
            }

            /*if(name is PHPVariableReference) {
            	name = ((PHPVariableReference)name).get();
            }*/

            if(name is int && this.isArray) {
                int index = ((int)name);
                if(returnReference) {
                    PHPVariableReference reference = new PHPVariableReference();
                    reference.construct(name, this, true);
                    return reference;
                }
                if(this.dictionaryArray.Count <= index) {
                    return new PHPUndefined();
                }
                //////////////////////////////System.Diagnostics.Debug.WriteLine("return value: "+this.dictionaryArray[index]);
                //////////////////////////////System.Diagnostics.Debug.WriteLine("return value: "+JsonConvert.SerializeObject(this.dictionaryArray));
                return this.dictionaryArray[index];
            }

            if(name is int) {
                name = this.makeIntoString(name);
            }
            object nameString = name; //.ToString();
            /*if(nameString.isEqualToString(new NSString("is_array")) {
                if(this.isArray) {
                    return true;
                }
                return false;
            }*/
            if(this.isArray) {
                if(nameString.ToString() == "length") {
                    return this.dictionaryArray.Count;
                }
            } else {
            	if(nameString.ToString() == "parent") {
            		if(this is PHPScriptFunction) {
            			return this.prototype.prototype;
            		}
            		return this.prototype;
           		} else if(nameString.ToString() == "length") {
                    return this.dictionary.Count;
                }
            }

            if(this.dictionary.ContainsKey(name)) {
                if(returnReference) {
                	//////////////////////////System.Diagnostics.Debug.WriteLine("return ref value: "+name);
                    PHPVariableReference reference = new PHPVariableReference();
                    reference.construct(name, this, true);
                	//////////////////////////System.Diagnostics.Debug.WriteLine("return ref reference: "+reference);

                    return reference;
                }

                string accessFlag = "public";
            	//////////////////System.Diagnostics.Debug.WriteLine("accessflag: "+accessFlag+" "+JsonConvert.SerializeObject(this.accessFlags));
                if(this.accessFlags.ContainsKey(nameString)) {
                    accessFlag = (string)this.accessFlags[nameString];
                }

                object result = this.dictionary[nameString];
                if(this.globalObject) {
                	return result; 
                }

                if(result is PHPScriptFunction) {
	            	accessFlag = ((PHPScriptFunction)result).accessFlag;
                }
                if(accessFlag != null) {
                    accessFlag = accessFlag.Replace(" ", "");
                }
                bool isPrototype = false;
                if(accessFlag != "public") {
	                isPrototype = caller.isPrototypeOf(this);
	                if(!isPrototype) {
	                    isPrototype = this.isPrototypeOf(caller);
	                }
	            }
            	//////////////////System.Diagnostics.Debug.WriteLine("accessflag: "+accessFlag);

                if(accessFlag == "private") {
                    if(isPrototype) {
                        return result;
                    }
                    return new PHPUndefined();
                }
            	//////////////////System.Diagnostics.Debug.WriteLine("accessflag: "+accessFlag);

                if(true || accessFlag == null || caller == this || (accessFlag == "protected" && isPrototype) || accessFlag == "public") {
                	//////////////////System.Diagnostics.Debug.WriteLine("res is : "+result);
                    return result;
                }
                return new PHPUndefined();
            } else if(createIfNotExists && returnReference) {

                //this.dictionary[nameString] = new PHPUndefined();
                PHPVariableReference reference = new PHPVariableReference();
                reference.construct(nameString, this, true);
                return reference;
            } else if(this.prototype != null) {
                string accessFlag = "public";
                if(this.prototype.accessFlags.ContainsKey(nameString)) {//[nameString] != null) {
                    accessFlag = this.prototype.accessFlags[nameString].ToString();
                }
                if(accessFlag != "private") {
                    object result = this.prototype.getDictionaryValue(name, returnReference, false, context);
                    if(result != null) {
                        return result;
                    }
                } else {
                    return new PHPUndefined();
                }
            }

            if(true || createIfNotExists) {
            	if(!this.dictionary.ContainsKey(nameString)) {
               		this.dictionary[nameString] = new PHPUndefined();
               	}
                if(true || returnReference) {
                    PHPVariableReference reference = new PHPVariableReference();
                    reference.construct(nameString, this, true);
                    return reference;
                }
                return this.dictionary[nameString];
            }
        }
        return null;
    }

    public object getDictionary() {
        if(this.isArray) {
            return this.dictionaryArray;
        }
        return this.dictionary;
    }

    public void setDictionaryValue(object name, object value, PHPScriptFunction context) {
    	//////////System.Diagnostics.Debug.WriteLine("set dictionary value obj: "+name+" value: "+value);
        if(value is PHPScriptFunction) {
            object nameParsed = name;
            if(name is PHPVariableReference) {
                nameParsed = ((PHPVariableReference)name).identifier;
            }

            PHPScriptFunction valueFunction = (PHPScriptFunction)value;
            if(valueFunction.identifier == null) {
                valueFunction.identifier = (string)nameParsed;
            }
        }

        if(value == null) {
            value = name;
            value = this.resolveValueArray(value);
            this.dictionaryAux.Add(value);
            return;
        }

        /*name = this.resolveValueReferenceVariableArray(name);
        name = this.resolveValueReferenceVariableArray(name);
        boo*/

        name = this.parseInputVariable(name);
        bool isNumeric = false;
        if(name is int) {
            isNumeric = true;
        }

        if(isNumeric && this.isArray) {
            this.dictionaryArray[(int)name] = value;
            return;
        }

        if(this.prototype != null) {
            PHPVariableReference varReference = (PHPVariableReference)this.prototype.getDictionaryValue(name, true, false, context);
            if(varReference != null && !(varReference.get(context) is PHPUndefined)) {
                object getValue = varReference.get(context);
                bool preventSetVarReference = false;
                if(getValue != null) {
                    if(getValue is PHPScriptFunction && value is PHPScriptFunction) {
                        preventSetVarReference = true;
                    }
                }
                if(!preventSetVarReference) {
                    varReference.set(value, context);
                    return;
                }
            }
        }
        value = this.parseInputVariable(value);

        /*if(value is PHPVariableReference) {

        }*/

        if(value is string) {
            string valueString = (string)value;
            if(valueString == "[]") {
            	////////////////System.Diagnostics.Debug.WriteLine("set [] as value: "+value);
                value = new PHPScriptObject();
                PHPScriptObject valueObj = (PHPScriptObject)value;
                valueObj.initArrays();
                valueObj.isArray = true;

                //valueObj.parentContext = (PHPScriptFunction)this;
            } else if(valueString == "''") {
                value = "";
            }
        }
        if(name == null && value is PHPScriptObject) {
            name = ((PHPScriptObject)value).identifier;
        }
        this.isArray = false;
        this.addDictionaryKeysItem(name);
        if(name is string) {
            this.dictionary[((string)name)] = value;
        } else {
            this.dictionary[name] = value;
        }

        ////////////////////////////System.Diagnostics.Debug.WriteLine("ended set dict value");
    }

    public void setDictionaryValue(object name, object value) {

        if(value == null) {
            value = name;
            value = this.parseInputVariable(value);
            this.dictionaryAux.Add(value);
            return;
        }
        value = this.parseInputVariable(value);
        /*if(name is NSString) {
            this.dictionary[((NSString)name).string_value] = value;
        } else {*/
       	this.addDictionaryKeysItem(name);
        this.dictionary[name] = value;
        

    }

    public void addDictionaryKeysItem(object item) {
        if(item is string) {
            item = ((string)item);
        }
        if(!this.dictionaryKeys.Contains(item)) {
            this.dictionaryKeys.Add(item);
        }
    }

    public PHPScriptFunction createScriptFunction(bool isAsync=false) {
        PHPScriptFunction scriptFunction = new PHPScriptFunction();
        scriptFunction.initArrays();
        scriptFunction.construct(this.parentContext);
        //////////////////////////////System.Diagnostics.Debug.WriteLine("create script function is async2: "+isAsync);
        scriptFunction.isAsync = isAsync;
        scriptFunction.prototype = this;
        return scriptFunction;
    }

    public PHPVariableReference setPropertyReference(object identifier) {
        PHPVariableReference variableReference = new PHPVariableReference();
        variableReference.construct(identifier, this, true);
        return variableReference;
    }

    public object returnPropResult(object value) {
        if(value == null) {

        }
        if(value is List<object>) {
            List<object> valueArray = (List<object>)value;
            value = valueArray[valueArray.Count-1];
        }
        if(value is PHPVariableReference) {
            value = ((PHPVariableReference)value).get(null);
        }
        return value;
    }

    public object returnLastPropResult(object value) {
        if(value != null) {
            List<object> valueArray = ((List<object>)value);
            value = valueArray[valueArray.Count-1];
            return value;
        }
        return value;
    }

    public PHPVariableReference setVariableReferenceIgnore(object identifier, bool ignore=true) {
        PHPVariableReference result = this.setVariableReference(identifier, ignore);
        return result;
    }

    public PHPVariableReference setVariableReference(object identifier, bool ignore=false, bool defineInContext=false) {
        //string identifierStr = this.makeIntoString(identifier);
        PHPVariableReference result = new PHPVariableReference();
        //////////////////////////////System.Diagnostics.Debug.WriteLine("result from setvariablereference : "+this);
        result.construct(identifier, this, false, defineInContext, ignore);
        //////////////////////////////System.Diagnostics.Debug.WriteLine("result from setvariablereference : "+result);
        return result;
    }

    public object returnParameterInput(object values) {
        return values;
    }

    public object returnParameterInputIdentifierValue(object valueIdentifier, object value) {
        if(value == null) {
                return valueIdentifier;
        }
        return new Dictionary<object, object>(){ 
            {
                "identifier", valueIdentifier
            },
            {
                "value", value
            }
        };
    }

    public object collectParameters(List<object> values) {
        /*List<object> valuesList = null;
        if(values is List<object>)) {
            valuesList = (List<object>)values;
        }*/
        /*if(values == null) {
            return values;
        }*/
        //////////////////////////////System.Diagnostics.Debug.WriteLine(values.Count);
        foreach(object debugItem in values) {
            
            //////////////////////////////System.Diagnostics.Debug.WriteLine(debugItem);
            if(debugItem is List<object>) {
                List<object> debugList = (List<object>)debugItem;
                //////////////////////////////System.Diagnostics.Debug.WriteLine(debugList.Count);
                foreach(object debugItems in debugList) {
                    
                    //////////////////////////////System.Diagnostics.Debug.WriteLine(debugItems);
                }
            }
        }
        if(values.Count == 1) {
            return values;
        }

        List<object> addition = new List<object>();
        //if(values.Count > 0) {
            addition.Add(values[0]);
        //}
        List<object> result = new List<object>();
        result.AddRange(addition);
        result.AddRange((List<object>)values[1]);
        List<object> finalResult = new List<object>();
        foreach(object value in result) {
            object additionValue = value;
            if(value is PHPVariableReference) {
                additionValue = ((PHPVariableReference)value).get(null);
            }
            finalResult.Add(additionValue);
        }
        return finalResult;
    }

    public object callScriptFunctionReference(object scriptFunction=null, List<object> parameterValues=null, bool awaited=false, bool returnObject=true) {
        if(parameterValues == null) {
            parameterValues = new List<object>();
        }
        return this.callScriptFunctionSub(scriptFunction, parameterValues, awaited, returnObject, null);
    }

    public object callScriptFunctionSub(object scriptFunction=null, object parameterValuesInput=null, object awaitedInput=null, bool returnObject=false, PHPScriptFunction preserveContext=null) {
        //////////////////////////////System.Diagnostics.Debug.WriteLine("in callsub "+scriptFunction+" "+parameterValuesInput+" "+awaitedInput+" "+returnObject+" "+preserveContext);
        List<object> parameterValues;
        bool awaited=false;
        if(scriptFunction is List<object> || (scriptFunction == null && this is PHPScriptFunction)) {
            /*if(parameterValuesInput != null) {
                awaited = (bool)parameterValuesInput;
            } else {
                awaited = false;
            }*/
            parameterValues = (List<object>)scriptFunction;
            scriptFunction = this;
            //////////////////////////////System.Diagnostics.Debug.WriteLine("set as this");
        } else {
            if(parameterValuesInput == null) {
                parameterValuesInput = new List<object>();
            }
            parameterValues = (List<object>)parameterValuesInput;
        }
        //////////////////////////////System.Diagnostics.Debug.WriteLine("in callsub2 "+scriptFunction+" "+parameterValuesInput+" "+awaitedInput+" "+returnObject+" "+preserveContext);

        if(scriptFunction is PHPReturnResult) {
            scriptFunction = ((PHPReturnResult)scriptFunction).get();
        }

        if(scriptFunction is PHPScriptFunction) {
            PHPScriptFunction scriptFunctionFunction = (PHPScriptFunction)scriptFunction;
            //////////////////////////////System.Diagnostics.Debug.WriteLine("scriptfunc : "+scriptFunctionFunction.identifier+ " - "+scriptFunctionFunction.debugText);
            //////////////////////////////System.Diagnostics.Debug.WriteLine(scriptFunctionFunction.closures.Count);
            if(scriptFunctionFunction.closures.ContainsKey("main")) {
                //////////////////////////////System.Diagnostics.Debug.WriteLine("contains main");
                /*NSObject* resblock = nil;
                @autoreleasepool {
                    id closure = (NSObject*(^)(NSObject*, NSObject*))[[scriptFunctionFunction closures] objectForKey:@"main"];
                
                    ////////////////////NSLog(@"return closure result %@", ((NSObject*(^)(NSObject*, NSObject*))closure)(parameterValues, scriptFunction));
                    resblock = ((NSObject*(^)(NSObject*, NSObject* __weak))closure)(parameterValues, scriptFunctionFunction);
                
                }
                return resblock;*/
                return (scriptFunctionFunction.closures["main"] as Func<List<object>, PHPScriptFunction, object>)(parameterValues, scriptFunctionFunction);
            }
            if(scriptFunction != null) {
                PHPScriptFunction scriptFunc = ((PHPScriptFunction)scriptFunction);
                List<object> additionalParameters = new List<object>();
                //if(!(awaited is bool)
                if(awaitedInput is List<object>) {
                    additionalParameters = (List<object>)awaitedInput;
                }
                bool async_call = false;
                if(scriptFunctionFunction is PHPScriptFunction) {
                    if(scriptFunctionFunction.isAsync) {
                        async_call = true;
                    }
                }

                scriptFunctionFunction = scriptFunctionFunction.copyScriptFunction();
                scriptFunctionFunction.completedExecution = false;
                //////////////////////////////System.Diagnostics.Debug.WriteLine("called reset this: "+((PHPScriptFunction)scriptFunction).prototype+" - "+scriptFunctionFunction.prototype);
                scriptFunctionFunction.resetThis(null);
                if(parameterValues != null && parameterValues.Count > 0) {
                    scriptFunctionFunction.setInputVariables((List<object>)parameterValues[0]);
                } else {
                    scriptFunctionFunction.setInputVariables(new List<object>());
                }
                object result = null;
                if(async_call) {

                	//System.Diagnostics.Debug.WriteLine("async call----"+scriptFunctionFunction.debugText);
                    TaskFactory t = new TaskFactory();
                    //t.
                    t.StartNew(() => {
                    /*DataInThread = new Thread(new ThreadStart(ThreadProcedure));
DataInThread.IsBackground = true;
DataInThread.Start();*/
                    //Action threadAction = () => {
                    //System.ServiceModel.Dispatcher.Invoke
                        /*if(scriptFunc.callCount == null) {
                            scriptFunc.retainCountValue = scriptFunc.retainCountValue+1;
                        }*/

                       // scriptFunc.scriptIndexReference.isAsync = true;
                       // object result2 = scriptFunc.scriptIndexReference.callFun(scriptFunctionFunction);
                        /*if(scriptFunc.callCount == null) {
                            scriptFunc.retainCountValue = scriptFunc.retainCountValue-1;
                        } else {
                            if(scriptFunc.callCount != -1) {
                                scriptFunc.callCount--;
                                scriptFunc.retainCountValue = scriptFunc.callCount;
                            }
                        }

                        if(scriptFunctionFunction.parentContext != null && scriptFunc.retainCountValue == 0) {
                            PHPScriptFunction parentContext = scriptFunctionFunction.parentContext;
                            if(parentContext is PHPScriptFunction && parentContext.containsCallbacks && !parentContext.identifier.Contains("__")) {
                            parentContext.canUnset();
                            }
                        }
                        scriptFunctionFunction.unset(null);*/
                    //};
                    //CoreDispatcher c = Window.Current.Dispatcher;
                    //c.RunAsync(CoreDispatcherPriority.Normal, () => {
                    //////////////////////////////System.Diagnostics.Debug.WriteLine("in async : "+scriptFunctionFunction.debugText);
                    //Task.Run(() => {  
                        scriptFunc.scriptIndexReference.isAsync = true;
                        object result2 = scriptFunc.scriptIndexReference.callFun(scriptFunctionFunction);
                    });
                    //Thread t = new Thread(new ThreadStart(threadAction));
                    //t.Start();
                    return null;
                } else {
                    bool perform_function = true;
                    
                    if(scriptFunctionFunction.isCallback == true) {
                        if(scriptFunc.callCount == null) {
                            scriptFunc.retainCountValue++;
                        }
                    }
                    scriptFunctionFunction.isReturnValueItem = true;

                    try {
                    	////////////////System.Diagnostics.Debug.WriteLine("call scriptfunction indexreference with: "+scriptFunctionFunction);
                        scriptFunctionFunction.scriptIndexReference.callFun(scriptFunctionFunction);
                    } catch(Exception exc) {
                        System.Diagnostics.Debug.WriteLine(exc);
                    }

                    result = scriptFunctionFunction.returnResultValue;
                }

                int counter = 0;
                while(additionalParameters.Count > 0) {
                	//System.Diagnostics.Debug.WriteLine("additional parameters: "+additionalParameters+" "+counter);
                    additionalParameters = (List<object>)additionalParameters[0];
                    if(additionalParameters.Count >= 2) {
                        additionalParameters = (List<object>)additionalParameters[1];
                    } else {
                        additionalParameters = new List<object>();
                    }
                    result = this.callScriptFunctionSub(result, additionalParameters, null, false, null);
               		//System.Diagnostics.Debug.WriteLine("add return result: "+result);
                    counter++;
                }

                if(returnObject) {
                	//System.Diagnostics.Debug.WriteLine("return object: "+scriptFunctionFunction);
                    result = scriptFunctionFunction.getObject();
                }

                //bool containsFunctionDefinition = false;
                //Dictionary<object, object> parseObject = scriptFunctionFunction.scriptIndexReference.subObjectDict;
                //System.Diagnostics.Debug.WriteLine("return result: "+result);
                return result;
            }

        }
        return null;
    }

    public PHPScriptObject createScriptObject(object values) {
        PHPScriptObject scriptObject = new PHPScriptObject();
        scriptObject.initArrays();
        return scriptObject;
    }

    public void deleteProperty(object identifier) {
        if(this.isArray) {
            this.dictionaryArray.RemoveAt(Convert.ToInt32(identifier));
        } else {
        	//////////////System.Diagnostics.Debug.WriteLine("delete property: "+identifier);
            this.dictionaryKeys.Remove(identifier);
            this.dictionary.Remove(identifier);
        }
    }

    public void deletePropertyReference(object variableReference) {
    	////////////////System.Diagnostics.Debug.WriteLine("delete property reference");

       	////////////////System.Diagnostics.Debug.WriteLine("delete property: "+variableReference);
       	while(variableReference is List<object>) {
    		variableReference = ((List<object>)variableReference)[0];
    	}
       	////////////////System.Diagnostics.Debug.WriteLine("delete property: "+variableReference);
        //variableReference = this.parseInputVariable(variableReference);
        if(variableReference is PHPVariableReference) {
            PHPVariableReference varReference = (PHPVariableReference)variableReference;
        	////////////////System.Diagnostics.Debug.WriteLine("delete property: "+varReference.identifier);
            varReference.context.deleteProperty(varReference.identifier);
        }
    }

    public object makeIntoNumber(object value) {
        if(value is int) {
            return value;
        }
        if(value is double) {
            return value;
        }
        if(value is bool) {
            return value;
        }
        return Convert.ToInt32(value);
    }

    public string makeIntoString(object value) {
        return value.ToString();
    }

    public object negativeValue(object value) {
        value = this.parseInputVariable(value);
        value = this.removeParanthesis(value);
        if(value is int) {
            return -(int)value;
        }
        if(value is double) {
            return -(double)value;
        }
        return value;
    }

    public object negateValue(object value) {
        value = this.parseInputVariable(value);
        value = this.removeParanthesis(value);
        if(value is string && (string)value == "true") {
        	value = true;
        }
        if(value is string && (string)value == "false") {
        	value = false;
        }
        if(value is bool) {
            return !(bool)value;
        }
        if(value is int && (int)value == 1) {
        	value = 0;
        } else if(value is int && (int)value == 0) {
        	value = 1;
        } 
        return value;
    }

    public bool equalsArrSub(object arrValueA, object arrValueB) {
        return false;
    }

    public bool equalsSub(object valueA, object valueB) {
        //System.Diagnostics.Debug.WriteLine("valueA: "+valueA);
        //System.Diagnostics.Debug.WriteLine("valueB: "+valueB);
        List<object> list1 = new List<object>() { valueA };
        List<object> list2 = new List<object>() { valueB };
        try {
        	///System.Diagnostics.Debug.WriteLine("equals : "+JsonConvert.SerializeObject(list1, new DecimalJsonConverter())+" + "+JsonConvert.SerializeObject(list2, new DecimalJsonConverter()));
	        /*if(JsonConvert.SerializeObject(valueA, new DecimalJsonConverter()) == JsonConvert.SerializeObject(valueB, new DecimalJsonConverter())) {
	        	return true;
	        }*/
	        toJSON toJsonInstance = new toJSON(true);
	        string list1JSON = toJsonInstance.toJSONString(list1);
	        //toJsonInstance = new toJSON();
	        //toJsonInstance = new toJSON(true);
	        string list2JSON = toJsonInstance.toJSONString(list2);
	        //System.Diagnostics.Debug.WriteLine("list1: "+list1JSON);
	        //System.Diagnostics.Debug.WriteLine("list2JSON: "+list2JSON);
	        if(list1JSON == list2JSON) {
	        	return true;
	        }
	        return false;
	    } catch(Exception exc) {
	    	//System.Diagnostics.Debug.WriteLine("exc : "+exc);
	    	//System.Diagnostics.Debug.WriteLine("exc : "+exc.Message);
	    }
	    return false;
        /*if(Enumerable.SequenceEqual<List<object>>((IEnumerable<List<object>>)valueA, (IEnumerable<List<object>>)valueB)) {
        	return true;
        }*/
        /*var firstNotSecond = list1.Except(list2).ToList();
		var secondNotFirst = list2.Except(list1).ToList();
        ////////////////////System.Diagnostics.Debug.WriteLine("equals : +"+JsonConvert.SerializeObject(firstNotSecond)+" + "+JsonConvert.SerializeObject(secondNotFirst));
		return !firstNotSecond.Any() && !secondNotFirst.Any();*/
    }

    public bool equals(object valueA, object valueB) {
    	////System.Diagnostics.Debug.WriteLine("equality");
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);


        return this.equalsSub(valueA, valueB);
    }

    public bool inequality(object valueA, object valueB) {
    	////System.Diagnostics.Debug.WriteLine("inequality");
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        return !this.equalsSub(valueA, valueB);
    }

    public bool equalsStrong(object valueA, object valueB) {
    	////System.Diagnostics.Debug.WriteLine("equality Strong");
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);
        
        string typeofA = this.getTypeOf(valueA);
        string typeofB = this.getTypeOf(valueB);

        if(typeofA != typeofB) {
            return false;
        }

        return this.equalsSub(valueA, valueB);
    }

    public bool inequalityStrong(object valueA, object valueB) {
    	////System.Diagnostics.Debug.WriteLine("inequality Strong");
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        string typeofA = this.getTypeOf(valueA);
        string typeofB = this.getTypeOf(valueB);

        if(typeofA != typeofB) {
            return true;
        }

        return !this.equalsSub(valueA, valueB);
    }

    public bool greater(object valueA, object valueB) {
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        double dvalueA = Convert.ToDouble(valueA);//this.makeIntoNumber(valueA);
        double dvalueB = Convert.ToDouble(valueB);//this.makeIntoNumber(valueB);

        if(dvalueA > dvalueB) {
            return true;
        }
        return false;
    }

    public bool less(object valueA, object valueB) {
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        double dvalueA = Convert.ToDouble(valueA);//this.makeIntoNumber(valueA);
        double dvalueB = Convert.ToDouble(valueB);//this.makeIntoNumber(valueB);
        //////////////////////////////System.Diagnostics.Debug.WriteLine("less : "+dvalueA+" - "+dvalueB);
        if(dvalueA < dvalueB) {
            return true;
        }
        return false;
    }

    public bool greaterEquals(object valueA, object valueB) {
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        double dvalueA = Convert.ToDouble(valueA);//this.makeIntoNumber(valueA);
        double dvalueB = Convert.ToDouble(valueB);//this.makeIntoNumber(valueB);

        if(dvalueA >= dvalueB) {
            return true;
        }
        return false;
    }

    public bool lessEquals(object valueA, object valueB) {
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        double dvalueA = Convert.ToDouble(valueA);//this.makeIntoNumber(valueA);
        double dvalueB = Convert.ToDouble(valueB);//this.makeIntoNumber(valueB);

        if(dvalueA <= dvalueB) {
            return true;
        }
        return false;
    }

    public bool andCondition(object valueA, object valueB) {
        while(valueA is PHPScriptEvaluationReference) {
            valueA = ((PHPScriptEvaluationReference)valueA).callFun((PHPScriptFunction)this);
        }
        
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);

        bool valueAString = Convert.ToBoolean(valueA);
        if(!valueAString) {
            return false;
        }

        while(valueB is PHPScriptEvaluationReference) {
            valueB = ((PHPScriptEvaluationReference)valueB).callFun((PHPScriptFunction)this);
        }
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        bool valueBString = Convert.ToBoolean(valueB);

        if(valueAString && valueBString) {
            return true;
        }
        return false;
    }

    public bool orCondition(object valueA, object valueB) {
        while(valueA is PHPScriptEvaluationReference) {
            valueA = ((PHPScriptEvaluationReference)valueA).callFun((PHPScriptFunction)this);
        }
        
        valueA = this.parseInputVariable(valueA);
        valueA = this.removeParanthesis(valueA);

        bool valueAString = Convert.ToBoolean(valueA);
        if(valueAString) {
            return false;
        }

        while(valueB is PHPScriptEvaluationReference) {
            valueB = ((PHPScriptEvaluationReference)valueB).callFun((PHPScriptFunction)this);
        }
        
        valueB = this.parseInputVariable(valueB);
        valueB = this.removeParanthesis(valueB);

        bool valueBString = Convert.ToBoolean(valueB);

        if(valueBString) {
            return true;
        }
        return false;
    }

    public object removeParanthesis(object valueA) {
        while(valueA is List<object>) {
            List<object> listValue = (List<object>)valueA;
            if(listValue.Count > 0) {
                valueA = listValue[0];
            }
            return null;
        }
        return valueA;
    }

    public object resolveValueArray(object values) {
    	//toJSON instance = new toJSON();
    	//System.Diagnostics.Debug.WriteLine(instance.toJSONString(values));
        if(values is List<object>) {
    		//System.Diagnostics.Debug.WriteLine("values : "+((List<object>)values).Count);
            object result = null;
            string nextOperator = null;
            bool returnString = false;
            if(((List<object>)values).Count == 3 && ((List<object>)values)[1] is PHPValuesOperator && ((PHPValuesOperator)((List<object>)values)[1]).operatorValue == "-" && ((List<object>)values)[0] is int && (int)((List<object>)values)[0] == 1 && ((List<object>)values)[2] == null) {
				return -1;
			}
            foreach(object value in (List<object>)values) {
            	//System.Diagnostics.Debug.WriteLine("value: "+value);
                object setValue = value;

                if(setValue is PHPValuesOperator) {
                    nextOperator = ((PHPValuesOperator)value).operatorValue;
                } else {
                    setValue = this.resolveValueReferenceVariableArray(setValue);

                    if(nextOperator == null) {
                        result = setValue;
                    } else {
                        if(nextOperator == ".") {
                            returnString = true;
                            if(result == null) {
                                result = "";
                            }
                            result = this.resolveValueReferenceVariableArray(result);
                            setValue = this.resolveValueReferenceVariableArray(setValue);
                            result = this.makeIntoString(result);
                            setValue = this.makeIntoString(setValue);

                            result = (string)result+(string)setValue;
                        } else if(nextOperator == "+") {
                            if(result == null) {
                                result = 0;
                            }
                            
                            result = this.resolveValueReferenceVariableArray(result);
                            setValue = this.resolveValueReferenceVariableArray(setValue);
                            result = Convert.ToDouble(result);//this.makeIntoNumber(result);
                            setValue = Convert.ToDouble(setValue);//this.makeIntoNumber(setValue);
                            result = (double)result + (double)setValue;

                        } else if(nextOperator == "-") {
                            if(result == null) {
                                result = 0;
                            }
                            if(result is string && result == "") {
                            	result = 0;
                            }
                            
                            result = this.resolveValueReferenceVariableArray(result);
                            setValue = this.resolveValueReferenceVariableArray(setValue);
                            result = Convert.ToDouble(result);//this.makeIntoNumber(result);
                            setValue = Convert.ToDouble(setValue);//this.makeIntoNumber(setValue);
                            System.Diagnostics.Debug.WriteLine("subtract: "+result+" "+setValue);
                            result = (double)result - (double)setValue;
                        } else if(nextOperator == "/") {
                            if(result == null) {
                                result = 0;
                            }
                            
                            result = this.resolveValueReferenceVariableArray(result);
                            setValue = this.resolveValueReferenceVariableArray(setValue);
                            result = Convert.ToDouble(result);//this.makeIntoNumber(result);
                            setValue = Convert.ToDouble(setValue);//this.makeIntoNumber(setValue);
                            result = (double)result / (double)setValue;
                        } else if(nextOperator == "%") {
                            if(result == null) {
                                result = 0;
                            }
                            
                            result = this.resolveValueReferenceVariableArray(result);
                            setValue = this.resolveValueReferenceVariableArray(setValue);
                            result = Convert.ToDouble(result);//this.makeIntoNumber(result);
                            setValue = Convert.ToDouble(setValue);//this.makeIntoNumber(setValue);
                            result = (double)result % (double)setValue;
                        } else if(nextOperator == "*") {
                            if(result == null) {
                                result = 0;
                            }
                            
                            result = this.resolveValueReferenceVariableArray(result);
                            setValue = this.resolveValueReferenceVariableArray(setValue);
                            result = Convert.ToDouble(result);//this.makeIntoNumber(result);
                            setValue = Convert.ToDouble(setValue);//this.makeIntoNumber(setValue);
                            result = (double)result * (double)setValue;
                        }
                    }
                }
            }
            return result;
        }
        return values;
    }

    public object returnResultReference(object value) {
        ////////System.Diagnostics.Debug.WriteLine("return result returnResultReference: "+value);
        /*if(value == null) {

        }*/
        if(value is List<object>) {
            List<object> valueArray = (List<object>)value;
            if(valueArray.Count == 1) {
                return valueArray[0];
            }
            return valueArray[1];
        }
        return value;
    }

    public object returnResultDereference(object value) {
        /*if(value == null) {

        }*/
        ////////System.Diagnostics.Debug.WriteLine("return result defeference: "+value);
        if(value is List<object>) {
            List<object> valueArray = (List<object>)value;
            if(valueArray.Count == 1) {
                return valueArray[0];
            }
            if(valueArray.Count > 1) {
                return valueArray[1];
            }
        }
        return value;
    }

    public object returnResult(object value, object b) {
        ////////System.Diagnostics.Debug.WriteLine("return result returnResult: "+value);
        //////////////////////////////System.Diagnostics.Debug.WriteLine("in return result: "+value);
        if(value is List<object>) {
            List<object> valueArray = (List<object>)value;
            //////////////////////////////System.Diagnostics.Debug.WriteLine("value Count: "+valueArray.Count);

            foreach(object valueItem in valueArray) {
                //////////////////////////////System.Diagnostics.Debug.WriteLine("return result valueitem : "+valueItem);
                if(valueItem is PHPReturnResult) {
                    return valueItem;
                }
            }
            if(valueArray.Count > 0) {
                value = valueArray[valueArray.Count-1];
            }
        }
        //////////////////////////////System.Diagnostics.Debug.WriteLine("return result: "+value);
        /*if(value is PHPReturnResult) {
            object debugvalue = ((PHPReturnResult)value).get();
            //////////////////////////////System.Diagnostics.Debug.WriteLine("return result: "+debugvalue);
        }*/
        return value;
    }

    public object cloneObject(object valueObj) {
    	/*if([object isKindOfClass:[PHPVariableReference class]]) {
	        object = [(PHPVariableReference*)object get:nil];
	    }
	    object = [self parseInputVariable:object];
	    //////NSLog(@"clone object: %@", object);
	    if([object isKindOfClass:[PHPScriptFunction class]]) {
	        object = [(PHPScriptFunction*)object copyScriptFunction];
	    } else if([object isKindOfClass:[PHPScriptObject class]]) {
	        object = [(PHPScriptObject*)object copyScriptObject];
	    }
	    [(PHPScriptObject*)object setInterpretation:[self getInterpretation]];
	    [(PHPScriptObject*)object setInterpretationForObject:[self getInterpretation]];
	    if([object isKindOfClass:[PHPScriptObject class]] && ![(PHPScriptObject*)object isArray]) {
	        PHPScriptObject* selfClone = object;
	        NSMutableDictionary* newDictionary = [[NSMutableDictionary alloc] init];
	        for(NSObject* key in [selfClone dictionary]) {
	            NSObject* dictionaryValue = [selfClone dictionary][key];
	            //////////////////////////////////NSLog/@"self clone dict value: %@", dictionaryValue);
	            if([dictionaryValue isKindOfClass:[PHPScriptFunction class]]) {
	                dictionaryValue = [(PHPScriptFunction*)dictionaryValue copyScriptFunction];
	                [(PHPScriptFunction*)dictionaryValue setPrototype:selfClone];
	                ////////////NSLog(@"ins2: %@", context);
	                [(PHPScriptFunction*)dictionaryValue setParentContext:self];
	                [(PHPScriptFunction*)dictionaryValue setParentContextStrong:self];
	                //////////////////////////////////NSLog/@"set selfclone as prototype: %@", selfClone);
	                //////////////////////////////////NSLog/@"set selfclone as prototype: %@", [(PHPScriptFunction*)dictionaryValue prototype]);
	            }
	            newDictionary[key] = dictionaryValue;
	        }
	        [selfClone setDictionary:newDictionary];
	    }*/
	    if(valueObj is PHPVariableReference) {
	    	valueObj = ((PHPVariableReference)valueObj).get(null);
	    }
	    valueObj = this.parseInputVariable(valueObj);
	    if(valueObj is PHPScriptFunction) {
	    	valueObj = ((PHPScriptFunction)valueObj).copyScriptFunction();
	    } else if(valueObj is PHPScriptObject) {
	    	valueObj = ((PHPScriptObject)valueObj).copyScriptObject();
	    }
	    ((PHPScriptObject)valueObj).interpretation = this.getInterpretation();
	    if(valueObj is PHPScriptObject && !((PHPScriptObject)valueObj).isArray) {
	    	PHPScriptObject selfClone = (PHPScriptObject)valueObj;
	    	Dictionary<object, object> newDictionary = new Dictionary<object, object>();
	    	foreach(KeyValuePair<object, object> keyPair in selfClone.dictionary) {
	    		object dictionaryValue = keyPair.Value;
	    		object key = keyPair.Key;

	    		if(dictionaryValue is PHPScriptFunction) {
	    			dictionaryValue = ((PHPScriptFunction)dictionaryValue).copyScriptFunction();
	    			((PHPScriptFunction)dictionaryValue).prototype = selfClone;
	    			if(this is PHPScriptFunction) {
		    			((PHPScriptFunction)dictionaryValue).parentContext = (PHPScriptFunction)this;
		    		} else {
		    			((PHPScriptFunction)dictionaryValue).parentContext = this.parentContext;
		    		}
	    		}
	    		newDictionary[key] = dictionaryValue;
	    	}
	    	selfClone.dictionary = newDictionary;
	    }
        return valueObj;
    }

    public object parseInputVariable(object input) {
        if(input is double || input is int || input is bool || input is string) {
            return input;
        }
        input = this.resolveValueArray(input);
        while(input is List<object>) {
            input = ((List<object>)input)[0];
        }
        input = this.resolveValueReferenceVariableArray(input);
        return input;
    }
}

class PHPUndefined : NSObject {

}

class PHPNull : NSObject {

}

class NSString : NSObject {
    public string string_value;


    public NSString(string input) {
        this.string_value = input;
    }

    public NSString(NSString input) {
        this.string_value = input.string_value;
    }

    public bool isEqualToString(NSString valueCompare) {
        if(this.string_value == valueCompare.string_value) {
            return true;
        }
        return false;
    }
}

class NSNumber : NSObject {
    private int? long_value = null;
    private double? double_value = null;
    private bool? bool_value = null;

    public NSNumber() {

    }

    public int? getLongActual() {
        return this.long_value;
    }

    public double? getDoubleActual() {
        return this.double_value;
    }

    public bool? getBoolActual() {
        return this.bool_value;
    }

    public NSNumber(NSNumber input) {
        this.long_value = input.getLongActual();
        this.double_value = input.getDoubleActual();
        this.bool_value = input.getBoolActual();
    }

    public NSNumber(int value) {
        this.long_value = value;
        this.double_value = null;
        this.bool_value = null;
    }

    public NSNumber(bool value) {
        this.long_value = null;
        this.double_value = null;
        this.bool_value = value;
    }

    public void setBoolValue(bool value) {
        this.bool_value = value;
        this.long_value = null;
        this.double_value = null;
    }

    public int longValue() {
        
        if(this.long_value != null) {
            int longValue = (int)this.long_value;
            return longValue;
        }

        
        if(this.bool_value != null) {
            if((bool)this.bool_value) {
                return 1;
            }
            return 0;
        }
        
        if(this.double_value != null) {
            double doubleValue = (double)this.double_value;
            return Convert.ToInt32(doubleValue);
        }
        return 0;
    }

    public bool boolValue() {
        if(this.bool_value != null) {
            if((bool)this.bool_value) {
                return true;
            }
            return false;
        }
        if(this.long_value != null) {
            int longValue = (int)this.long_value;
            if(longValue != 0) {
                return true;
            }
        }
        if(this.double_value != null) {
            double doubleValue = (double)this.double_value;
            if(doubleValue != 0) {
                return true;
            }
        }
        return false;
    }
}