//using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
/*using Windows.ApplicationModel.UserDataTasks;
using Windows.Foundation.Diagnostics;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LibVLCSharp;
using LibVLCSharp.Shared;
using LibVLCSharp.Shared.MediaPlayerElement;
using LibVLCSharp.Platforms.Windows;
using System;
using System.Drawing;*/

class PHPIncludedObjects : PHPScriptObject {
    public void init(PHPScriptFunction context) {
        this.initArrays();
        this.globalObject = true;

        PHPRegex regex = new PHPRegex();
        regex.init(context);

        this.setDictionaryValue("regex", regex);

        PHPStrings strings = new PHPStrings();
        strings.init(context);
        
        this.setDictionaryValue("strings", strings);



        PHPScriptFunction reverse = new PHPScriptFunction();
        reverse.initArrays();
        this.setDictionaryValue("reverse", reverse);
        reverse.prototype = this;
        reverse.setClosure((values, self_instance) => {
            object value = ((List<object>)values[0])[0];
            value = self_instance.parseInputVariable(value);
            PHPScriptObject scriptObjectValue = (PHPScriptObject)value;
            scriptObjectValue = scriptObjectValue.copyScriptObject();

            object dictionary = scriptObjectValue.getDictionary();
            if(scriptObjectValue.isArray) {
                List<object> reverseArray = new List<object>((List<object>)dictionary);
                reverseArray.Reverse();
                scriptObjectValue.dictionaryArray = reverseArray;
            } else {
                List<object> keys = new List<object>(scriptObjectValue.dictionaryKeys);
                keys.Reverse();
                scriptObjectValue.dictionaryKeys = keys;
            }
            return scriptObjectValue;
        }, "main");

        PHPScriptFunction create = new PHPScriptFunction();
        create.initArrays();
        this.setDictionaryValue("create", create);
        create.prototype = this;
        create.setClosure((values, self_instance) => {
            return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>());
        }, "main");

        PHPScriptFunction array_shift = new PHPScriptFunction();
        array_shift.initArrays();
        this.setDictionaryValue("array_shift", array_shift);
        array_shift.prototype = this;
        array_shift.setClosure((values, self_instance) => {
            object input = values[0];
            input = ((List<object>)input)[0];
            input = this.parseInputVariable(input);
            PHPScriptObject scriptobj = (PHPScriptObject)input;
            return scriptobj.scriptArrayShift();
            //return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>());
        }, "main");

        PHPScriptFunction array_pop = new PHPScriptFunction();
        array_pop.initArrays();
        this.setDictionaryValue("array_pop", array_pop);
        array_pop.prototype = this;
        array_pop.setClosure((values, self_instance) => {
            object input = values[0];
            input = ((List<object>)input)[0];
            input = this.parseInputVariable(input);
            PHPScriptObject scriptobj = (PHPScriptObject)input;
            return scriptobj.scriptArrayPop();
            //return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>());
        }, "main");

        PHPScriptFunction item_is_object = new PHPScriptFunction();
        item_is_object.initArrays();
        this.setDictionaryValue("item_is_object", item_is_object);
        item_is_object.prototype = this;
        item_is_object.setClosure((values, self_instance) => {
            object input = values[0];
            if(input is PHPScriptObject) {
            	return true;
            }
            return false;
            //return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>());
        }, "main");

        PHPScriptFunction array_unshift = new PHPScriptFunction();
        array_unshift.initArrays();
        this.setDictionaryValue("array_unshift", array_unshift);
        array_unshift.prototype = this;
        array_unshift.setClosure((values, self_instance) => {
            object input = values[0];
            object input_item = ((List<object>)input)[1];
            input = ((List<object>)input)[0];
            input = this.parseInputVariable(input);
            input_item = this.parseInputVariable(input_item);
            PHPScriptObject scriptobj = (PHPScriptObject)input;
            scriptobj.scriptArrayUnshift(input_item);
            return 0;
            //return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>());
        }, "main");

        PHPScriptFunction item_is_array = new PHPScriptFunction();
        item_is_array.initArrays();
        this.setDictionaryValue("item_is_array", item_is_array);
        item_is_array.prototype = this;
        item_is_array.setClosure((values, self_instance) => {
            object input = values[0];
            bool inputB = false;
            if(((List<object>)input).Count > 1) {
                inputB = true;
            }
            input = ((List<object>)input)[0];
            input = this.parseInputVariable(input);
            if(inputB) {
                if(input is PHPScriptObject) {
                    return true;
                }
                return false;
            }
            if(input is PHPScriptObject) {
                PHPScriptObject scriptobj = (PHPScriptObject)input;
                if(scriptobj.isArray) {
                    return true;
                }
            }
            return false;
            //return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>());
        }, "main");
        
        PHPScriptFunction func_length = new PHPScriptFunction();
        func_length.initArrays();
        this.setDictionaryValue("func_length", func_length);
        func_length.prototype = this;
        func_length.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            if(value is PHPScriptFunction) {
                PHPScriptFunction func = (PHPScriptFunction)value;
                return func.inputParameterKeys.Count;
            }
            return 0;
        }, "main");

        PHPScriptFunction get_dict_value = new PHPScriptFunction();
        get_dict_value.initArrays();
        this.setDictionaryValue("get_dict_value", get_dict_value);
        get_dict_value.prototype = this;
        get_dict_value.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object parameterIndex = this.parseInputVariable(((List<object>)values[0])[1]);
            if(((List<object>)values[0]).Count == 3) {

            } else {
            	toJSON jsonInstance = new toJSON(true);
            	parameterIndex = jsonInstance.toJSONString(parameterIndex);
            }
            if(value is PHPScriptObject) {
                PHPScriptObject func = (PHPScriptObject)value;
                if(func.dictionary.ContainsKey(parameterIndex)) {
	                return func.dictionary[parameterIndex];
	            }
            }
            return 0;
        }, "main");

        PHPScriptFunction set_dict_value = new PHPScriptFunction();
        set_dict_value.initArrays();
        this.setDictionaryValue("set_dict_value", set_dict_value);
        set_dict_value.prototype = this;
        set_dict_value.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object parameterIndex = this.parseInputVariable(((List<object>)values[0])[1]);
            object parameterValue = this.parseInputVariable(((List<object>)values[0])[2]);
            if(((List<object>)values[0]).Count == 4) {

            } else {
            	toJSON jsonInstance = new toJSON(true);
            	parameterIndex = jsonInstance.toJSONString(parameterIndex);
            }
            if(value is PHPScriptObject) {
                PHPScriptObject func = (PHPScriptObject)value;
                func.dictionary[parameterIndex] = parameterValue;
            }
            return 0;
        }, "main");

        PHPScriptFunction to_number = new PHPScriptFunction();
        to_number.initArrays();
        this.setDictionaryValue("to_number", to_number);
        to_number.prototype = this;
        to_number.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);

            return Convert.ToInt32(value);
        }, "main");


        PHPScriptFunction to_string = new PHPScriptFunction();
        to_string.initArrays();
        this.setDictionaryValue("to_string", to_string);
        to_string.prototype = this;
        to_string.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);

            return value.ToString();
        }, "main");

        PHPScriptFunction sort = new PHPScriptFunction();
        sort.initArrays();
        this.setDictionaryValue("sort", sort);
        sort.prototype = this;
        sort.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object callback = this.parseInputVariable(((List<object>)values[0])[1]);

            PHPScriptObject scriptObj = (PHPScriptObject)value;
            scriptObj.sort((PHPScriptFunction)callback);

            return 0;
        }, "main");

        PHPScriptFunction remove_item = new PHPScriptFunction();
        remove_item.initArrays();
        this.setDictionaryValue("remove_item", remove_item);
        remove_item.prototype = this;
        remove_item.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object callback = this.parseInputVariable(((List<object>)values[0])[1]);

            PHPScriptObject scriptObj = (PHPScriptObject)value;
            scriptObj.removeItem(callback);

            return 0;
        }, "main");

        

        PHPScriptFunction once = new PHPScriptFunction();
        once.initArrays();
        this.setDictionaryValue("once", once);
        once.prototype = this;
        once.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            PHPScriptFunction main = (PHPScriptFunction)value;
            lock(main) {
                if(!main.hasRunOnce) {
                    main.hasRunOnce = true;
                    main.callCallback(new List<object>());
                }
            }
            return 0;
        }, "main");

        PHPScriptFunction toJSON = new PHPScriptFunction();
        toJSON.initArrays();
        this.setDictionaryValue("toJSON", toJSON);
        toJSON.prototype = this;
        toJSON.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            /*PHPScriptFunction main = (PHPScriptFunction)value;
            lock(main) {
                if(!main.hasRunOnce) {
                    main.hasRunOnce = true;
                    main.callCallback(new List<object>());
                }
            }
            return null;*/
            toJSON toJSON_instance = new toJSON();
            return toJSON_instance.toJSONString(value);
        }, "main");

        
        PHPScriptFunction equals = new PHPScriptFunction();
        equals.initArrays();
        this.setDictionaryValue("equals", equals);
        equals.prototype = this;
        equals.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object valueB = this.parseInputVariable(((List<object>)values[0])[1]);
            if(value.Equals(valueB)) {
                return true;
            }
            return false;
        }, "main");

        PHPScriptFunction fromJSON = new PHPScriptFunction();
        fromJSON.initArrays();
        this.setDictionaryValue("fromJSON", fromJSON);
        fromJSON.prototype = this;
        fromJSON.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            System.Diagnostics.Debug.WriteLine("json from: "+value.ToString());
            if(value is PHPScriptObject) {
            	toJSON jsonInstance = new toJSON();
            	value = jsonInstance.toJSONString(value);

            	System.Diagnostics.Debug.WriteLine("json from scriptobject: "+value.ToString());
            }
            return this.getInterpretation().makeIntoObjects(JsonHelper2.Deserialize(value.ToString()));
        }, "main");

        PHPScriptFunction objectValues = new PHPScriptFunction();
        objectValues.initArrays();
        this.setDictionaryValue("values", objectValues);
        objectValues.prototype = this;
        objectValues.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            
            PHPScriptObject scriptobj = (PHPScriptObject)value;
            return this.getInterpretation().makeIntoObjects(scriptobj.getDictionaryValues());
        }, "main");

        

        PHPScriptFunction objectKeys = new PHPScriptFunction();
        objectKeys.initArrays();
        this.setDictionaryValue("keys", objectKeys);
        objectKeys.prototype = this;
        objectKeys.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            
            PHPScriptObject scriptobj = (PHPScriptObject)value;
            return this.getInterpretation().makeIntoObjects(scriptobj.getKeys());
        }, "main");

        PHPScriptFunction isset = new PHPScriptFunction();
        isset.initArrays();
        this.setDictionaryValue("isset", isset);
        isset.prototype = this;
        isset.setClosure((values, self_instance) => {
        	//System.Diagnostics.Debug.WriteLine("in isset");
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            
        	//System.Diagnostics.Debug.WriteLine("in isset: "+value);
        	//debug
        	/*if(value is PHPScriptObject) {
        		//System.Diagnostics.Debug.WriteLine("in isset: "+JsonConvert.SerializeObject(((PHPScriptObject)value).getKeys()));
        	}*/

            if(value == null) { // || value == 0
                return false;
            }
            if(value is PHPUndefined) {
                return false;
            }
            if(value == "undefined") {
            	return false;
            }
            return true;
        }, "main");
        
        PHPScriptFunction instance_of = new PHPScriptFunction();
        instance_of.initArrays();
        this.setDictionaryValue("instance_of", instance_of);
        instance_of.prototype = this;
        instance_of.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object className = this.parseInputVariable(((List<object>)values[0])[0]);
            
            if(((PHPScriptObject)value).instanceOf((string)className)) {
                return true;
            }
            return false;
        }, "main");
        
        PHPScriptFunction type_of = new PHPScriptFunction();
        type_of.initArrays();
        this.setDictionaryValue("typeof", type_of);
        type_of.prototype = this;
        type_of.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            
            return this.getTypeOf(value);
        }, "main");

        
        
        PHPScriptFunction concat = new PHPScriptFunction();
        concat.initArrays();
        this.setDictionaryValue("concat", concat);
        concat.prototype = this;
        concat.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object valueB = this.parseInputVariable(((List<object>)values[0])[0]);
            
            PHPScriptObject objA = (PHPScriptObject)value;
            PHPScriptObject objB = (PHPScriptObject)valueB;

            List<object> results = new List<object>((IEnumerable<object>)objA.getDictionary());
            results.AddRange((IEnumerable<object>)objB.getDictionary());

            return this.getInterpretation().makeIntoObjects(results);
        }, "main");
        
        
        PHPScriptFunction send = new PHPScriptFunction();
        send.initArrays();
        this.setDictionaryValue("send", send);
        send.prototype = this;
        send.setClosure((values, self_instance) => {
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            object valueB = this.parseInputVariable(((List<object>)values[0])[0]);
            
            ////System.Diagnostics.Debug.WriteLine("send: "+value);

            return 0;
        }, "main");


        PHPScriptFunction array_unique = new PHPScriptFunction();
        array_unique.initArrays();
        this.setDictionaryValue("array_unique", array_unique);
        array_unique.prototype = this;
        array_unique.setClosure((values, self_instance) => {
        	toJSON jsonInstance = new toJSON();
            object value = this.parseInputVariable(((List<object>)values[0])[0]);
            List<object> valueArr = (List<object>)jsonInstance.convert(value);
            valueArr = valueArr.Distinct().ToList<object>();
            return this.getInterpretation().makeIntoObjects(valueArr);
            
            ////System.Diagnostics.Debug.WriteLine("send: "+value);

            return 0;
        }, "main");

        PHPScriptFunction var_dump = new PHPScriptFunction();
        var_dump.initArrays();
        this.setDictionaryValue("log", var_dump);
        var_dump.prototype = this;
        var_dump.setClosure((values, self_instance) => {
            object value = ((List<object>)values[0])[0];
            value = self_instance.parseInputVariable(value);
            if(value is PHPScriptObject && ((PHPScriptObject)value).isArray) {
                foreach(object phpobjitem in (List<object>)((PHPScriptObject)value).getDictionary()) {
                    ////System.Diagnostics.Debug.WriteLine("output_sub: "+phpobjitem);
                }
            } else if(value is PHPScriptObject && !((PHPScriptObject)value).isArray) {
                foreach(KeyValuePair<object, object> phpobjitem in (Dictionary<object, object>)((PHPScriptObject)value).getDictionary()) {
                    ////System.Diagnostics.Debug.WriteLine("output_sub1: "+phpobjitem.Value);
                }
            }
            //Console.WriteLine("output: "+value);
            System.Diagnostics.Debug.WriteLine("output: "+value);
            return 0;
        }, "main");
        var_dump.identifier = "var_dump";
            
        ////System.Diagnostics.Debug.WriteLine("init vardump "+var_dump.closures.Count);
            //Func<List<object>, PHPScriptFunction, object> (values, 
    }
}

/*class PHPMedia : PHPScriptObject {

    public LibVLC mainLib;

    private Grid contentLayout;

    public void init(PHPScriptFunction context, Grid contentLayout) {
        
        this.initArrays();
        this.globalObject = true;

        this.mainLib = new LibVLC();
        this.contentLayout = contentLayout;

        PHPScriptFunction add_tab = new PHPScriptFunction();
        add_tab.initArrays();
        this.setDictionaryValue("add_tab", add_tab);
        add_tab.prototype = this;
        add_tab.setClosure((values, self_instance) => {
            PHPMediaTab newMediaTab = new PHPMediaTab();
            newMediaTab.setPhpMedia(this);
            newMediaTab.init(context, this.contentLayout);
            return newMediaTab;
        }, "main");

        //chooseScreens
        //chooseScreensAlt
    }
}

class PHPMediaTab : PHPScriptObject {
    private PHPMedia media;
    public VideoView v;
    public MediaPlayer mediaPlayer;

    public void setPhpMedia(PHPMedia media) {
        this.media = media;
    }

    public void init(PHPScriptFunction context, Grid contentLayout) {
        this.mediaPlayer = new MediaPlayer(this.media.mainLib);
        this.v = new VideoView();
        this.v.MediaPlayer = this.mediaPlayer;

        this.v.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));
        
        contentLayout.Children.Add(this.v);

        this.v.MediaPlayer.Media = new Media(this.media.mainLib, new Uri("https://www.sample-videos.com/video321/mp4/240/big_buck_bunny_240p_2mb.mp4"));
        this.v.MediaPlayer.Play();
    }
}*/

class PHPDates : PHPScriptObject {

    public void init(PHPScriptFunction context) {
        
        this.initArrays();
        this.globalObject = true;

        PHPScriptFunction get_current_date = new PHPScriptFunction();
        get_current_date.initArrays();
        this.setDictionaryValue("get_current_date", get_current_date);
        get_current_date.prototype = this;
        get_current_date.setClosure((values, self_instance) => {
            /*object captureRegex = ((List<object>)values[0])[0];*/
        	DateTime date = DateTime.Now;
        	return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>() {
        		{
        			"year",
        			date.Year
        		},
        		{
        			"month",
        			date.Month
        		}
        	});
        }, "main");

        PHPScriptFunction month_days = new PHPScriptFunction();
        month_days.initArrays();
        this.setDictionaryValue("month_days", month_days);
        month_days.prototype = this;
        month_days.setClosure((values, self_instance) => {
            int year = Convert.ToInt32(((List<object>)values[0])[0]);
            int month = Convert.ToInt32(((List<object>)values[0])[1]);
            /*NoobMonth instanceMonth = new NoobMonth();
            return instanceMonth.get_dates(year, month);*/
            if(year == -1) {
                year = DateTime.Now.Year;
            }
            if(month == -1) {
                month = DateTime.Now.Month;
            }
            DateTime date = new DateTime(year, month, 1);
            DateTime nextDate = date.AddMonths(1);
            TimeSpan diff = nextDate-date;

            string daystring = date.DayOfWeek.ToString();

            return this.getInterpretation().makeIntoObjects(new Dictionary<object, object>() {
                {
                    "start_date", date.ToString("yyyy-MM-dd")
                },
                {
                    "number_of_days", diff.Days
                },
                {
                    "day", daystring
                }
            });
        }, "main");

        PHPScriptFunction get_date = new PHPScriptFunction();
        get_date.initArrays();
        this.setDictionaryValue("get_date", get_date);
        get_date.prototype = this;
        get_date.setClosure((values, self_instance) => {
        	object input = ((List<object>)values[0])[0];
        	input = this.parseInputVariable(input);

        	int yearOffset = 0;
        	if(((List<object>)values[0]).Count > 0) {
        		object offsetInput1 = ((List<object>)values[0])[1];
        		offsetInput1 = this.parseInputVariable(offsetInput1);
        		if(yearOffset != null) {
	        		yearOffset = Convert.ToInt32(offsetInput1);
	        	}
        	}
        	int monthOffset = 0;
        	if(((List<object>)values[0]).Count > 1) {
        		object offsetInput2 = ((List<object>)values[0])[2];
        		offsetInput2 = this.parseInputVariable(offsetInput2);
        		if(monthOffset != null) {
	        		monthOffset = Convert.ToInt32(offsetInput2);
	        	}
        	}
        	int dayOffset = 0;
        	if(((List<object>)values[0]).Count > 2) {
        		object offsetInput3 = ((List<object>)values[0])[3];
        		offsetInput3 = this.parseInputVariable(offsetInput3);
        		if(dayOffset != null) {
	        		dayOffset = Convert.ToInt32(offsetInput3);
	        	}
        	}

        	//DateTimeOffset offset = new DateTimeOffset(-yearOffset, -monthOffset, -dayOffset, 0, 0, 0, 0, 0, TimeSpan.Zero/*, null*/);

        	//TimeSpan valueDate = DateTime.Now - offset.DateTime;//DateTime.Now.Add(offset.DateTime);

        	DateTime valueDate = DateTime.Now;
        	valueDate = valueDate.AddYears(yearOffset);
        	valueDate = valueDate.AddMonths(monthOffset);
        	valueDate = valueDate.AddDays(dayOffset);

        	return valueDate.ToString("yyyy-MM-dd");
        }, "main");

        PHPScriptFunction diff_from_now = new PHPScriptFunction();
        diff_from_now.initArrays();
        this.setDictionaryValue("diff_from_now", diff_from_now);
        diff_from_now.prototype = this;
        diff_from_now.setClosure((values, self_instance) => {
        	object input = ((List<object>)values[0])[0];
        	input = this.parseInputVariable(input);

        	int yearOffset = 0;
        	if(((List<object>)values[0]).Count > 0) {
        		object offsetInput1 = ((List<object>)values[0])[1];
        		offsetInput1 = this.parseInputVariable(offsetInput1);
        		if(yearOffset != null) {
	        		yearOffset = Convert.ToInt32(offsetInput1);
	        	}
        	}
        	int monthOffset = 0;
        	if(((List<object>)values[0]).Count > 1) {
        		object offsetInput2 = ((List<object>)values[0])[2];
        		offsetInput2 = this.parseInputVariable(offsetInput2);
        		if(monthOffset != null) {
	        		monthOffset = Convert.ToInt32(offsetInput2);
	        	}
        	}
        	int dayOffset = 0;
        	if(((List<object>)values[0]).Count > 2) {
        		object offsetInput3 = ((List<object>)values[0])[3];
        		offsetInput3 = this.parseInputVariable(offsetInput3);
        		if(dayOffset != null) {
	        		dayOffset = Convert.ToInt32(offsetInput3);
	        	}
        	}

        	DateTimeOffset offset = new DateTimeOffset(yearOffset, monthOffset, dayOffset, 0, 0, 0, 0, 0, TimeSpan.Zero);

        	TimeSpan valueDate = DateTime.Now - offset.DateTime;

        	return valueDate.Days;
        }, "main");
    }
}

class NoobMonth {
    /*public Dictionary<object, object> date_value;
	public int number_of_days;

	public void assign_values(string date_value, int number_of_days, int day) {
		List<string> date_value_split
	}*/

    public Dictionary<object, object> get_dates(int year, int month) {
        var dates = new List<string>();
        var date = new DateTime(year, month, 1);
        var dateStatic = new DateTime(year, month, 1);
        var dateStaticLast = dateStatic.AddMonths(-1);
        var dateStaticNext = dateStatic.AddMonths(1);
        date = date.AddDays(-7);
        Dictionary<object, object> interval = new Dictionary<object, object>();
        while(date.DayOfWeek != DayOfWeek.Monday) {
            date = date.AddDays(1);
            if(date.Month == month && !interval.ContainsKey("from")) {
                interval["from"] = date.ToString("yyyy-MM-dd");
            }
        }
        while(date.Month == month - 1) {
            dates.Add(date.ToString("yyyy-MM-dd"));
            date = date.AddDays(1);
            if(date.Month == month && !interval.ContainsKey("from")) {
                interval["from"] = date.ToString("yyyy-MM-dd");
            }
        }
        while(date.Month == month) {
            dates.Add(date.ToString("yyyy-MM-dd"));
            date = date.AddDays(1);
            if(date.Month == month && !interval.ContainsKey("from")) {
                interval["from"] = date.ToString("yyyy-MM-dd");
            }
        }
        while(date.DayOfWeek != DayOfWeek.Monday) {
            if(date.Month > month && !interval.ContainsKey("to")) {
                interval["to"] = date.ToString("yyyy-MM-dd");
            }
            dates.Add(date.ToString("yyyy-MM-dd"));
            date = date.AddDays(1);
        }

        return new Dictionary<object, object>() {
            {
                "interval",
                interval
            },
            {
                "month_days",
                dates
            },
            {
                "set_year",
                year.ToString()
            },
            {
                "set_month",
                month.ToString()
            },
            {
                "last_month",
                dateStaticLast.Month.ToString()
            },
            {
                "last_month_year",
                dateStaticLast.Year.ToString()
            },
            {
                "next_month",
                dateStaticNext.Month.ToString()
            },
            {
                "next_month_year",
                dateStaticNext.Year.ToString()
            }
        };
    }
}

class PHPRegex : PHPScriptObject {

    public void init(PHPScriptFunction context) {
        
        this.initArrays();
        this.globalObject = true;

        PHPScriptFunction reg_match = new PHPScriptFunction();
        reg_match.initArrays();
        this.setDictionaryValue("reg_match", reg_match);
        reg_match.prototype = this;
        reg_match.setClosure((values, self_instance) => {
            object captureRegex = ((List<object>)values[0])[0];
            object value = ((List<object>)values[0])[1];
            
            Regex reg = new Regex((string)captureRegex);

            string valueString = (string)value;

            Dictionary<object, object> results = new Dictionary<object, object>();

            MatchCollection matches = reg.Matches(valueString);
            foreach(Match m in matches) {
                if(!results.ContainsKey(m.Index)) {
                    results[m.Index] = new Dictionary<object, object>() {
                        { "length_value", m.Length },
                        { "value", m.Value }
                    };
                }
            }
            return this.getInterpretation().makeIntoObjects(results);
        }, "main");
    }
}

class PHPStrings : PHPScriptObject {
    public void init(PHPScriptFunction context) {
        
        this.initArrays();
        this.globalObject = true;

        PHPScriptFunction substr = new PHPScriptFunction();
        substr.initArrays();
        this.setDictionaryValue("substr", substr);
        substr.prototype = this;
        substr.setClosure((values, self_instance) => {
            object input = this.parseInputVariable(((List<object>)values[0])[0]);
            int inputStart = (int)this.parseInputVariable(((List<object>)values[0])[1]);
            string stringValue = input.ToString();
            int inputLengthNumber = stringValue.Length - 1;
            if(((List<object>)values[0]).Count > 2) {
                inputLengthNumber = Convert.ToInt32(this.parseInputVariable(((List<object>)values[0])[2]));
            } else {
                inputLengthNumber = inputLengthNumber - inputStart + 1;
            }
            if(inputLengthNumber < 0) {
                inputLengthNumber = stringValue.Length - inputLengthNumber;
            }

            return stringValue.Substring(inputStart, inputLengthNumber);
        }, "main");

        PHPScriptFunction strlen = new PHPScriptFunction();
        strlen.initArrays();
        this.setDictionaryValue("strlen", strlen);
        strlen.prototype = this;
        strlen.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue =  this.parseInputVariable(input).ToString();
            return stringValue.Length;
        }, "main");

        PHPScriptFunction lower = new PHPScriptFunction();
        lower.initArrays();
        this.setDictionaryValue("lower", lower);
        lower.prototype = this;
        lower.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue =  this.parseInputVariable(input).ToString();
            return stringValue.ToLower();
        }, "main");

        PHPScriptFunction apos = new PHPScriptFunction();
        apos.initArrays();
        this.setDictionaryValue("apos", apos);
        apos.prototype = this;
        apos.setClosure((values, self_instance) => {
            return "'";
        }, "main");

        PHPScriptFunction encaps_quotes = new PHPScriptFunction();
        encaps_quotes.initArrays();
        this.setDictionaryValue("encaps_quotes", encaps_quotes);
        encaps_quotes.prototype = this;
        encaps_quotes.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue = input.ToString();
            return "'"+stringValue+"'";
        }, "main");
        
        
        PHPScriptFunction str_split = new PHPScriptFunction();
        str_split.initArrays();
        this.setDictionaryValue("str_split", str_split);
        str_split.prototype = this;
        str_split.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue =  this.parseInputVariable(input).ToString();
            //object input2 = ((List<object>)values[0])[1];
            //string delimiter = input2.ToString();
            List<object> res = new List<object>();
            foreach(char c in stringValue.ToCharArray().ToList<char>()) {
                res.Add(c.ToString());
            }
            return this.getInterpretation().makeIntoObjects(res);
        }, "main");

        PHPScriptFunction split = new PHPScriptFunction();
        split.initArrays();
        this.setDictionaryValue("split", split);
        split.prototype = this;
        split.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue = this.parseInputVariable(input).ToString();
            object input2 = ((List<object>)values[0])[1];
            string delimiter = this.parseInputVariable(input2).ToString();
            return this.getInterpretation().makeIntoObjects(new List<object>(stringValue.Split(delimiter)));
        }, "main");

        
        PHPScriptFunction strrev = new PHPScriptFunction();
        strrev.initArrays();
        this.setDictionaryValue("strrev", strrev);
        strrev.prototype = this;
        strrev.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue = this.parseInputVariable(input).ToString();
            List<string> res = stringValue.Split().ToList<string>();
            res.Reverse();
            return string.Join("", res);
        }, "main");

        PHPScriptFunction trim = new PHPScriptFunction();
        trim.initArrays();
        this.setDictionaryValue("trim", trim);
        trim.prototype = this;
        trim.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue = this.parseInputVariable(input).ToString();
            return stringValue.Trim();
        }, "main");

        PHPScriptFunction strpos = new PHPScriptFunction();
        strpos.initArrays();
        this.setDictionaryValue("strpos", strpos);
        strpos.prototype = this;
        strpos.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue = this.parseInputVariable(input).ToString();
            object inputB = ((List<object>)values[0])[1];
            string stringValueB = this.parseInputVariable(inputB).ToString();
            return stringValue.IndexOf(stringValueB);
        }, "main");

        PHPScriptFunction compare = new PHPScriptFunction();
        compare.initArrays();
        this.setDictionaryValue("compare", compare);
        compare.prototype = this;
        compare.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue =  this.parseInputVariable(input).ToString();
            object inputB = ((List<object>)values[0])[1];
            string stringValueB =  this.parseInputVariable(inputB).ToString();
            return stringValue.CompareTo(stringValueB);
        }, "main");

        PHPScriptFunction str_replace = new PHPScriptFunction();
        str_replace.initArrays();
        this.setDictionaryValue("str_replace", str_replace);
        str_replace.prototype = this;
        str_replace.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            string stringValue = this.parseInputVariable(input).ToString();
            object inputB = ((List<object>)values[0])[1];
            string stringValueB = this.parseInputVariable(inputB).ToString();
            object inputC = ((List<object>)values[0])[1];
            string stringValueC = this.parseInputVariable(inputC).ToString();

            return stringValue.Replace(stringValueB, stringValueC);
        }, "main");

        
        PHPScriptFunction join = new PHPScriptFunction();
        join.initArrays();
        this.setDictionaryValue("join", join);
        join.prototype = this;
        join.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            //string stringValue = input.ToString();
            toJSON jsonInstance = new toJSON();
            List<string> listInput = new List<string>();
            List<object> listInputValue = (List<object>)jsonInstance.convert(this.parseInputVariable(input));
            foreach(object o in listInputValue) {
            	listInput.Add(o.ToString());
            }
            object inputB = ((List<object>)values[0])[1];
            string stringValueB = this.parseInputVariable(inputB).ToString();
            return string.Join(stringValueB, listInput);
        }, "main");
    }
}

class PHPDataWrap : PHPScriptObject {
	 public void init(PHPScriptFunction context, string basePath) {//Dictionary<string, object> instances) {
        
        this.initArrays();
        this.globalObject = true;
        Dictionary<string, object> instances = null;

        if(instances == null) {
        	instances = new Dictionary<string, object>();
        }

        instances["main"] = new PHPData();
        ((PHPData)instances["main"]).init(context, basePath);

        PHPScriptFunction fetch = new PHPScriptFunction();
        fetch.initArrays();
        this.setDictionaryValue("fetch", fetch);
        fetch.prototype = this;
        fetch.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            input = self_instance.parseInputVariable(input);
            string inputString = input.ToString();
            return instances[inputString];
            /*int inputStart = (int)((List<object>)values[0])[1];
            string stringValue = input.ToString();
            int inputLengthNumber = stringValue.Length - 1;
            if(((List<object>)values[0]).Count > 2) {
                inputLengthNumber = (int)((List<object>)values[0])[2];
            } else {
                inputLengthNumber = inputLengthNumber - inputStart + 1;
            }
            if(inputLengthNumber < 0) {
                inputLengthNumber = stringValue.Length - inputLengthNumber;
            }

            return stringValue.Substring(inputStart, inputLengthNumber);*/
        }, "main");

    }
}

class PHPData : PHPScriptObject {

	public local_data local_instance;

	public void init(PHPScriptFunction context, string basePath) {
        
        this.initArrays();
        this.globalObject = true;

        this.local_instance = new local_data();
        this.local_instance.init(basePath);

        PHPScriptFunction execute = new PHPScriptFunction();
        execute.initArrays();
        this.setDictionaryValue("execute", execute);
        execute.prototype = this;
        execute.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            input = self_instance.parseInputVariable(input);
            string inputString = input.ToString();

            object input_parameters = ((List<object>)values[0])[1];
            input_parameters = self_instance.parseInputVariable(input_parameters);
 
            if(input_parameters is string && input_parameters.ToString() == "[]") {
            	input_parameters = new List<object>();
            } else {
	            toJSON tojsonInstance = new toJSON();
	            input_parameters = tojsonInstance.convert(input_parameters);
	        }

			this.local_instance.execute(inputString, (List<object>)input_parameters);
			return true;
            //return this.getInterpretation().makeIntoObjects();
        }, "main");


        PHPScriptFunction get_rows = new PHPScriptFunction();
        get_rows.initArrays();
        this.setDictionaryValue("get_rows", get_rows);
        get_rows.prototype = this;
        get_rows.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            input = self_instance.parseInputVariable(input);
            string inputString = input.ToString();

            object input_parameters = ((List<object>)values[0])[1];
            input_parameters = self_instance.parseInputVariable(input_parameters);
            if(input_parameters is string && input_parameters.ToString() == "[]") {
            	input_parameters = new List<object>();
            } else {
	            toJSON tojsonInstance = new toJSON();
	            input_parameters = tojsonInstance.convert(input_parameters);
	        }

            return this.getInterpretation().makeIntoObjects(this.local_instance.query_(inputString, (List<object>)input_parameters));
        }, "main");

        PHPScriptFunction get_row = new PHPScriptFunction();
        get_row.initArrays();
        this.setDictionaryValue("get_row", get_row);
        get_row.prototype = this;
        get_row.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            input = self_instance.parseInputVariable(input);
            string inputString = input.ToString();

            object input_parameters = ((List<object>)values[0])[1];
            input_parameters = self_instance.parseInputVariable(input_parameters);

            if((input_parameters is string && input_parameters.ToString() == "[]") || input_parameters == null) {
            	input_parameters = new List<object>();
            } else {
	            toJSON tojsonInstance = new toJSON();
	            input_parameters = tojsonInstance.convert(input_parameters);
	        }

	        System.Diagnostics.Debug.WriteLine("query_: "+inputString+" "+input_parameters);

            List<object> res = this.local_instance.query_(inputString, (List<object>)input_parameters);
            if(res.Count == 0) {
            	return 0;
            }
            return this.getInterpretation().makeIntoObjects(res[0]);
        }, "main");

        PHPScriptFunction insert = new PHPScriptFunction();
        insert.initArrays();
        this.setDictionaryValue("_", insert);
        insert.prototype = this;
        insert.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            input = self_instance.parseInputVariable(input);
            string inputString = input.ToString();

            object input_parameters = ((List<object>)values[0])[1];
            input_parameters = self_instance.parseInputVariable(input_parameters);

            PHPScriptObject inputObj = (PHPScriptObject)input_parameters;
            List<object> keys = inputObj.getKeys();

            List<object> valuesDict = (List<object>)((Dictionary<object, object>)inputObj.getDictionary()).Values.ToList<object>();

            /*List<object> input_parameter_values = new List<object>();
            foreach(object key in keys) {

            }*/
	        List<object> inputPars = new List<object>();
	        bool isUpdate = false;
            if(inputString.IndexOf("UPDATE ") == 0) {
            	isUpdate = true;
            }
            toJSON tojsonInstance = new toJSON();
            input_parameters = tojsonInstance.convert(inputObj);
            string id = null;
            if(((Dictionary<object, object>)input_parameters).ContainsKey("id") && isUpdate) {
            	id = ((Dictionary<object, object>)input_parameters)["id"].ToString();
            	((Dictionary<object, object>)input_parameters).Remove("id");
            }

            foreach(object key in keys) {
            	if(key.ToString() != "id" || !isUpdate) {
            		inputPars.Add(((Dictionary<object, object>)input_parameters)[key]);
            	}
            }
            if(id != null && isUpdate) {
            	inputPars.Add(id);
            }
			/*} else {
			}*/
            //input_parameters = ((Dictionary<object, object>)input_parameters).Values.ToList<object>();
			System.Diagnostics.Debug.WriteLine("query: "+inputString+" pars: "+JsonConvert.SerializeObject(inputPars));
            this.local_instance.execute(inputString, (List<object>)inputPars);
            return 0;
            //return this.getInterpretation().makeIntoObjects();
        }, "main");


        PHPScriptFunction generate_uuid = new PHPScriptFunction();
        generate_uuid.initArrays();
        this.setDictionaryValue("generate_uuid", generate_uuid);
        generate_uuid.prototype = this;
        generate_uuid.setClosure((values, self_instance) => {
            return System.Guid.NewGuid().ToString();
        }, "main");

        PHPScriptFunction table_columns = new PHPScriptFunction();
        table_columns.initArrays();
        this.setDictionaryValue("table_columns", table_columns);
        table_columns.prototype = this;
        table_columns.setClosure((values, self_instance) => {
            object input = ((List<object>)values[0])[0];
            input = self_instance.parseInputVariable(input);
            string inputString = input.ToString();
            return this.getInterpretation().makeIntoObjects(this.local_instance.table_columns(inputString));
        }, "main");
        //System.Guid.NewGuid()
    }
}

class local_data {

    SqliteConnection connection;

    public void init(string inputPath) {
    	string path = inputPath;//Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        /*if(set_path != null) {
            path = set_path;
        }*/
        //Directory.CreateDirectory(path);
        //string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string instance_name = "media_index_main";
        /*if(set_instance_name != null) {
            instance_name = set_instance_name;
        }*/
        path = Path.Combine(path, instance_name+".db");
        //System.Diagnostics.Debug.WriteLine("dbpath: "+path);

        try {
            if(!System.IO.File.Exists(path)) {
                FileStream fs = File.Create(path);
                fs.Close();
            }

            /*SqliteConnectionStringBuilder connectionBuilder = new SqliteConnectionStringBuilder() {
                DataSource = path,
                //thread
                Pooling = true,
            };*/
            //connectionBuilder.

            this.connection = new SqliteConnection("Data Source="+path+";");
            this.connection.Open();
            //this.connection.t

        } catch(Exception e) {
            //System.Diagnostics.Debug.WriteLine("error");
            //System.Diagnostics.Debug.WriteLine(e);
            //page.error_message("Could not initialize database");
        }

    }

   public void execute(string query_string,  List<object> query_values=null) {
        
        int counter = 0;
        int set_id = -1;
        if(query_values == null || query_values.ToString() == "[]") {
        	query_values = new List<object>();
        }
        if(query_values.Count > 0) {
            /*if(query_values.ContainsKey("id")) {
                set_id = Convert.ToInt32(query_values["id"]);
                this.set_insert_id = set_id;
                query_values.Remove("id");
            }*/
            List<string> split = new List<string>(query_string.Split('?'));
            string set_query = "";
            foreach(string split_value in split) {
            
                set_query += split_value;
                if(counter < split.Count-1) {
                    set_query += "@param"+counter.ToString();
                }
                counter++;
            }
            query_string = set_query;
            /*if(this.set_insert_id != -1) {
                query_values.Add("id", this.set_insert_id);
            }*/
        }
        ////System.Diagnostics.Debug.WriteLine(query_string);
        SqliteCommand query = new SqliteCommand(query_string, this.connection);
        if(query_values != null) {
            counter = 0;
            foreach(object value in query_values) {
                //SqliteParameter par = new SqliteParameter(
                string key = "param"+counter.ToString();
                SqliteParameter parameter = new SqliteParameter(key, value);
                ////System.Diagnostics.Debug.WriteLine(value.Value);
                query.Parameters.Add(parameter);
                //query.Parameters.A
                counter++;
            }
            /*if(this.set_insert_id != -1) {
                string key = "param"+counter.ToString();
                SqliteParameter parameter = new SqliteParameter(key, set_id);
                ////System.Diagnostics.Debug.WriteLine(set_id);
                query.Parameters.Add(parameter);
            }*/
        }
        query.ExecuteNonQuery();
    }

     public string insert(Dictionary<string, object> query_values, string query_string) {
        string return_id = null;
        //statement s = new statement(this.connection);
        SqliteCommand query = new SqliteCommand(query_string, this.connection);
        if(query_values != null && query_values.ToString() != "[]") {
            foreach(KeyValuePair<string, object> value in query_values) {
                SqliteParameter parameter = new SqliteParameter(value.Key, value.Value);
                query.Parameters.Add(parameter);
            }
        }
        int result = query.ExecuteNonQuery();
        return "-1";
    }

    /*public List<Dictionary<string, string>> query(string query_string, List<string> columns, Dictionary<string, string> query_values=null, Func<SqliteDataReader, bool> callback=null) {
        SqliteCommand query = new SqliteCommand(query_string, this.connection);
        if(query_values != null) {
            foreach(KeyValuePair<string, string> value in query_values) {
                SqliteParameter parameter = new SqliteParameter(value.Key, value.Value);
                query.Parameters.Add(parameter);
            }
        }
        SqliteDataReader result = query.ExecuteReader();

        List<Dictionary<string, string>> return_result = new List<Dictionary<string, string>>();

        if(callback != null) {
            callback(result);
            return return_result;
        }

        while(result.Read()) {
            Dictionary<string, string> row = new Dictionary<string, string>();
            int counter = 0;
            foreach(string column in columns) {
                if(column == "id" || column.Contains("_id") || column.Contains("_count")) {
                    row.Add(column, result.GetInt32(counter).ToString());
                } else {
                    row.Add(column, result.GetString(counter));
                }
                counter++;
            }
            return_result.Add(row);
        }
        return return_result;
    }

    public List<Dictionary<string, string>> query(string query_string, Dictionary<string, string> query_values=null) {
        SqliteCommand query = new SqliteCommand(query_string, this.connection);
        if(query_values != null) {
            foreach(KeyValuePair<string, string> value in query_values) {
                SqliteParameter parameter = new SqliteParameter(value.Key, value.Value);
                query.Parameters.Add(parameter);
            }
        }
        SqliteDataReader result = query.ExecuteReader();

        List<Dictionary<string, string>> return_result = new List<Dictionary<string, string>>();

        DataTable data_table = result.GetSchemaTable();

        List<string> columns = new List<string>();

        foreach(DataRow row in data_table.Rows) {
            columns.Add(row.ItemArray[0].ToString());

        }

        while(result.Read()) {
            Dictionary<string, string> row = new Dictionary<string, string>();
            int counter = 0;
            while(counter < result.FieldCount) {
                if(row.ContainsKey(columns[counter])) {
                    row[columns[counter]] = result[counter].ToString();
                } else {
                    row.Add(columns[counter], result[counter].ToString());
                }
                counter++;
            }
            return_result.Add(row);
        }
        return return_result;
    }

    public string query_json(string query_string, string query_values_json=null) {
        Dictionary<string, string> query_values = new Dictionary<string, string>();
        if(query_values_json != null) {
            query_values = JsonConvert.DeserializeObject<Dictionary<string, string>>(query_values_json);
           
        }
        return JsonConvert.SerializeObject(this.query(query_string, query_values));
    }

    public string query_json_alt(string query_string, string query_values_json=null) {
        ////////System.Diagnostics.Debug.WriteLine(query_values_json);
        Dictionary<string, object> query_values = new Dictionary<string, object>();;
        if(query_values_json != null && query_values_json != "[]") {
            query_values = JsonConvert.DeserializeObject<Dictionary<string, object>>(query_values_json);

        }
        return JsonConvert.SerializeObject(this.query_(query_string, query_values));
    }

    public void execute_json(string query, string json_values="[]") {
        ////////System.Diagnostics.Debug.WriteLine(json_values);
        Dictionary<string, object> values = new Dictionary<string, object>();
        if(json_values != "[]") {
            values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_values);
        }
        this.execute(query, values);
    }

    public string insert_json(string json_values, string table) {
        Dictionary<string, object> values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json_values);
        return this.insert(values, table);
    }

    private int set_insert_id = -1;

    public string insert(Dictionary<string, object> values, string table) {
        string return_id = null;
        statement s = new statement(this.connection);
        SqliteCommand command = s.generate(values, table);
        int result = command.ExecuteNonQuery();
        return "-1";
    }

    public int last_insert_id() {
        return Convert.ToInt32(this.query_("SELECT last_insert_rowid() as id", new Dictionary<string, object>())[0]["id"]);
    }*/

    public List<object> query_(string query_string, List<object> query_values=null) {
		//SqliteDataReader sqliteDataReader = new SqliteCommand(query_string, this.connection).ExecuteReader();
		//System.Diagnostics.Debug.WriteLine("query values : "+JsonConvert.SerializeObject(query_values));
        if(query_values == null || query_values.ToString() == "[]") {
        	query_values = new List<object>();
        }
        int counter = 0;
        if(query_values.Count > 0) {
            List<string> split = new List<string>(query_string.Split('?'));
            string set_query = "";
            foreach(string split_value in split) {
            
                set_query += split_value;
                if(counter < split.Count-1) {
                    set_query += "@param"+counter.ToString();
                }
                counter++;
            }
            query_string = set_query;
        }
        
        //System.Diagnostics.Debug.WriteLine(query_string);
        SqliteCommand query = new SqliteCommand(query_string, this.connection);
        if(query_values != null) {
            counter = 0;
            foreach(object value in query_values) {
                SqliteParameter parameter = new SqliteParameter("param"+counter.ToString(), value.ToString());
                query.Parameters.Add(parameter);

                //System.Diagnostics.Debug.WriteLine("params: "+"param"+counter.ToString()+" value: "+value);

                counter++;
            }
        }
        //System.Diagnostics.Debug.WriteLine("execute reader");
        List<object> dictionaryList = new List<object>();
        SqliteDataReader sqliteDataReader;
        try {
        sqliteDataReader = query.ExecuteReader(); //CommandBehavior.SchemaOnly
        ////System.Diagnostics.Debug.WriteLine("after execute reader");
        ////System.Diagnostics.Debug.WriteLine("after execute reader1");
		DataTable schemaTable = ((DbDataReader) sqliteDataReader).GetSchemaTable();
		////System.Diagnostics.Debug.WriteLine(schemaTable);
		////System.Diagnostics.Debug.WriteLine(schemaTable.Rows.Count);
		
		List<string> stringList = new List<string>();
		foreach (DataRow row in (InternalDataCollectionBase) schemaTable.Rows) {
			//stringList.Add(row.Field<string>("ColumnName"));
            //System.Diagnostics.Debug.WriteLine("column: "+ row.ItemArray[0].ToString());
			stringList.Add(row.ItemArray[0].ToString());
		}
		while(((DbDataReader) sqliteDataReader).Read()) {
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			for(int index = 0; index < ((DbDataReader) sqliteDataReader).FieldCount; ++index) {
				string keyString = stringList[index].ToString();
				if(!dictionary.ContainsKey(keyString)) {
					if(((DbDataReader) sqliteDataReader).IsDBNull(index)) {
						dictionary.Add(keyString, "");
					} else {
						dictionary.Add(keyString, ((DbDataReader) sqliteDataReader)[index]);
					}
				}
			}
			////System.Diagnostics.Debug.WriteLine("add dict "+dictionary.Keys.Count);
			dictionaryList.Add(dictionary);
		}
		//System.Diagnostics.Debug.WriteLine("dict list : "+dictionaryList.Count);
		} catch(Exception exc) {
			System.Diagnostics.Debug.WriteLine(exc);
			System.Diagnostics.Debug.WriteLine(exc.Message);
		}
		return dictionaryList;
	}

    public List<object> table_columns(string table_name) {
        List<object> columns = new List<object>();
        List<object> result = this.query_("DROP TABLE IF EXISTS table_info; CREATE TEMPORARY TABLE table_info as SELECT * FROM pragma_table_info('" + table_name + "'); SELECT name FROM table_info;");
        foreach(object result_item in result) {
            columns.Add(((Dictionary<object, object>)result_item)["name"].ToString());
        }
        return columns;
    }

    /*public string get_table_columns(string table_name) {
        ////////System.Diagnostics.Debug.WriteLine("inside get_table_columns: "+table_name);
        List<string> columns = new List<string>();
        ////////System.Diagnostics.Debug.WriteLine("DROP TABLE IF EXISTS table_info; CREATE TEMPORARY TABLE table_info as SELECT * FROM pragma_table_info('" + table_name + "'); SELECT name FROM table_info;");
        List<Dictionary<string, object>> result = this.query_("DROP TABLE IF EXISTS table_info; CREATE TEMPORARY TABLE table_info as SELECT * FROM pragma_table_info('" + table_name + "'); SELECT name FROM table_info;");
        foreach(Dictionary<string, object> result_item in result) {
            columns.Add(result_item["name"].ToString());
        }
        return JsonConvert.SerializeObject(columns);
    }*/
}