//-------- Common functions --------
function GetPermissionArr(val) {
    var permissionArr = [];
    var configArr = val.split('||');
    for (var i = 0; i < configArr.length; i++) {
        var singleConfigArr = configArr[i].split('::');
        permissionArr[i] = { key: singleConfigArr[0], value: singleConfigArr[1] };
    }
    return permissionArr;
}

function GetPermissionWithKey(val, key) {
    var permissionArr = GetPermissionArr(val);
    for (var i = 0; i < permissionArr.length; i++) {
        if (permissionArr[i].key == key) {
            return permissionArr[i].value;
        }
    }
    return '-1';
}

function IsNull(val) {
    return (typeof val == 'undefined');
}

function IsUndefinedOrNull(val) {
    return (typeof val == 'undefined' || val==null);
}

function GetPermission_Arr_Str(permissionArr, key) {
    for (var i = 0; i < permissionArr.length; i++) {
        if (permissionArr[i].key == key) {
            return permissionArr[i].value;
        }
    }
    return '-1';
}

function SlideDown(div) {
    $(div).slideDown(700);
}

function SlideUp(div) {
    $(div).slideUp(500);
}

function GetIncrementalName(name) {
    var numStr = name.match(/\d+/); // 123456
    var incrementalName = name.replace(numStr, '' + (parseInt(numStr) + 1));
    return incrementalName;
}

function SetData(element, key, data) {
    $(element).data(key, data);
}

function GetData(element, key) {
    return $(element).data(key);
}