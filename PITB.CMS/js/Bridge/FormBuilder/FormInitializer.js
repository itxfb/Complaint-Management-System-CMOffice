Bridge.assembly("BridgeClassLib", function ($asm, globals) {
    "use strict";

    Bridge.define("BridgeClassLib.Form.FormInitializer", {
        statics: {
            methods: {
                Main: function (formParams) {
                    var pramsDiv = Bridge.cast(document.getElementById(formParams), HTMLDivElement);

                    var formAjaxParams = Newtonsoft.Json.JsonConvert.DeserializeObject(pramsDiv.innerHTML, BridgeDTO.Form.DynamicForm.FormAjaxParams);

                    $.ajax({ url: formAjaxParams.Url, cache: false, success: function (data, str, jqxhr) {

                        var template = new BridgeDTO.Form.DynamicForm.DynamicForm.$ctor2(data);
                        var formBuilder = new BridgeClassLib.Form.FormBuilder();
                        formBuilder.CreateForm($("#" + (formAjaxParams.FormId || "")), formAjaxParams, template);
                    } });
                }
            }
        }
    });
});
