/*!
 * MTAoars Security Cache Prevention Script
 * Prevents cached pages from being accessible after logout using browser back button
 */

(function () {
    'use strict';

    // Prevent caching of the current page
    function preventPageCaching() {
        // Set cache-busting headers using JavaScript
        if (window.history && window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }

        // Disable browser caching
        document.addEventListener('DOMContentLoaded', function () {
            // Add meta tags to prevent caching
            var metaNoCache = document.createElement('meta');
            metaNoCache.httpEquiv = 'Cache-Control';
            metaNoCache.content = 'no-cache, no-store, must-revalidate';
            document.head.appendChild(metaNoCache);

            var metaPragma = document.createElement('meta');
            metaPragma.httpEquiv = 'Pragma';
            metaPragma.content = 'no-cache';
            document.head.appendChild(metaPragma);

            var metaExpires = document.createElement('meta');
            metaExpires.httpEquiv = 'Expires';
            metaExpires.content = '0';
            document.head.appendChild(metaExpires);
        });
    }

    // Handle browser back button for authenticated pages
    function handleBrowserBackButton() {
        // Check if user is on an authenticated page
        var isAuthenticatedPage = document.body.classList.contains('authenticated') ||
            document.querySelector('[data-authenticated="true"]') ||
            window.location.pathname.indexOf('/account/logon') === -1;

        if (isAuthenticatedPage) {
            // Prevent back button from showing cached content
            window.addEventListener('pageshow', function (event) {
                if (event.persisted) {
                    // Page was loaded from cache, redirect to login
                    window.location.reload();
                }
            });

            // Handle browser navigation
            window.addEventListener('beforeunload', function () {
                // Clear any cached data
                if (window.sessionStorage) {
                    window.sessionStorage.clear();
                }
                if (window.localStorage) {
                    // Only clear app-specific localStorage items
                    var keysToRemove = [];
                    for (var i = 0; i < window.localStorage.length; i++) {
                        var key = window.localStorage.key(i);
                        if (key && (key.startsWith('MTAoars') || key.startsWith('auth') || key.startsWith('user'))) {
                            keysToRemove.push(key);
                        }
                    }
                    keysToRemove.forEach(function (key) {
                        window.localStorage.removeItem(key);
                    });
                }
            });

            // Disable browser cache using JavaScript
            window.addEventListener('beforeunload', function () {
                // Force browser to not cache the page
                document.body.innerHTML = '';
            });
        }
    }

    // Enhanced logout function
    function enhancedLogout() {
        // Clear all client-side storage
        if (window.sessionStorage) {
            window.sessionStorage.clear();
        }

        if (window.localStorage) {
            // Clear app-specific localStorage
            var keysToRemove = [];
            for (var i = 0; i < window.localStorage.length; i++) {
                var key = window.localStorage.key(i);
                if (key && (key.startsWith('MTAoars') || key.startsWith('auth') || key.startsWith('user'))) {
                    keysToRemove.push(key);
                }
            }
            keysToRemove.forEach(function (key) {
                window.localStorage.removeItem(key);
            });
        }

        // Clear browser cache if supported
        if ('caches' in window) {
            caches.keys().then(function (names) {
                names.forEach(function (name) {
                    caches.delete(name);
                });
            });
        }

        // Replace current history entry to prevent back button access
        if (window.history && window.history.replaceState) {
            window.history.replaceState(null, null, '/Account/LogOn');
        }

        // Disable back button functionality after logout
        window.history.pushState(null, null, window.location.href);
        window.addEventListener('popstate', function (event) {
            window.location.href = '/Account/LogOn';
        });

        // Add a small delay before redirect to ensure cleanup completes
        setTimeout(function () {
            window.location.href = '/Account/Logout';
        }, 100);
    }

    // Session timeout handling
    function handleSessionTimeout() {
        var sessionWarningShown = false;
        var sessionTimeout = 20 * 60 * 1000; // 20 minutes in milliseconds
        var warningTime = 18 * 60 * 1000; // Show warning at 18 minutes

        // Reset timeout on user activity
        var activityEvents = ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart', 'click'];
        var resetTimer;

        function resetSessionTimer() {
            clearTimeout(resetTimer);
            sessionWarningShown = false;
            
            resetTimer = setTimeout(function () {
                if (!sessionWarningShown) {
                    sessionWarningShown = true;
                    if (confirm('Your session will expire in 2 minutes. Click OK to continue or Cancel to logout.')) {
                        resetSessionTimer(); // Reset if user chooses to continue
                    } else {
                        enhancedLogout();
                    }
                }
            }, warningTime);

            // Force logout after total timeout
            setTimeout(function () {
                enhancedLogout();
            }, sessionTimeout);
        }

        // Add activity listeners
        activityEvents.forEach(function (event) {
            document.addEventListener(event, resetSessionTimer, true);
        });

        // Start the timer
        resetSessionTimer();
    }

    // Initialize security measures
    function initializeSecurity() {
        preventPageCaching();
        handleBrowserBackButton();

        // Only enable session timeout for authenticated pages
        var isAuthenticated = document.body.classList.contains('authenticated') ||
            document.querySelector('[data-authenticated="true"]') ||
            window.location.pathname.indexOf('/account/logon') === -1;

        if (isAuthenticated) {
            handleSessionTimeout();
        }

        // Bind enhanced logout to logout links
        var logoutLinks = document.querySelectorAll('a[href*="LogOut"], a[href*="logout"]');
        logoutLinks.forEach(function (link) {
            link.addEventListener('click', function (e) {
                e.preventDefault();
                enhancedLogout();
            });
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeSecurity);
    } else {
        initializeSecurity();
    }

    // Expose enhanced logout function globally
    window.MTAoarsLogout = enhancedLogout;

})();


