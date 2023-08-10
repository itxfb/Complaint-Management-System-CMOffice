Bridge.assembly("BridgeClassLib", function ($asm, globals) {
    "use strict";

    Bridge.define("BridgeClassLib.Form.FormBuilder", {
        statics: {
            ctors: {
                init: function () {
                    Bridge.ready(this.Main);
                }
            },
            methods: {
                Main: function () {

                }
            }
        },
        $entryPoint: true,
        fields: {
            formEventHandler: null
        },
        methods: {
            CreateForm: function (container, formAjaxParams, template) {
                var $t, $t1;
                this.formEventHandler = new BridgeClassLib.Form.FormEventHandler();
                $t = Bridge.getEnumerator(template.Fields);
                try {
                    while ($t.moveNext()) {
                        var field = $t.Current;
                        container.append(this.CreateFormField(field));
                    }
                } finally {
                    if (Bridge.is($t, System.IDisposable)) {
                        $t.System$IDisposable$Dispose();
                    }
                }
                var htmlFormElement = Bridge.cast(document.getElementById(formAjaxParams.FormId), HTMLFormElement);
                htmlFormElement.addEventListener("submit", Bridge.fn.cacheBind(this.formEventHandler, this.formEventHandler.OnFormSubmit));

                var htmlFormSubmitBtn = ($t1 = document.createElement("input"), $t1.type = "submit", $t1.value = "Submit", $t1.className = "btn btn-primary", $t1.formAction = formAjaxParams.UrlAfterPost, $t1.formMethod = "POST", $t1);



                container.append($("<div>").addClass("form-group").append($("<div>").addClass("col-sm-offset-2 col-sm-10").append($(htmlFormSubmitBtn))));
            },
            CreateFormField: function (template) {
                switch (template.Kind) {
                    case BridgeDTO.Form.DynamicForm.FormFieldType.RadioList: 
                        return this.CreateRadioInput(template.Id, template.Label, Bridge.cast(template, BridgeDTO.Form.DynamicForm.RadioFormField).Options);
                    case BridgeDTO.Form.DynamicForm.FormFieldType.OptionList: 
                        return this.CreateOptionList(Bridge.cast(template, BridgeDTO.Form.DynamicForm.OptionsFormField));
                    case BridgeDTO.Form.DynamicForm.FormFieldType.TextField: 
                        return this.CreateTextInput(Bridge.cast(template, BridgeDTO.Form.DynamicForm.TextFormField));
                    default: 
                        return this.CreateTextInput(Bridge.cast(template, BridgeDTO.Form.DynamicForm.TextFormField));
                }
            },
            CreateRadioInput: function (id, label, listOptions) {
                var $t;
                var divRadio = $("<div>");
                divRadio.addClass("col-sm-10");

                for (var i = 0; i < listOptions.Count; i = (i + 1) | 0) {
                    divRadio.append($("<label>").addClass("radio-inline").append(($t = document.createElement("input"), $t.type = "radio", $t.id = listOptions.getItem(i).Value, $t.name = id, $t.value = listOptions.getItem(i).Text, $t)).append(listOptions.getItem(i).Text));
                }

                return $("<div>").addClass("form-group").append(($t = document.createElement("label"), $t.className = "control-label col-sm-2", $t.htmlFor = id, $t.innerHTML = (label || "") + ":", $t)).append(divRadio);
            },
            CreateOptionList: function (optionsFormField) {
                var $t;

                var divOptionList = $("<div>");
                divOptionList.addClass("col-sm-10");

                var selectElement = document.createElement("select");
                selectElement.className = "form-control";
                for (var i = 0; i < optionsFormField.Options.Count; i = (i + 1) | 0) {

                    var htmlInputElement = ($t = document.createElement("option"), $t.value = optionsFormField.Options.getItem(i).Value, $t.innerHTML = optionsFormField.Options.getItem(i).Text, $t);
                    selectElement.appendChild(htmlInputElement);
                }
                divOptionList.append(selectElement);
                selectElement.addEventListener("change", Bridge.fn.cacheBind(this, this.OnOptionListChange));

                return $("<div>").addClass("form-group").append(($t = document.createElement("label"), $t.className = "control-label col-sm-2", $t.htmlFor = optionsFormField.Id, $t.innerHTML = (optionsFormField.Label || "") + ":", $t)).append(divOptionList);
            },
            CreateTextInput: function (formField) {
                var $t;
                var htmlInputElement = ($t = document.createElement("input"), $t.type = "text", $t.id = formField.Id, $t.name = formField.Label, $t.className = "form-control", $t.placeholder = formField.GetStrPermissionValue("Placeholder"), $t);
                htmlInputElement.setAttribute("data-is-valid", "false");
                var htmlValidationSpanElement = null;
                if (formField.IsPermissionSubStringPresent("Validation")) {
                    htmlValidationSpanElement = ($t = document.createElement("span"), $t.id = "Validate__" + (formField.Id || ""), $t.className = formField.GetStrPermissionValue("Validation_Span_Class_Valid"), $t);
                    htmlInputElement.htmlValidationSpanElement = htmlValidationSpanElement;
                }

                if (formField.IsPermissionSubStringPresent("Input")) {
                    htmlInputElement.FormField = formField;
                    htmlInputElement.addEventListener("input", Bridge.fn.cacheBind(this.formEventHandler, this.formEventHandler.OnTextInput));
                }
                return $("<div>").addClass("form-group").append(($t = document.createElement("label"), $t.className = "control-label col-sm-" + (formField.GetStrPermissionValue("Label_Column") || ""), $t.htmlFor = formField.Id, $t.innerHTML = formField.Label, $t)).append($("<div>").addClass("col-sm-" + (formField.GetStrPermissionValue("Field_Column") || "")).append(htmlInputElement).append(htmlValidationSpanElement));
            },
            AjaxCallOnDropdownChange: function (optionFormField) {
                /* 
                jQuery.Ajax(
                   new AjaxOptions()
                   {
                       Url = url,
                       Type = type,
                       Data = JsonConvert.SerializeObject(optionFormField),
                       Success = delegate(object data, string str, jqXHR jqxhr)
                       {
                           //Window.Alert("Munnay kakon");
                           //JavaScriptSerializer asd = new JavaScriptSerializer();
                           string str2 = JsonConvert.SerializeObject(data);
                           DynamicForm dynamicForm = JsonConvert.DeserializeObject<DynamicForm>(str2);

                           object sd = dynamicForm;
                           DynamicForm asd2 = (DynamicForm)sd;
                           DynamicForm template = new DynamicForm(dynamicForm);
                           App.CreateForm(jQuery.Select(FORM_CONTAINER), template);
                       }
                   });
                */
            },
            OnOptionListChange: function (e) {
                var selectElement = e.currentTarget;

                window.alert(selectElement.value);
            }
        }
    });
});
