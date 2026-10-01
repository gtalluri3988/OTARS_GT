

function onResultSuccess(data) {
    var jsonObject = new parseJSON(null, null);
    jsonObject.parseResult(data);

}


function parseJSON(onAssignError, elementObject) {
    if (elementObject) {
        this.elementObject = elementObject;
    }
    if (onAssignError) {
        this.onAssignError = onAssignError;
    }


    // parse json result
    this.parseResult = function (data) {

        // var jsonObject = eval('(' + data + ')');


        $("[data-valsummary]").text("");
        $("[data-valmsg-for]").text("");
        $("[data-valmsg-for]").parent().hide();
        $().removeClass("ui-state-error");
        $().removeClass("field-validation-error");
        this.assignMessage(data.Message);
        this.assignErrors(data.Errors);
        if (data.IsRedirect) {
            this.setRedirect(data.Redirect, data.RedirectKey);
        }
    }


    //Assign Validation message.
    this.assignMessage = function (message) {
        if (message === "undefined") return;
        $("[data-valsummary]").append(message);
        $("[data-valsummary]").append("<br/>");
        $("[data-valsummary]").addClass("field-validation-error");
    }
    //Assign Errors
    this.assignErrors = function (errors) {

        for (i = 0; i < errors.length; i++) {
            var func;
            if (errors[i].Key != "") {
                func = this.assignPropertyMessage;
            }
            else {
                func = this.assignMessage;
            }
            for (j = 0; j < errors[i].Messages.length; j++) {
                func(errors[i].Messages[j], errors[i].Key);
                if (this.onAssignError) {
                    this.onAssignError(errors[i].Messages[j], errors[i].Key);
                }
                if (this.elementObject) {
                    this.elementObject.assignError(errors[i].Key);
                }
            }
        }
    }
    //Assign Errors to the Elements
    this.assignPropertyMessage = function (value, key) {
        $("[data-valmsg-for='" + key + "']").text(value);
        $("[data-valmsg-for='" + key + "']").parent().show();
        $("#" + key).addClass("error");
    }

    //set Redirect
    this.setRedirect = function (url, key) {
        window.location.replace(url + "/" + key);
    }

}



// Class for tab Options


function tab(tabContainer,tabHeader)
{
    this.tabContainer = tabContainer;
    if (tabHeader != null) {
        this.tabHeader = tabHeader;
    }
    this.tabHeaderCSS="ui-tabs-nav"    
    this.tabHeaderActiveCSS = "ui-tabs-selected ui-state-active";
    this.tabHeaderErrorCSS = "ui-tabs-error";
    this.tabCSS = "ui-tabs-panel"
    this.tabHideCSS = "ui-tabs-hide";
    this.tabActivsCSS = "";
    this.tabErrorCss = "ui-tabs-error";
    this.tabHeaders;
    this.tabs;
    this.errorAssigned = false;
    this.load = function () {

        if (this.tabHeader) {
            this.tabHeaders = $([]).add($(this.tabHeader).find("." + this.tabHeaderCSS)).find("li");
        }
        else {
            this.tabHeaders = $([]).add($(this.tabContainer).find("." + this.tabHeaderCSS)).find("li");
        }
        this.tabs = $([]).add($("." + this.tabCSS))
        this.tabHeaders.removeClass(this.tabHeaderErrorCSS);
        this.tabs.removeClass(this.tabErrorCss);
    }

    this.assignError = function (elementID) {
        element = $("[data-valmsg-for='" + elementID + "']").parents("." + this.tabCSS);
        if (element.length == 0) {
            return;
        }
        header = this.tabHeaders.find("[href='#" + element[0].id + "']");
        if (!header) {
            return;
        }

        if (!this.errorAssigned) {
            header.click();
            $("[data-valmsg-for='" + elementID + "']").parent().find("a").focus();
            $("[name='" + elementID + "']").focus();
          
        }
        header.parent().addClass(this.tabHeaderErrorCSS);
        this.errorAssigned = true;


    }
}