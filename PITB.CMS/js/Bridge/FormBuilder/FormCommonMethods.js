Bridge.assembly("BridgeClassLib", function ($asm, globals) {
    "use strict";

    Bridge.define("BridgeClassLib.Form.FormCommonMethods", {
        methods: {
            EnableMultiselect: function () {
                 
                $.EnableMultiselect = function (element) {
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
        
                    $('#' + element).multiselect('selectAll', false);
                    $('#' + element).multiselect('updateButtonText');
                };
            }
        }
    });
});
