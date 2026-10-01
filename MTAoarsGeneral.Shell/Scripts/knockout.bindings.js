
/*---   knockout-----------------------*/
$(document).ready(function () {

    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel, context) {
            //initialize datepicker with some optional options
            var options = allBindingsAccessor().datepickerOptions || {};
            $(element).kendoDatePicker({ format: "dd/MM/yyyy" });
            var datetimepicker = $(element).data("kendoDatePicker");

            var value = ko.selectExtensions.readValue(element);
            if (valueAccessor()()) {
                var modelValue = valueAccessor();
                var input = new Date(Date(modelValue()));

                if (input) {
                    var defaultValue = input.getDate() + "/" + (input.getMonth() + 1) + "/" + input.getFullYear();
                    ko.selectExtensions.writeValue(element, defaultValue);
                    var elementValue = ko.selectExtensions.readValue(element);
                    writeValueToProperty(modelValue, allBindingsAccessor, 'datepicker', new Date(getDate(elementValue)), /* checkIfDifferent: */true);
                }
            }

            //handle the field changing
            ko.utils.registerEventHandler(element, "change", function () {
                var modelValue = valueAccessor();
                var elementValue = ko.selectExtensions.readValue(element);
                writeValueToProperty(modelValue, allBindingsAccessor, 'datepicker', new Date(getDate(elementValue)), /* checkIfDifferent: */true);
            });

            //handle disposal (if KO removes by the template binding)
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                // $(element).datepicker("destroy");
            });


        },
        update: function (element, valueAccessor) {

            var value = ko.utils.unwrapObservable(valueAccessor());
            current = $(element).kendoDatePicker("value");
            if (value - current !== 0) {
                $(element).kendoDatePicker("value", value);
            }
        }


    };

    ko.bindingHandlers['jsonHidden'] = {
        'init': function (element, valueAccessor, allBindingsAccessor) {
            // Always catch "change" event; possibly other events too if asked
            var eventsToCatch = ["change"];
            var requestedEventsToCatch = allBindingsAccessor()["valueUpdate"];
            if (requestedEventsToCatch) {
                if (typeof requestedEventsToCatch == "string") // Allow both individual event names, and arrays of event names
                    requestedEventsToCatch = [requestedEventsToCatch];
                ko.utils.arrayPushAll(eventsToCatch, requestedEventsToCatch);
                eventsToCatch = ko.utils.arrayGetDistinctValues(eventsToCatch);
            }

            var valueUpdateHandler = function () {
                var modelValue = valueAccessor();
                var elementValue = ko.selectExtensions.readValue(element);
                // alert(elementValue);
                ko.expressionRewriting.writeValueToProperty(modelValue, allBindingsAccessor, 'value', elementValue, /* checkIfDifferent: */true);
            }


        },
        'update': function (element, valueAccessor) {
            var valueIsSelectOption = false;
            var newValue = ko.toJSON(ko.utils.unwrapObservable(valueAccessor()));
            var elementValue = ko.selectExtensions.readValue(element);
            var valueHasChanged = (newValue != elementValue);

            // JavaScript's 0 == "" behavious is unfortunate here as it prevents writing 0 to an empty text box (loose equality suggests the values are the same).
            // We don't want to do a strict equality comparison as that is more confusing for developers in certain cases, so we specifically special case 0 != "" here.
            if ((newValue === 0) && (elementValue !== 0) && (elementValue !== "0"))
                valueHasChanged = true;

            if (valueHasChanged) {
                var applyValueAction = function () { ko.selectExtensions.writeValue(element, newValue); };
                applyValueAction();

                // Workaround for IE6 bug: It won't reliably apply values to SELECT nodes during the same execution thread
                // right after you've changed the set of OPTION nodes on it. So for that node type, we'll schedule a second thread
                // to apply the value as well.
                var alsoApplyAsynchronously = valueIsSelectOption;
                if (alsoApplyAsynchronously)
                    setTimeout(applyValueAction, 0);
            }

            // If you try to set a model value that can't be represented in an already-populated dropdown, reject that change,
            // because you're not allowed to have a model value that disagrees with a visible UI selection.
            if (valueIsSelectOption && (element.length > 0))
                ensureDropdownSelectionIsConsistentWithModelValue(element, newValue, /* preferModelValue */false);
        }
    };

    function writeValueToProperty(property, allBindingsAccessor, key, value, checkIfDifferent) {
        if (!property || !ko.isWriteableObservable(property)) {
            var propWriters = allBindingsAccessor()['_ko_property_writers'];
            if (propWriters && propWriters[key]) propWriters[key](value);
        } else if (!checkIfDifferent || property() !== value) { property(value); }
    }

});


function getDate(date) {
    var dates = date.split("/");
    return dates[1] + "/" + dates[0] + "/" + dates[2];
}