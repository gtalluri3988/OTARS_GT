
function submitData(success) {
    var url = $("form", "#mainContent").attr("action");
    showProgressbar();
    $.post(url,
    $("form", "#mainContent").serialize(),
    function (data, textStatus, jqxhr) {
        hideProgressbar();
        success(data);
    })
    .error(function (data, textStatus, jqxhr) {
        hideProgressbar();
        $("#exceptionDialog").html(data.responseText).dialog();
    })
    ;
}

$(function () {

    $("form").submit(function () {
        showProgressbar();
    });
});
function onRegistrationSuccess(data, tabId) {
    var tabObject = new tab($("#" + tabId));
    tabObject.load();
    var jsonObject = new parseJSON(null, tabObject);
    jsonObject.parseResult(data);
}
function serializeForm() {
    var str = "";
    $("form[data-canserialize=true]").each(function () {
        if (str != "") str += "&";
        str += $(this).serialize();
    });
    return str;
}

$(function () {
   
});

var progressBarWindow;
$(function () {
    progressBarWindow = $("#progress-bar")
                        .kendoWindow({
                            title: false,
                            visible: false,
                            modal: true,
                            resizable: false,
                            width:"150",
                            height:"15"
                        }).data("kendoWindow");

});

function showProgressbar() {
    progressBarWindow.center().open();
    /* $("#progressbar").css("position", "absolute");
    $("#progressbar").css("top", (($(window).height() - 50) / 2) + $(window).scrollTop()  + "px");
    $("#progressbar").css("left", (($(window).width() - 300) / 2) + $(window).scrollLeft() + "px");
    $("#progressbar").css("z-index", "500");
    $("#progressbar").progressbar({value:100}).dialog();*/
    /*$.blockUI({ css: {
        border: 'none',
        padding: '15px',
        backgroundColor: '#000',
        '-webkit-border-radius': '10px',
        '-moz-border-radius': '10px',
        opacity: .5,
        color: '#fff'
    }
    });*/
}

function hideProgressbar() {
    progressBarWindow.close();
    //$("#progressbar").hide();
    //$.unblockUI();
}