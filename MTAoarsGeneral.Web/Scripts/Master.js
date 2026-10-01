

    function Master(options) {
        var currentItem = {};
        var defaults = {
            gridSelector: "", detailsSelector: "", url: masterDataServiceUrl, modelItem: {}, validationUrl: "",
            beforeDetailsBind: function () { },
            beforeValidate: function () { },
            afterSave: function () { },
            beforeAdd: function () { },
            beforeStringify: function (data, original) { return data; }
        };
        var settings = $.extend(true, defaults, options);


        this.editClick = function () {
            var selectedrow = $(settings.gridSelector).find("tbody tr.k-state-selected");
            currentItem = $(settings.gridSelector).data("kendoGrid").dataItem(selectedrow);
            currentAction = "Update";
            settings.beforeDetailsBind(currentItem, currentAction);
            kendo.bind($(settings.detailsSelector), currentItem);
            $(settings.detailsSelector).show();
        }

        this.addClick = function () {
            currentItem = kendo.observable($.extend(true, {}, settings.modelItem));
            currentAction = "Add";
            settings.beforeDetailsBind(currentItem, currentAction);
            kendo.bind($(settings.detailsSelector), currentItem);
            
            $(settings.detailsSelector).show();
        }

        this.getCurrent = function () {
            return currentItem;
        }

        this.refresh = function () {
            kendo.bind($(settings.detailsSelector), currentItem);
            $(settings.gridSelector).data("kendoGrid").dataSource.read();
        }

        this.initActions = function() {
            $("#save-button", settings.detailsSelector).click(saveMaster);
            $("#cancel-button", settings.detailsSelector).click(cancelMaster);
        }

        this.save = function () {
            saveMaster();
        }

        var saveMaster = function () {
            settings.beforeValidate(currentItem);
            jsonPost(settings.detailsSelector, settings.validationUrl, currentItem, function (response) {
                if (response.Result) {
                    if (currentAction == "Update") {
                        updateMaster(currentItem);
                    }
                    if (currentAction == "Add") {
                        addMaster(currentItem);
                    }

                }
            });
        }

        var cancelMaster = function () {
            $(settings.detailsSelector).hide();
        }

        var updateMaster = function (currentItem) {

            var data = copyNew(currentItem, settings.modelItem);
            $.ajax({
                 url: settings.url + "(" + currentItem.ID + "L)",
                //url: settings.url,
                 data: kendo.stringify(settings.beforeStringify(data, currentItem)),
                //type: "PUT",
                type: "POST",
                beforeSend: function (request) {
                    request.setRequestHeader("DataServiceVersion", "3.0");
                    request.setRequestHeader("MinDataServiceVersion", "3.0");
                    request.setRequestHeader("CurrentOperation", "Update");
                },
                dataType: "json",
                contentType: "application/json;odata=verbose",
                success: function (response) {
                    settings.afterSave(currentAction, currentItem);
                    $(settings.detailsSelector).hide();
                }
            });
        }

        var addMaster = function (item) {
            var data = copyNew(item, settings.modelItem);
            settings.beforeAdd(data);
            $.ajax({
                url: settings.url,
                data: kendo.stringify(settings.beforeStringify(data, item)),
                type: "POST",
                beforeSend: function (request) {
                    request.setRequestHeader("DataServiceVersion", "3.0");
                    request.setRequestHeader("MinDataServiceVersion", "3.0");
                },
                dataType: "json",
                contentType: "application/json;odata=verbose",
                success: function (response) {
                    $(settings.detailsSelector).hide();
                }
            }).complete(function (data, status) {
                settings.afterSave(currentAction, currentItem);
                if (data.status == 201) {
                    $(settings.detailsSelector).hide();
                }
            });
        }

        var copyNew = function (src, destination) {
            var newDestination = $.extend(true, {}, destination);
            for (var prop in newDestination) {
                if (src[prop] == null) continue;

                if (typeof (src[prop]) === "object") {
                    if (src[prop] != null && src[prop].ID != undefined) {
                        newDestination[prop] = src[prop].ID + "";
                    } else if (src[prop] instanceof Date) {
                        // Format DateTime values for OData
                        newDestination[prop] = src[prop].toISOString();
                    } else {
                        newDestination[prop] = src[prop];
                    }
                } else {
                    // Check if this is a DateTime property by name and handle .NET date strings
                    if (typeof(src[prop]) === "string" && 
                        (prop.indexOf("Date") !== -1 || prop.indexOf("date") !== -1) &&
                        src[prop].indexOf("/Date(") !== -1) {
                        // Parse .NET DateTime string format /Date(timestamp)/
                        var timestamp = parseInt(src[prop].replace(/\/Date\((\d+)\)\//, "$1"));
                        var date = new Date(timestamp);
                        newDestination[prop] = date.toISOString();
                    } else {
                        newDestination[prop] = src[prop] + "";
                    }
                }

            }
            return newDestination;
        }

        var validate = function (item) {
            
        }
        this.initActions();
    }



function getAsyncFunction(lookupUrl) {
    return function () {
        var deferred = $.Deferred();
        var loadItems = function () {
            new kendo.data.DataSource({
                transport: {
                    read: lookupUrl

                }
            }).fetch(function(data) {
                deferred.resolve($.map(data.items, function(item) {
                    return {
                        value: item.ID,
                        text: item.Description
                    };
                }));
            });
        };
        window.setTimeout(loadItems, 1);
        return deferred.promise();
    };
}


function getMasterGridDataSource(url, model, deleteUrl) {
    if (arguments.length < 3) deleteUrl = url;
    return new kendo.data.DataSource({
        type: "odata",
        serverPaging: true,
        serverFiltering: true,
        pageSize: 5,
        serverSorting: true,
        transport: {
            read: {
                url: url,
                headers: {
                    DataServiceVersion: "2.0",
                    MaxDataServiceVersion: "2.0"
                }
            },
            destroy: {
                url: function (data) {
                    return deleteUrl + "(" + data.ID + "L)";
                },
                type: "POST",
                dataType: "json",
                headers: {
                    CurrentOperation: "Delete"
                }
            }
        },
        schema: {
            model: model
        }
    });
}