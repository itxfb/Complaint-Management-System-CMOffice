Bridge.assembly("BridgeClassLib", function ($asm, globals) {
    "use strict";

    Bridge.define("BridgeClassLib.Form.FormValidationHandler", {
        methods: {
            ValidateForm: function (listInputElement) {
                var inputElement = null;
                var isValid = true;

                for (var i = 0; i < listInputElement.Count; i = (i + 1) | 0) {
                    inputElement = listInputElement.getItem(i);
                    if (inputElement.type === "text") {
                        if (!this.OnTextBoxValidate(inputElement, Bridge.cast(inputElement.FormField, BridgeDTO.Form.DynamicForm.TextFormField), true)) {
                            isValid = false;
                        }

                    }
                }
                return isValid;
            },
            OnTextBoxValidate: function (inputElement, fieldData, isFormValidation) {
                if (isFormValidation === void 0) { isFormValidation = false; }
                var isValid = true;
                var spanElement = Bridge.cast(inputElement.htmlValidationSpanElement, HTMLSpanElement);
                var minChar = fieldData.GetPermissionValueInt("Validation_Input_Min_Char");
                var maxChar = fieldData.GetPermissionValueInt("Validation_Input_Max_Char");

                if (isFormValidation) {
                    if (System.String.equals(fieldData.GetStrPermissionValue("Validation_Is_Required"), "True") && inputElement.value.length === 0) {
                        spanElement.className = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                        spanElement.innerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                        isValid = false;
                    } else if (minChar !== -1 && inputElement.value.length < minChar) {
                        spanElement.className = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                        spanElement.innerHTML = fieldData.GetStrPermissionValue("Validation_Input_Min_Char_Message");
                        isValid = false;
                    }
                    return isValid;
                }


                if (System.String.equals(fieldData.GetStrPermissionValue("Input_Case"), "Upper")) {
                    inputElement.value = inputElement.value.toUpperCase();
                } else if (System.String.equals(fieldData.GetStrPermissionValue("Input_Case"), "Lower")) {
                    inputElement.value = inputElement.value.toLowerCase();
                }

                if (System.String.equals(fieldData.GetStrPermissionValue("Input_Format"), "Numeric")) {
                    inputElement.value = System.String.fromCharArray(System.Linq.Enumerable.from(inputElement.value).where(function (c) {
                                return System.Char.isDigit(c);
                            }).ToArray(System.Char));
                }

                if (System.String.equals(fieldData.GetStrPermissionValue("Validation_Is_Required"), "True")) {
                    if (inputElement.value.length > 0) {
                        spanElement.className = fieldData.GetStrPermissionValue("Validation_Span_Class_Valid");
                        spanElement.innerHTML = "";
                        isValid = true;
                    } else {
                        spanElement.className = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                        spanElement.innerHTML = fieldData.GetStrPermissionValue("Validation_Is_Required_Message");
                        isValid = false;
                    }
                }


                if (maxChar !== -1) {
                    inputElement.value = inputElement.value.substr(0, maxChar);
                }

                if (minChar !== -1) {
                    if (inputElement.value.length === minChar) {
                        spanElement.className = fieldData.GetStrPermissionValue("Validation_Span_Class_Valid");
                        spanElement.innerHTML = "";
                        isValid = true;
                    } else if (inputElement.value.length > 0 && inputElement.value.length < minChar) {
                        spanElement.className = fieldData.GetStrPermissionValue("Validation_Span_Class_Error");
                        spanElement.innerHTML = fieldData.GetStrPermissionValue("Validation_Input_Min_Char_Message");
                        isValid = false;
                    }
                }

                inputElement.setAttribute("data-is-valid", System.Boolean.toString(isValid));
                return isValid;
            }
        }
    });
});
