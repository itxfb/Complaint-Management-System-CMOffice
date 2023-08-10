
function RegisterEventChanges(formId, valTag) {

    //PopulateConfiguration(formId,"data-val-config");

    $(formId + " select").on("change", { formId: formId }, function () {
        var id = $(this).attr("id");
        ShowHideDivOnDropDownChange(id, $(this).val(), formId, valTag);
    });

    $(formId + " button[data-val-tag]").on("click", { formId: formId }, function () {
        var name = $(this).attr("name");
        ShowHideDivOnDropDownChange(name, $(this).val(), formId, valTag);
    });

    $(formId + " input").on("change", { formId: formId }, function () {
        var id = $(this).attr("id");
        ShowHideDivOnDropDownChange(id, $(this).val(), formId, valTag);
    });
}

//function ShowHideDivOnButtonClick(dropdownId, selectedValue, formId, valTag, canRegisterValidation) {
    
//}

//function PopulateConfiguration(formId, configAttrName) {
//    $(formId + " [" + configAttrName+"]").each(
//        function (index, v) {
//            var element = PopulateConfigurationAgainstElement($(this)[0], configAttrName, $($(this)[0]).attr(configAttrName));
//        }
//    );
//}

//function PopulateConfigurationAgainstElement(element, configAttrName, configVal) {
//    var dictConfig = GetDictConfiguration(configVal);
//    $(element).data(configAttrName, dictConfig);
//}

function ShowHideDivOnDropDownChange(dropdownId, selectedValue, formId, valTag, canRegisterValidation) {
    $(formId + ' div[data-display-if]').each(
        function (index, v) {
            var divToShow = $(this)[0];
            var displayIfConfig = $(divToShow).attr('data-display-if');
            var permissionArr = GetPermissionArr(displayIfConfig);

            for (var i = 0; i < permissionArr.length; i++) {
                var elementId = permissionArr[i].key;
                if (elementId == dropdownId) {
                    var elementValArr = permissionArr[i].value.split(',');
                    var hasFound = false;
                    for (var j = 0; j < elementValArr.length && !hasFound; j++) {
                        if (elementValArr[j].split('___')[0] == selectedValue.split('___')[0]) { // show
                            SlideDown(divToShow);
                            hasFound = true;
                        }
                    }
                    if (!hasFound) {
                        SlideUp(divToShow);
                    }
                }
            }
        });
}


function EnableMultiselectBootstrap(selectEl) {
    //return;
    var isMultiselectEnabled = $(selectEl).attr("data-val-isMultiselectEnabled");
    if (isMultiselectEnabled == "True") {
        $(selectEl).multiselect('destroy');

        $(selectEl).multiselect({
            includeSelectAllOption: true,
            //checkboxName: 'multiselect[]',
            enableCaseInsensitiveFiltering: true,
            maxHeight: 200,
            enableFiltering: true,
            allSelectedText: 'All selected',
            buttonWidth: '100%'

        });

        $(selectEl).multiselect('selectAll', false);
        $(selectEl).multiselect('updateButtonText');
    }
}

//------------------ BindDropDownWithUrl ------------------

function BindDropDownWithUrl(formId, selectEl, apiUrl, idVal, apiData) {
    var configurationVal = $(selectEl).attr('data-val-config');
    var dontAppendDefaultSelect = $(selectEl).attr('data-val-dont-Append-default-select');
    var canCombineIdsWithVal = false;

    if (typeof configurationVal != 'undefined' && GetPermissionWithKey(configurationVal, 'combineIdWithVal') == 'True') {
        canCombineIdsWithVal = true;
    }

    var formData = new FormData();
    if (!IsUndefinedOrNull(apiData)) {
        for (var i = 0; i < apiData.length; i++) {
            formData.append(apiData[i].key, apiData[i].value);
        }
    }


    if (idVal == -1) {
        $(selectEl).html("");
        if (IsUndefinedOrNull(dontAppendDefaultSelect)) {
            $(selectEl).append($("<option></option>")
                .attr("value", "-1")
                .text("--Select--"));
        }
    }
    else {
        $.ajax({
            url: apiUrl,
            type: 'POST',
            async: false,
            contentType: false,
            processData: false,
            data: formData,
            success: function (divdata) {
                $(selectEl).html("");
                if (IsUndefinedOrNull(dontAppendDefaultSelect)) {
                    $(selectEl).append($("<option></option>")
                        .attr("value", "-1")
                        .text("--Select--"));
                }
                $.each(divdata, function (index, data) {
                    if (canCombineIdsWithVal) {
                        $(selectEl).append($('<option/>', { value: data.Value + '___' + data.Text, text: data.Text }));
                    }
                    else {
                        $(selectEl).append($('<option/>', { value: data.Value, text: data.Text }));
                    }
                });

            }
        });
        EnableMultiselectBootstrap(selectEl);
    }
    $(selectEl).trigger('change');
}

function RegisterCloning(formId, valTag) {

    $(formId + " div[data-clone-config]").each(
        function (index, v) {
            var parentEl = this;
            var cloneElConfigArr = GetPermissionArr($(parentEl).attr("data-clone-config"));

            var cloneOnEventOfName = GetPermission_Arr_Str(cloneElConfigArr, 'OnEventOfName');
            var cloneEventType = GetPermission_Arr_Str(cloneElConfigArr, 'EventType');
            //var cloneNamesToReplicate = GetPermission_Arr_Str(cloneElConfigArr, 'NamesToReplicate');
            var cloneName = GetPermission_Arr_Str(cloneElConfigArr, 'CloneName');

            // set index
            SetData(parentEl, 'Index', 0);

            $(formId + " [name='" + cloneOnEventOfName + "']").on(cloneEventType, { formId: formId, valTag: valTag, parentEl: parentEl, cloneName: cloneName }, function () {
                var cloneEl = $(formId + " [name='" + cloneName + "']");
                var afterCloneEl = $(cloneEl).clone();
                // remove same name
                afterCloneEl.removeAttr('name');
                var nameSuffix = '_' + GetData(parentEl, 'Index');
                $(afterCloneEl).find("input").each(
                    function (index, element) {
                        var incrementedName = $(this).attr("name") + nameSuffix;
                        if (!IsNull($(this).attr("data-group-name"))) {
                            $(this).attr("data-group-name", $(this).attr("data-group-name") + nameSuffix);
                        }
                        $(this).attr("name", incrementedName);
                    });
                // add validation attribute
                $(afterCloneEl).find("span").each(
                    function (index, element) {
                        var incrementedName = $(this).attr("data-val-for-name") + nameSuffix;
                        $(this).attr("data-val-for-name", incrementedName);
                    });
                //var replicateNames = cloneNamesToReplicate.split(',');

                //for (var i = 0; i < replicateNames.length; i++) {
                //    var incrementalEl = $(afterCloneEl).find(" [name='" + replicateNames[i] + "']");
                //    //$(incrementalEl).attr("name", GetIncrementalName(replicateNames[i]));
                //    $(incrementalEl).attr("name", $(incrementalEl).attr("name") + '_' + GetData(parentEl, 'Index'));
                //}

                //$(formId + " [name='" + cloneChildOfName + "']").append(afterCloneEl);
                $(afterCloneEl).attr("id", $(afterCloneEl).attr("id") + nameSuffix);
                $(parentEl).append(afterCloneEl);
                SlideDown(afterCloneEl);
                RegisterLocalValidation("#" + $(afterCloneEl).attr("id"), valTag);

                SetData(parentEl, 'Index', GetData(parentEl, 'Index') + 1);
            });


        }
    );
}

function IncrementValueOnButtonClick(element, incrementalVal, maxLimit) {
    var currentVal = parseInt($(element).val());
    var minLimit = 0;
    if ((currentVal + incrementalVal) <= maxLimit && (currentVal + incrementalVal) >= minLimit) {
        $(element).val(currentVal + incrementalVal);
        $(element).trigger('change');
    }
}