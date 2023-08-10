//alert('zeeshi12344');


jQuery.validator.unobtrusive.adapters.add
    ("mycustomvalidation", ['otherproperty0', 'otherproperty1'],
    function (options) {
        options.rules['mycustomvalidation'] = {
            other: options.params.other,
            otherproperty0: options.params.otherproperty0,
            otherproperty1: options.params.otherproperty1
        };
        options.messages['mycustomvalidation'] = options.message;
    }
);
jQuery.validator.addMethod("mycustomvalidation", function (value, element, params) {
    //alert('zeehi55');
    var isChecked = $('#PersonalInfoVm_IsEmail').is(':checked');
    var retVal = false;
    if ($(element)) {
        retVal = $(element).attr("checked");
    }
    if (retVal == true) {
        return retVal;
    }
    if (params.otherproperty0) {
        if ($('#' + params.otherproperty0)) {
            retVal = $('#' + params.otherproperty0).is(':checked');
            //retVal = $('#' + params.otherproperty0).attr("checked");
        }
    }
    if (retVal == true) {
        return retVal;
    }
    if (params.otherproperty1) {
        if ($('#' + params.otherproperty1)) {
            retVal = $('#' + params.otherproperty1).is(':checked');
            //retVal = $('#' + params.otherproperty1).attr("checked");
        }
    }
    if (retVal == true) {
        return retVal;
    }
    return false;
});