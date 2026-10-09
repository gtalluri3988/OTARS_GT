// PCI DSS session timeouts in the browser (the server enforces the same rules in SessionTimeoutManager).
// - Idle timeout: no request to the server for idleSeconds -> logout and go to SSO.
// - Absolute timeout: absoluteSeconds after login -> logout and go to SSO, regardless of activity.
// A warning is shown warningSeconds before the idle timeout so the user can continue the session.
var sessionTimeout = (function ($) {
    var settings, idleTimer, warningTimer, absoluteTimer, expired = false;

    function expire() {
        if (expired) return;
        expired = true;
        window.location.href = settings.expiredUrl;
    }

    function getWarning() {
        var warning = $("#session-timeout-warning");
        if (warning.length > 0) return warning;

        warning = $("<div id='session-timeout-warning'></div>").css({
            position: "fixed", top: "10px", left: "50%", width: "420px", marginLeft: "-210px", zIndex: 100000,
            padding: "12px", background: "#fff3cd", border: "1px solid #c69500", color: "#000", textAlign: "center"
        }).hide();
        $("<span></span>").text("Your session is about to expire due to inactivity. ").appendTo(warning);
        $("<button type='button' class='k-button'>Continue session</button>").on("click", keepAlive).appendTo(warning);
        $("body").append(warning);
        return warning;
    }

    function showWarning() { getWarning().show(); }
    function hideWarning() { $("#session-timeout-warning").hide(); }

    function keepAlive() {
        hideWarning();
        $.ajax({ url: settings.keepAliveUrl, type: "GET", cache: false })
            .done(function (response) { if (!response || response.Result !== true) expire(); });
    }

    function resetIdle() {
        if (expired) return;
        clearTimeout(idleTimer);
        clearTimeout(warningTimer);
        hideWarning();
        idleTimer = setTimeout(expire, settings.idleSeconds * 1000);
        if (settings.idleSeconds > settings.warningSeconds)
            warningTimer = setTimeout(showWarning, (settings.idleSeconds - settings.warningSeconds) * 1000);
    }

    function init(options) {
        settings = $.extend({ idleSeconds: 900, absoluteSeconds: 3600, warningSeconds: 60 }, options);

        absoluteTimer = setTimeout(expire, settings.absoluteSeconds * 1000);
        resetIdle();

        // Every AJAX request reaches the server and refreshes its idle timer, so refresh ours too.
        $(document).ajaxComplete(function (event, xhr) {
            if (xhr && xhr.status === 401 && xhr.getResponseHeader("X-Session-Expired")) {
                expire();
                return;
            }
            resetIdle();
        });
    }

    return { init: init };
})(jQuery);
