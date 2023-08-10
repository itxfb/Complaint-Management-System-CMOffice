//var ApplicationPathFolderPrefix = "/crm";

var ApplicationPathFolderPrefix = "";


(function ($) {
    //alert('Munna kaka');
    $(".datepicker").datepicker({
        format: "yyyy-mm-dd",
        autoclose: true
    });
    $('.numbers-only').on("keypress", function (event) {
       return isNumeric(event);
    });

    $('.characters-only-with-space').on('keydown keyup onpaste', function (e) {
        var currentValueOfElement = $(this).val();
        var stringToReplace = '';
        var character = '';
        for (var i = 0; i < currentValueOfElement.length; i++) {
            character = currentValueOfElement[i];
            if ((character.match(/^[a-zA-Z ]*$/))) {
                stringToReplace = stringToReplace + character;
            }
        }
        $(this).val(stringToReplace);
    });
  

    $.GetBarChartHeight = function (dict) {
        //alert('zeeshi');
        var barsInOnePage = 15;
        var lengthOfOnePage = 400;
        var graphHeight = Math.ceil(dict[0].data.length / barsInOnePage) * lengthOfOnePage;
        return graphHeight;
    }


    $.DeleteLowerHierarchyDivs = function(divPrefix, divLevel) {
        var divMaxLevel = $.GeDivMaxLevel(divPrefix);
            for (var j = divLevel+1; j <= divMaxLevel; j++) {
                $('.' + divPrefix+j).remove();
            }
        
    }
    
    $.GeDivMaxLevel = function(divPrefix) {
        var counter = 1;
        while ($('.' + divPrefix+counter).length > 0) {
            counter++;
        }
        return (counter-1);
    }

    
    $.infoMessage = function (text) { $.baseMessage(text, "info"); }
    $.successMessage = function (text) {$.baseMessage(text, "success");}
    $.errorMessage = function (text) {$.baseMessage(text, "error");}
    $.baseMessage = function (text,type) {
        swal({
            text: (text),
            animation: false,
            type: type
        });
    }
    $.htmlInModal=function(html) {
        swal({ title: 'Content', html: html ,animation:false });
    }
    $.enableMultiselectOf = function (element) {
        $('#' + element).multiselect('destroy');
        
        $('#' + element).multiselect({
            includeSelectAllOption: true,
            //checkboxName: 'multiselect[]',
            enableCaseInsensitiveFiltering: true,
            maxHeight: 200,
            enableFiltering: true,
            allSelectedText: 'All selected',
            buttonWidth: '100%'

        });
        
        $("#" + element).multiselect('selectAll', false);
        $("#" + element).multiselect('updateButtonText');
        
    }

    $.enableMultiselectWithAlreadySelectedValues = function (element, selectedValues) {

        $('#' + element).multiselect('destroy');

        $('#' + element).multiselect({
            includeSelectAllOption: true,
            //checkboxName: 'multiselect[]',
            enableCaseInsensitiveFiltering: true,
            maxHeight: 200,
            enableFiltering: true,
            allSelectedText: 'All selected',
            buttonWidth: '100%'

        });

        
        //$("#" + element).multiselect('selectAll', false);
        $("#" + element).multiselect('updateButtonText');
        $('#' + element).val(selectedValues);
        $('#' + element).multiselect("refresh");
        
    }


    $.enableMultiselect=function() {
        $('select[multiple]').each(function (index,elem) {
            $.enableMultiselectOf(elem.id);
        });
    }
    $.adjustNavTab=function() {
        $('.box-primary').each(function (index, element) {
            var e = $(element).parent('div').parent('div');
            if (e != null && e.hasClass('tab-content')) {
                $(e).css({
                    'background': '#fff',
                    'padding': '0px',
                    'border-bottom-right-radius': '3px',
                    'border-bottom-left-radius': '3px'
                });
            }
        });

    }
    //var counter = 1;
    //$.loadAgentListings = function (api, from, to, campaign) {

    //        if (counter == 1) {

    //            counter++;
    //            var table = $('#agentListing').dataTable({
    //                "dom": '<"top"<"col-md-6"l><"col-md-6"f><"col-md-6"i><"col-md-6"p>>',
    //                fnInitComplete: function (oSettings, json) {
    //                    counter = 1;
    //                },
    //                "bDestroy":true,
    //                "iDisplayLength": 50,
    //                "processing": false,
    //                "responsive": true,
    //                "serverSide": false,
    //                "ajax": {
    //                    "url": '../GeneralApi/GetAgentComplaints',
    //                    "type": "POST",
    //                    data:{from:from,to:to,campaign:campaign}
    //            },
    //             "columns": [
    //                        { "data": "ComplaintNo" },
    //                        { "data": "Campaign" },
    //                        { "data": "Name" },
    //                        { "data": "Date" },
    //                        { "data": "Category" },
    //                        { "data": "Status" },
    //                        { "data": "Id" }
    //             ],
    //             aoColumnDefs: [{
    //                                "aTargets": [6],
    //             "bSearchable": false,
    //             "bSortable": false,
    //             "fnCreatedCell": function (nTd, sData, oData, iRow, iCol) {
    //                 $(nTd).css('text-align', 'center');
    //             },
    //             "mData": null,
    //             "mRender": function (data, type, full) {
                    
    //                 return '<td style="text-align:center">' +
    //                    // '<a href="user-registration.html" ><span class="fa fa-pencil"></span></a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
    //                     '<a data-original-title="View details" data-toggle="tooltip" href="javascript:void(0);" " id="' + full.Id + '" onclick="OpenDetailPopup(this.id);"><span class="fa fa-eye"></span></a>' +
    //                     '</td>';
    //             }}],
                

    //    });
    //        }

    //    }
    $.loadDataTable=function(api, parameters) {
        var pause = 1;

    }
    $.loadDropdown = function (elementId, apiAction, selectedIndexValue, addSelectAsFirstElement) {
        var elem = $("#" + elementId);
        elem.empty();
        if (selectedIndexValue != '' && selectedIndexValue != null) {
          
            if (addSelectAsFirstElement != '' && addSelectAsFirstElement) {
                elem.append($('<option/>', { value: "", text: "--Select--" }));

            }
            var api = "../GeneralApi/" + apiAction;
            $.ajax({
                    url: api,
                    type: 'POST',
                    data: { id: selectedIndexValue },
                    async: false,
                    success: function(resultSet) {
                        $.each(resultSet, function(index, data) {
                            elem.append($('<option/>', { value: data.Value, text: data.Text }));
                        });
                    }
                }
            );
        }
    }
    $.loading = function (elementId,isHiding) {
        
        var overlay = '<div class="overlay"><i class="fa fa-refresh fa-spin"></i></div>';
        
       // var form = $("#" + elementId).parent('form');
       // var formParent = form.parent('div').parent('div');
        var formParent = $("#" + elementId.id).parent('div').parent('div');
        if (isHiding) {
            var e = $('[data-psrc=' + elementId.id + ']');
            e.remove();
            //formParent.remove()
            //formParent.after('<div class="overlay" data-psrc="' + elementId.id + '"><i class="fa fa-refresh fa-spin"></i></div>');
        } else {
            if (formParent != null) {
                formParent.after('<div class="overlay" data-psrc="' + elementId.id + '"><i class="fa fa-refresh fa-spin"></i></div>');
            }
        }
        

    }
    $.adjustNavTab();
})(jQuery);


function GetLinkStrOnPage() {

    var styleSheets = '';

    $('link[rel="stylesheet"]').each(function () {
        var getUrl = window.location;
        var baseUrl = getUrl.protocol + "//" + getUrl.host;//+ "/" + getUrl.pathname.split('/')[1];
        var hrefPath = $(this).attr('href');

        if (hrefPath.indexOf(baseUrl) >= 0) { // if base url is present in href path
            $(this).attr('href', hrefPath);
        } else if (hrefPath.indexOf(baseUrl) == -1) {
            //$(this).attr('href',baseUrl+hrefPath);
            if (hrefPath.indexOf('//') == 0) {
                $(this).attr('href', getUrl.protocol + hrefPath);
            }
            else if (hrefPath.indexOf("/") == 0) {
                $(this).attr('href', baseUrl + hrefPath);
            } else {
                $(this).attr('href', hrefPath);
            }
        }
        styleSheets = styleSheets + this.outerHTML + "\n";
    });

    return styleSheets;
}

function OpenDetailPopup(complaintId)
{
    $('#PopupDiv').load("../Complaint/Detail?complaintId=" + complaintId);
    $('#PopupDiv').modal();
    //window.location.href = "../Complaint/Detail?complaintId="+complaintId;
    //window.location.href = '@Url.Action("AttendanceLogs", "Attendance")' + '?id=' + dataId;
    //alert('zeeshi'+complaintId);
}


function isNumeric(evt) {

    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function GetDateFormat(date) {
    var dayArr = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    var monthArr = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    var year = date.getFullYear();
    var day = date.getDay();
    var month = date.getMonth();
    var dateNo = date.getDate();
    return monthArr[month] + " " + dateNo + ", " + year;
}
function GetDateFormatSlash(date) {
    var dayArr = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    var monthArr = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    var year = date.getFullYear();
    var day = date.getDay();
    var month = date.getMonth();
    var dateNo = date.getDate();
    return dateNo + ' ' + monthArr[month] + ' ' + year;
}
var DivData = [];

function SetDivData(divId, divData) {
    //this.DivData = divData;
    //this.DivId = divId;
    DivData.push(divData);
}

function GetDivData(divId) {
    return DivData[divId];
}
function formatNumber(num) {
    if (num === undefined || num == null)
        return num;
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}
function disableEnableDataTablePaginationAndFilters(dataTableObject, IsRemovingPagination) {
    var orgRows = dataTableObject.fnGetData();
    var oSettings = dataTableObject.fnSettings();

    dataTableObject.fnClearTable(false);
    //dataTableObject.fnDestroy();
    //dataTableObject.rows(orgRows);



    var isSettingEnable = true;
    if (IsRemovingPagination) {
        isSettingEnable = false;
    }
    var tableId = oSettings.sTableId;
    oSettings.oInit.bInfo = isSettingEnable;
    oSettings.oInit.bPaginate = isSettingEnable;
    oSettings.oInit.bFilter = isSettingEnable;

    var aoColumns = oSettings.aoColumns;
    var options = {
        "searching": isSettingEnable,
        "paging": isSettingEnable,
        "info": isSettingEnable,
        "lengthChange": isSettingEnable,
        "aoColumns": aoColumns,
        
    }
    //$.extend(true, $.fn.dataTable.defaults, {
    //    "searching": isSettingEnable,
    //    "paging": isSettingEnable,
    //    "info": isSettingEnable,
    //    "lengthChange": isSettingEnable
    //});
    //dataTableObject.DataTable(options);
    //dataTableObject.settings[0] = oSettings;


    //dataTableObject.draw();
    dataTableObject.fnDestroy();
   
    // var newDataTableObject = $("#" + tableId).dataTable(options);
    dataTableObject = $("#" + tableId).dataTable(options);
    for (i = 0; i < orgRows.length; i++) {
        // newDataTableObject.fnAddData(orgRows[i]);
        dataTableObject.fnAddData(orgRows[i]);
    }

    dataTableObject.fnDraw();
    dataTableObject.fnSort([0,'asc']);
    // dataTableObject = newDataTableObject;
}


var printDatatableOptions = {
    "paging": false,
    "ordering": false,
    "info": false,
    "searching": false
};
var revertPrintDatatableOptions = {
    "paging": true,
    "ordering": true,
    "info": true,
    "searching": true
};

function reInitializeDatatableForPrint(tableId, tableOptions) {
    var table = $(tableId);
    table.DataTable().destroy()
    table.DataTable(tableOptions);
}