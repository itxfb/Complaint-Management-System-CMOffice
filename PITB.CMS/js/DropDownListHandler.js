

function RegisterDropDownListChangeEventsForProvinceDistrict(provinceDivId,districtDivId,campaignId) {
    var dict = []; // create an empty array
    dict.push({
        key: provinceDivId,
        value: "Districts?campaignId=" + campaignId
    });
    dict.push({
        key: districtDivId,
        value: "TownTehsils?campaignId=" + campaignId
    });
    for (var keyVal in dict) {
        if (parseInt(keyVal) < dict.length - 1) {
            $("#" + dict[keyVal].key + "").change(function () {
                populateDropDownList($(this).val(), FindNexDivInDict(dict, $(this).attr('id')), FindCurrentValue(dict, $(this).attr('id')), dict);
            });
        }
    }

    $("#" + dict["0"].key).trigger('change');
   
}


function RegisterDropDownListChangeEventsCustomDictionary(dict) {
    for (var keyVal in dict) {
        if (parseInt(keyVal) < dict.length - 1) {

          
           
            
            //alert($(this).val() + "," + dict[keyVal + 1].key + "," + dict[keyVal].value);
            $("#" + dict[keyVal].key + "").on("change", { dict: dict, keyVal: keyVal }, function () {
                //alert($(this).val() + $(this).attr('id'));
                //populateDropDownList($(this).val(), dict[parseInt(keyVal + 1).toString()].key, dict[keyVal].value);
                //alert('zeeshi '+$(this).attr('id'));
                
                var nextDivId = FindNexDivInDict(dict, $(this).attr('id'));
              
                populateDropDownList($(this).val(), nextDivId, /*dict[0].value*/FindCurrentValue(dict, $(this).attr('id')), dict);
                $("#" + nextDivId).trigger('change');
            });
        }
    }

    $("#" + dict["0"].key).trigger('change');
    
}

function RegisterDropDownListChangeEventsCustomDictionaryWithSkipCount(dict, skipCount) {
    var currSkipCount = 0;
    for (var keyVal in dict) {
        if (parseInt(keyVal) < dict.length - 1) {
            //alert($(this).val() + "," + dict[keyVal + 1].key + "," + dict[keyVal].value);
            $("#" + dict[keyVal].key + "").change(function () {
                //alert($(this).val() + $(this).attr('id'));
                //populateDropDownList($(this).val(), dict[parseInt(keyVal + 1).toString()].key, dict[keyVal].value);
                currSkipCount++;
                //alert('zeeshi ' + $(this).attr('id') + 'skip count = ' + skipCount + 'curr skip count = '+currSkipCount);
                if (currSkipCount > skipCount) {
                    populateDropDownList($(this).val(), FindNexDivInDict(dict, $(this).attr('id')), /*dict[0].value*/FindCurrentValue(dict, $(this).attr('id')), dict);
                }
            });
        }
    }

    $("#" + dict["0"].key).trigger('change');

}


//function RegisterDropDownListChangeEventsForProvinceDistrict(provinceDivId, districtDivId, tehsilDivId, ucDivId)
//{
//    var dict = []; // create an empty array
//    dict.push({
//        key: provinceDivId,
//        value: "Districts"
//    });

//    dict.push({
//        key: districtDivId,
//        value: "TownTehsils"
//    });

//    dict.push({
//        key: tehsilDivId,
//        value: "UnionCouncils"
//    });

//    dict.push({
//        key: ucDivId,
//        value: null
//    });


//    for (var keyVal in dict)
//    {
//        if ( parseInt(keyVal) < dict.length-1) {
//            //alert($(this).val() + "," + dict[keyVal + 1].key + "," + dict[keyVal].value);
//            $("#" + dict[keyVal].key + "").change(function () {
//                //alert($(this).val() + $(this).attr('id'));
//                //populateDropDownList($(this).val(), dict[parseInt(keyVal + 1).toString()].key, dict[keyVal].value);
//                populateDropDownList($(this).val(), FindNexDivInDict(dict, $(this).attr('id')), /*dict[0].value*/FindCurrentValue(dict, $(this).attr('id')), dict);
//            });
//        }
//    }

//    $("#" + dict["0"].key).trigger('change');
    
//}


// Start Category Load On Campaign Change
function RegisterDropDownListChangeEventsForCampaignCategory(CampaignDivId, CategoryDivId, hasDefaultValues) {
    if (typeof (hasDefaultValues) === 'undefined') hasDefaultValues = false;
    var dict = []; // create an empty array
    
    dict.push({
        key: CampaignDivId,
        value: "ComplaintTypes"
    });

    dict.push({
        key: CategoryDivId,
        value: null
    });


    for (var keyVal in dict) {
        if (parseInt(keyVal) < dict.length - 1) {
            //alert($(this).val() + "," + dict[keyVal + 1].key + "," + dict[keyVal].value);
            $("#" + dict[keyVal].key + "").change(function () {
                //alert($(this).val() + $(this).attr('id'));
                //populateDropDownList($(this).val(), dict[parseInt(keyVal + 1).toString()].key, dict[keyVal].value);
                populateDropDownList($(this).val(), FindNexDivInDict(dict, $(this).attr('id')), /*dict[0].value*/FindCurrentValue(dict, $(this).attr('id')), dict, false, FindNexDivInDict(dict, $(this).attr('id')), hasDefaultValues);
            });
        }
    }

    $("#" + dict["0"].key).trigger('change');
    //$('#' + CategoryDivId).multiselect();
    //$('#' + CategoryDivId).multiselect('destroy');
    //$('#' + CategoryDivId).multiselect();

}
// End Category Load on Compaign Change


function RegisterDropDownListChangeEventsComplaintTypes(complaintTypeDivId, complaintSubtypeDivId) {
    
    var dict = []; // create an empty array
    dict.push({
        key: complaintTypeDivId,
        value: "ComplaintSubType"
    });

    dict.push({
        key: complaintSubtypeDivId,
        value: "null"
    });

    for (var keyVal in dict) {
        if (parseInt(keyVal) < dict.length + 1) {
            $("#" + dict[keyVal].key + "").change(function () {
                populateDropDownList($(this).val(), FindNexDivInDict(dict, $(this).attr('id')), FindCurrentValue(dict,$(this).attr('id')), dict);
            });
        }
    }

    $("#" + dict["0"].key).trigger('change');
}

function FindNexDivInDict(dict, divId)
{
    for (var keyVal in dict) {
     
        if (dict[keyVal].key == divId)
        {
            return dict[(parseInt(keyVal) + 1).toString()].key;
        }
    }
}

function FindCurrentValue(dict, divId) {
    for (var keyVal in dict) {
        if (dict[keyVal].key == divId) {
            return dict[keyVal].value;
        }
    }
}

function populateDropDownList(id, divId, apiName, dict, canAppendSelectInFirstIndex, divToChangeToMultiselect, hasDefaultValues)
{
   
    var currSelectedValues = $('#' + divId).val();
    if (typeof (divToChangeToMultiselect) === 'undefined') divToChangeToMultiselect = null;
    if (typeof (canAppendSelectInFirstIndex) === 'undefined') canAppendSelectInFirstIndex = true;
    if (typeof (hasDefaultValues) === 'undefined') hasDefaultValues = false;
    var division = $("#"+divId+"");
    division.empty();
    if (id == '' || id == null) {
        $.enableMultiselectOf(divToChangeToMultiselect);
    }
    if (canAppendSelectInFirstIndex) {
        division.append($('<option/>', { value: "", text: "--Select--" }));
    }
    //var divisionApi = "../../GeneralApi/" + apiName;
    var divisionApi = ApplicationPathFolderPrefix + "/GeneralApi/" + apiName;
    $.ajax({
        url: divisionApi,
        type: 'POST',
        data: { id: id },
        async:false,
        success: function (divdata) {
            
            $.each(divdata, function (index, data) {
                division.append($('<option/>', { value: data.Value, text: data.Text }));
            });
            if (divdata.length==1)
                setSelectedListOfDropDown(divId, divdata[0].Value);
             
            if (divToChangeToMultiselect != null) {
                if (hasDefaultValues) {
                    $.enableMultiselectWithAlreadySelectedValues(divToChangeToMultiselect, currSelectedValues);
                } else {
                    $.enableMultiselectOf(divToChangeToMultiselect);
                }
            }
            emptyAllNextDDList(dict, divId);

        }
    }
    );
}

function emptyAllNextDDList(dict, divId)
{
    var canEmpty = false;
    for (var keyVal in dict) {

        if (canEmpty)
        {
            $("#" + dict[keyVal].key).empty();
            $("#" + dict[keyVal].key).append($('<option/>', { value: "", text: "--Select--" }));
        }

        if ( dict[keyVal].key == divId) {
            canEmpty = true;
        }
    }
}
function setSelectedListOfDropDown(elementId, elementToSetSelected) {
    $("#" + elementId).val(elementToSetSelected);
}