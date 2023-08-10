function PopulateTable(tableId, result) {

    $('#' + tableId).empty();
    PopulateTableHeader(tableId, result);
    PopulateTableBody(tableId, result);
    CreateEmptySelectedRows();

    $('#' + tableId).DataTable({
        scrollY: "300px",
        scrollX: true,
        scrollCollapse: true,
        paging: false,
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 1
        }
    });
    //$('#' + tableId).DataTable();
}

function PopulateTableHeader(tableId, result) {
    var content = '"<thead><tr class="info">';
    for (var i = 0; i < result.ListHeaderModel.length; i++) {
        var style = '';
        if (result.ListHeaderModel[i].Css.Style) {
            style = 'style = "' + result.ListHeaderModel[i].Css.Style + '"';
        }

        var trClass = '';
        if (result.ListHeaderModel[i].ClassName) {
            trClass = 'class="' + result.ListHeaderModel[i].ClassName + '"';
        }

        content += '<th ' + style + ' ' + trClass + '>' + result.ListHeaderModel[i].InnerHtml + '</th>';
    }
    content += '</tr></thead>';

    $('#' + tableId).append(content);
}

function PopulateTable(tableId, result) {
    var content = '';//'<tbody>';
    var count = 0;
    var funcArr = [];
    for (var i = 0; i < result.ListRowModel.length; i++) {
        var row = result.ListRowModel[i];
        //if(content)
        //content += '<tr class="info">';

        for (var j = 0; j < row.length; j++) {
            var cell = row[j].cell;
            if (cell.PreviousHtml) {
                content += cell.PreviousHtml;
            }

            
            var cellId = tableId + '_Cell_' + (count);
            var className = row[j].ClassName;
            var tdClass = '';


            // populate attributes
            if (cell.DictAttributes) {
                for (var n = 0; n < row.length; n++) {

                }
            }


            if (className) {
                tdClass = 'class="' + className + '"';
            }
            var style = '';
            if (row[j].Css.Style) {
                style = 'style = "' + row[j].Css.Style + '"';
            }
            content += '<td id="' + cellId + '" ' + style + ' ' + tdClass + '>' + row[j].InnerHtml + '</td>';
            if (row[j].Data) {
                funcArr.push({ funcName: BindData, cellId: cellId, cellData: row[j].Data });
            }
            count++;
        }
        content += '</tr>';
    }
    content += '</tbody>';

    $('#' + tableId).append(content);

    for (var i = 0; i < funcArr.length; i++) {
        funcArr[i].funcName(funcArr[i].cellId, funcArr[i].cellData);
    }
}


function PopulateTableBody(tableId, result) {
    var content = '<tbody>';
    var count = 0;
    var funcArr = [];
    for (var i = 0; i < result.ListRowModel.length; i++) {
        var row = result.ListRowModel[i];

        content += '<tr class="info">';
        for (var j = 0; j < row.length; j++) {
            var cellId = tableId + '_Cell_' + (count);
            var className = row[j].ClassName;
            var tdClass = '';

            if (className) {
                tdClass = 'class="' + className + '"';
            }
            var style = '';
            if (row[j].Css.Style) {
                style = 'style = "' + row[j].Css.Style + '"';
            }
            content += '<td id="' + cellId + '" ' + style + ' ' + tdClass + '>' + row[j].InnerHtml + '</td>';
            if (row[j].Data) {
                funcArr.push({ funcName: BindData, cellId: cellId, cellData: row[j].Data });
            }
            count++;
        }
        content += '</tr>';
    }
    content += '</tbody>';

    $('#' + tableId).append(content);

    for (var i = 0; i < funcArr.length; i++) {
        funcArr[i].funcName(funcArr[i].cellId, funcArr[i].cellData);
    }
}

function BindData(cellId, cellData) {
    $('#' + cellId).data('model', cellData);
}

function CreateEmptySelectedRows() {
    $(".SelectAll").each(function (i, element) { // for each row

        $(this).parent().parent().parent().find('tbody').find('tr').each(function (j, td) { // for each row
            selectedCells2[j] = [];
        });

    });
}