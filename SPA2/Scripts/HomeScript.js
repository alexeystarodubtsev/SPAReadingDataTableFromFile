function otherDel() {
    var del = document.getElementById("Other");
    del.checked = true;
}
function choosefile(e) {

    e.preventDefault();
    $.get("/Home/ChooseFile", function (data) {
    $('#dialogContent').html(data);
    $('#modDialog').modal('show');
    });
}
function fileclick(name,dir) {
    var filepath = document.getElementById("filePath");
    filepath.value = dir + "\\" + name;
    var db = document.getElementById("DBName");
    db.value = name;
    var x = document.getElementById("closefilechoose");
    x.click();
}
function EditCeil(idp) {
    var td = document.getElementById(idp);
    td.style.display = 'none';
    var inpt = document.getElementById(idp + "inpt");
    inpt.type = "text";
    inpt.focus();
}



function Close(id) {
    var inpt = document.getElementById(id);
    var td = document.getElementById(id.replace('inpt', ''));
    td.style.display = 'inline-block';
    inpt.type = "hidden";
    if (td.innerHTML != inpt.value) {
    form = document.getElementById(id.replace('inpt', 'form'))
        form.type = "submit";
    form.click();
    form.type = "hidden"
    var th = document.getElementById(id.replace('inpt', 'h'));
    th.innerHTML = inpt.value;
}

}
function Press(e,id) {
    if (e.keyCode == 13) {
    //Close(id);
        var inpt = document.getElementById(id);
        inpt.type = "hidden";
    }

}

function newRow(numCol) {
    var tbody = document.getElementById("dataBody")
    var row = document.createElement("TR")
    var nrow = document.getElementById("rowsnum").value;
    for (let i = 0; i < numCol; i++) {
        var td = document.createElement("TD");
        td.setAttribute("ondblclick", 'EditCeil("td_' + nrow + '_' + i + '")')
        td.innerHTML = "col";
        CopyingTD = document.getElementById("copytd")
        td.innerHTML = CopyingTD.innerHTML;
        td.innerHTML = td.innerHTML.replace("td_a_a", "td_" + nrow + "_" + i);
        td.innerHTML = td.innerHTML.replace("td_a_a", "td_" + nrow + "_" + i);
        td.innerHTML = td.innerHTML.replace("td_a_a", "td_" + nrow + "_" + i);
        td.innerHTML = td.innerHTML.replace("td_a_a", "td_" + nrow + "_" + i);
        td.innerHTML = td.innerHTML.replace("curCol", i);
        td.innerHTML = td.innerHTML.replace("curRow", nrow);
        row.appendChild(td);
    }
    document.getElementById("rowsnum").value = Number(nrow) + 1;
    tbody.appendChild(row);
}