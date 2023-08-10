function RegisterLocalValidation(formId, valTag) {
    RegisterValidation(formId, valTag);
    RegisterEventChanges(formId, valTag);
    SetDefaultPostBehaviour(formId)
}

function RegisterValidation(formId, valTag) {

    var isValid = true;
    var isValidToRet = true;
    var tagStr = (IsNull(valTag) || valTag == null) ? '' : '[data-val-tag=' + valTag + ']';

    //$(formId + " input[data-val-tag=" + valTag + "]").each(
    $(formId + " input" + tagStr).each(
        function (index, v) {
            var inputEl = this;//$(this);
            //var dictConfig = GetDictConfiguration($(inputEl).attr("data-val-config"));

            if ($(inputEl).attr("type") == "radio") {
                $(formId + " input[name='" + $(inputEl).attr('data-group-name') + "']").on("change", { formId: formId, valTag: valTag }, function () {
                    isValid = ApplyRadioValidation(formId, inputEl);
                    ShowHideDivOnDropDownChange(inputEl.id, $(inputEl).val(), formId, valTag);
                });
            }
            if ($(inputEl).attr("type") == "text") {
                if ($(inputEl).attr("data-input-format") == "Date") {
                    $(inputEl).datepicker({
                        format: $(inputEl).attr("data-input-date-format"),//'mm/dd/yyyy',
                        autoclose: true,
                        todayHighlight: true,
                    });
                }
                $(formId + " input[name='" + inputEl.name + "']").on("input", { formId: formId, valTag: valTag }, function (event) {
                    isValid = ApplyTextBoxValidation(formId, inputEl/*, dictConfig*/);
                    //return isValid;
                });
            }
            //if ($(inputEl).attr("type") == "textarea") {
            //    $(formId + " input[name='" + inputEl.name + "']").on("input", { formId: formId, valTag: valTag }, function (event) {
            //        isValid = ApplyTextBoxValidation(formId, inputEl/*, dictConfig*/);
            //        //return isValid;
            //    });
            //}
            if (!isValid) {
                isValidToRet = false;
            }
        }
    );

    $(formId + " textarea" + tagStr).each(
        function (index, v) {
            var inputEl = this; //$(this);
            $(formId + " textarea[name='" + inputEl.name + "']").on("input", { formId: formId, valTag: valTag }, function (event) {
                isValid = ApplyTextBoxValidation(formId, inputEl/*, dictConfig*/);
                //return isValid;
            });
        }
    );

    //$(formId + " select[data-val-tag=" + valTag + "]").each(
    $(formId + " select" + tagStr).each(
        function (index, v) {
            var selectEl = this;
            var canPopulateOnStart = $(selectEl).attr("data-val-populate-on-start");

            EnableMultiselectBootstrap(selectEl);



            if (canPopulateOnStart == "True") {
                var apiUrl = $(selectEl).attr("data-val-api-url");
                BindDropDownWithUrl(formId, selectEl, apiUrl);
            }

            var apiHitsCount = 0;
            $(formId + " select[name='" + selectEl.name + "']").on("change", { formId: formId, valTag: valTag }, function () {

                // Bind drop down change on select change
                var selectElToPopulateOnChangeOf = $(formId + " select[data-val-repopulate-onchange-of='" + selectEl.name + "']").each(
                    function (index, element) {
                        var selectElementToPopulate = this;
                        var apiData = null;
                        var apiUrl = $(selectElementToPopulate).attr("data-val-api-url");
                        var apiDataFunc = $(selectElementToPopulate).attr("data-val-api-url-data-func");
                        var selectDefaultId = $(selectElementToPopulate).attr("data-val-select-defaultId-onchange");
                        var ignoreApiHitsCount = $(selectElementToPopulate).attr("data-val-ignore-api-hits-count");
                        ignoreApiHitsCount = !IsUndefinedOrNull(ignoreApiHitsCount) ? parseInt(ignoreApiHitsCount) : 0;

                        if (apiHitsCount >= ignoreApiHitsCount) {
                            var idVal = $(selectEl).val().split('___')[0];
                            if (!IsUndefinedOrNull(idVal) && idVal == '') {
                                idVal = -1;
                            }
                            if (!IsUndefinedOrNull(apiDataFunc) && apiDataFunc != "") {
                                apiData = window[apiDataFunc]();
                            }
                            if (!IsUndefinedOrNull(apiUrl) && apiUrl != '') {
                                BindDropDownWithUrl(formId, selectElementToPopulate, apiUrl.replace("{id}", idVal), idVal, apiData);
                            }
                            if (!IsUndefinedOrNull(selectDefaultId) && selectDefaultId != "") {
                                $(selectElementToPopulate).val(selectDefaultId);
                                $(selectElementToPopulate).trigger("change");
                            }
                        }
                        apiHitsCount++;
                    }
                );

                isValid = ApplySelectValidation(formId, selectEl);
            });

            if (!isValid) {
                isValidToRet = false;
            }
        }
    );

    return isValidToRet;
}

function SetDefaultPostBehaviour(formId) {
    var dataPostConfig = $(formId).attr('data-post-config');
    if (!IsNull(dataPostConfig)) {

        var dataPostElConfigArr = GetPermissionArr(dataPostConfig);

        //var dataPostElConfigArr = GetPermissionArr($(inputEl).attr("data-post-config"));
        var postBehaviour = GetPermission_Arr_Str(dataPostElConfigArr, 'postBehaviour');
        postBehaviour = (/*IsNull(postBehaviour) ||*/ postBehaviour == 'default') ? 'default' : 'custom';

        if (postBehaviour == "default") {
            $("" + formId).submit(function (event) {

                var elementsToPost = [];
                var isValid = ValidateFormOnSubmit("" + formId, null, elementsToPost);

                if ($("" + formId).valid() && isValid /*$('#AddComplaintForm').valid()*/) {
                    ShowLoading();
                    var response = SubmitForm("" + formId, elementsToPost);
                }
                return false;
            });
        }
    }
}

function ValidateFormOnSubmit(formId, valTag, elementsToPost) {

    var isValid = true;
    var isValidToRet = true;
    var tagStr = (IsNull(valTag) || valTag == null) ? '' : '[data-val-tag=' + valTag + ']';
    elementsToPost = (IsNull(elementsToPost) ? [] : elementsToPost);

    //$(formId + " textarea" + tagStr).each(
    //    function (index, v) {
    //        var inputEl = this;
    //        elementsToPost.push(inputEl);
    //    }
    //);

    //$(formId + " input[data-val-tag=" + valTag + "]").each(
    $(formId + " input" + tagStr).each(
        function (index, v) {
            var inputEl = this;
            //var dictConfig = GetDictConfiguration($(inputEl).attr("data-val-config"));

            if ($(inputEl).attr("type") == "radio") {
                var name = $(inputEl).attr("data-group-name");
                var isRadioVisible = $($(formId + " input[name='" + name + "']")[0]).is(":visible");
                if (isRadioVisible) {
                    isValid = ApplyRadioValidation(formId, inputEl);// apply radio validation on change
                    elementsToPost.push(inputEl);
                }
            }
            if ($(inputEl).attr("type") == "checkbox") {
                $(inputEl).val($(this).prop("checked"));
                elementsToPost.push(inputEl);
                //$(this).prop("checked");
            }
            if ($(inputEl).attr("type") == "text" && $(inputEl).is(":visible")) {
                isValid = ApplyTextBoxValidation(formId, inputEl/*, dictConfig*/);
                elementsToPost.push(inputEl);
            }
            if ($(inputEl).attr("type") == "file" && $(inputEl).is(":visible")) {
                isValid = ApplyFileValidation(formId, inputEl);
                elementsToPost.push(inputEl);
            }
            if (!isValid) {
                isValidToRet = false;
            }
        }
    );

    $(formId + " textarea" + tagStr).each(
        function (index, v) {
            var inputEl = this; //$(this);
            isValid = ApplyTextBoxValidation(formId, inputEl/*, dictConfig*/);
            elementsToPost.push(inputEl);
            if (!isValid) {
                isValidToRet = false;
            }
            //return isValid;
        }
    );

    //$(formId + " select[data-val-tag=" + valTag + "]").each(
    $(formId + " select" + tagStr).each(
        function (index, v) {
            var inputEl = this;//$(this);

            if ($(inputEl).is(":visible")) {
                isValid = ApplySelectValidation(formId, inputEl);
                elementsToPost.push(inputEl);
            }
            if (!isValid) {
                isValidToRet = false;
            }
        }
    );

    return isValidToRet;
}

var ListPartialViewToLoadAfterRedirect = {};

function SubmitForm(formId, elementsToPost) {
    var response = null;
    elementsToPost = (IsNull(elementsToPost) ? [] : elementsToPost);
    $(formId + " input[data-force-post]").each(
        function (index, v) {
            var inputEl = this;
            elementsToPost.push(inputEl);
        }
    );

    $(formId + " select[data-force-post]").each(
        function (index, v) {
            var inputEl = this;
            elementsToPost.push(inputEl);
        }
    );
    var formData = new FormData();
    var elementsArrToPost = [];
    var elementDataToPost = {};
    var elementsAttributesToPost = [];
    for (var i = 0; i < elementsToPost.length; i++) {
        elementsAttributesToPost = [];
        if ($(elementsToPost[i]).attr("type") == "file") {

            //elementsArrToPost.push({ name: $(elementsToPost[i]).attr('name'), value: elementsToPost[i].files });
            formData.append("file-" + $(elementsToPost[i]).attr('name'), elementsToPost[i].files[0]);
        } else {
            $.each(elementsToPost[i].attributes, function () {
                if (this.specified && (this.name.indexOf('data-val-') != -1 || this.name == 'type')) {
                    
                    elementsAttributesToPost.push({ key: this.name, value: this.value });
                }
            });
            elementDataToPost.Attributes = elementsAttributesToPost;
            elementDataToPost.Value = $(elementsToPost[i]).val();
            var elementJson = JSON.stringify(elementDataToPost);

            elementsArrToPost.push({ name: $(elementsToPost[i]).attr('name'), value: $(elementsToPost[i]).val() });
            formData.append($(elementsToPost[i]).attr('name'), elementJson);
        }
    }
    // append antiforgery token
 
    var verficationTokenVal = $(formId + " input[name='__RequestVerificationToken'] ").val();
    if (!IsNull(verficationTokenVal)) {
        formData.append('__RequestVerificationToken', verficationTokenVal);
    }
    // end append antiforgery token

    var dataPostElConfigArr = GetPermissionArr($(formId).attr('data-post-config'));
    var dataPostMethod = GetPermission_Arr_Str(dataPostElConfigArr, 'method');
    var dataPostType = GetPermission_Arr_Str(dataPostElConfigArr, 'type');
    var dataPostUrl = GetPermission_Arr_Str(dataPostElConfigArr, 'url');
    var dataCallBackFunction = GetPermission_Arr_Str(dataPostElConfigArr, 'callback');
    //var dataToPost = JSON.stringify({ data: elementsArrToPost });
    var dataToPost = elementsArrToPost;
    if (dataPostMethod == 'ajax') {
        $.ajax({
            url: dataPostUrl,
            type: dataPostType,
            async: true,
            //contentType:"multipart/form-data",
            contentType: false,
            processData: false,
            data: formData,

            success: function (data) {
                
                response = data;
                //if (data.RedirectUrl != null) {
                //    window.location = data.RedirectUrl;
                //}
                if (data.ListPartialViewToLoadAfterRedirect != null && data.ListPartialViewToLoadAfterRedirect.length > 0) {
                    
                    ListPartialViewToLoadAfterRedirect = data.ListPartialViewToLoadAfterRedirect;
                    var partialViewStr = JSON.stringify(ListPartialViewToLoadAfterRedirect);
                    var encrypeData = sjcl.encrypt("password", partialViewStr);
                    sessionStorage.setItem('ListPartialViewToLoadAfterRedirect', encrypeData);
                    //document.cookie = "ListPartialViewToLoadAfterRedirect=" + encrypeData; //+ partialViewStr;
                }
                if (data.ListPartialView != null) {
                    for (var i = 0; i < data.ListPartialView.length; i++) {
                        var partialView = data.ListPartialView[i];
                        //$(partialView.SelectorId).empty();
                        $(partialView.SelectorId).html(partialView.HtmlString);
                        //$(partialView.SelectorId).modal();
                        //$('#PopupDiv').empty();
                        //$('#PopupDiv').load("../Police/ComplaintAction?complaintId=" + complaintId);
                        //$('#PopupDiv').modal();
                    }
                }
                if (!IsNull(dataCallBackFunction)) {
                    try {
                        window[dataCallBackFunction](data);
                    }
                    catch (e) {
                        // handle an exception here if lettering doesn't exist or throws an exception
                    }
                }
                if (data.RedirectUrl != null) {
                    try {
                        window.location = data.RedirectUrl;
                    }
                    catch (e) {
                        // handle an exception here if lettering doesn't exist or throws an exception
                    }
                }
                HideLoading();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                HideLoading();

                if (errorThrown == 'Request Entity Too Large')
                    alert('Files allowed by maximum 5mb.')
                else
                    alert("An error has occured");
            }

        });

    }

    return response;
}

//---------- Select Validation ------------------
function ApplySelectValidation(formId, selectEl) {
    var isValid = true;
    if ($(selectEl).attr("data-val-is-required") == "True") {
        var val = $(selectEl).val();
        if (val == '' || val == '-1') {
            SetSpanMessage(formId, selectEl.name, true); //  set requied message
            isValid = false;
        } else {
            SetSpanMessage(formId, selectEl.name, false); // remove required message
            isValid = true;
        }
    }
    return isValid;
}



//---------- Textbox Validation ------------------
function ApplyTextBoxValidation(formId, inputEl/*, dictConfig*//*, evt*/) {

    var isValid = true;
    var canValidateIsRequired = true;
    var dictConfig = GetDictConfiguration($(inputEl).attr("data-val-config"));
    if (!IsNull(dictConfig) && dictConfig != null) {
        // Runtime validation
        if (dictConfig["data-val-inputFormat"] == "numbers*") { // numbers only
            var str = $(inputEl).val();
            $(inputEl).val(str.replace(/[^0-9]/g, ''));
        } else if (dictConfig["data-val-inputFormat"] == "!numbers*") { // no numbers
            var str = $(inputEl).val();
            $(inputEl).val(str.replace(/[0-9]/g, ''));
        }
        else { // no numbers

            canValidateIsRequired = true;
            var str = $(inputEl).val();
            var arr = str.match(dictConfig["data-val-inputFormat"]);
            if (arr != null) { // Did it match?
                //$(inputEl).val(arr.input);
                SetSpanMessage(formId, inputEl.name, false); // remove required message
                isValid = true;
                //alert(arr[1]);
            }
            else {
                //str = str.substring(0, str.length - 1);
                //$(inputEl).val(str);
                SetSpanMessage(formId, inputEl.name, true, dictConfig["data-val-inputFormat_message"]); //  set requied message
                isValid = false;
            }
            //$(inputEl).val(str.replace(dictConfig["data-val-inputFormat"], ''));
        }

        if (dictConfig["data-val-Length"]) {
            var minMax = dictConfig["data-val-Length"].split('-');
            var min = minMax[0];
            var max = minMax[1];
            var str = $(inputEl).val();

            if (min != '*') {
                if (str.length < parseInt(min)) {
                    if (dictConfig["data-val-is-required"] == "True" && str.length == 0) {
                        canValidateIsRequired = true;
                    }
                    else {
                        var msg = '';
                        if (dictConfig["data-val-Length-min-message"]) {
                            msg = dictConfig["data-val-Length-min-message"];
                        } else {
                            msg = 'Minimum length = ' + min;
                        }
                        SetSpanMessage(formId, inputEl.name, true, msg); //  set requied message
                        canValidateIsRequired = false;
                        isValid = false;
                    }
                }
            }
            if (max != '*') {
                if (str.length > parseInt(max)) {
                    $(inputEl).val(str.substring(0, str.length - 1));
                }
            }

        }
        if (canValidateIsRequired && dictConfig["data-val-is-required"] == "True") {
            var val = $(inputEl).val();
            if (val == '') {
                SetSpanMessage(formId, inputEl.name, true, dictConfig["data-val-is-required-message"]); //  set requied message
                isValid = false;
            } else if (isValid) {
                SetSpanMessage(formId, inputEl.name, false); // remove required message
                isValid = true;
            }
        }
        else if (canValidateIsRequired && dictConfig["data-val-is-required"] == "False") {
            var val = $(inputEl).val();
            if (val == '') {
                SetSpanMessage(formId, inputEl.name, false); // remove required message
                isValid = true;
            }
        }
    } else {

        if ($(inputEl).attr("data-val-is-required") == "True") {
            var val = $(inputEl).val();
            if (val == '') {
                SetSpanMessage(formId, inputEl.name, true); //  set requied message
                isValid = false;
            } else {
                SetSpanMessage(formId, inputEl.name, false); // remove required message
                isValid = true;
            }
        }
    }
    return isValid;
}

//---------- File Validation ------------------
function ApplyFileValidation(formId, inputEl) {

    var isValid = true;
    var inputValue = $(inputEl).val();
    if (inputValue != null && inputValue != '') {
        if ($(inputEl).attr("data-val-file-ext") !== undefined) {
            isValid = $(inputEl).attr("data-val-file-ext").includes(inputValue.split('.').pop().toLowerCase());
            if (!isValid) {
                SetSpanValidationVoilationMessage(formId, inputEl.name, true);
                return isValid;
            }
        }
    }

    if ($(inputEl).attr("data-val-is-required") == "True") {
        var val = $(inputEl).val();
        if (val == '') {
            SetSpanMessage(formId, inputEl.name, true); //  set requied message
            isValid = false;
        } else {
            SetSpanMessage(formId, inputEl.name, false); // remove required message
            isValid = true;
        }
    }
    return isValid;
}



//---------- Radio Validation -------------
function ApplyRadioValidation(formId, inputEl) {
    var isValid = true;
    var name = $(inputEl).attr("data-group-name");
    var val = $(formId + " input[name='" + name + "']:checked").val();

    if (!val) {
        //alert('Nothing is checked!');
        if ($(inputEl).attr("data-val-is-required") == "True") {
            SetSpanMessage(formId, name, true);
            isValid = false;
        }
    } else {
        $(inputEl).val(val);
        SetSpanMessage(formId, name, false);
        isValid = true;
    }
    return isValid;
}

function SetSpanMessage(formId, name, isAdding, message) {
    var spanElement = $(formId + " span[data-val-for-name='" + name + "']");
    message = (IsNull(message) || message == null) ? spanElement.attr("data-val-is-required-message") : message;
    if (isAdding) {
        spanElement.removeClass('hidden');
        spanElement.html('');
        spanElement.append('<span>' + message + '</span>');
    } else {
        spanElement.addClass('hidden');
        spanElement.html('');
    }
}

function SetSpanValidationVoilationMessage(formId, name, isAdding, message) {
    var spanElement = $(formId + " span[data-val-for-name='" + name + "']");
    message = (IsNull(message) || message == null) ? spanElement.attr("data-val-format-voilation-message") : message;
    if (isAdding) {
        spanElement.removeClass('hidden');
        spanElement.html('');
        spanElement.append('<span>' + message + '</span>');
    } else {
        spanElement.addClass('hidden');
        spanElement.html('');
    }
}

$(function () {
    
    DeletePartialViewAfterRedirect();
});

function DeletePartialViewAfterRedirect() {

    var cookieVal = sessionStorage.getItem('ListPartialViewToLoadAfterRedirect');
    //var cookieVal = getCookie("ListPartialViewToLoadAfterRedirect");

    //var asd3 = jQuery.parseJSON(cookieVal);
    if (!IsNull(cookieVal) && cookieVal != "" && cookieVal != null) {
        //deleteCookie("ListPartialViewToLoadAfterRedirect");
        sessionStorage.removeItem('ListPartialViewToLoadAfterRedirect');
        var decryptData = sjcl.decrypt("password", cookieVal);
        ListPartialViewToLoadAfterRedirect = jQuery.parseJSON(decryptData);
        //document.cookie = "ListPartialViewToLoadAfterRedirect=" + JSON.parse(ListPartialViewToLoadAfterRedirect);
        for (var i = 0; i < ListPartialViewToLoadAfterRedirect.length; i++) {
            var partialView = ListPartialViewToLoadAfterRedirect[i];
            //$(partialView.SelectorId).empty();
            $(partialView.SelectorId).html(partialView.HtmlString);
            //$(partialView.SelectorId).modal();
        }

        ListPartialViewToLoadAfterRedirect = [];
    }
}

function getCookie(cname) {

    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function GetDictConfiguration(val) {
    var dict = {};
    //var dict = null;
    if (IsNull(val) || val == null) {
        //return dict;
        return null;
    }

    var keyVal = val.split('__');
    for (var v in keyVal) {
        var k = keyVal[v].split('::');
        dict[k[0]] = k[1];
    }
    return dict;
}

function SetElementConfiguration(element, c1, c2) {
    var configStr = $(element).attr("data-val-config");
    configStr = configStr.replace(c1, c2);
    $(element).attr("data-val-config", configStr);
}

function RemoveValidationMessage(formId, nameOfElement) {
    // if($(element).children().length>0).remove();
    var spanElement = $(formId + " span[data-val-for-name='" + nameOfElement + "']");
    spanElement.addClass('hidden');
    spanElement.html('');
}

function deleteCookie(cname) {
    document.cookie = cname + '=;expires=Thu, 01 Jan 1970 00:00:01 GMT;';
};

