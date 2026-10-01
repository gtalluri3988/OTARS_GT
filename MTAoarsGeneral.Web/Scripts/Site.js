var infoWindow = null;
$(function () {
    infoWindow = $("#info-window").kendoWindow({
        modal: true,
        width: "300px",
        visible: false,
        animation: { open: { effects: "fadeIn" }, close: { effects: "fadeIn", reverse: true} }
    }).data("kendoWindow");

    $("input[type='text']").blur(function () {
        $(this).val($(this).val().toUpperCase());
    });

});

function getValidator(selector, messages) {
    if (typeof messages === "undefined") messages = [];
    var summary = $("ul.mta-error-summary", selector);
    var options = {
        rules: {},
        messages: {}
    };
    hideSummary();
    for (var i = 0; i < messages.length; i++) {
        var msg = messages[i];
        var key = msg.Key;
        options.rules[msg.Key] = (function (key) {
            return function (input) {
                if (input.attr("name") == key) return false;
                return true;
            };
        })(msg.Key);
        options.messages[msg.Key] = msg.Value;
        if (msg.Key == "") {
            addErrorToSummary(selector, msg.Value);
        }
    }
    return $(selector).kendoValidator(options).data("kendoValidator");
}

function hideSummary(selector) {
    var summary = $("ul.mta-error-summary", selector);
    summary.children().remove();
    summary.hide();
}

function addErrorToSummary(selector, error) {
    var summary = $("ul.mta-error-summary", selector);
    summary.append('<li><span class="k-icon k-warning"></span>' + error + '</li>');
    summary.show();
}


function jsonPost(selector, url, viewModel, successMethod, errorMethod, handleValidateErrors) {
    var validator = getValidator(selector);
    if (validator.validate() == false) {
        showTabError();
        return false;
    }
    showTabError();
    var form = $("form", selector);
    showProgressBar();
    //alert(kendo.stringify(viewModel));
    $.ajax({
        url: url,
        type: "POST",
        data: kendo.stringify(viewModel),
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (response, xhr) {
            hideProgressBar();
            //alert(kendo.stringify(response));
            if (response.Result == false) {
                validator = getValidator(selector, response.Errors);
                validator.validate();
                showTabError();
                if (handleValidateErrors)
                    handleValidateErrors(response);
                return false;
            }
            successMethod(response);
        }
    })
    .error(function (data, textStatus, jqxhr) {
        if (errorMethod) errorMethod(data, textStatus, jqxhr);
        else checkError(data, textStatus, jqxhr);
    });
    return false;
}

function viewPost(selector, url, viewModel, successMethod) {
    var validator = getValidator(selector);
    if (validator.validate() == false) {
        showTabError();
        return false;
    }
    showTabError();
    var form = $("form", selector);
    showProgressBar();
    $.ajax({
        url: url,
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        data: kendo.stringify(viewModel),
        success: function (response) {
            hideProgressBar();
            successMethod(response);
        }
    })
    .error(function (data, textStatus, jqxhr) {
        checkError(data, textStatus, jqxhr);
    });
}

function reportPost(selector, baseUrl, viewModel, iframe) {
    var validator = getValidator(selector);
    if (validator.validate() == false) {
        return false;
    }
    var model = eval("(" + kendo.stringify(viewModel) + ")");
    var params = $.param(model);
    $(iframe).attr("src", baseUrl + "&" + params);
}

function checkError(data, textStatus, jqxhr) {
    if (data.responseText.indexOf("Unauthorized to view the page") >= 0) {
        location.href = unauthorizedUrl;
    } else {
        //alert(kendo.stringify(data));
        alert(data.responseText);
        hideProgressBar();
    }
}

function showTabError() {
    $("li[aria-controls]").removeClass("mta-tab-error");
    $("div.k-content:has(span.k-invalid-msg[style!='display: none;'])").each(function (index, item) {
        var id = $(this).attr("id");
        $("li[aria-controls='" + id + "']").addClass("mta-tab-error");
        if (index == 0) $(".k-tabstrip").data("kendoTabStrip").select("li[aria-controls='" + id + "']");
    });
}


function formatDate(date) {
    if (typeof date === "undefined") return "";
    if (date == null) return "";
    if(date.toString().indexOf("Date") > 0) date = new Date(parseInt(date.substr(6)));
    return date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear();
}

function getO3Date(date) {
    return new Date(parseInt(date.substr(6)));
}

function getDate(date) {
    if (date === "undefined") return "";
    if (date == null) return "";
    var arr = date.split(/[\/|\-]/);
    if(arr == null) return "";
    if (arr.length != 3) return;
    return new Date(arr[2], parseInt(arr[1]) - 1, arr[0]);
}

function getDate2(date) {
    if (date === "undefined") return "";
    if (date == null) return "";
    var arr = date.split(/[\/|\-]/);
    if (arr == null) return "";
    if (arr.length != 3) return;
    return new Date(arr[2], parseInt(arr[1]) - 1, arr[0], 8);
}


function addBreadCrumb(title, url, index) {
    $(".breadcrumb li:gt(" + (index - 1) + ")").remove();
    $("<li><a href=\"" + url + "\">" + title + "</li>").appendTo($(".breadcrumb"));
}

function showProgressBar() {
    $("button").prop("disabled", true);
    $("span", "button").addClass("k-loading");
}

function hideProgressBar() {
    $("button").prop("disabled", false);
    $("span", "button").removeClass("k-loading");
}

function coalesce() {
    for (var i = 0; i < arguments.length; i++) {
        if (arguments[i] != null) return arguments[i];
    }
}

function print(selector) {
    var newWindow = window.open();
    $.get(printUrl, function (result) {
        newWindow.document.write(result);
        newWindow.document.write($(selector).html());
        newWindow.print();
        newWindow.close();
    });
 
}


function getGuidQueryString() {
    return "time=" + (new Date()).getTime();
}


