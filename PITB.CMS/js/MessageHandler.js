$.Show = function (messageObject,elementId) {

    if (!messageObject.CanShow)
        return;
    var m = {};
    m.html = messageObject.Message;
    m.title = messageObject.Title;
   // m.text = messageObject.Message;
    var isConfirmDialog = messageObject.NeedConfirmation;
    var messageType = messageObject.DialogType;
    var modalType = '';
    switch (messageType) {
        case Enum.DialogType.Error:
            m.type = 'error';
            break;
        case Enum.DialogType.Success:
            m.type = 'success';
            break;
        case Enum.DialogType.Warning:
            m.type = 'warning';
            break;
        case Enum.DialogType.Info:

            break;
        case Enum.DialogType.Confirmation:
            m.type = 'error';
            break;
        default:
    }
    if (typeof (elementId) === 'undefined') {
        
    } else {
        $("#" + elementId).hide();
        //$("#" + elementId).modal('hide');
    }
   // $("#data").modal('hide');
    swal(m);
}