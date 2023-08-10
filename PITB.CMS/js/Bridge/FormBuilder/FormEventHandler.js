Bridge.assembly("BridgeClassLib", function ($asm, globals) {
    "use strict";

    Bridge.define("BridgeClassLib.Form.FormEventHandler", {
        methods: {
            OnFormSubmit: function (e) {
                try {
                    Bridge.Reflection.midel(Bridge.Reflection.getMembers(System.Console, 8, 284, "WriteLine", System.Array.init([System.String], Function)), null).apply(null, System.Array.init(["Hello"], System.String));
                    var htmlFormElement = Bridge.cast(e.currentTarget, HTMLFormElement);
                    var jqFormElement = $(htmlFormElement);
                    var formParams = Bridge.cast(jqFormElement.data("data"), BridgeDTO.Form.DynamicForm.FormAjaxParams);




                    var listElement = new (System.Collections.Generic.List$1(HTMLInputElement)).ctor();
                    $("input[data-form-tag='" + (formParams.FormTag || "") + "']").each(function (id, element) {
                        listElement.add(Bridge.cast(element, HTMLInputElement));
                    });
                    var formValidationHandler = new BridgeClassLib.Form.FormValidationHandler();
                    var isFormValid = formValidationHandler.ValidateForm(listElement);
                    if (!isFormValid) {
                        e.preventDefault();
                    }
                } catch (ex) {
                    ex = System.Exception.create(ex);

                    throw ex;
                }
            },
            ProcessFormInput: function (jq) {
                return "asdasd";
            },
            MunnaKaka: function () {
                return "asdaiiii ---- asdasdasdasdsdfsdfsdf";
            },
            OnOptionListChange: function (e) {
                var $t;
                var selectElement = e.currentTarget;
                var optionFormField = Bridge.cast(selectElement.FormField, BridgeDTO.Form.DynamicForm.OptionsFormField);
                var parentOf = optionFormField.GetStrPermissionValue("Parent_Of");
                if (!System.String.isNullOrEmpty(parentOf)) {
                    var childIdArr = System.String.split(parentOf, [44].map(function (i) {{ return String.fromCharCode(i); }}));
                    $t = Bridge.getEnumerator(childIdArr);
                    try {
                        while ($t.moveNext()) {
                            var childId = $t.Current;
                            var childElement = document.getElementById(childId);
                            var childFF = Bridge.cast(childElement.FormField, BridgeDTO.Form.DynamicForm.OptionsFormField);
                            var apiParams = System.String.replaceAll(childFF.GetStrPermissionValue("Api_Params"), "@val", selectElement.value);
                            this.PouplateDropdownList("/DynamicForm/GetDynamicCategories", apiParams, childElement);
                        }
                    } finally {
                        if (Bridge.is($t, System.IDisposable)) {
                            $t.System$IDisposable$Dispose();
                        }
                    }

                }
                window.alert(selectElement.value);
            },
            PouplateDropdownList: function (url, urlParams, elementToPopulate) {
                $.ajax({ url: url, type: "Post", cache: false, data: urlParams, success: function (data, str, jqxhr) {
                    var $t;
                    var str2 = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    var listOption = Newtonsoft.Json.JsonConvert.DeserializeObject(str2, System.Collections.Generic.List$1(BridgeDTO.Form.DynamicForm.Option));
                    $(elementToPopulate).empty();
                    for (var i = 0; i < listOption.Count; i = (i + 1) | 0) {

                        var htmlInputElement = ($t = document.createElement("option"), $t.value = listOption.getItem(i).Value, $t.innerHTML = listOption.getItem(i).Text, $t);
                        elementToPopulate.appendChild(htmlInputElement);
                    }
                } });
            },
            OnTextInput: function (e) {
                var kEvent = e;
                var inputElement = kEvent.currentTarget;

                var spanElement = Bridge.cast(inputElement.htmlValidationSpanElement, HTMLSpanElement);
                var formField = Bridge.cast(inputElement.FormField, BridgeDTO.Form.DynamicForm.TextFormField);

                var formVHandler = new BridgeClassLib.Form.FormValidationHandler();
                formVHandler.OnTextBoxValidate(inputElement, formField);
            }
        }
    });
});
