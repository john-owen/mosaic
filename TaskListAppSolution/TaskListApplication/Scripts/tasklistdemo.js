var list_offset = 0;
const MAX_LIST_SIZE = 5;

window.onload = function () {
    getTasks(list_offset, MAX_LIST_SIZE);

    //disable double clicks
    $('btn').click(function (event) {
        if (!event.detail || event.detail == 1) {
            return true;
        } else {
            return false;
        }
    });
};

function getNext(btn) {
    getTasks(list_offset, 1, false, btn);
}
function getPrev(btn) {
    getTasks(list_offset, 1, true, btn);
}

function composeListEntryHtml(listEntry) {
    var listHtml = "";

    if (listEntry.isComplete) {
        listHtml = "<li>" +
            "<div class='task_list_col1'><input type='checkbox' onclick='toggleComplete(" + listEntry.id + ",this)' checked></div>" +
            "<div class='task_list_col2'><del><h3>" + listEntry.title + "</h3></del></div>" +
            "<div class='task_list_col3'><button onclick='removeTask(" + listEntry.id + ",this)'>Remove</button></div>" +
            "</li>"
    } else {
        listHtml = "<li>" +
            "<div class='task_list_col1'><input type='checkbox' onclick='toggleComplete(" + listEntry.id + ",this)'></div>" +
            "<div class='task_list_col2'><h3>" + listEntry.title + "</h3></div>" +
            "<div class='task_list_col3'><button onclick='removeTask(" + listEntry.id + ",this)'>Remove</button></div>" +
            "</li>"
    }

    return listHtml;
}

// adds entry to either top or bottom of list
function getTasks(offset, count, prepend, element) {
    var task_list_length = $("#todo_list li").length;
    var options = {};
    if (prepend) {
        offset = offset - task_list_length - count;
        if (offset < 0) return;
        options.url = "/api/values/get/" + offset + "/" + count;
    } else {
        options.url = "/api/values/get/" + offset + "/" + count;
    }
    options.type = "GET";
    options.dataType = "json";
    options.success = function (list) {
        if (prepend) {
            for (let index = (list.length - 1); index >= 0; index--) {
                // Remove entry from end of list
                if (task_list_length >= MAX_LIST_SIZE) {
                    $('#todo_list>li').last().remove();
                    list_offset--;
                }

                // Add entry to top of list
                var listHtml = composeListEntryHtml(list[index]);
                $("#todo_list").prepend(listHtml);

                $("#list_offset").val(list_offset);
            }
        } else {
            for (let index = 0; index < list.length; index++) {
                // Remove entry from begining of list
                if (task_list_length >= MAX_LIST_SIZE) $('#todo_list>li').first().remove();

                // Add entry to bottom of list
                var listHtml = composeListEntryHtml(list[index]);
                $("#todo_list").append(listHtml);

                list_offset++;
                $("#list_offset").val(list_offset);
            }
        }
        if (element) element.removeAttribute("disabled");
    };
    options.error = function () {
        alert("Error while calling the Web API!");
        if (element) element.removeAttribute("disabled");
    };

    if (element) element.setAttribute("disabled", "disabled");
    $.ajax(options);
}

// toggles checkbox
function toggleComplete(id, element) {
    var options = {};
    options.url = "/api/values/toggle/" + id;
    options.type = "GET";
    options.success = function () {
        if (element.checked) {
            $(element).parent().parent().find("h3").wrap("<del>");
        } else {
            $(element).parent().parent().find("h3").unwrap();
        }
        
        if (element) element.removeAttribute("disabled");
    };
    options.error = function () {
        alert("Error while calling the Web API!");
        if (element) element.removeAttribute("disabled");
    };

    if (element) element.setAttribute("disabled", "disabled");
    $.ajax(options);
}

// removes entry from list
function removeTask(id, element) {
    var options = {};
    options.url = "/api/values/remove/" + id;
    options.type = "GET";
    options.success = function () {
        // Remove list element from document
        $(element).parent().parent().remove();
        list_offset--;
        $("#list_offset").val(list_offset);
        getNext(null);
        $("#list_offset").val(list_offset);
    };
    options.error = function () {
        alert("Error while calling the Web API!");
        if (element) element.removeAttribute("disabled");
    };

    if (element) element.setAttribute("disabled", "disabled");
    $.ajax(options);
}

// removes entry from list
function addTask(element) {
    var options = {};
    options.url = "/api/values/insert/" + $('#input_new_entry').val();
    options.type = "GET";
    options.success = function () {
        if (element) element.removeAttribute("disabled");
        $(element).parent().find("input").val('');
    };
    options.error = function () {
        alert("Error while calling the Web API!");
        if (element) element.removeAttribute("disabled");
    };

    if (element) element.setAttribute("disabled", "disabled");
    $.ajax(options);
}

// removes entry from list
function rebuildList(btnElement) {
    var options = {};
    options.url = "/api/values/rebuild";
    options.type = "GET";
    options.success = function () {
        // refresh entire page on rebuild
        location.reload();
    };
    options.error = function () {
        alert("Error while calling the Web API!");
        if (btnElement) btnElement.removeAttribute("disabled");
    };

    if (btnElement) btnElement.setAttribute("disabled", "disabled");
    $.ajax(options);
}
